import baostock as bs
import pandas as pd
import os
import sys
from datetime import datetime


def fetch_stock_data(stock_code, start_date='2025-01-01', end_date=None, verbose=False):
    """
    获取股票数据并保存为CSV文件

    参数:
        stock_code: 股票代码，如 'sz.002594'
        start_date: 开始日期，格式 'YYYY-MM-DD' (默认: 2025-01-01)
        end_date: 结束日期，格式 'YYYY-MM-DD' (默认: 今天)
        verbose: 是否显示详细日志

    返回:
        bool: 是否成功获取数据
    """
    if end_date is None:
        end_date = datetime.now().strftime("%Y-%m-%d")

    if verbose:
        print(f"[数据获取] 开始获取股票数据")
        print(f"[数据获取] 股票代码: {stock_code}")
        print(f"[数据获取] 日期范围: {start_date} 到 {end_date}")

    try:
        #### 登陆系统 ####
        if verbose:
            print(f"[数据获取] 正在登录baostock系统...")

        lg = bs.login()

        if verbose:
            print(f"[数据获取] 登录响应错误码: {lg.error_code}")
            print(f"[数据获取] 登录响应错误信息: {lg.error_msg}")

        if lg.error_code != '0':
            print(f"[数据获取] 登录失败: {lg.error_msg}")
            return False

        #### 获取沪深A股历史K线数据 ####
        # 详细指标参数，参见"历史行情指标参数"章节
        rs = bs.query_history_k_data_plus(
            stock_code,
            "date,code,open,high,low,close,preclose,volume,amount,adjustflag,turn,tradestatus,pctChg,isST",
            start_date=start_date,
            end_date=end_date,
            frequency="d",
            adjustflag="3"
        )

        if verbose:
            print(f"[数据获取] 查询响应错误码: {rs.error_code}")
            print(f"[数据获取] 查询响应错误信息: {rs.error_msg}")

        if rs.error_code != '0':
            print(f"[数据获取] 查询失败: {rs.error_msg}")
            bs.logout()
            return False

        #### 处理结果集 ####
        data_list = []
        record_count = 0

        while (rs.error_code == '0') and rs.next():
            # 获取一条记录
            data_list.append(rs.get_row_data())
            record_count += 1

        if verbose:
            print(f"[数据获取] 获取到 {record_count} 条记录")

        if data_list:
            result = pd.DataFrame(data_list, columns=rs.fields)

            #### 结果集输出到csv文件 ####
            # 提取股票代码数字部分用于目录名
            code_num = stock_code.split('.')[-1]

            # 构建输出目录路径
            script_dir = os.path.dirname(os.path.abspath(__file__))
            history_dir = os.path.join(script_dir, "history")
            output_dir = os.path.join(history_dir, code_num)

            # 创建目录（如果不存在）
            os.makedirs(output_dir, exist_ok=True)

            # 生成基于当前日期的文件名
            current_date = datetime.now().strftime("%Y%m%d")
            output_file = os.path.join(output_dir, f"{current_date}.csv")

            # 保存数据
            result.to_csv(output_file, index=False)

            if verbose:
                print(f"[数据获取] 数据已保存到: {output_file}")
                print(f"[数据获取] 文件大小: {os.path.getsize(output_file)//1024} KB")
                print(f"[数据获取] 数据示例:")
                print(result.head(3))

            #### 登出系统 ####
            bs.logout()

            return True
        else:
            if verbose:
                print("[数据获取] 未获取到数据")

            bs.logout()
            return False

    except Exception as e:
        print(f"[数据获取] 获取数据时出现异常: {e}")
        # 尝试登出系统
        try:
            bs.logout()
        except:
            pass
        return False


def main():
    """命令行入口函数"""
    print("=" * 60)
    print("股票数据获取工具")
    print("=" * 60)

    # 获取用户输入的股票代码
    stock_code = input("请输入股票代码（例如：sz.002594）: ").strip()

    if not stock_code:
        print("错误: 股票代码不能为空")
        return

    # 设置日期范围：开始日期固定为2025-01-01，结束日期为今天
    start_date = '2025-01-01'
    end_date = datetime.now().strftime("%Y-%m-%d")

    print(f"开始获取数据: {stock_code} ({start_date} 到 {end_date})")

    # 获取数据
    success = fetch_stock_data(stock_code, start_date, end_date, verbose=True)

    if success:
        print(f"\n✓ 数据获取成功!")
    else:
        print(f"\n✗ 数据获取失败，请检查网络连接和股票代码")


if __name__ == "__main__":
    main()
