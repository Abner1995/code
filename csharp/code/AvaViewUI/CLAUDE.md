# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

**Prerequisites:** The Browser/WASM project requires the `wasm-tools` workload:
```bash
dotnet workload install wasm-tools
```

```bash
# Build the entire solution
dotnet build

# Run desktop app (primary development target)
dotnet run --project AvaViewUI.Desktop/AvaViewUI.Desktop.csproj

# Run browser/WASM app
dotnet run --project AvaViewUI.Browser/AvaViewUI.Browser.csproj
```

## Architecture

This is a cross-platform Avalonia UI 11.2 app targeting .NET 8. The solution uses **Central Package Management** (`Directory.Packages.props`) — NuGet versions are defined there, not in individual `.csproj` files.

### Project Structure

- **`AvaViewUI`** — Shared class library containing all UI (views, viewmodels, styles, assets). This is where most code lives.
- **`AvaViewUI.Desktop`** — Desktop entry point. Starts with `IClassicDesktopStyleApplicationLifetime`, creates a `MainWindow`.
- **`AvaViewUI.Browser`** — WebAssembly entry point. Uses `ISingleViewApplicationLifetime` + `MainView` (no Window).
- **`AvaViewUI.iOS`** / **`AvaViewUI.Android`** — Mobile entry points. Also use `ISingleViewApplicationLifetime`.

### Patterns

**MVVM with CommunityToolkit.Mvvm:** ViewModels extend `ViewModelBase` (which extends `ObservableObject`). Use `[ObservableProperty]` source generators for bindable properties — the field must be private and start with `_`.

**View resolution:** `ViewLocator` implements `IDataTemplate` and maps `*ViewModel` types to `*View` types by convention — performs a string replace of "ViewModel" with "View" on the full type name, then resolves via `Activator`. This means ViewModels and their corresponding Views must live in the same assembly with namespace names that differ only by the `ViewModels`/`Views` segment (e.g., `AvaViewUI.ViewModels.FooViewModel` → `AvaViewUI.Views.FooView`).

**View hierarchy:** `MainWindow` (desktop-only) wraps `MainView` (a `UserControl`) inside a `<Window>`. On Browser/iOS/Android, `MainView` is used directly as the application root. All new views should be built as `UserControl` subclasses so they compose on both desktop and mobile.

**Avalonia data validation:** The `App` disables Avalonia's built-in `DataAnnotationsValidationPlugin` to avoid conflicts with CommunityToolkit.Mvvm's validation.

**Axis files:** All `.axaml` files use `x:DataType` for compiled bindings (enabled globally via `AvaloniaUseCompiledBindingsByDefault`). Design-time `DataContext` is set with `<Design.DataContext>`.

### App Lifecycle
The shared `App` class (`App.axaml.cs`) branches on the application lifetime:
- **Desktop:** Creates `MainWindow` with `MainViewModel` as DataContext
- **Browser / Mobile:** Creates `MainView` (a `UserControl`) with `MainViewModel` as DataContext

### Key Dependencies
- Avalonia 11.2.7 with Fluent theme and Inter font
- CommunityToolkit.Mvvm 8.3.2 (source generators for MVVM)
- .NET 8 with nullable enabled and latest C# lang version

### Debugging
Press **F12** while the desktop app is running to open the Avalonia DevTools overlay (included in Debug builds only via conditional `PackageReference`).
