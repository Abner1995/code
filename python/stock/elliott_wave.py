import numpy as np
import matplotlib.pyplot as plt


class ElliottWave():
    def detect_swing_points(data, window=5):
        """Detect swing highs (1) and lows (-1) in price data."""
        data["Swing"] = 0
        # Swing High: high > high[window left] and high > high[window right]
        data["Swing"] = np.where(
            (data["high"] == data["high"].rolling(
                window=2*window+1, center=True).max()),
            1, data["Swing"]
        )
        # Swing Low: low < low[window left] and low < low[window right]
        data["Swing"] = np.where(
            (data["low"] == data["low"].rolling(
                window=2*window+1, center=True).min()),
            -1, data["Swing"]
        )
        return data

    def fibonacci_levels(swing_high, swing_low):
        """Compute Fibonacci retracement (for corrections) and extension (for impulses)."""
        diff = swing_high - swing_low
        retracements = {
            "23.6%": swing_high - 0.236 * diff,
            "38.2%": swing_high - 0.382 * diff,
            "50%": swing_high - 0.5 * diff,
            "61.8%": swing_high - 0.618 * diff
        }
        extensions = {
            "161.8%": swing_low + 1.618 * diff,
            "261.8%": swing_low + 2.618 * diff
        }
        return retracements, extensions

    def plot_wave(data, swings, retracements):
        plt.figure(figsize=(12, 6))
        plt.plot(data["close"], label="close Price", color="blue")
        plt.scatter(
            swings.index, swings["high"], marker="^", color="green", label="Swing High")
        plt.scatter(swings.index, swings["low"],
                    marker="v", color="red", label="Swing Low")

        # Plot Fibonacci levels
        for level, price in retracements.items():
            plt.axhline(y=price, color="orange",
                        linestyle="--", label=f"Fib {level}")

        plt.title("Elliott Waves with Fibonacci Levels")
        plt.legend()
        plt.show()
