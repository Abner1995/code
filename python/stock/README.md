# 采集

## 创建虚拟环境  

1. 根据当前系统安装conda 
2. 根据当前系统创建所需的虚拟环境 

## 文档
https://baostock.com/
https://akshare.akfamily.xyz/introduction.html
https://tushare.pro/
https://github.com/ranaroussi/yfinance
https://github.com/pydata/pandas-datareader

### 艾略特波浪
https://www.pythontutorials.net/blog/elliot-wave-calculator-chart-patterns-recognition/

## windows/linux系统

```bash
# 1. 在windows创建Python虚拟环境
conda create -p D:/code/python/stock/envs/stock310 python=3.10 -y

# 在linux创建Python虚拟环境
conda create -p /www/wwwroot/stock/envs/stock310 python=3.10 -y

# 2. 在windows激活Python虚拟环境
conda activate D:/code/python/stock/envs/stock310
# 在linux激活Python虚拟环境
conda activate /www/wwwroot/stock/envs/stock310

# 3. 安装 Python 依赖
pip install -r requirements.txt

# 基本用法
python main.py --code sz.002594

# 指定参数
python main.py --code sz.002594 --start 2024-01-01 --window 8

# 使用缓存数据
python main.py --code sz.002594 --no-fetch --output my_analysis

# 详细日志
python main.py --code sz.002594 --verbose

# 控制报告详细程度
python main.py --code sz.002594 --report-detail brief  # 仅摘要
python main.py --code sz.002594 --report-detail standard  # 标准（默认）
python main.py --code sz.002594 --report-detail detailed  # 详细

# 不包含交易建议
python main.py --code sz.002594 --no-trading-recommendations
```

## 命令行参数

| 参数 | 说明 | 默认值 |
|------|------|--------|
| `--code` | **必需** 股票代码，如 `sz.002594` (比亚迪) | 无 |
| `--start` | 开始日期，格式 `YYYY-MM-DD` | `2025-01-01` |
| `--end` | 结束日期，格式 `YYYY-MM-DD`，留空为今天 | 今天 |
| `--window` | 摆动点检测窗口大小 | `5` |
| `--no-fetch` | 不获取新数据，使用本地缓存数据 | 否 |
| `--output` | 输出文件名前缀（不含扩展名） | `analysis_result` |
| `--report-detail` | 报告详细程度：`brief`（摘要）、`standard`（标准）、`detailed`（详细） | `standard` |
| `--no-trading-recommendations` | 不包含交易建议部分 | 否 |
| `--verbose` | 显示详细日志信息 | 否 |

## 输出文件

分析完成后，系统会在 `output/<股票代码数字部分>/<当前日期>/` 目录下生成以下文件：

- `<前缀>.png` – 艾略特波浪分析图表
- `<前缀>.txt` – 详细分析报告（包含波浪识别、斐波那契分析、交易建议等）

例如，运行 `python main.py --code sz.002594 --output byd_analysis` 将生成：

```
output/002594/20260414/byd_analysis.png
output/002594/20260414/byd_analysis.txt
```

## 更多示例

### 1. 完整分析（默认参数）
```bash
python main.py --code sz.002594
```

### 2. 指定日期范围和分析窗口
```bash
python main.py --code sz.002594 --start 2025-01-01 --end 2026-04-14 --window 10
```

### 3. 仅使用本地数据（不重新下载）
```bash
python main.py --code sz.002594 --no-fetch --output my_local_analysis
```

### 4. 生成简要报告（不含交易建议）
```bash
python main.py --code sz.002594 --report-detail brief --no-trading-recommendations
```

### 5. 调试模式（显示详细日志）
```bash
python main.py --code sz.002594 --verbose
```

