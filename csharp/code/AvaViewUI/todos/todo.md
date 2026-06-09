# AvaViewUI 组件画廊 — 开发计划

> 在此文件中详细描述每个待开发任务。任务完成后在行首打 `[x]`。

参考开源项目：
- D:\dotnet\Semi.Avalonia-main
- D:\dotnet\Material.Avalonia-master

## 当前任务

### 1. 实现 Button 第一块 Demo：4 主题 × 7 色 = 28 个按钮

参考: `D:\dotnet\Semi.Avalonia-main\demo\Semi.Avalonia.Demo\Pages\ButtonDemo.axaml` (第 20-81 行)

**子任务：**
- [ ] 1a. Palette.axaml: 补 `AvaColorWhite`
- [x] 1b. ButtonTokens.axaml: 补 Solid / Outline / Borderless 三种主题的 Token
- [ ] 1c. ButtonStyles.axaml: 补 `SolidButton` / `OutlineButton` / `BorderlessButton` 三个 ControlTheme
- [ ] 1d. ButtonDemoView.axaml: 改为带 Header 的 4 行 × 7 按钮布局

## 已完成

- [x] Button 基础 ControlTheme (Light 默认主题)
- [x] ButtonTokens: Light 主题 Token + Palette 语义色板
- [x] ButtonDemoView 第一行 (Light 主题 8 按钮)

## 待定 / 想法
