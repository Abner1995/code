# /avalonia — Avalonia UI 学习助手

## 这是什么？

`/avalonia` 是一个 Claude Code skill，专门为**入门学习 Avalonia UI** 设计。它会让 AI 切换为**导师模式**——只给思路和建议，不替写代码。

## 为什么需要它？

直接用 AI 写代码，代码是 AI 写的，不是你的。真正学会 Avalonia 需要：
- 理解布局系统的设计思路
- 掌握 MVVM 的数据流
- 亲手踩坑、亲手改 bug

AI 的作用是**指路**，不是**替你走路**。

## 如何使用

在 Claude Code 中输入：

```
/avalonia
```

然后描述你的需求。根据场景不同，AI 会给出不同形式的帮助。

### 场景 1：开发一个功能

**示例输入：**

```
/avalonia 我想做一个左边菜单、右边内容的布局，菜单点击后右边切换不同页面，怎么做？
```

**AI 会回答：**

- 推荐用什么 Panel（Grid / SplitView / DockPanel），为什么
- ViewModel 里需要哪些属性（比如 `SelectedMenuItem`）
- 内容切换用 ContentControl 还是其他方案
- 数据结构怎么设计

**AI 不会回答：**

- 一份可以直接复制粘贴的 XAML 代码
- 完整的 ViewModel.cs 文件

### 场景 2：遇到报错 / 问题

**示例输入：**

```
/avalonia 我的 ListBox 点了之后右边 ContentControl 没变化，绑定好像没生效
```

**AI 会回答：**

- 分析可能的原因（DataContext 没传下去？属性没通知变更？编译绑定没配？）
- 排查步骤
- 修复方向

### 场景 3：问方案建议

**示例输入：**

```
/avalonia 我要做一个数据表格，Avalonia 里用什么方案比较好？
```

**AI 会回答：**

- DataGrid 官方控件 vs 自建 ListBox + Grid vs 第三方库
- 各方案的优缺点
- 你的场景适合哪个

### 场景 4：代码审阅

把你写的代码贴出来（不要附图片，文本格式），问：

```
/avalonia 帮我看看这段布局代码有没有改进空间
```

AI 会逐项指出问题和改进方向。

## 可用知识领域

| 领域 | 内容 |
|------|------|
| 布局 | Grid, StackPanel, DockPanel, WrapPanel, SplitView |
| MVVM | CommunityToolkit.Mvvm, ObservableProperty, RelayCommand |
| 绑定 | 编译绑定, DataContext, 转换器 |
| 样式 | Style, ControlTheme, 资源字典 |
| 模板 | DataTemplate, ControlTemplate |
| 事件 | 路由事件, 命令绑定 |
| 跨平台 | Desktop / Browser / iOS / Android |

## 注意事项

- 如果你确实需要某段关键代码（不超过 5 行），可以明确说"帮我写这段"，AI 会写并逐行解释
- skill 文件位置：`.claude/skills/avalonia/SKILL.md`，可以自己调整规则
- 建议配合 Avalonia 官方文档一起学习
