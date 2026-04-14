#!/usr/bin/env python3
"""
Simple integration test for Elliott Wave analysis system
"""

import sys
import os

print("=" * 60)
print("Elliott Wave Analysis System - Integration Test")
print("=" * 60)

# Test 1: Import modules
print("\n1. Testing module imports...")
try:
    from data_loader import load_stock_data
    from elliott_wave_enhanced import ElliottWaveAnalyzer
    print("   [OK] Modules imported successfully")
except ImportError as e:
    print(f"   [ERROR] Failed to import modules: {e}")
    sys.exit(1)

# Test 2: Data loading
print("\n2. Testing data loading...")
try:
    # Use existing data, don't fetch new data
    data = load_stock_data(
        stock_code='sz.002594',
        start_date='2025-01-01',
        end_date='2025-12-31',
        fetch_if_missing=False,
        verbose=False  # Set to True for detailed logs
    )

    if data is not None and len(data) > 0:
        print(f"   [OK] Data loaded successfully")
        print(f"        Records: {len(data)}")
        print(f"        Columns: {list(data.columns)}")

        # Check required columns
        required_cols = ['high', 'low', 'close']
        missing_cols = [col for col in required_cols if col not in data.columns]
        if missing_cols:
            print(f"   [WARNING] Missing required columns: {missing_cols}")
        else:
            print(f"   [OK] All required columns present")
    else:
        print("   [ERROR] Data loading failed or no data")
        sys.exit(1)

except Exception as e:
    print(f"   [ERROR] Error during data loading: {e}")
    import traceback
    traceback.print_exc()
    sys.exit(1)

# Test 3: Wave analysis
print("\n3. Testing wave analysis...")
try:
    analyzer = ElliottWaveAnalyzer(window=5, verbose=False)
    result = analyzer.analyze(data)

    print(f"   [OK] Wave analysis completed")
    print(f"        Valid result: {result.get('valid', False)}")
    print(f"        Message: {result.get('message', 'N/A')}")

    if 'confidence' in result:
        print(f"        Confidence: {result.get('confidence', 0):.1%}")

    # Check wave patterns
    waves = result.get('waves', {})
    impulse_count = len(waves.get('impulse', []))
    corrective_count = len(waves.get('corrective', []))

    print(f"        5-wave patterns found: {impulse_count}")
    print(f"        3-wave patterns found: {corrective_count}")

except Exception as e:
    print(f"   [ERROR] Error during wave analysis: {e}")
    import traceback
    traceback.print_exc()
    sys.exit(1)

# Test 4: Visualization
print("\n4. Testing visualization...")
try:
    # Create test output directory
    output_dir = 'test_output'
    os.makedirs(output_dir, exist_ok=True)

    chart_path = os.path.join(output_dir, 'test_chart.png')
    report_path = os.path.join(output_dir, 'test_report.txt')

    analyzer.plot(result, data, save_path=chart_path)
    analyzer.save_report(result, report_path)

    if os.path.exists(chart_path):
        size_kb = os.path.getsize(chart_path) // 1024
        print(f"   [OK] Chart generated: {chart_path} ({size_kb} KB)")
    else:
        print(f"   [ERROR] Chart generation failed")

    if os.path.exists(report_path):
        size_kb = os.path.getsize(report_path) // 1024
        print(f"   [OK] Report generated: {report_path} ({size_kb} KB)")
    else:
        print(f"   [ERROR] Report generation failed")

except Exception as e:
    print(f"   [ERROR] Error during visualization: {e}")
    import traceback
    traceback.print_exc()

# Test 5: Main program simulation
print("\n5. Testing main program flow...")
try:
    # Simulate main program arguments
    test_code = 'sz.002594'
    test_start = '2025-01-01'
    test_window = 5

    print(f"   [OK] Simulating main program with:")
    print(f"        Stock code: {test_code}")
    print(f"        Start date: {test_start}")
    print(f"        Window size: {test_window}")

    # Load data
    test_data = load_stock_data(
        stock_code=test_code,
        start_date=test_start,
        fetch_if_missing=False,
        verbose=False
    )

    if test_data is not None:
        # Analyze
        test_analyzer = ElliottWaveAnalyzer(window=test_window, verbose=False)
        test_result = test_analyzer.analyze(test_data)

        print(f"   [OK] Analysis completed successfully")
        print(f"        Result valid: {test_result.get('valid', False)}")

except Exception as e:
    print(f"   [ERROR] Error in main program simulation: {e}")

print("\n" + "=" * 60)
print("Integration test completed!")
print("=" * 60)

if os.path.exists('test_output'):
    print(f"\nGenerated files in 'test_output' directory:")
    for file in os.listdir('test_output'):
        filepath = os.path.join('test_output', file)
        size = os.path.getsize(filepath) // 1024
        print(f"  - {file} ({size} KB)")

print("\nNext steps:")
print("1. Run full analysis: python main.py --code sz.002594")
print("2. View test chart: test_output/test_chart.png")
print("3. View test report: test_output/test_report.txt")
print("\nNote: The system uses cached data. Use --no-fetch to avoid")
print("      downloading new data, or omit to update data.")