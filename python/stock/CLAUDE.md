# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Python stock data collection and analysis project. Currently it uses baostock to fetch historical K-line data for Chinese A-shares, with plans to integrate additional data sources (AKShare, tushare, yfinance, pandas-datareader) and technical analysis libraries (TA-Lib). The initial script (`ElliotWave.py`) demonstrates data fetching and CSV export.

## Environment Setup

The project uses Conda virtual environments with separate paths for Windows and Linux development.

**Windows environment path:** `D:/code/python/stock/envs/stock310`  
**Linux environment path:** `/www/wwwroot/stock/envs/stock310`

To create and activate the environment:

```bash
# Windows
conda create -p D:/code/python/stock/envs/stock310 python=3.10 -y
conda activate D:/code/python/stock/envs/stock310

# Linux
conda create -p /www/wwwroot/stock/envs/stock310 python=3.10 -y
conda activate /www/wwwroot/stock/envs/stock310
```

## Dependencies

Install Python packages from `requirements.txt`:

```bash
pip install -r requirements.txt
```

Current dependencies:
- `pandas` – data manipulation
- `numpy` – numerical computing
- `matplotlib` – plotting
- `ta-lib` – technical analysis indicators
- `baostock` – Chinese stock market data

## Running the Script

Execute the existing ElliotWave analysis script:

```bash
python ElliotWave.py
```

This script:
1. Logs into the baostock system
2. Fetches daily K-line data for stock `sz.002594` (BYD) from 2025-01-01 to current date
3. Saves the result as CSV under `history/002594/` with a date‑stamped filename
4. Logs out

Output directory structure is created on‑the‑fly; the script expects a `history` folder at the project root.

## Data Sources & Documentation

The project references multiple financial data APIs:

- **Baostock** – <https://baostock.com/> (Chinese A‑share historical data)
- **AKShare** – <https://akshare.akfamily.xyz/introduction.html> (Chinese financial data)
- **Tushare** – <https://tushare.pro/> (Chinese market data)
- **yfinance** – <https://github.com/ranaroussi/yfinance> (Yahoo Finance wrapper)
- **pandas‑datareader** – <https://github.com/pydata/pandas-datareader> (FRED, Yahoo, etc.)

When adding new data sources, follow the pattern in `ElliotWave.py`:
- Handle login/authentication if required
- Use pandas DataFrames for data manipulation
- Write results to `history/{symbol}/` with appropriate naming

## Project Structure

```
.
├── ElliotWave.py          # Main data‑fetching script (example)
├── requirements.txt       # Python dependencies
├── README.md             # Setup instructions (Chinese)
├── envs/                 # Conda virtual environment (git‑ignored)
└── history/              # Output CSV files (created by script)
```

## Common Commands

| Purpose | Command |
|---------|---------|
| Create environment (Win) | `conda create -p D:/code/python/stock/envs/stock310 python=3.10 -y` |
| Activate environment (Win) | `conda activate D:/code/python/stock/envs/stock310` |
| Install dependencies | `pip install -r requirements.txt` |
| Run ElliotWave script | `python ElliotWave.py` |
| Update requirements | `pip freeze > requirements.txt` |

## Notes for Development

- The script currently hard‑codes a stock symbol (`sz.002594`) and date range. Future extensions should parameterize these.
- TA‑Lib is listed as a dependency but not yet used in the example script.
- The `history` folder is not version‑controlled; ensure any persistent data storage strategy aligns with the project’s goals.
- When adding new analysis modules, consider separating data fetching, indicator calculation, and visualization into distinct modules.
- The output path in `ElliotWave.py` uses Windows‑style backslashes (`D:\\code\\python\\stock\\history\\...`). For cross‑platform compatibility, use `os.path.join` or forward slashes.