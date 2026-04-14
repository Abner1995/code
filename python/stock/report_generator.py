"""
报告生成器模块

提供可配置的艾略特波浪分析报告生成功能，支持不同详细程度的报告输出。

主要功能：
1. 执行摘要：1-2页关键信息摘要
2. 详细分析：按需展开的详细内容
3. 交易建议：具体可操作的交易建议
4. 可配置的详细程度：brief, standard, detailed

使用示例：
    generator = ReportGenerator()
    report = generator.generate_report(result, detail_level='standard')
    generator.save_report(report, 'analysis_report.txt')
"""

import os
from typing import Dict, List, Any, Optional
import pandas as pd
from datetime import datetime


class ReportGenerator:
    """
    艾略特波浪分析报告生成器

    生成高信息密度、易读的分析报告，突出关键信息。
    """

    def __init__(self, verbose: bool = False):
        """
        初始化报告生成器

        参数:
            verbose: 是否显示详细日志
        """
        self.verbose = verbose

    def calculate_risk_score(self, result: Dict[str, Any]) -> float:
        """
        计算分析结果的风险评分 (0-10分，分数越高风险越高)

        参数:
            result: 分析结果字典

        返回:
            风险评分 (0-10)
        """
        score = 5.0  # 基础分

        # 1. 有效性权重 (40%)
        is_valid = result.get('valid', False)
        if not is_valid:
            score += 4.0  # 无效分析风险高

        # 2. 置信度权重 (30%)
        confidence = result.get('confidence', 0.0)
        if confidence < 0.3:
            score += 3.0
        elif confidence < 0.6:
            score += 1.5
        elif confidence >= 0.8:
            score -= 1.0

        # 3. 规则违反权重 (20%)
        validation = result.get('validation', {})
        violations = len(validation.get('rule_violations', []))
        score += min(violations * 0.5, 2.0)  # 每条违反加0.5，最多2分

        # 4. 波浪质量权重 (10%)
        waves = result.get('waves', {})
        impulse_waves = waves.get('impulse', [])
        if impulse_waves:
            quality_scores = [w.get('quality_score', 0.5) for w in impulse_waves if 'quality_score' in w]
            if quality_scores:
                avg_quality = sum(quality_scores) / len(quality_scores)
                if avg_quality < 0.3:
                    score += 1.0
                elif avg_quality >= 0.8:
                    score -= 0.5

        # 确保分数在0-10之间
        return max(0.0, min(score, 10.0))

    def generate_report(self, result: Dict[str, Any], detail_level: str = 'standard',
                       include_trading_recommendations: bool = True,
                       metadata: Optional[Dict[str, Any]] = None) -> str:
        """
        生成完整报告

        参数:
            result: 分析结果字典
            detail_level: 报告详细程度 ('brief', 'standard', 'detailed')
            include_trading_recommendations: 是否包含交易建议
            metadata: 额外元数据，如股票代码、时间范围等

        返回:
            报告内容字符串
        """
        if self.verbose:
            print(f"[报告生成] 开始生成报告，详细程度: {detail_level}")

        report_parts = []

        # 1. 报告头
        report_parts.append(self._generate_header(result, metadata))

        # 2. 执行摘要 (所有详细程度都包含)
        report_parts.append(self.generate_executive_summary(result))

        # 3. 详细分析 (根据详细程度决定内容)
        if detail_level in ['standard', 'detailed']:
            report_parts.append(self.generate_detailed_analysis(result, detail_level))

        # 4. 交易建议 (根据配置决定是否包含)
        if include_trading_recommendations:
            report_parts.append(self.generate_trading_recommendations(result))

        # 5. 报告尾
        report_parts.append(self._generate_footer())

        # 合并所有部分
        full_report = "\n\n".join(report_parts)

        if self.verbose:
            print(f"[报告生成] 报告生成完成，总长度: {len(full_report)} 字符")

        return full_report

    def generate_executive_summary(self, result: Dict[str, Any]) -> str:
        """
        生成执行摘要 (1-2页关键信息)

        参数:
            result: 分析结果字典

        返回:
            执行摘要字符串
        """
        lines = []
        lines.append("=" * 60)
        lines.append("执行摘要")
        lines.append("=" * 60)
        lines.append("")

        # 基本信息
        lines.append("📊 分析概览")
        lines.append("-" * 40)

        # 分析有效性
        is_valid = result.get('valid', False)
        confidence = result.get('confidence', 0.0) * 100
        message = result.get('message', 'N/A')

        lines.append(f"分析结果: {'✅ 有效' if is_valid else '⚠️ 需要谨慎'}")
        lines.append(f"置信度: {confidence:.1f}%")
        lines.append(f"结果摘要: {message}")
        lines.append("")

        # 波浪模式统计
        waves = result.get('waves', {})
        impulse_count = len(waves.get('impulse', []))
        corrective_count = len(waves.get('corrective', []))

        lines.append(f"📈 波浪模式识别")
        lines.append("-" * 40)
        lines.append(f"5浪推动模式: {impulse_count} 个")
        lines.append(f"3浪调整模式: {corrective_count} 个")
        lines.append("")

        # 规则验证结果
        validation = result.get('validation', {})
        passed_rules = len(validation.get('passed_rules', []))
        violations = len(validation.get('rule_violations', []))

        lines.append(f"✅ 规则验证")
        lines.append("-" * 40)
        lines.append(f"通过规则: {passed_rules} 条")
        lines.append(f"违反规则: {violations} 条")
        if violations == 0 and passed_rules > 0:
            lines.append("✓ 所有核心规则验证通过")
        elif violations > 0:
            lines.append(f"⚠️ 有 {violations} 条规则需要关注")
        lines.append("")

        # 关键斐波那契水平
        fibonacci = result.get('fibonacci', {})
        if 'retracements' in fibonacci and fibonacci['retracements']:
            lines.append(f"📐 关键斐波那契水平")
            lines.append("-" * 40)
            # 获取第一个回撤水平作为示例
            for wave_name, levels in fibonacci['retracements'].items():
                if levels:
                    # 显示关键水平: 38.2%, 50%, 61.8%
                    key_levels = ['38.2%', '50.0%', '61.8%']
                    for level in key_levels:
                        if level in levels:
                            price = levels[level]
                            lines.append(f"{wave_name} {level}: {price:.2f}")
                    break  # 只显示第一个波浪的回撤
            lines.append("")

        # 下一浪预测
        prediction = result.get('prediction', {})
        next_wave = prediction.get('next_wave')
        pred_confidence = prediction.get('confidence', 0.0) * 100

        if next_wave:
            lines.append(f"🔮 下一浪预测")
            lines.append("-" * 40)
            lines.append(f"预期下一浪: {next_wave}")
            lines.append(f"预测置信度: {pred_confidence:.1f}%")

            target_prices = prediction.get('target_prices', [])
            if target_prices:
                lines.append("关键目标位:")
                for target in target_prices[:3]:  # 显示前3个目标
                    lines.append(f"  • {target['description']}: {target['price']:.2f}")
            lines.append("")

        # 风险评分
        risk_score = self.calculate_risk_score(result)
        risk_level = "低风险" if risk_score < 3 else "中风险" if risk_score < 7 else "高风险"
        lines.append(f"⚠️ 风险评分: {risk_score:.1f}/10 ({risk_level})")
        lines.append("-" * 40)
        lines.append(f"评分解读:")
        if risk_score < 3:
            lines.append("  • 风险较低，波浪分析可靠性高")
            lines.append("  • 可考虑正常仓位交易")
        elif risk_score < 7:
            lines.append("  • 风险中等，需结合其他指标验证")
            lines.append("  • 建议降低仓位，设置严格止损")
        else:
            lines.append("  • 风险较高，波浪分析可靠性低")
            lines.append("  • 建议观望或极小仓位测试")
        lines.append("")

        # 关键建议
        lines.append(f"💡 关键建议")
        lines.append("-" * 40)
        if is_valid and confidence > 70:
            lines.append("✓ 分析结果可靠，可参考波浪模式进行交易决策")
            lines.append("✓ 建议关注关键斐波那契支撑/阻力位")
            lines.append("✓ 结合成交量和其他技术指标确认信号")
        elif is_valid and confidence > 50:
            lines.append("✓ 分析结果基本可靠，但需谨慎验证")
            lines.append("✓ 建议等待更多确认信号")
            lines.append("✓ 关注关键规则验证结果")
        else:
            lines.append("⚠️ 分析结果可靠性较低，建议:")
            lines.append("  • 使用更长的数据周期重新分析")
            lines.append("  • 调整摆动点检测参数")
            lines.append("  • 结合其他技术分析方法")
        lines.append("")

        # 风险提示
        lines.append(f"⚠️ 风险提示")
        lines.append("-" * 40)
        lines.append("• 波浪分析具有一定主观性，需结合其他指标验证")
        lines.append("• 市场风险始终存在，请合理设置止损")
        lines.append("• 本报告仅供参考，不构成投资建议")

        return "\n".join(lines)

    def generate_detailed_analysis(self, result: Dict[str, Any],
                                  detail_level: str = 'standard') -> str:
        """
        生成详细分析内容

        参数:
            result: 分析结果字典
            detail_level: 详细程度 ('standard', 'detailed')

        返回:
            详细分析字符串
        """
        lines = []
        lines.append("=" * 60)
        lines.append("详细分析")
        lines.append("=" * 60)
        lines.append("")

        # 波浪模式详情
        waves = result.get('waves', {})
        impulse_waves = waves.get('impulse', [])
        corrective_waves = waves.get('corrective', [])

        # 5浪推动模式
        if impulse_waves:
            lines.append(f"📊 5浪推动模式详情 (共 {len(impulse_waves)} 个)")
            lines.append("-" * 40)

            for i, impulse in enumerate(impulse_waves, 1):
                lines.append(f"模式 {i}:")

                # 显示各浪价格和日期
                for wave_name in ['Wave1', 'Wave2', 'Wave3', 'Wave4', 'Wave5']:
                    if wave_name in impulse:
                        wave = impulse[wave_name]
                        date_str = wave.get('date', 'N/A')
                        if isinstance(date_str, pd.Timestamp):
                            date_str = date_str.strftime('%Y-%m-%d')
                        lines.append(f"  {wave_name}: 价格={wave['price']:.2f}, 日期={date_str}")

                # 显示质量分数（如果有）
                if 'quality_score' in impulse:
                    lines.append(f"  质量分数: {impulse['quality_score']:.3f}")

                # 显示关键比例（如果有）
                if 'retracement_ratio' in impulse:
                    lines.append(f"  浪2回撤比例: {impulse['retracement_ratio']:.3f}")
                if 'extension_ratio' in impulse:
                    lines.append(f"  浪3扩展比例: {impulse['extension_ratio']:.3f}")

                lines.append("")  # 空行分隔模式

            if detail_level == 'detailed':
                # 详细模式下显示更多统计信息
                if impulse_waves:
                    quality_scores = [imp.get('quality_score', 0) for imp in impulse_waves
                                    if 'quality_score' in imp]
                    if quality_scores:
                        avg_quality = sum(quality_scores) / len(quality_scores)
                        lines.append(f"质量分数统计: 平均={avg_quality:.3f}, 最高={max(quality_scores):.3f}, 最低={min(quality_scores):.3f}")
                        lines.append("")

        # 3浪调整模式
        if corrective_waves:
            lines.append(f"📊 3浪调整模式详情 (共 {len(corrective_waves)} 个)")
            lines.append("-" * 40)

            for i, corrective in enumerate(corrective_waves, 1):
                lines.append(f"模式 {i}:")

                for wave_name in ['WaveA', 'WaveB', 'WaveC']:
                    if wave_name in corrective:
                        wave = corrective[wave_name]
                        date_str = wave.get('date', 'N/A')
                        if isinstance(date_str, pd.Timestamp):
                            date_str = date_str.strftime('%Y-%m-%d')
                        lines.append(f"  {wave_name}: 价格={wave['price']:.2f}, 日期={date_str}")

                lines.append("")

        # 规则验证详情
        validation = result.get('validation', {})
        passed_rules = validation.get('passed_rules', [])
        violations = validation.get('rule_violations', [])

        lines.append(f"✅ 规则验证详情")
        lines.append("-" * 40)

        if passed_rules:
            lines.append("通过的规则:")
            # 去重显示
            unique_passed = list(set(passed_rules))
            for rule in unique_passed[:10]:  # 最多显示10条
                lines.append(f"  ✓ {rule}")
            if len(unique_passed) > 10:
                lines.append(f"  ... 还有 {len(unique_passed) - 10} 条规则通过")
            lines.append("")

        if violations:
            lines.append("违反的规则:")
            unique_violations = list(set(violations))
            for rule in unique_violations[:10]:  # 最多显示10条
                lines.append(f"  ✗ {rule}")
            if len(unique_violations) > 10:
                lines.append(f"  ... 还有 {len(unique_violations) - 10} 条规则违反")
            lines.append("")

        # 斐波那契分析详情
        fibonacci = result.get('fibonacci', {})
        if fibonacci:
            lines.append(f"📐 斐波那契分析详情")
            lines.append("-" * 40)

            if 'retracements' in fibonacci and fibonacci['retracements']:
                lines.append("回撤水平:")
                for wave_name, levels in fibonacci['retracements'].items():
                    lines.append(f"  {wave_name}:")
                    for level_name, price in levels.items():
                        lines.append(f"    {level_name}: {price:.2f}")
                lines.append("")

            if 'extensions' in fibonacci and fibonacci['extensions']:
                lines.append("扩展水平:")
                for wave_name, levels in fibonacci['extensions'].items():
                    lines.append(f"  {wave_name}:")
                    for level_name, price in levels.items():
                        lines.append(f"    {level_name}: {price:.2f}")
                lines.append("")

            if 'projections' in fibonacci and fibonacci['projections']:
                lines.append("目标位预测:")
                for wave_name, projections in fibonacci['projections'].items():
                    lines.append(f"  {wave_name}:")
                    for desc, price in projections.items():
                        lines.append(f"    {desc}: {price:.2f}")
                lines.append("")

        # 摆动点统计
        swing_points = result.get('swing_points', [])
        if swing_points:
            lines.append(f"📍 摆动点统计")
            lines.append("-" * 40)
            lines.append(f"总摆动点数量: {len(swing_points)}")

            # 计算高点和低点数量
            highs = sum(1 for p in swing_points if p.get('type') == 1)
            lows = sum(1 for p in swing_points if p.get('type') == -1)
            lines.append(f"摆动高点: {highs} 个, 摆动低点: {lows} 个")

            if swing_points and detail_level == 'detailed':
                # 显示前5个摆动点
                lines.append("前5个摆动点:")
                for i, point in enumerate(swing_points[:5], 1):
                    point_type = "高点" if point.get('type') == 1 else "低点"
                    date_str = point.get('date', 'N/A')
                    if isinstance(date_str, pd.Timestamp):
                        date_str = date_str.strftime('%Y-%m-%d')
                    lines.append(f"  {i}. {date_str}: {point_type} 价格={point.get('price', 0):.2f}")
            lines.append("")

        return "\n".join(lines)

    def generate_trading_recommendations(self, result: Dict[str, Any]) -> str:
        """
        生成具体交易建议

        参数:
            result: 分析结果字典

        返回:
            交易建议字符串
        """
        lines = []
        lines.append("=" * 60)
        lines.append("交易建议")
        lines.append("=" * 60)
        lines.append("")

        is_valid = result.get('valid', False)
        confidence = result.get('confidence', 0.0) * 100
        fibonacci = result.get('fibonacci', {})
        prediction = result.get('prediction', {})

        # 根据分析结果生成不同级别的建议
        if not is_valid or confidence < 50:
            lines.append("⚠️ 当前分析结果可靠性较低，建议:")
            lines.append("-" * 40)
            lines.append("1. 观望为主，等待更明确的信号")
            lines.append("2. 如果必须交易，使用极小仓位测试")
            lines.append("3. 设置严格止损（建议3-5%）")
            lines.append("4. 关注关键支撑/阻力位突破")
            lines.append("")
            lines.append("📊 关键观察位:")
            if 'retracements' in fibonacci and fibonacci['retracements']:
                for wave_name, levels in fibonacci['retracements'].items():
                    # 显示关键斐波那契水平
                    key_levels = {'38.2%': '短期支撑/阻力', '50.0%': '中期平衡点', '61.8%': '关键黄金分割'}
                    for level, desc in key_levels.items():
                        if level in levels:
                            price = levels[level]
                            lines.append(f"  • {wave_name} {level} ({desc}): {price:.2f}")
            return "\n".join(lines)

        # 可靠分析下的具体建议
        lines.append(f"📈 基于波浪分析的具体建议 (置信度: {confidence:.1f}%)")
        lines.append("-" * 40)

        # 确定趋势方向（简化判断）
        waves = result.get('waves', {})
        impulse_waves = waves.get('impulse', [])

        if impulse_waves:
            # 使用第一个5浪模式判断趋势
            first_impulse = impulse_waves[0]
            if 'Wave1' in first_impulse and 'Wave5' in first_impulse:
                wave1 = first_impulse['Wave1']
                wave5 = first_impulse['Wave5']
                trend = "上涨" if wave5['price'] > wave1['price'] else "下跌"
                lines.append(f"主要趋势: {trend}趋势")

        # 入场建议
        lines.append("")
        lines.append("🎯 入场建议:")

        # 使用斐波那契回撤水平作为入场参考
        if 'retracements' in fibonacci and fibonacci['retracements']:
            for wave_name, levels in fibonacci['retracements'].items():
                lines.append(f"基于{wave_name}回撤:")
                # 推荐关键入场水平
                key_levels = [
                    ('38.2%', '轻度回撤，适合激进入场'),
                    ('50.0%', '平衡点，适合稳健入场'),
                    ('61.8%', '黄金分割，关键支撑/阻力')
                ]
                for level, desc in key_levels:
                    if level in levels:
                        price = levels[level]
                        lines.append(f"  • {level}: {price:.2f} ({desc})")

        # 止损建议
        lines.append("")
        lines.append("🛡️ 止损建议:")
        lines.append("  保守止损: 关键斐波那契水平下方/上方 1-2%")
        lines.append("  激进止损: 最近摆动点下方/上方 0.5-1%")
        lines.append("  移动止损: 价格突破关键水平后上移/下移止损")

        # 目标价位
        lines.append("")
        lines.append("🎯 目标价位:")

        if 'projections' in fibonacci and fibonacci['projections']:
            for wave_name, projections in fibonacci['projections'].items():
                lines.append(f"基于{wave_name}投影:")
                for desc, price in projections.items():
                    lines.append(f"  • {desc}: {price:.2f}")

        # 仓位管理
        lines.append("")
        lines.append("💰 仓位管理建议:")
        lines.append("  初始仓位: 总资金的 5-10%")
        lines.append("  加仓条件: 价格触及关键支撑/阻力后反弹")
        lines.append("  仓位调整: 根据置信度调整，高置信度可适当增加")

        # 风险管理
        lines.append("")
        lines.append("⚠️ 风险管理:")
        lines.append("  最大风险: 单笔交易不超过总资金的 2%")
        lines.append("  风险回报比: 建议至少 1:2")
        lines.append("  持仓时间: 根据波浪周期，通常数天至数周")

        # 特别提醒
        lines.append("")
        lines.append("💡 特别提醒:")
        lines.append("  • 波浪分析需结合成交量确认")
        lines.append("  • 关注宏观经济数据和市场情绪")
        lines.append("  • 严格执行止损纪律")
        lines.append("  • 本建议仅供参考，实际交易需谨慎")

        return "\n".join(lines)

    def save_report(self, result: Dict[str, Any], filepath: str,
                   detail_level: str = 'standard',
                   include_trading_recommendations: bool = True,
                   metadata: Optional[Dict[str, Any]] = None):
        """
        生成并保存报告到文件

        参数:
            result: 分析结果字典
            filepath: 报告文件路径
            detail_level: 报告详细程度
            include_trading_recommendations: 是否包含交易建议
            metadata: 额外元数据，如股票代码、时间范围等
        """
        # 确保目录存在
        save_dir = os.path.dirname(filepath)
        if save_dir and not os.path.exists(save_dir):
            os.makedirs(save_dir, exist_ok=True)
            if self.verbose:
                print(f"[报告保存] 创建目录: {save_dir}")

        # 生成报告
        report_content = self.generate_report(
            result,
            detail_level=detail_level,
            include_trading_recommendations=include_trading_recommendations,
            metadata=metadata
        )

        # 保存到文件
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(report_content)

        if self.verbose:
            print(f"[报告保存] 报告已保存到: {filepath}")
            print(f"[报告保存] 文件大小: {os.path.getsize(filepath)//1024} KB")

    def export_report(self, result: Dict[str, Any], output_format: str = 'txt',
                     detail_level: str = 'standard',
                     include_trading_recommendations: bool = True,
                     metadata: Optional[Dict[str, Any]] = None) -> str:
        """
        导出报告为指定格式

        参数:
            result: 分析结果字典
            output_format: 输出格式 ('txt', 'md', 'html', 'json')
            detail_level: 报告详细程度
            include_trading_recommendations: 是否包含交易建议
            metadata: 额外元数据

        返回:
            格式化后的报告字符串
        """
        if output_format == 'txt':
            return self.generate_report(result, detail_level, include_trading_recommendations, metadata)
        elif output_format == 'md':
            return self._generate_markdown(result, detail_level, include_trading_recommendations, metadata)
        elif output_format == 'html':
            return self._generate_html(result, detail_level, include_trading_recommendations, metadata)
        elif output_format == 'json':
            return self._generate_json(result, detail_level, include_trading_recommendations, metadata)
        else:
            raise ValueError(f"不支持的格式: {output_format}，支持格式: txt, md, html, json")

    def _generate_markdown(self, result: Dict[str, Any], detail_level: str,
                          include_trading_recommendations: bool,
                          metadata: Optional[Dict[str, Any]] = None) -> str:
        """生成Markdown格式报告"""
        # 首先生成文本报告
        text_report = self.generate_report(result, detail_level, include_trading_recommendations, metadata)
        # 简单转换：将分隔线替换为Markdown分隔线，标题添加#
        lines = text_report.split('\n')
        md_lines = []
        for line in lines:
            if line.startswith('=' * 60):
                md_lines.append('# ' + line.replace('=', '').strip())
                md_lines.append('')
            elif line.startswith('-' * 40):
                md_lines.append('## ' + line.replace('-', '').strip())
                md_lines.append('')
            else:
                md_lines.append(line)
        return '\n'.join(md_lines)

    def _generate_html(self, result: Dict[str, Any], detail_level: str,
                      include_trading_recommendations: bool,
                      metadata: Optional[Dict[str, Any]] = None) -> str:
        """生成HTML格式报告"""
        # 简单实现：将文本报告包装在HTML标签中
        text_report = self.generate_report(result, detail_level, include_trading_recommendations, metadata)
        lines = text_report.split('\n')
        html_lines = ['<!DOCTYPE html>',
                      '<html>',
                      '<head>',
                      '<meta charset="utf-8">',
                      '<title>艾略特波浪分析报告</title>',
                      '<style>',
                      'body { font-family: Arial, sans-serif; margin: 40px; }',
                      'h1 { color: #333; border-bottom: 2px solid #333; }',
                      'h2 { color: #666; margin-top: 30px; }',
                      '.section { margin-bottom: 20px; }',
                      '.risk-high { color: #d00; }',
                      '.risk-medium { color: #c60; }',
                      '.risk-low { color: #090; }',
                      '</style>',
                      '</head>',
                      '<body>']
        for line in lines:
            if line.startswith('=' * 60):
                html_lines.append(f'<h1>{line.replace("=", "").strip()}</h1>')
            elif line.startswith('-' * 40):
                html_lines.append(f'<h2>{line.replace("-", "").strip()}</h2>')
            elif line.strip() == '':
                html_lines.append('<br>')
            else:
                html_lines.append(f'<p>{line}</p>')
        html_lines.append('</body>')
        html_lines.append('</html>')
        return '\n'.join(html_lines)

    def _generate_json(self, result: Dict[str, Any], detail_level: str,
                      include_trading_recommendations: bool,
                      metadata: Optional[Dict[str, Any]] = None) -> str:
        """生成JSON格式报告"""
        import json
        # 构建结构化报告
        report_data = {
            'metadata': metadata or {},
            'analysis_result': result,
            'risk_score': self.calculate_risk_score(result),
            'generated_at': datetime.now().isoformat(),
            'detail_level': detail_level,
            'include_trading_recommendations': include_trading_recommendations
        }
        return json.dumps(report_data, ensure_ascii=False, indent=2)

    def generate_multi_timeframe_report(self, results: Dict[str, Dict[str, Any]],
                                       metadata: Optional[Dict[str, Any]] = None) -> str:
        """
        生成多时间框架综合报告

        参数:
            results: 时间框架到分析结果的映射，如 {'D': result_daily, 'W': result_weekly}
            metadata: 额外元数据

        返回:
            综合报告字符串
        """
        lines = []
        lines.append("=" * 60)
        lines.append("多时间框架艾略特波浪分析报告")
        lines.append("=" * 60)
        lines.append(f"生成时间: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
        if metadata:
            if 'stock_code' in metadata:
                lines.append(f"股票代码: {metadata['stock_code']}")
        lines.append("")

        # 各时间框架摘要
        for timeframe, result in results.items():
            lines.append(f"时间框架: {timeframe}")
            lines.append("-" * 40)
            is_valid = result.get('valid', False)
            confidence = result.get('confidence', 0.0) * 100
            lines.append(f"分析有效性: {'✅ 有效' if is_valid else '⚠️ 需要谨慎'}")
            lines.append(f"置信度: {confidence:.1f}%")
            # 波浪统计
            waves = result.get('waves', {})
            impulse_count = len(waves.get('impulse', []))
            corrective_count = len(waves.get('corrective', []))
            lines.append(f"5浪推动模式: {impulse_count} 个, 3浪调整模式: {corrective_count} 个")
            # 风险评分
            risk_score = self.calculate_risk_score(result)
            risk_level = "低风险" if risk_score < 3 else "中风险" if risk_score < 7 else "高风险"
            lines.append(f"风险评分: {risk_score:.1f}/10 ({risk_level})")
            lines.append("")

        # 综合建议
        lines.append("综合建议")
        lines.append("-" * 40)
        # 计算平均风险评分
        risk_scores = [self.calculate_risk_score(r) for r in results.values()]
        avg_risk = sum(risk_scores) / len(risk_scores) if risk_scores else 0
        if avg_risk < 3:
            lines.append("✅ 多时间框架分析显示整体风险较低")
            lines.append("   建议关注主要趋势时间框架的交易机会")
        elif avg_risk < 7:
            lines.append("⚠️ 多时间框架分析显示风险中等")
            lines.append("   建议等待时间框架共振信号，谨慎交易")
        else:
            lines.append("🚨 多时间框架分析显示整体风险较高")
            lines.append("   建议观望，避免盲目交易")
        lines.append("")

        # 时间框架一致性评估
        valid_results = [r for r in results.values() if r.get('valid', False)]
        if len(valid_results) >= 2:
            lines.append("时间框架一致性评估")
            lines.append("-" * 40)
            consistency = len(valid_results) / len(results)
            lines.append(f"一致性分数: {consistency:.0%}")
            if consistency > 0.8:
                lines.append("✅ 各时间框架分析结果高度一致，信号可靠")
            elif consistency > 0.5:
                lines.append("⚠️ 各时间框架分析结果部分一致，需谨慎验证")
            else:
                lines.append("🚨 各时间框架分析结果不一致，建议观望")
            lines.append("")

        return "\n".join(lines)

    def _generate_header(self, result: Dict[str, Any], metadata: Optional[Dict[str, Any]] = None) -> str:
        """生成报告头"""
        lines = []
        lines.append("=" * 60)
        lines.append("艾略特波浪分析报告")
        lines.append("=" * 60)
        lines.append(f"生成时间: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
        if metadata:
            if 'stock_code' in metadata:
                lines.append(f"股票代码: {metadata['stock_code']}")
            if 'start_date' in metadata and 'end_date' in metadata:
                lines.append(f"分析周期: {metadata['start_date']} 至 {metadata['end_date']}")
            if 'timeframe' in metadata:
                lines.append(f"时间框架: {metadata['timeframe']}")
            if 'analyzer_params' in metadata:
                params = metadata['analyzer_params']
                if isinstance(params, dict):
                    lines.append(f"分析参数: {params}")
        lines.append("")
        return "\n".join(lines)

    def _generate_footer(self) -> str:
        """生成报告尾"""
        lines = []
        lines.append("=" * 60)
        lines.append("报告结束")
        lines.append("=" * 60)
        lines.append("重要声明: 本报告基于技术分析生成，仅供参考。")
        lines.append("          市场有风险，投资需谨慎。")
        return "\n".join(lines)


def test_report_generator():
    """测试报告生成器"""
    print("测试报告生成器...")

    # 创建测试数据
    test_result = {
        'valid': True,
        'message': '识别到有效的5浪推动模式',
        'confidence': 0.85,
        'waves': {
            'impulse': [
                {
                    'Wave1': {'price': 100.0, 'date': '2025-01-01', 'type': -1},
                    'Wave2': {'price': 95.0, 'date': '2025-01-10', 'type': 1},
                    'Wave3': {'price': 120.0, 'date': '2025-01-20', 'type': -1},
                    'Wave4': {'price': 115.0, 'date': '2025-01-30', 'type': 1},
                    'Wave5': {'price': 125.0, 'date': '2025-02-05', 'type': -1},
                    'quality_score': 0.8,
                    'retracement_ratio': 0.5,
                    'extension_ratio': 1.6
                }
            ],
            'corrective': [
                {
                    'WaveA': {'price': 125.0, 'date': '2025-02-05', 'type': 1},
                    'WaveB': {'price': 120.0, 'date': '2025-02-10', 'type': -1},
                    'WaveC': {'price': 115.0, 'date': '2025-02-15', 'type': 1}
                }
            ]
        },
        'validation': {
            'valid': True,
            'message': '波浪规则验证通过',
            'passed_rules': ['浪3不是最短的推动浪', '浪4未进入浪1价格区间', '浪2调整深度符合常见模式'],
            'rule_violations': []
        },
        'fibonacci': {
            'retracements': {
                'Wave2': {
                    '23.6%': 118.0,
                    '38.2%': 115.5,
                    '50.0%': 113.0,
                    '61.8%': 110.5,
                    '78.6%': 107.0
                }
            },
            'projections': {
                'Wave5': {
                    '浪5目标 (61.8% 浪1-3)': 130.0,
                    '浪5目标 (100.0% 浪1-3)': 135.0,
                    '浪5目标 (161.8% 浪1-3)': 140.0
                }
            }
        },
        'prediction': {
            'next_wave': 'WaveA (调整开始)',
            'confidence': 0.6,
            'target_prices': [
                {'description': '浪A目标 (38.2%回撤)', 'price': 120.0},
                {'description': '浪A目标 (50.0%回撤)', 'price': 117.5}
            ]
        },
        'swing_points': [
            {'type': 1, 'price': 105.0, 'date': '2025-01-05'},
            {'type': -1, 'price': 95.0, 'date': '2025-01-10'},
            {'type': 1, 'price': 120.0, 'date': '2025-01-20'}
        ]
    }

    # 创建报告生成器
    generator = ReportGenerator(verbose=True)

    # 测试不同详细程度的报告
    for detail_level in ['brief', 'standard', 'detailed']:
        print(f"\n{'='*60}")
        print(f"测试 {detail_level} 详细程度报告")
        print(f"{'='*60}")

        report = generator.generate_report(
            test_result,
            detail_level=detail_level,
            include_trading_recommendations=True
        )

        print(f"报告长度: {len(report)} 字符")
        # 跳过预览打印以避免编码问题
        # print(f"报告预览 (前500字符):")
        # print(report[:500] + "...")

    # 测试保存报告
    test_dir = 'test_output'
    os.makedirs(test_dir, exist_ok=True)

    test_file = os.path.join(test_dir, 'test_report.txt')
    generator.save_report(test_result, test_file, detail_level='standard')

    # 测试风险评分
    print(f"\n{'='*60}")
    print(f"测试风险评分功能")
    print(f"{'='*60}")
    risk_score = generator.calculate_risk_score(test_result)
    print(f"风险评分: {risk_score:.1f}/10")

    # 测试多格式输出
    print(f"\n{'='*60}")
    print(f"测试多格式输出")
    print(f"{'='*60}")
    for fmt in ['txt', 'md', 'html', 'json']:
        try:
            report = generator.export_report(test_result, output_format=fmt, detail_level='standard')
            print(f"格式 {fmt}: 长度 {len(report)} 字符")
            if fmt == 'json':
                print(f"  预览: {report[:200]}...")
        except Exception as e:
            print(f"格式 {fmt} 失败: {e}")

    # 测试多时间框架报告
    print(f"\n{'='*60}")
    print(f"测试多时间框架报告")
    print(f"{'='*60}")
    multi_results = {
        '日线': test_result,
        '周线': test_result,  # 使用相同测试数据
        '60分钟': test_result
    }
    multi_report = generator.generate_multi_timeframe_report(multi_results)
    print(f"多时间框架报告长度: {len(multi_report)} 字符")
    # print(f"预览 (前300字符):")
    # print(multi_report[:300] + "...")

    print(f"\n测试完成!")
    print(f"报告已保存到: {test_file}")
    print(f"文件大小: {os.path.getsize(test_file)//1024} KB")


if __name__ == "__main__":
    test_report_generator()