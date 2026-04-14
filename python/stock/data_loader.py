"""
数据加载模块

功能:
1. 加载本地缓存的股票数据
2. 如果本地没有数据或需要更新，调用数据获取模块
3. 数据预处理和清洗
"""

import pandas as pd
import os
import sys
from datetime import datetime, timedelta
import glob

# 添加当前目录到Python路径
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))


def find_latest_data_file(directory):
    """
    在指定目录中查找最新的数据文件

    参数:
        directory: 目录路径

    返回:
        最新文件的完整路径，如果没有文件则返回None
    """
    if not os.path.exists(directory):
        return None

    # 查找所有CSV文件
    csv_files = glob.glob(os.path.join(directory, "*.csv"))
    if not csv_files:
        return None

    # 按修改时间排序，返回最新的文件
    latest_file = max(csv_files, key=os.path.getmtime)
    return latest_file


def load_stock_data(stock_code, start_date, end_date=None, fetch_if_missing=True,
                   verbose=False):
    """
    加载股票数据

    参数:
        stock_code: 股票代码，如 'sz.002594'
        start_date: 开始日期，格式 'YYYY-MM-DD'
        end_date: 结束日期，格式 'YYYY-MM-DD' (默认: 今天)
        fetch_if_missing: 如果本地没有数据是否获取新数据
        verbose: 是否显示详细日志

    返回:
        pandas DataFrame 包含股票数据，如果失败返回None
    """
    if end_date is None:
        end_date = datetime.now().strftime("%Y-%m-%d")

    if verbose:
        print(f"[数据加载] 股票代码: {stock_code}")
        print(f"[数据加载] 日期范围: {start_date} 到 {end_date}")
        print(f"[数据加载] 获取新数据: {'是' if fetch_if_missing else '否'}")

    # 提取股票代码数字部分用于目录名
    if '.' in stock_code:
        code_num = stock_code.split('.')[-1]
    else:
        code_num = stock_code

    # 构建数据目录路径
    script_dir = os.path.dirname(os.path.abspath(__file__))
    history_dir = os.path.join(script_dir, "history")
    stock_dir = os.path.join(history_dir, code_num)

    # 检查是否需要获取新数据
    latest_file = find_latest_data_file(stock_dir)

    use_cached_data = False
    data_file = None

    if latest_file:
        # 检查缓存文件是否足够新（比如7天内）
        file_mtime = datetime.fromtimestamp(os.path.getmtime(latest_file))
        days_old = (datetime.now() - file_mtime).days

        if days_old < 7 and not fetch_if_missing:
            # 使用缓存数据
            use_cached_data = True
            data_file = latest_file
            if verbose:
                print(f"[数据加载] 使用缓存数据: {os.path.basename(data_file)}")
                print(f"[数据加载] 数据年龄: {days_old} 天")
        elif verbose:
            print(f"[数据加载] 缓存数据较旧 ({days_old} 天)，需要更新")

    # 如果需要获取新数据
    if not use_cached_data and fetch_if_missing:
        if verbose:
            print(f"[数据加载] 正在获取新数据...")

        try:
            # 导入数据获取模块
            from get_stock_data import fetch_stock_data

            # 调用数据获取函数
            success = fetch_stock_data(
                stock_code=stock_code,
                start_date=start_date,
                end_date=end_date,
                verbose=verbose
            )

            if success:
                # 重新查找最新的数据文件
                latest_file = find_latest_data_file(stock_dir)
                if latest_file:
                    use_cached_data = True
                    data_file = latest_file
                    if verbose:
                        print(f"[数据加载] 数据获取成功: {os.path.basename(data_file)}")
                else:
                    if verbose:
                        print(f"[数据加载] 警告: 数据获取成功但未找到数据文件")
            else:
                if verbose:
                    print(f"[数据加载] 数据获取失败，尝试使用缓存数据")

                # 数据获取失败，尝试使用缓存数据
                if latest_file:
                    use_cached_data = True
                    data_file = latest_file
                    if verbose:
                        print(f"[数据加载] 使用旧的缓存数据: {os.path.basename(data_file)}")

        except ImportError as e:
            if verbose:
                print(f"[数据加载] 导入数据获取模块失败: {e}")
                print(f"[数据加载] 尝试直接运行脚本...")

            # 备用方案: 直接运行原脚本
            try:
                import subprocess

                # 构建命令
                cmd = [sys.executable, "get_stock_data.py"]
                if verbose:
                    print(f"[数据加载] 运行命令: {' '.join(cmd)}")

                # 运行脚本
                result = subprocess.run(cmd, capture_output=True, text=True)

                if result.returncode == 0:
                    latest_file = find_latest_data_file(stock_dir)
                    if latest_file:
                        use_cached_data = True
                        data_file = latest_file
                        if verbose:
                            print(f"[数据加载] 脚本执行成功")
                    else:
                        if verbose:
                            print(f"[数据加载] 脚本执行成功但未生成数据文件")
                else:
                    if verbose:
                        print(f"[数据加载] 脚本执行失败: {result.stderr}")

            except Exception as subprocess_error:
                if verbose:
                    print(f"[数据加载] 运行脚本失败: {subprocess_error}")

    # 加载数据
    if use_cached_data and data_file:
        try:
            if verbose:
                print(f"[数据加载] 正在加载数据文件: {os.path.basename(data_file)}")

            # 读取CSV文件
            data = pd.read_csv(data_file)

            if verbose:
                print(f"[数据加载] 原始数据记录数: {len(data)}")

            # 确保日期列是字符串类型
            if 'date' in data.columns:
                data['date'] = data['date'].astype(str)

                # 过滤日期范围
                mask = (data['date'] >= start_date) & (data['date'] <= end_date)
                data = data[mask].copy()

                if verbose:
                    print(f"[数据加载] 过滤后数据记录数: {len(data)}")

                    if len(data) > 0:
                        print(f"[数据加载] 实际日期范围: {data['date'].iloc[0]} 到 {data['date'].iloc[-1]}")

            # 数据预处理
            data = preprocess_data(data, verbose)

            return data

        except Exception as e:
            if verbose:
                print(f"[数据加载] 加载数据文件失败: {e}")

    # 如果到这里还没有返回数据，说明加载失败
    if verbose:
        print(f"[数据加载] 无法加载股票数据")

    return None


def preprocess_data(data, verbose=False):
    """
    数据预处理

    参数:
        data: pandas DataFrame
        verbose: 是否显示详细日志

    返回:
        处理后的DataFrame
    """
    if data is None or len(data) == 0:
        return data

    original_count = len(data)

    # 1. 确保必要的列存在
    required_columns = ['date', 'open', 'high', 'low', 'close']
    for col in required_columns:
        if col not in data.columns:
            if verbose:
                print(f"[数据预处理] 警告: 缺少必要列 '{col}'")
            return data

    # 2. 转换数值列
    numeric_columns = ['open', 'high', 'low', 'close', 'volume', 'amount']
    for col in numeric_columns:
        if col in data.columns:
            # 尝试转换为数值类型
            try:
                data[col] = pd.to_numeric(data[col], errors='coerce')
            except:
                pass

    # 3. 按日期排序
    if 'date' in data.columns:
        data = data.sort_values('date').reset_index(drop=True)

    # 4. 处理缺失值
    # 对于价格数据，使用前向填充
    price_columns = ['open', 'high', 'low', 'close']
    for col in price_columns:
        if col in data.columns:
            data[col] = data[col].fillna(method='ffill')

    # 5. 计算额外的技术指标
    # 计算每日收益率
    if 'close' in data.columns:
        data['return'] = data['close'].pct_change()

    # 计算简单移动平均
    if 'close' in data.columns:
        data['sma_5'] = data['close'].rolling(window=5).mean()
        data['sma_20'] = data['close'].rolling(window=20).mean()

    # 6. 移除完全为空的行
    data = data.dropna(subset=['open', 'high', 'low', 'close'], how='all')

    if verbose:
        filtered_count = len(data)
        if filtered_count < original_count:
            print(f"[数据预处理] 移除 {original_count - filtered_count} 条无效记录")
        print(f"[数据预处理] 处理后记录数: {filtered_count}")

    return data


def test_data_loading():
    """测试数据加载功能"""
    print("测试数据加载功能...")

    # 测试用例1: 加载现有数据
    print("\n1. 测试加载现有数据 (不获取新数据)...")
    data = load_stock_data(
        stock_code='sz.002594',
        start_date='2025-01-01',
        end_date='2025-12-31',
        fetch_if_missing=False,
        verbose=True
    )

    if data is not None:
        print(f"✓ 数据加载成功，记录数: {len(data)}")
        print(f"  列: {list(data.columns)}")
        if len(data) > 0:
            print(f"  日期范围: {data['date'].iloc[0]} 到 {data['date'].iloc[-1]}")
    else:
        print("✗ 数据加载失败")

    # 测试用例2: 尝试获取新数据
    print("\n2. 测试获取新数据...")
    data2 = load_stock_data(
        stock_code='sz.000001',  # 平安银行
        start_date='2025-01-01',
        end_date=None,
        fetch_if_missing=True,
        verbose=True
    )

    if data2 is not None:
        print(f"✓ 数据加载成功，记录数: {len(data2)}")
    else:
        print("✗ 数据加载失败 (可能网络问题或股票代码无效)")


if __name__ == "__main__":
    test_data_loading()