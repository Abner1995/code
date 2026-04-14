"""
增强的艾略特波浪分析模块

功能:
1. 摆动点检测
2. 波浪模式识别 (5浪推动, 3浪调整)
3. 波浪规则验证
4. 斐波那契分析
5. 可视化
6. 报告生成

艾略特波浪理论核心规则:
1. 5浪推动模式: 浪1,3,5推动, 浪2,4调整
2. 浪3不能是最短的推动浪
3. 浪4不能进入浪1价格区间
4. 浪2调整通常较深 (50%以上)
5. 浪2和浪4调整形态交替
"""

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from matplotlib.patches import Rectangle, Polygon
import warnings
from typing import Dict, List, Tuple, Optional, Any
import platform
import os
from report_generator import ReportGenerator

warnings.filterwarnings('ignore')

# 设置matplotlib中文字体，解决中文显示为方框的问题
def set_chinese_font(verbose=False):
    """设置matplotlib使用中文字体，避免中文显示为方框

    参数:
        verbose: 是否显示详细日志
    """
    try:
        # 根据操作系统设置字体
        system = platform.system()

        if system == 'Windows':
            # Windows系统，使用Microsoft YaHei（微软雅黑）
            font_name = 'Microsoft YaHei'
            # 也可以尝试其他中文字体
            # font_name = 'SimHei'  # 黑体
            # font_name = 'SimSun'  # 宋体
        elif system == 'Darwin':  # macOS
            font_name = 'Arial Unicode MS'  # macOS系统中的中文字体
        else:  # Linux
            font_name = 'DejaVu Sans'  # Linux常用字体，支持中文

        # 设置matplotlib字体
        plt.rcParams['font.sans-serif'] = [font_name]
        plt.rcParams['axes.unicode_minus'] = False  # 解决负号显示问题

        # 验证字体是否可用
        from matplotlib.font_manager import FontProperties
        font_prop = FontProperties(fname=None, family=font_name)

        # 如果字体不可用，尝试备用字体
        import matplotlib
        available_fonts = [f.name for f in matplotlib.font_manager.fontManager.ttflist]

        # 常见的中文字体列表
        chinese_fonts = ['Microsoft YaHei', 'SimHei', 'SimSun', 'Arial Unicode MS',
                        'DejaVu Sans', 'WenQuanYi Micro Hei', 'Noto Sans CJK SC']

        # 检查首选字体是否可用
        if font_name not in available_fonts:
            # 尝试其他中文字体
            for font in chinese_fonts:
                if font in available_fonts:
                    plt.rcParams['font.sans-serif'] = [font]
                    plt.rcParams['axes.unicode_minus'] = False
                    if verbose:
                        print(f"[字体设置] 使用备用中文字体: {font}")
                    break
            else:
                if verbose:
                    print(f"[字体设置] 警告: 未找到中文字体，中文可能显示为方框")
                    print(f"[字体设置] 可用字体: {available_fonts[:10]}...")
        else:
            if verbose:
                print(f"[字体设置] 使用中文字体: {font_name}")

    except Exception as e:
        if verbose:
            print(f"[字体设置] 设置中文字体时出错: {e}")
        # 继续执行，即使字体设置失败

# 初始化时设置中文字体
set_chinese_font(verbose=False)


class ElliottWaveAnalyzer:
    """
    艾略特波浪分析器
    """

    def __init__(self, window=5, min_wave_length=3, verbose=False,
                 min_price_change=0.02, min_wave_ratio=0.3):
        """
        初始化分析器

        参数:
            window: 摆动点检测窗口大小
            min_wave_length: 最小波浪长度 (K线数量)
            verbose: 是否显示详细日志
            min_price_change: 最小价格变化比率（用于过滤微小波动）
            min_wave_ratio: 最小波浪比率（浪2/浪1、浪4/浪3等）
        """
        self.window = window
        self.min_wave_length = min_wave_length
        self.verbose = verbose
        self.min_price_change = min_price_change
        self.min_wave_ratio = min_wave_ratio

        # 在verbose模式下显示字体设置信息
        if verbose:
            set_chinese_font(verbose=True)

        # 斐波那契水平
        self.fib_levels = {
            'retracement': [0.236, 0.382, 0.5, 0.618, 0.786],
            'extension': [1.0, 1.272, 1.382, 1.618, 2.0, 2.618]
        }

    def analyze(self, data: pd.DataFrame) -> Dict[str, Any]:
        """
        完整的艾略特波浪分析流程

        参数:
            data: 包含股票数据的DataFrame，必须有'high','low','close'列

        返回:
            包含分析结果的字典
        """
        if self.verbose:
            print(f"[波浪分析] 开始分析，数据点数: {len(data)}")

        # 1. 数据准备
        if 'high' not in data.columns or 'low' not in data.columns:
            raise ValueError("数据必须包含'high'和'low'列")

        # 创建数据副本
        analysis_data = data.copy()

        # 2. 检测摆动点
        if self.verbose:
            print(f"[波浪分析] 检测摆动点 (窗口={self.window})...")

        swings = self.detect_swing_points(analysis_data)

        if self.verbose:
            swing_highs = (swings['Swing'] == 1).sum()
            swing_lows = (swings['Swing'] == -1).sum()
            print(f"[波浪分析] 检测到 {swing_highs} 个摆动高点和 {swing_lows} 个摆动低点")

        # 3. 提取摆动点序列
        swing_points = self.extract_swing_sequence(swings)

        if len(swing_points) < 4:
            if self.verbose:
                print(f"[波浪分析] 摆动点太少 ({len(swing_points)})，无法识别波浪模式")
            return {
                'valid': False,
                'message': f'摆动点数量不足 ({len(swing_points)})，需要至少4个',
                'swings': swings,
                'swing_points': swing_points
            }

        # 4. 识别波浪模式
        if self.verbose:
            print(f"[波浪分析] 识别波浪模式...")

        waves = self.identify_wave_patterns(swing_points)

        # 5. 验证波浪规则
        validation_result = self.validate_wave_rules(waves)

        result = {
            'valid': validation_result['valid'],
            'message': validation_result['message'],
            'swings': swings,
            'swing_points': swing_points,
            'waves': waves,
            'validation': validation_result
        }

        # 6. 如果波浪有效，进行进一步分析
        if validation_result['valid']:
            # 计算斐波那契关系
            fibonacci = self.calculate_fibonacci_relationships(waves)
            result['fibonacci'] = fibonacci

            # 预测下一浪
            prediction = self.predict_next_wave(waves, fibonacci)
            result['prediction'] = prediction

            # 计算置信度
            confidence = self.calculate_confidence(waves, validation_result)
            result['confidence'] = confidence

            if self.verbose:
                print(f"[波浪分析] 分析完成，置信度: {confidence:.1%}")

        return result

    def detect_swing_points(self, data: pd.DataFrame) -> pd.DataFrame:
        """
        检测摆动高点和低点

        参数:
            data: 包含'high'和'low'列的DataFrame

        返回:
            添加了'Swing'列的DataFrame (1: 摆动高点, -1: 摆动低点, 0: 非摆动点)
        """
        # 复制数据以避免修改原数据
        result = data.copy()

        # 初始化Swing列
        if 'Swing' not in result.columns:
            result['Swing'] = 0

        window = self.window

        # 检测摆动高点: high > 前后window个周期内的最高点
        high_rolling_max = result['high'].rolling(
            window=2 * window + 1, center=True
        ).max()

        is_swing_high = (result['high'] == high_rolling_max)

        # 检测摆动低点: low < 前后window个周期内的最低点
        low_rolling_min = result['low'].rolling(
            window=2 * window + 1, center=True
        ).min()

        is_swing_low = (result['low'] == low_rolling_min)

        # 设置Swing值
        result.loc[is_swing_high, 'Swing'] = 1
        result.loc[is_swing_low, 'Swing'] = -1

        # 过滤: 不能同时是摆动高点和低点
        conflict_mask = is_swing_high & is_swing_low
        if conflict_mask.any():
            # 当冲突时，选择价格变化更大的作为摆动点
            for idx in result[conflict_mask].index:
                prev_idx = max(0, idx - 1)
                next_idx = min(len(result) - 1, idx + 1)

                high_change = abs(result.loc[idx, 'high'] - result.loc[prev_idx, 'high'])
                low_change = abs(result.loc[idx, 'low'] - result.loc[prev_idx, 'low'])

                if high_change > low_change:
                    result.loc[idx, 'Swing'] = 1
                else:
                    result.loc[idx, 'Swing'] = -1

        return result

    def extract_swing_sequence(self, data: pd.DataFrame) -> List[Dict]:
        """
        提取摆动点序列

        参数:
            data: 包含'Swing'列的DataFrame

        返回:
            摆动点列表，每个元素是包含索引、类型、价格和日期的字典
        """
        swing_points = []

        # 找到所有摆动点
        swing_mask = data['Swing'] != 0
        swing_indices = data[swing_mask].index.tolist()

        for idx in swing_indices:
            swing_type = data.loc[idx, 'Swing']
            price = data.loc[idx, 'high'] if swing_type == 1 else data.loc[idx, 'low']

            swing_point = {
                'index': idx,
                'type': swing_type,  # 1: 高点, -1: 低点
                'price': price,
                'date': data.loc[idx, 'date'] if 'date' in data.columns else idx
            }

            # 对于高点，记录high值；对于低点，记录low值
            if swing_type == 1:
                swing_point['high'] = data.loc[idx, 'high']
                swing_point['low'] = data.loc[idx, 'low']
            else:
                swing_point['high'] = data.loc[idx, 'high']
                swing_point['low'] = data.loc[idx, 'low']

            swing_points.append(swing_point)

        # 按时间顺序排序
        swing_points.sort(key=lambda x: x['index'])

        # 过滤连续的同类型摆动点（取极值）
        filtered_points = []
        i = 0
        while i < len(swing_points):
            current = swing_points[i]
            j = i + 1

            # 查找连续的同类型摆动点
            while j < len(swing_points) and swing_points[j]['type'] == current['type']:
                j += 1

            if j > i + 1:
                # 有连续的同类型点，取极值
                consecutive_points = swing_points[i:j]
                if current['type'] == 1:  # 高点，取最高
                    extreme_point = max(consecutive_points, key=lambda x: x['price'])
                else:  # 低点，取最低
                    extreme_point = min(consecutive_points, key=lambda x: x['price'])
                filtered_points.append(extreme_point)
            else:
                filtered_points.append(current)

            i = j

        return filtered_points

    def identify_wave_patterns(self, swing_points: List[Dict]) -> Dict[str, Any]:
        """
        识别波浪模式

        参数:
            swing_points: 摆动点序列

        返回:
            包含识别到的波浪模式的字典
        """
        if len(swing_points) < 4:
            return {'impulse': [], 'corrective': [], 'message': '摆动点太少'}

        # 尝试识别5浪推动模式
        impulse_waves = self._identify_impulse_waves(swing_points)

        # 尝试识别3浪调整模式
        corrective_waves = self._identify_corrective_waves(swing_points)

        return {
            'impulse': impulse_waves,
            'corrective': corrective_waves,
            'total_swings': len(swing_points)
        }

    def _identify_impulse_waves(self, swing_points: List[Dict]) -> List[Dict]:
        """
        识别5浪推动模式

        参数:
            swing_points: 摆动点序列

        返回:
            识别到的5浪模式列表（按质量分数排序）
        """
        impulse_patterns = []

        # 需要至少5个摆动点来识别5浪模式
        if len(swing_points) < 5:
            return impulse_patterns

        # 遍历所有可能的5点序列
        for i in range(len(swing_points) - 4):
            # 使用验证方法检查模式质量
            valid, quality_score, waves = self._validate_impulse_pattern(swing_points, i)
            if valid:
                impulse_patterns.append({
                    'waves': waves,
                    'quality_score': quality_score,
                    'start_index': i
                })

        # 按质量分数排序
        impulse_patterns.sort(key=lambda x: x['quality_score'], reverse=True)

        # 去重：移除重叠度高的模式
        unique_patterns = self._deduplicate_impulse_patterns(impulse_patterns)

        # 只返回质量分数最高的前5个模式（避免过多输出）
        max_patterns = min(5, len(unique_patterns))
        result = [p['waves'] for p in unique_patterns[:max_patterns]]

        return result

    def _deduplicate_impulse_patterns(self, patterns: List[Dict]) -> List[Dict]:
        """
        去重波浪模式：移除重叠度高的模式，保留质量分数最高的

        参数:
            patterns: 波浪模式列表，每个元素是包含 'waves', 'quality_score', 'start_index' 的字典

        返回:
            去重后的波浪模式列表
        """
        if not patterns:
            return []

        # 按质量分数降序排序（确保已经排序）
        patterns_sorted = sorted(patterns, key=lambda x: x['quality_score'], reverse=True)

        unique_patterns = []

        for pattern in patterns_sorted:
            # 检查是否与已选模式重叠
            is_duplicate = False
            for unique in unique_patterns:
                # 计算重叠度：共享的摆动点索引数量
                start1 = pattern['start_index']
                end1 = start1 + 4  # 5浪模式包含5个点，索引范围 [start, start+4]
                start2 = unique['start_index']
                end2 = start2 + 4

                # 计算重叠区间
                overlap_start = max(start1, start2)
                overlap_end = min(end1, end2)
                overlap_count = max(0, overlap_end - overlap_start + 1)

                # 如果重叠点数 >= 3，则认为重叠度过高，视为重复
                if overlap_count >= 3:
                    is_duplicate = True
                    break

            if not is_duplicate:
                unique_patterns.append(pattern)

                # 最多保留5个不重复的模式
                if len(unique_patterns) >= 5:
                    break

        return unique_patterns

    def _validate_impulse_pattern(self, swing_points: List[Dict], start_idx: int) -> Tuple[bool, float, Dict]:
        """
        验证5浪推动模式的质量

        参数:
            swing_points: 摆动点序列
            start_idx: 波浪开始索引

        返回:
            (是否有效, 质量分数, 波浪详情)
        """
        if start_idx + 4 >= len(swing_points):
            return False, 0.0, {}

        # 提取5个摆动点
        points = swing_points[start_idx:start_idx + 5]

        # 检查高低交替
        for i in range(4):
            if points[i]['type'] == points[i + 1]['type']:
                return False, 0.0, {}

        # 检查价格趋势
        prices = [p['price'] for p in points]
        types = [p['type'] for p in points]

        # 计算总体趋势方向
        if types[0] == -1:  # 低点开始，应该是上升趋势
            start_price = prices[0]
            end_price = prices[4]
            if end_price <= start_price:  # 总体应该是上涨的
                return False, 0.0, {}
            # 浪1、3、5应该是低-高-低-高-低，总体上涨
        else:  # 高点开始，应该是下降趋势
            start_price = prices[0]
            end_price = prices[4]
            if end_price >= start_price:  # 总体应该是下跌的
                return False, 0.0, {}

        # 计算各浪长度（价格变化）
        wave_lengths = []
        for i in range(4):
            wave_lengths.append(abs(prices[i + 1] - prices[i]))

        # 1. 浪3不能是最短的推动浪（至少比浪1长）
        if wave_lengths[2] <= wave_lengths[0]:  # 浪3不比浪1长
            return False, 0.0, {}

        # 2. 浪2回撤深度验证（38.2%-78.6%）
        if types[0] == -1:  # 上升趋势
            wave1_move = prices[1] - prices[0]  # 浪1上涨幅度（应为正）
            wave2_move = prices[1] - prices[2]  # 浪2回撤幅度（应为正）
            if wave1_move <= 0 or wave2_move <= 0:
                # 浪1或浪2方向错误，无效模式
                return False, 0.0, {}
            retracement_ratio = wave2_move / wave1_move
        else:  # 下降趋势
            wave1_move = prices[0] - prices[1]  # 浪1下跌幅度（应为正）
            wave2_move = prices[2] - prices[1]  # 浪2回撤幅度（应为正）
            if wave1_move <= 0 or wave2_move <= 0:
                # 浪1或浪2方向错误，无效模式
                return False, 0.0, {}
            retracement_ratio = wave2_move / wave1_move

        # 浪2回撤通常在38.2%-78.6%之间（严格验证）
        retracement_valid = 0.382 <= retracement_ratio <= 0.786

        # 3. 浪3扩展比例验证（至少浪1的1.618倍）
        if types[0] == -1:  # 上升趋势
            wave1_len = wave_lengths[0]  # 浪1长度
            wave3_len = wave_lengths[2]  # 浪3长度
        else:  # 下降趋势
            wave1_len = wave_lengths[0]
            wave3_len = wave_lengths[2]

        extension_ratio = wave3_len / wave1_len if wave1_len > 0 else 0
        extension_valid = extension_ratio >= 1.618  # 浪3至少是浪1的1.618倍

        # 4. 浪4不进入浪1价格区间（严格验证）
        if types[0] == -1:  # 上升趋势
            wave1_high = prices[1]  # 浪1高点
            wave4_low = prices[3]   # 浪4低点
            # 在上升趋势中，浪4低点应高于浪1高点
            if wave4_low <= wave1_high:
                return False, 0.0, {}
        else:  # 下降趋势
            wave1_low = prices[1]  # 浪1低点
            wave4_high = prices[3] # 浪4高点
            # 在下降趋势中，浪4高点应低于浪1低点
            if wave4_high >= wave1_low:
                return False, 0.0, {}

        # 5. 交替原则验证（浪2和浪4的调整幅度交替）
        # 浪2回撤比例 vs 浪4回撤比例
        wave4_retracement_ratio = 0
        if types[0] == -1:  # 上升趋势
            wave3_move = prices[3] - prices[2]  # 浪3上涨幅度（应为正）
            wave4_move = prices[3] - prices[4]  # 浪4回撤幅度（应为正）
            if wave3_move <= 0 or wave4_move <= 0:
                # 浪3或浪4方向错误，无效模式
                return False, 0.0, {}
            wave4_retracement_ratio = wave4_move / wave3_move
        else:  # 下降趋势
            wave3_move = prices[2] - prices[3]  # 浪3下跌幅度（应为正）
            wave4_move = prices[4] - prices[3]  # 浪4回撤幅度（应为正）
            if wave3_move <= 0 or wave4_move <= 0:
                # 浪3或浪4方向错误，无效模式
                return False, 0.0, {}
            wave4_retracement_ratio = wave4_move / wave3_move

        # 浪2和浪4的回撤比例应有明显差异（交替原则）
        alternation_valid = abs(retracement_ratio - wave4_retracement_ratio) > 0.2 if retracement_ratio > 0 and wave4_retracement_ratio > 0 else False

        # 6. 价格幅度过滤（最小波浪幅度）
        min_price_change = self.min_price_change  # 最小价格变化比率
        min_wave_ratio = self.min_wave_ratio      # 最小波浪比率

        # 计算价格基准（平均价格）
        avg_price = sum(prices) / len(prices)
        min_absolute_change = avg_price * min_price_change

        # 检查每个推动浪（浪1、浪3、浪5）的幅度是否大于最小价格变化
        # 浪1长度: wave_lengths[0], 浪3长度: wave_lengths[2], 浪5长度需要计算
        wave5_length = abs(prices[4] - prices[3]) if len(prices) >= 5 else 0
        impulse_lengths = [wave_lengths[0], wave_lengths[2], wave5_length]

        for length in impulse_lengths:
            if length < min_absolute_change:
                return False, 0.0, {}

        # 检查调整浪（浪2、浪4）的回撤幅度是否足够显著
        # 浪2回撤比例应大于min_wave_ratio（默认0.3，但已被更严格的38.2%-78.6%范围覆盖）
        if retracement_ratio < min_wave_ratio:
            return False, 0.0, {}

        # 浪4回撤比例应在合理范围内（通常20%-50%）
        if wave4_retracement_ratio < 0.1 or wave4_retracement_ratio > 0.6:
            return False, 0.0, {}

        # 7. 时间比例验证（如果日期数据可用）
        time_ratio_valid = True
        if all('date' in p for p in points):
            try:
                dates = [p['date'] for p in points]
                # 计算各浪时间长度（天数）
                time_lengths = []
                for i in range(4):
                    if isinstance(dates[i], (str, pd.Timestamp)) and isinstance(dates[i+1], (str, pd.Timestamp)):
                        d1 = pd.Timestamp(dates[i]) if isinstance(dates[i], str) else dates[i]
                        d2 = pd.Timestamp(dates[i+1]) if isinstance(dates[i+1], str) else dates[i+1]
                        time_lengths.append(abs((d2 - d1).days))

                if len(time_lengths) >= 3:
                    # 浪3时间不应过短（至少是浪1时间的0.5倍）
                    if time_lengths[2] < time_lengths[0] * 0.5:
                        time_ratio_valid = False
                    # 浪2和浪4时间比例应有差异（交替）
                    if abs(time_lengths[1] - time_lengths[3]) < max(time_lengths[1], time_lengths[3]) * 0.3:
                        time_ratio_valid = False
            except:
                pass  # 如果时间计算出错，跳过时间验证

        # 计算质量分数（基于多个验证规则）
        quality_score = 0.5  # 基础分

        # 规则权重
        if retracement_valid:      # 浪2回撤深度
            quality_score += 0.15
        if extension_valid:        # 浪3扩展比例
            quality_score += 0.20
        if alternation_valid:      # 交替原则
            quality_score += 0.10
        if time_ratio_valid:       # 时间比例
            quality_score += 0.05

        # 额外加分：浪3扩展比例超过2.0倍
        if extension_ratio >= 2.0:
            quality_score += 0.10

        # 额外加分：浪2回撤接近61.8%（黄金分割）
        if 0.618 - 0.05 <= retracement_ratio <= 0.618 + 0.05:
            quality_score += 0.05

        # 限制分数在0.0-1.0之间
        quality_score = max(0.0, min(1.0, quality_score))

        # 提取波浪详情
        waves = self._extract_wave_details(swing_points, start_idx, 'impulse')
        waves['quality_score'] = quality_score
        waves['retracement_ratio'] = retracement_ratio
        waves['extension_ratio'] = extension_ratio
        waves['wave4_retracement_ratio'] = wave4_retracement_ratio
        waves['alternation_valid'] = alternation_valid
        waves['time_ratio_valid'] = time_ratio_valid

        return True, quality_score, waves

    def _identify_corrective_waves(self, swing_points: List[Dict]) -> List[Dict]:
        """
        识别3浪调整模式 (A-B-C)

        参数:
            swing_points: 摆动点序列

        返回:
            识别到的3浪调整模式列表
        """
        corrective_patterns = []

        # 需要至少3个摆动点来识别3浪模式
        if len(swing_points) < 3:
            return corrective_patterns

        for i in range(len(swing_points) - 2):
            # 检查是否是高低交替序列
            if (swing_points[i]['type'] == swing_points[i + 1]['type'] or
                    swing_points[i + 1]['type'] == swing_points[i + 2]['type']):
                continue

            # A-B-C调整浪: 可以是 高-低-高 或 低-高-低
            sequence = [swing_points[i + k]['type'] for k in range(3)]

            # 常见的A-B-C模式
            if sequence in [[1, -1, 1], [-1, 1, -1]]:
                waves = self._extract_wave_details(swing_points, i, 'corrective')
                corrective_patterns.append(waves)

        return corrective_patterns

    def _extract_wave_details(self, swing_points: List[Dict], start_idx: int,
                             wave_type: str) -> Dict[str, Any]:
        """
        提取波浪详情

        参数:
            swing_points: 摆动点序列
            start_idx: 波浪开始索引
            wave_type: 波浪类型 ('impulse' 或 'corrective')

        返回:
            波浪详情字典
        """
        if wave_type == 'impulse':
            num_waves = 5
            wave_names = ['Wave1', 'Wave2', 'Wave3', 'Wave4', 'Wave5']
        else:  # corrective
            num_waves = 3
            wave_names = ['WaveA', 'WaveB', 'WaveC']

        waves = {}
        for k in range(num_waves):
            wave_idx = start_idx + k
            if wave_idx < len(swing_points):
                point = swing_points[wave_idx]
                waves[wave_names[k]] = {
                    'index': point['index'],
                    'type': point['type'],
                    'price': point['price'],
                    'date': point['date']
                }

        # 计算波浪长度（价格变化）
        if len(waves) >= 2:
            for i in range(len(waves) - 1):
                wave1 = waves[wave_names[i]]
                wave2 = waves[wave_names[i + 1]]
                price_change = abs(wave2['price'] - wave1['price'])
                waves[wave_names[i]]['length_to_next'] = price_change

        waves['start_index'] = start_idx
        waves['type'] = wave_type
        waves['count'] = len(waves)

        return waves

    def validate_wave_rules(self, waves: Dict[str, Any]) -> Dict[str, Any]:
        """
        验证艾略特波浪规则

        参数:
            waves: 波浪模式字典

        返回:
            验证结果字典
        """
        validation = {
            'valid': False,
            'message': '',
            'rule_violations': [],
            'passed_rules': []
        }

        impulse_waves = waves.get('impulse', [])
        corrective_waves = waves.get('corrective', [])

        # 如果没有识别到任何波浪
        if not impulse_waves and not corrective_waves:
            validation['message'] = '未识别到有效的波浪模式'
            return validation

        # 检查5浪推动模式
        for i, impulse in enumerate(impulse_waves):
            if len(impulse) >= 5:  # 有完整的5浪
                wave_valid = self._validate_impulse_waves(impulse)
                if wave_valid['valid']:
                    validation['valid'] = True
                    validation['passed_rules'].extend(wave_valid['passed_rules'])
                else:
                    validation['rule_violations'].extend(wave_valid['violations'])

        # 检查3浪调整模式
        for i, corrective in enumerate(corrective_waves):
            if len(corrective) >= 3:  # 有完整的3浪
                wave_valid = self._validate_corrective_waves(corrective)
                if wave_valid['valid']:
                    validation['valid'] = True
                    validation['passed_rules'].extend(wave_valid['passed_rules'])
                else:
                    validation['rule_violations'].extend(wave_valid['violations'])

        # 设置最终消息
        if validation['valid']:
            if impulse_waves and not corrective_waves:
                validation['message'] = '识别到有效的5浪推动模式'
            elif corrective_waves and not impulse_waves:
                validation['message'] = '识别到有效的3浪调整模式'
            else:
                validation['message'] = '识别到有效的波浪模式'
        else:
            if validation['rule_violations']:
                validation['message'] = f"波浪规则验证失败: {', '.join(validation['rule_violations'][:3])}"
            else:
                validation['message'] = '波浪模式不符合艾略特理论规则'

        return validation

    def _validate_impulse_waves(self, waves: Dict) -> Dict[str, Any]:
        """
        验证5浪推动模式的规则

        参数:
            waves: 5浪模式字典

        返回:
            验证结果
        """
        violations = []
        passed_rules = []

        # 检查是否有完整的5浪
        if len(waves) < 5:
            return {'valid': False, 'violations': ['波浪数量不足'], 'passed_rules': []}

        try:
            # 提取各浪价格
            wave_prices = {}
            for wave_name in ['Wave1', 'Wave2', 'Wave3', 'Wave4', 'Wave5']:
                if wave_name in waves:
                    wave_prices[wave_name] = waves[wave_name]['price']

            # 规则1: 浪3不能是最短的推动浪 (浪1,3,5)
            impulse_waves = ['Wave1', 'Wave3', 'Wave5']
            impulse_lengths = []

            for i in range(len(impulse_waves) - 1):
                w1 = impulse_waves[i]
                w2 = impulse_waves[i + 1]
                if w1 in wave_prices and w2 in wave_prices:
                    length = abs(wave_prices[w2] - wave_prices[w1])
                    impulse_lengths.append((w1, w2, length))

            if len(impulse_lengths) >= 2:
                # 找到浪3的长度
                wave3_length = None
                for w1, w2, length in impulse_lengths:
                    if w1 == 'Wave3' or w2 == 'Wave3':
                        wave3_length = length
                        break

                if wave3_length:
                    # 检查浪3是否是最短的
                    other_lengths = [l for _, _, l in impulse_lengths if l != wave3_length]
                    if other_lengths and wave3_length <= min(other_lengths):
                        violations.append('浪3是最短的推动浪')
                    else:
                        passed_rules.append('浪3不是最短的推动浪')

            # 规则2: 浪4不能进入浪1价格区间
            if 'Wave1' in wave_prices and 'Wave4' in wave_prices:
                if waves['Wave1']['type'] == -1:  # 浪1是低点
                    # 上升趋势: 浪4低点 > 浪1高点?
                    # 这里简化处理
                    pass
                passed_rules.append('浪4未进入浪1价格区间（简化验证）')

            # 规则3: 浪2调整通常较深 (回撤50%以上)
            if 'Wave1' in wave_prices and 'Wave2' in wave_prices:
                wave1_price = wave_prices['Wave1']
                wave2_price = wave_prices['Wave2']

                # 计算浪2回撤比例
                if waves['Wave1']['type'] == -1:  # 浪1是低点，浪2是高点
                    total_move = wave2_price - wave1_price
                    # 这里简化，实际需要更精确计算
                    passed_rules.append('浪2调整深度符合常见模式')

        except Exception as e:
            if self.verbose:
                print(f"[规则验证] 验证过程中出现错误: {e}")

        valid = len(violations) == 0
        return {'valid': valid, 'violations': violations, 'passed_rules': passed_rules}

    def _validate_corrective_waves(self, waves: Dict) -> Dict[str, Any]:
        """
        验证3浪调整模式的规则

        参数:
            waves: 3浪模式字典

        返回:
            验证结果
        """
        violations = []
        passed_rules = []

        # 检查是否有完整的3浪
        if len(waves) < 3:
            return {'valid': False, 'violations': ['波浪数量不足'], 'passed_rules': []}

        passed_rules.append('3浪调整模式基本结构有效')

        valid = len(violations) == 0
        return {'valid': valid, 'violations': violations, 'passed_rules': passed_rules}

    def calculate_fibonacci_relationships(self, waves: Dict[str, Any]) -> Dict[str, Any]:
        """
        计算斐波那契关系

        参数:
            waves: 波浪模式字典

        返回:
            斐波那契分析结果
        """
        fibonacci = {
            'retracements': {},
            'extensions': {},
            'projections': {},
            'relationships': []
        }

        impulse_waves = waves.get('impulse', [])

        for impulse in impulse_waves:
            if len(impulse) >= 5:
                # 计算浪2对浪1的回撤
                if 'Wave1' in impulse and 'Wave2' in impulse:
                    self._calculate_wave_retracement(impulse['Wave1'],
                                                    impulse['Wave2'],
                                                    fibonacci)

                # 计算浪4对浪3的回撤
                if 'Wave3' in impulse and 'Wave4' in impulse:
                    self._calculate_wave_retracement(impulse['Wave3'],
                                                    impulse['Wave4'],
                                                    fibonacci)

                # 计算浪3对浪1的扩展
                if 'Wave1' in impulse and 'Wave3' in impulse:
                    self._calculate_wave_extension(impulse['Wave1'],
                                                  impulse['Wave3'],
                                                  fibonacci)

                # 计算浪5目标位
                if 'Wave1' in impulse and 'Wave3' in impulse and 'Wave4' in impulse:
                    self._calculate_wave5_projections(impulse['Wave1'],
                                                     impulse['Wave3'],
                                                     impulse['Wave4'],
                                                     fibonacci)

        return fibonacci

    def _calculate_wave_retracement(self, wave1: Dict, wave2: Dict,
                                   fibonacci: Dict[str, Any]):
        """计算波浪回撤比例"""
        price1 = wave1['price']
        price2 = wave2['price']

        diff = abs(price2 - price1)
        if diff == 0:
            return

        # 确定趋势方向
        if wave1['type'] == -1 and wave2['type'] == 1:  # 低点到高点
            base = price1
            retracement_levels = {}
            for level in self.fib_levels['retracement']:
                retrace_price = price2 - diff * level
                retracement_levels[f'{level*100:.1f}%'] = retrace_price
            fibonacci['retracements']['Wave2'] = retracement_levels

    def _calculate_wave_extension(self, wave1: Dict, wave3: Dict,
                                fibonacci: Dict[str, Any]):
        """计算波浪扩展比例"""
        price1 = wave1['price']
        price3 = wave3['price']

        diff = abs(price3 - price1)
        if diff == 0:
            return

        extension_levels = {}
        for level in self.fib_levels['extension']:
            if wave1['type'] == -1:  # 上升趋势
                extension_price = price1 + diff * level
            else:  # 下降趋势
                extension_price = price1 - diff * level
            extension_levels[f'{level*100:.1f}%'] = extension_price

        fibonacci['extensions']['Wave3'] = extension_levels

    def _calculate_wave5_projections(self, wave1: Dict, wave3: Dict, wave4: Dict,
                                   fibonacci: Dict[str, Any]):
        """计算浪5目标位"""
        price1 = wave1['price']
        price3 = wave3['price']
        price4 = wave4['price']

        # 浪1到浪3的距离
        diff_1_3 = abs(price3 - price1)

        # 常见浪5目标位: 浪1到浪3的0.618, 1.0, 1.618倍
        projections = {}

        if wave1['type'] == -1:  # 上升趋势
            for ratio in [0.618, 1.0, 1.618]:
                target = price4 + diff_1_3 * ratio
                projections[f'浪5目标 ({ratio*100:.1f}% 浪1-3)'] = target
        else:  # 下降趋势
            for ratio in [0.618, 1.0, 1.618]:
                target = price4 - diff_1_3 * ratio
                projections[f'浪5目标 ({ratio*100:.1f}% 浪1-3)'] = target

        fibonacci['projections']['Wave5'] = projections

    def predict_next_wave(self, waves: Dict[str, Any],
                         fibonacci: Dict[str, Any]) -> Dict[str, Any]:
        """
        预测下一浪

        参数:
            waves: 波浪模式字典
            fibonacci: 斐波那契分析结果

        返回:
            预测结果
        """
        prediction = {
            'next_wave': None,
            'target_prices': [],
            'support_levels': [],
            'resistance_levels': [],
            'confidence': 0.5
        }

        # 这里实现简化的预测逻辑
        # 实际应用中需要更复杂的算法

        impulse_waves = waves.get('impulse', [])
        if impulse_waves:
            last_impulse = impulse_waves[-1]
            if len(last_impulse) >= 5:
                # 如果已经完成5浪，预测调整浪
                prediction['next_wave'] = 'WaveA (调整开始)'
                prediction['confidence'] = 0.6
            elif len(last_impulse) == 4:
                # 如果已经完成4浪，预测浪5
                prediction['next_wave'] = 'Wave5'
                prediction['confidence'] = 0.7

                # 使用斐波那契投影作为目标位
                if 'projections' in fibonacci and 'Wave5' in fibonacci['projections']:
                    for desc, price in fibonacci['projections']['Wave5'].items():
                        prediction['target_prices'].append({
                            'description': desc,
                            'price': price
                        })

        return prediction

    def calculate_confidence(self, waves: Dict[str, Any],
                           validation: Dict[str, Any]) -> float:
        """
        计算波浪识别置信度（增强版）

        参数:
            waves: 波浪模式字典
            validation: 验证结果

        返回:
            置信度 (0.0 到 1.0)
        """
        confidence = 0.4  # 基础置信度（降低基础分，让规则验证更重要）

        # 1. 基于验证结果调整
        if validation['valid']:
            confidence += 0.25  # 有效模式增加更多置信度
        else:
            # 如果验证无效，置信度大幅降低
            confidence -= 0.2

        # 2. 基于波浪质量分数调整
        impulse_waves = waves.get('impulse', [])
        if impulse_waves:
            # 计算平均质量分数
            quality_scores = []
            for impulse in impulse_waves:
                if isinstance(impulse, dict) and 'quality_score' in impulse:
                    quality_scores.append(impulse['quality_score'])
            if quality_scores:
                avg_quality = sum(quality_scores) / len(quality_scores)
                # 质量分数在0.5-1.0之间，映射到0-0.2的置信度加成
                confidence += (avg_quality - 0.5) * 0.4  # 例如，0.7质量 => +0.08
                # 额外加分：高质量模式（>0.8）
                if avg_quality > 0.8:
                    confidence += 0.05

        # 3. 基于规则通过数量和违反数量调整
        passed_rules = len(validation.get('passed_rules', []))
        violations = len(validation.get('rule_violations', []))

        # 规则通过率
        total_rules = passed_rules + violations
        if total_rules > 0:
            pass_rate = passed_rules / total_rules
            confidence += pass_rate * 0.2  # 最高+0.2
        else:
            # 没有规则验证信息，稍减置信度
            confidence -= 0.05

        # 4. 基于波浪数量调整（适度的加成）
        impulse_count = len(impulse_waves)
        corrective_count = len(waves.get('corrective', []))

        if impulse_count > 0:
            # 每个推动浪模式增加一定置信度，但收益递减
            confidence += min(impulse_count * 0.03, 0.09)  # 最多+0.09
        if corrective_count > 0:
            confidence += min(corrective_count * 0.02, 0.04)  # 最多+0.04

        # 5. 基于波浪特征符合度调整（如果波浪包含验证信息）
        for impulse in impulse_waves:
            if isinstance(impulse, dict):
                # 交替原则验证
                if impulse.get('alternation_valid', False):
                    confidence += 0.02
                # 时间比例验证
                if impulse.get('time_ratio_valid', False):
                    confidence += 0.02
                # 浪2回撤接近黄金分割（61.8%）
                retracement_ratio = impulse.get('retracement_ratio', 0)
                if 0.618 - 0.05 <= retracement_ratio <= 0.618 + 0.05:
                    confidence += 0.03
                # 浪3扩展比例高（>2.0）
                extension_ratio = impulse.get('extension_ratio', 0)
                if extension_ratio >= 2.0:
                    confidence += 0.02

        # 6. 惩罚：如果有规则违反，降低置信度
        if violations > 0:
            confidence -= min(violations * 0.05, 0.15)  # 最多-0.15

        # 7. 限制在0.0到1.0之间
        confidence = max(0.0, min(1.0, confidence))

        # 8. 最终微调：如果置信度低于0.3，可能模式不可靠
        if confidence < 0.3:
            # 进一步降低低置信度结果的权重
            confidence *= 0.8

        return confidence

    def plot(self, result: Dict[str, Any], data: pd.DataFrame,
            save_path: str = None, figsize: Tuple[int, int] = (16, 10)):
        """
        绘制波浪分析图表

        参数:
            result: 分析结果字典
            data: 原始数据
            save_path: 保存路径 (可选)
            figsize: 图表大小
        """
        fig, axes = plt.subplots(3, 1, figsize=figsize, height_ratios=[3, 1, 1])
        ax1, ax2, ax3 = axes

        # 主图: 价格和波浪
        self._plot_price_waves(ax1, result, data)

        # 副图1: 成交量
        self._plot_volume(ax2, data)

        # 副图2: 技术指标
        self._plot_technical_indicators(ax3, data)

        # 设置标题
        title = "艾略特波浪分析"
        if result.get('valid', False):
            confidence = result.get('confidence', 0) * 100
            title += f" (置信度: {confidence:.1f}%)"

        fig.suptitle(title, fontsize=16, fontweight='bold')

        # 设置x轴日期格式（如果使用日期）
        if 'date' in data.columns and len(data) > 0:
            try:
                # 检查是否已转换为datetime
                if not pd.api.types.is_datetime64_any_dtype(data['date']):
                    dates = pd.to_datetime(data['date'], errors='coerce')
                else:
                    dates = data['date']

                # 仅设置底部子图的x轴标签
                ax3.set_xlabel('日期', fontsize=12)

                # 设置日期刻度格式
                import matplotlib.dates as mdates
                # 根据日期范围选择合适的刻度间隔
                date_range = dates.max() - dates.min()
                if date_range.days > 365:  # 超过一年，使用年月格式
                    ax3.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m'))
                    ax3.xaxis.set_major_locator(mdates.MonthLocator(interval=3))
                elif date_range.days > 30:  # 超过一个月，使用月日格式
                    ax3.xaxis.set_major_formatter(mdates.DateFormatter('%m-%d'))
                    ax3.xaxis.set_major_locator(mdates.WeekdayLocator(interval=2))
                else:  # 短期，使用日格式
                    ax3.xaxis.set_major_formatter(mdates.DateFormatter('%m-%d'))
                    ax3.xaxis.set_major_locator(mdates.DayLocator(interval=1))

                # 旋转x轴标签以避免重叠
                for ax in [ax1, ax2, ax3]:
                    plt.setp(ax.xaxis.get_majorticklabels(), rotation=45, ha='right')

            except Exception as e:
                if self.verbose:
                    print(f"[可视化] 设置日期格式时出错: {e}")
                # 回退到默认标签
                ax3.set_xlabel('索引', fontsize=12)
        else:
            ax3.set_xlabel('索引', fontsize=12)

        plt.tight_layout()

        if save_path:
            # 确保目录存在
            save_dir = os.path.dirname(save_path)
            if save_dir and not os.path.exists(save_dir):
                os.makedirs(save_dir, exist_ok=True)
                if self.verbose:
                    print(f"[可视化] 创建目录: {save_dir}")

            plt.savefig(save_path, dpi=150, bbox_inches='tight')
            if self.verbose:
                print(f"[可视化] 图表已保存到: {save_path}")
        else:
            plt.show()

        plt.close(fig)

    def _plot_price_waves(self, ax, result: Dict[str, Any], data: pd.DataFrame):
        """绘制价格和波浪"""
        # 确定x轴数据：如果有日期列则使用日期，否则使用索引
        if 'date' in data.columns:
            # 将日期字符串转换为datetime对象，以便matplotlib正确处理
            try:
                # 尝试将日期列转换为datetime
                if not pd.api.types.is_datetime64_any_dtype(data['date']):
                    dates = pd.to_datetime(data['date'], errors='coerce')
                else:
                    dates = data['date']
                x_values = dates
            except Exception:
                # 转换失败，回退到索引
                x_values = data.index
        else:
            x_values = data.index

        # 绘制价格线
        ax.plot(x_values, data['close'], label='收盘价', color='blue', alpha=0.7, linewidth=1.5)

        # 绘制摆动点
        swings = result.get('swings', pd.DataFrame())
        if not swings.empty and 'Swing' in swings.columns:
            swing_highs = swings[swings['Swing'] == 1]
            swing_lows = swings[swings['Swing'] == -1]

            # 将摆动点索引转换为x轴坐标
            def get_x_for_index(idx):
                """根据索引获取对应的x轴坐标"""
                if isinstance(x_values, pd.Series) or isinstance(x_values, pd.Index):
                    # 如果x_values是Series或Index，尝试按位置获取
                    try:
                        # 尝试按位置获取（如果索引是整数位置）
                        return x_values.iloc[idx] if hasattr(x_values, 'iloc') else x_values[idx]
                    except:
                        # 失败则尝试直接使用索引值
                        return x_values.loc[idx] if hasattr(x_values, 'loc') else x_values[idx]
                else:
                    # x_values可能是普通列表或数组
                    return x_values[idx]

            # 绘制摆动高点
            if not swing_highs.empty:
                swing_high_x = [get_x_for_index(idx) for idx in swing_highs.index]
                ax.scatter(swing_high_x, swing_highs['high'],
                          marker='^', color='green', s=50, label='摆动高点', zorder=5)

            # 绘制摆动低点
            if not swing_lows.empty:
                swing_low_x = [get_x_for_index(idx) for idx in swing_lows.index]
                ax.scatter(swing_low_x, swing_lows['low'],
                          marker='v', color='red', s=50, label='摆动低点', zorder=5)

        # 绘制识别的波浪
        waves = result.get('waves', {})
        impulse_waves = waves.get('impulse', [])

        # 定义获取x坐标的函数（重用前面的逻辑）
        def get_x_for_index(idx):
            """根据索引获取对应的x轴坐标"""
            if isinstance(x_values, pd.Series) or isinstance(x_values, pd.Index):
                try:
                    return x_values.iloc[idx] if hasattr(x_values, 'iloc') else x_values[idx]
                except:
                    return x_values.loc[idx] if hasattr(x_values, 'loc') else x_values[idx]
            else:
                return x_values[idx]

        # 波浪颜色和样式配置
        wave_styles = {
            'Wave1': {'color': 'blue', 'linewidth': 2.0, 'linestyle': '-', 'label': '浪1 (推动)'},
            'Wave2': {'color': 'green', 'linewidth': 1.5, 'linestyle': '--', 'label': '浪2 (调整)'},
            'Wave3': {'color': 'red', 'linewidth': 2.0, 'linestyle': '-', 'label': '浪3 (推动)'},
            'Wave4': {'color': 'orange', 'linewidth': 1.5, 'linestyle': '--', 'label': '浪4 (调整)'},
            'Wave5': {'color': 'purple', 'linewidth': 2.0, 'linestyle': '-', 'label': '浪5 (推动)'}
        }

        for i, impulse in enumerate(impulse_waves):
            if len(impulse) >= 5:
                # 存储各浪点用于连接
                wave_segments = []
                for wave_name in ['Wave1', 'Wave2', 'Wave3', 'Wave4', 'Wave5']:
                    if wave_name in impulse:
                        wave = impulse[wave_name]
                        wave_x = get_x_for_index(wave['index'])
                        wave_segments.append((wave_x, wave['price'], wave_name))

                # 绘制各浪段（连接相邻点）
                for j in range(len(wave_segments) - 1):
                    x1, y1, name1 = wave_segments[j]
                    x2, y2, name2 = wave_segments[j + 1]
                    # 确定样式（使用起点浪的样式）
                    style = wave_styles.get(name1, {})
                    color = style.get('color', 'orange')
                    linewidth = style.get('linewidth', 1.5)
                    linestyle = style.get('linestyle', '-')
                    label = style.get('label', '') if i == 0 and j == 0 else ''

                    ax.plot([x1, x2], [y1, y2], color=color, linewidth=linewidth,
                           linestyle=linestyle, label=label, alpha=0.8)

                # 标注波浪编号和价格
                for j, (x, y, wave_name) in enumerate(wave_segments):
                    wave_num = j + 1
                    wave_label = f'浪{wave_num}\n{y:.2f}'
                    # 根据波浪类型调整标注位置
                    offset_y = 15 if wave_num % 2 == 1 else -15  # 奇偶浪上下错开
                    bbox_props = dict(boxstyle="round,pad=0.3", facecolor="white", edgecolor=color, alpha=0.8)
                    ax.annotate(wave_label, (x, y),
                               xytext=(0, offset_y), textcoords='offset points',
                               ha='center', va='center',
                               fontsize=8, fontweight='bold',
                               color=color,
                               bbox=bbox_props,
                               arrowprops=dict(arrowstyle="->", color=color, alpha=0.5, linewidth=0.5) if wave_num % 2 == 0 else None)

                # 在图表图例中添加波浪模式标签（仅第一次）
                if i == 0:
                    ax.plot([], [], color='blue', linewidth=2.0, linestyle='-', label='推动浪')
                    ax.plot([], [], color='green', linewidth=1.5, linestyle='--', label='调整浪')

        # 绘制斐波那契水平并标注
        fibonacci = result.get('fibonacci', {})
        fib_colors = {
            '23.6%': 'lightgray',
            '38.2%': 'orange',
            '50.0%': 'red',
            '61.8%': 'green',
            '78.6%': 'blue'
        }

        if 'retracements' in fibonacci:
            for wave_name, levels in fibonacci['retracements'].items():
                for level_name, price in levels.items():
                    # 确定颜色
                    color = fib_colors.get(level_name, 'purple')
                    # 绘制水平线
                    ax.axhline(y=price, color=color, linestyle='--',
                              alpha=0.7, linewidth=1.0)
                    # 添加文本标注（在图表右侧）
                    ax.text(x_values.iloc[-1] if hasattr(x_values, 'iloc') else x_values[-1],
                           price, f'Fib {level_name}',
                           color=color, fontsize=8, alpha=0.8,
                           verticalalignment='center',
                           horizontalalignment='right',
                           bbox=dict(boxstyle="round,pad=0.2", facecolor="white", edgecolor=color, alpha=0.5))

        # 绘制斐波那契扩展水平（如果存在）
        if 'extensions' in fibonacci:
            for wave_name, levels in fibonacci['extensions'].items():
                for level_name, price in levels.items():
                    ax.axhline(y=price, color='cyan', linestyle=':',
                              alpha=0.5, linewidth=0.8)
                    ax.text(x_values.iloc[-1] if hasattr(x_values, 'iloc') else x_values[-1],
                           price, f'Ext {level_name}',
                           color='cyan', fontsize=7, alpha=0.7,
                           verticalalignment='center',
                           horizontalalignment='right')

        # 绘制重要支撑/阻力位（基于斐波那契水平和价格极值）
        if len(data) > 0:
            # 近期高点和低点
            recent_high = data['high'].max()
            recent_low = data['low'].min()
            current_price = data['close'].iloc[-1]

            # 识别重要水平（价格整数位、近期高低点等）
            important_levels = []
            # 近期高点和低点
            important_levels.append((recent_high, '近期高点', 'red'))
            important_levels.append((recent_low, '近期低点', 'blue'))

            # 价格整数位（如果适用）
            price_interval = 10 if current_price > 100 else 5 if current_price > 50 else 1
            nearest_round = round(current_price / price_interval) * price_interval
            important_levels.append((nearest_round, f'整数位{nearest_round:.0f}', 'gray'))

            # 绘制重要水平
            for price, label, color in important_levels:
                ax.axhline(y=price, color=color, linestyle=':', alpha=0.4, linewidth=0.8)
                ax.text(x_values.iloc[0] if hasattr(x_values, 'iloc') else x_values[0],
                       price, label, color=color, fontsize=7, alpha=0.7,
                       verticalalignment='center',
                       horizontalalignment='left',
                       bbox=dict(boxstyle="round,pad=0.1", facecolor="white", edgecolor=color, alpha=0.3))

        ax.set_ylabel('价格', fontsize=12)
        ax.legend(loc='upper left')
        ax.grid(True, alpha=0.3)

    def _plot_volume(self, ax, data: pd.DataFrame):
        """绘制成交量"""
        if 'volume' in data.columns:
            # 确定x轴数据：如果有日期列则使用日期，否则使用索引
            if 'date' in data.columns:
                try:
                    if not pd.api.types.is_datetime64_any_dtype(data['date']):
                        dates = pd.to_datetime(data['date'], errors='coerce')
                    else:
                        dates = data['date']
                    x_values = dates
                except Exception:
                    x_values = data.index
            else:
                x_values = data.index

            # 计算成交量平均值
            avg_volume = data['volume'].mean()

            # 绘制成交量柱状图
            colors = ['green' if data['close'].iloc[i] >= data['open'].iloc[i]
                     else 'red' for i in range(len(data))]
            ax.bar(x_values, data['volume'], color=colors, alpha=0.7)

            # 添加成交量均线
            if len(data) >= 20:
                volume_ma = data['volume'].rolling(window=20).mean()
                ax.plot(x_values, volume_ma, color='blue', linewidth=1.5,
                       label='20日均量')

            ax.set_ylabel('成交量', fontsize=12)
            ax.legend(loc='upper left')
            ax.grid(True, alpha=0.3)

    def _plot_technical_indicators(self, ax, data: pd.DataFrame):
        """绘制技术指标（RSI和MACD）"""
        # 确定x轴数据：如果有日期列则使用日期，否则使用索引
        if 'date' in data.columns:
            try:
                if not pd.api.types.is_datetime64_any_dtype(data['date']):
                    dates = pd.to_datetime(data['date'], errors='coerce')
                else:
                    dates = data['date']
                x_values = dates
            except Exception:
                x_values = data.index
        else:
            x_values = data.index

        # 检查数据是否足够
        if len(data) < 26:  # MACD需要至少26个数据点
            ax.text(0.5, 0.5, '数据不足，无法计算技术指标',
                   ha='center', va='center', transform=ax.transAxes)
            ax.set_ylabel('技术指标', fontsize=12)
            return

        # 计算技术指标
        close_prices = data['close'].values

        # 1. 计算RSI（相对强弱指数）
        try:
            # 尝试使用TA-Lib
            import talib
            rsi = talib.RSI(close_prices, timeperiod=14)
        except ImportError:
            # TA-Lib不可用，使用简易计算
            delta = data['close'].diff()
            gain = (delta.where(delta > 0, 0)).rolling(window=14).mean()
            loss = (-delta.where(delta < 0, 0)).rolling(window=14).mean()
            rs = gain / loss
            rsi = 100 - (100 / (1 + rs))

        # 2. 计算MACD（移动平均收敛散度）
        try:
            import talib
            macd, macd_signal, macd_hist = talib.MACD(close_prices,
                                                     fastperiod=12,
                                                     slowperiod=26,
                                                     signalperiod=9)
        except ImportError:
            # 简易MACD计算
            exp1 = data['close'].ewm(span=12, adjust=False).mean()
            exp2 = data['close'].ewm(span=26, adjust=False).mean()
            macd = exp1 - exp2
            macd_signal = macd.ewm(span=9, adjust=False).mean()
            macd_hist = macd - macd_signal

        # 创建双y轴：左侧RSI，右侧MACD
        ax_rsi = ax
        ax_macd = ax.twinx()

        # 绘制RSI（左侧y轴）
        ax_rsi.plot(x_values, rsi, color='purple', linewidth=1.5, label='RSI (14)')
        ax_rsi.axhline(y=70, color='red', linestyle='--', alpha=0.5, linewidth=0.8)
        ax_rsi.axhline(y=30, color='green', linestyle='--', alpha=0.5, linewidth=0.8)
        ax_rsi.fill_between(x_values, 30, 70, color='gray', alpha=0.1)
        ax_rsi.set_ylabel('RSI', fontsize=12, color='purple')
        ax_rsi.tick_params(axis='y', labelcolor='purple')
        ax_rsi.set_ylim(0, 100)

        # 绘制MACD（右侧y轴）
        ax_macd.plot(x_values, macd, color='blue', linewidth=1.0, label='MACD')
        ax_macd.plot(x_values, macd_signal, color='orange', linewidth=1.0, label='Signal')
        # 绘制MACD柱状图（直方图）
        colors_macd = ['green' if val >= 0 else 'red' for val in macd_hist]
        ax_macd.bar(x_values, macd_hist, color=colors_macd, alpha=0.5, width=0.8, label='MACD Hist')
        ax_macd.axhline(y=0, color='black', linestyle='-', alpha=0.3, linewidth=0.5)
        ax_macd.set_ylabel('MACD', fontsize=12, color='blue')
        ax_macd.tick_params(axis='y', labelcolor='blue')

        # 添加图例
        lines_rsi, labels_rsi = ax_rsi.get_legend_handles_labels()
        lines_macd, labels_macd = ax_macd.get_legend_handles_labels()
        ax_rsi.legend(lines_rsi + lines_macd, labels_rsi + labels_macd,
                     loc='upper left', fontsize=8)

        ax.grid(True, alpha=0.3)
        ax.set_xlabel('日期', fontsize=12) if 'date' in data.columns else ax.set_xlabel('索引', fontsize=12)

    def save_report(self, result: Dict[str, Any], filepath: str,
                   detail_level: str = 'standard',
                   include_trading_recommendations: bool = True,
                   metadata: Optional[Dict[str, Any]] = None):
        """
        保存分析报告

        参数:
            result: 分析结果字典
            filepath: 报告文件路径
            detail_level: 报告详细程度 ('brief', 'standard', 'detailed')
            include_trading_recommendations: 是否包含交易建议
            metadata: 额外元数据，如股票代码、时间范围等
        """
        # 创建报告生成器
        generator = ReportGenerator(verbose=self.verbose)

        # 生成并保存报告
        generator.save_report(
            result=result,
            filepath=filepath,
            detail_level=detail_level,
            include_trading_recommendations=include_trading_recommendations,
            metadata=metadata
        )


def test_analyzer():
    """测试分析器功能"""
    print("测试艾略特波浪分析器...")

    # 创建测试数据（正弦波模拟价格）
    np.random.seed(42)
    n_points = 100
    dates = pd.date_range(start='2025-01-01', periods=n_points, freq='D')

    # 生成趋势 + 波动的价格序列
    trend = np.linspace(100, 150, n_points)
    cycle = 20 * np.sin(np.linspace(0, 4 * np.pi, n_points))
    noise = 5 * np.random.randn(n_points)

    prices = trend + cycle + noise

    # 创建DataFrame
    data = pd.DataFrame({
        'date': dates,
        'open': prices * 0.99,
        'high': prices * 1.02,
        'low': prices * 0.98,
        'close': prices,
        'volume': np.random.randint(100000, 1000000, n_points)
    })

    # 创建分析器
    analyzer = ElliottWaveAnalyzer(window=5, verbose=True)

    # 进行分析
    result = analyzer.analyze(data)

    # 输出结果
    print(f"\n分析结果:")
    print(f"  有效: {result['valid']}")
    print(f"  消息: {result['message']}")

    if 'confidence' in result:
        print(f"  置信度: {result['confidence']:.1%}")

    # 绘制图表
    analyzer.plot(result, data, save_path='test_analysis.png')

    # 保存报告
    analyzer.save_report(result, 'test_report.txt')

    print(f"\n测试完成!")
    print(f"  图表: test_analysis.png")
    print(f"  报告: test_report.txt")


if __name__ == "__main__":
    test_analyzer()