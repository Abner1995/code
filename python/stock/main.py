#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
艾略特波浪分析系统主入口

用法:
    python main.py --code sz.002594 [--start 2025-01-01] [--end 2026-04-14]

示例:
    python main.py --code sz.002594
    python main.py --code sz.002594 --start 2024-01-01 --end 2025-12-31
    python main.py --code sz.002594 --window 10 --no-fetch
    python main.py --code sz.002594 --output my_analysis
"""

import argparse
import sys
import os

# 尝试设置控制台编码为UTF-8，解决Windows控制台中文乱码问题
try:
    if sys.platform == 'win32':
        # Windows系统，尝试设置控制台编码
        import io
        sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
        sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')
except:
    pass

# 添加当前目录到Python路径，以便导入本地模块
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

def main():
    parser = argparse.ArgumentParser(
        description='艾略特波浪分析系统 - 根据股票代码生成艾略特波浪分析',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
示例:
  %(prog)s --code sz.002594
  %(prog)s --code sz.002594 --start 2024-01-01 --window 8
  %(prog)s --code sz.002594 --no-fetch --output byd_analysis

数据源: baostock (中国A股历史数据)
输出: 图表(png)和分析报告(txt)
        """
    )

    # 必需参数
    parser.add_argument('--code', required=True,
                       help='股票代码，如 sz.002594 (比亚迪)')

    # 可选参数
    parser.add_argument('--start', default='2025-01-01',
                       help='开始日期 (默认: 2025-01-01)')
    parser.add_argument('--end', default=None,
                       help='结束日期 (默认: 今天)')
    parser.add_argument('--window', type=int, default=5,
                       help='摆动点检测窗口大小 (默认: 5)')
    parser.add_argument('--no-fetch', action='store_true',
                       help='不获取新数据，使用本地缓存数据')
    parser.add_argument('--output', default='analysis_result',
                       help='输出文件名前缀 (默认: analysis_result)')
    parser.add_argument('--report-detail', choices=['brief', 'standard', 'detailed'],
                       default='standard',
                       help='报告详细程度: brief(摘要), standard(标准), detailed(详细) (默认: standard)')
    parser.add_argument('--no-trading-recommendations', action='store_true',
                       help='不包含交易建议部分')
    parser.add_argument('--verbose', action='store_true',
                       help='显示详细日志信息')

    args = parser.parse_args()

    if args.verbose:
        print(f"[INFO] 开始艾略特波浪分析")
        print(f"[INFO] 股票代码: {args.code}")
        print(f"[INFO] 日期范围: {args.start} 到 {args.end or '今天'}")
        print(f"[INFO] 窗口大小: {args.window}")
        print(f"[INFO] 数据获取: {'禁用' if args.no_fetch else '启用'}")
        print(f"[INFO] 报告详细程度: {args.report_detail}")
        print(f"[INFO] 交易建议: {'禁用' if args.no_trading_recommendations else '启用'}")

    try:
        # 导入模块（延迟导入，以便在参数解析失败时不加载）
        from data_loader import load_stock_data
        from elliott_wave_enhanced import ElliottWaveAnalyzer

        # 1. 加载数据
        if args.verbose:
            print(f"[INFO] 正在加载股票数据...")

        data = load_stock_data(
            stock_code=args.code,
            start_date=args.start,
            end_date=args.end,
            fetch_if_missing=not args.no_fetch,
            verbose=args.verbose
        )

        if data is None or len(data) == 0:
            print(f"[错误] 无法加载股票数据，请检查股票代码和网络连接")
            sys.exit(1)

        if args.verbose:
            print(f"[INFO] 数据加载成功，共 {len(data)} 条记录")
            print(f"[INFO] 日期范围: {data['date'].iloc[0]} 到 {data['date'].iloc[-1]}")

        # 2. 分析波浪
        if args.verbose:
            print(f"[INFO] 正在进行艾略特波浪分析...")

        analyzer = ElliottWaveAnalyzer(window=args.window, verbose=args.verbose)
        result = analyzer.analyze(data)

        if not result.get('valid', False):
            print(f"[警告] 波浪分析结果可能不可靠: {result.get('message', '未知原因')}")
            # 继续执行，但标注结果可能不准确

        # 3. 输出结果
        if args.verbose:
            print(f"[INFO] 正在生成输出文件...")

        # 创建输出目录: output/股票代码/当前日期/
        # 提取股票代码数字部分
        if '.' in args.code:
            code_num = args.code.split('.')[-1]
        else:
            code_num = args.code

        # 获取当前日期
        from datetime import datetime
        current_date = datetime.now().strftime("%Y%m%d")

        # 构建输出目录路径
        script_dir = os.path.dirname(os.path.abspath(__file__))
        output_base_dir = os.path.join(script_dir, "output")
        output_dir = os.path.join(output_base_dir, code_num, current_date)

        # 创建目录（如果不存在）
        os.makedirs(output_dir, exist_ok=True)

        if args.verbose:
            print(f"[INFO] 输出目录: {output_dir}")

        # 生成输出文件名（使用原输出参数作为前缀，或使用股票代码）
        if args.output == 'analysis_result':  # 默认值
            file_prefix = f"{code_num}_{current_date}"
        else:
            file_prefix = args.output

        # 图表文件
        chart_file = os.path.join(output_dir, f"{file_prefix}.png")
        analyzer.plot(result, data, save_path=chart_file)

        # 报告文件
        report_file = os.path.join(output_dir, f"{file_prefix}.txt")

        # 准备报告元数据
        metadata = {
            'stock_code': args.code,
            'start_date': args.start,
            'end_date': args.end or datetime.now().strftime('%Y-%m-%d'),
            'analyzer_params': {
                'window': args.window
            }
        }

        # 生成报告
        analyzer.save_report(
            result=result,
            filepath=report_file,
            detail_level=args.report_detail,
            include_trading_recommendations=not args.no_trading_recommendations,
            metadata=metadata
        )

        # 4. 显示摘要
        print(f"\n{'='*60}")
        print(f"艾略特波浪分析完成!")
        print(f"{'='*60}")
        print(f"股票代码: {args.code}")
        print(f"分析日期: {args.start} 到 {args.end or '今天'}")
        print(f"数据点数: {len(data)}")

        if result.get('valid', False):
            waves = result.get('waves', {})
            if 'impulse' in waves:
                impulse = waves['impulse']
                print(f"识别到 {len(impulse)} 浪推动模式")

            fib = result.get('fibonacci', {})
            if 'retracements' in fib:
                print(f"斐波那契分析: 关键支撑/阻力位已计算")

        print(f"\n输出文件:")
        print(f"  - 图表: {chart_file}")
        print(f"  - 报告: {report_file} (详细程度: {args.report_detail}, 交易建议: {'禁用' if args.no_trading_recommendations else '启用'})")

        if os.path.exists(chart_file):
            print(f"  - 图表大小: {os.path.getsize(chart_file)//1024} KB")
        if os.path.exists(report_file):
            print(f"  - 报告大小: {os.path.getsize(report_file)//1024} KB")

        print(f"\n使用方法:")
        print(f"  查看图表: 使用图片查看器打开 {chart_file}")
        print(f"  查看报告: 使用文本编辑器打开 {report_file} 或命令: cat {report_file}")

    except ImportError as e:
        print(f"[错误] 导入模块失败: {e}")
        print(f"[提示] 请确保已安装所有依赖: pip install -r requirements.txt")
        print(f"[提示] 或检查模块文件是否存在")
        sys.exit(1)
    except Exception as e:
        print(f"[错误] 分析过程中出现错误: {e}")
        if args.verbose:
            import traceback
            traceback.print_exc()
        sys.exit(1)

if __name__ == "__main__":
    main()