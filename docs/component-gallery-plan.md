# AvaViewUI 组件画廊开发计划

https://irihitech.github.io/Semi.Avalonia/
https://avaloniacommunity.github.io/Material.Avalonia/

目标：构建类似 Ant Design Vue 组件页面的组件展示应用 —— 左侧分类导航，右侧对应 demo，桌面端和移动端均有良好体验。

## 一、布局方案：SplitView 响应式布局

### 为什么选 SplitView

桌面端（宽屏）侧栏始终可见，移动端（窄屏）侧栏收起为汉堡菜单。SplitView 通过切换 `DisplayMode` 即可同时满足两端需求，不需要写两套布局。

### DisplayMode 自适应策略

| 模式 | 窗口宽度 | 行为 |
|------|---------|------|
| `Inline` | > 768px | 侧栏常驻左侧，内容区被挤压 |
| `Overlay` | ≤ 768px | 侧栏覆盖弹出，点击汉堡按钮或边缘滑动打开 |

### SplitView 结构

```
SplitView
├── Pane（侧栏）
│   └── ListBox  —— 组件分类列表
└── Content（主体）
    ├── HamburgerButton  —— 窄屏时显示，点击打开 Pane
    └── ContentControl   —— 绑定 CurrentDemoVM，ViewLocator 解析
```

### 实现关键点

- ViewModel 中维护 `DisplayMode` 属性，监听窗口 `Bounds` 变化，根据宽度切换模式
- Hamburger 按钮的 `IsVisible` 绑定到 `DisplayMode != Inline`
- Avalonia 在移动端 Overlay 模式自带边缘滑动手势，只需额外处理按钮触发
- `IsPaneOpen` 属性控制 Overlay/CompactOverlay 模式下侧栏的显隐

Side-by-side:
  桌面宽屏                        手机窄屏
  ┌────────┬──────────────────┐    ┌─────────────────────┐
  │ Pane   │ Content          │    │ ☰  Content          │
  │        │                  │    │                     │
  │ ListBox│ ContentControl   │    │ 点击☰ → Pane覆盖弹出  │
  │        │                  │    │                     │
  └────────┴──────────────────┘    └─────────────────────┘

### 内容切换机制（不变）

右侧内容切换利用 ViewLocator 机制：
1. 侧栏 ListBox 选中条目 → 绑定给 `MainViewModel.CurrentDemoVM`
2. 右侧 ContentControl 的 `Content` 绑定到 `CurrentDemoVM`
3. ViewLocator 自动把 ViewModel 解析成对应的 View 并渲染

## 二、目录结构

```
AvaViewUI/
├── ViewModels/
│   ├── MainViewModel.cs          ← 入口，侧栏 + 导航 + DisplayMode 切换
│   └── DemoPages/                ← 每个组件 demo 一个 VM
│       ├── ButtonDemoViewModel.cs
│       ├── MenuDemoViewModel.cs
│       ├── FormDemoViewModel.cs
│       ├── TimelineDemoViewModel.cs
│       └── ...
├── Views/
│   ├── MainView.axaml            ← Gallery 主布局（SplitView）
│   └── DemoPages/                ← 每个组件 demo 一个 View
│       ├── ButtonDemoView.axaml
│       ├── MenuDemoView.axaml
│       ├── FormDemoView.axaml
│       ├── TimelineDemoView.axaml
│       └── ...
├── Models/
│   └── GalleryItem.cs            ← 侧栏条目（名称、图标、类型等）
```

为何按功能分子目录而非全部扁平放：
- ViewLocator 仍能工作（命名空间含 `DemoPages` 即可自动匹配）
- 每加一个组件只需在 `DemoPages/` 下加一对文件，不碰其他代码
- Models 放纯数据模型，与 VM/View 职责分离

## 三、ViewModel 职责

- **MainViewModel**（主控）：
  - 维护侧栏条目列表 `ObservableCollection<GalleryItem>`
  - `CurrentDemoVM` 属性，选中条目时切换
  - `DisplayMode` 属性，根据窗口宽度自动切换 `Inline` / `Overlay`
  - `IsPaneOpen` 属性，窄屏时控制侧栏弹出
- **每个 DemoViewModel**：管自己 demo 的状态（如按钮的点击计数、开关状态等），与主控完全解耦
- **GalleryItem 模型**：分类名称、对应的 ViewModel 类型（通过 `Activator.CreateInstance` 创建实例）

## 四、开发步骤

| 步骤 | 做什么 | 验证方式 |
|------|--------|---------|
| 1. 搭建 SplitView 布局 | 改 `MainView.axaml`，Pane 放 ListBox，Content 放 ContentControl | 桌面端 `dotnet run` 看到左右分栏 |
| 2. 实现响应式切换 | MainViewModel 监听窗口宽度，自动切 DisplayMode + 汉堡按钮显隐 | 拖窄窗口，侧栏变为 Overlay 模式 |
| 3. 实现导航 | MainViewModel 加 GalleryItem 列表 + CurrentDemoVM + 选中切换 | 点击左侧条目，右侧切换 |
| 4. 写第一个 demo | 创建 ButtonDemoView + ButtonDemoViewModel，展示 Button 变体 | 点击 "Button" 看到 demo |
| 5. 逐个加组件 | 每次加一对 VM+View，在列表中加条目 | 逐个验证 |

## 五、跨平台注意事项

- **Desktop / Browser（宽屏）**：SplitView Inline 模式，Side-by-side 布局，和 Ant Design Vue 效果一致
- **Android / iOS（窄屏）**：SplitView Overlay 模式，全屏内容 + 汉堡菜单，原生 App 风格
- **Browser（窄窗口）**：窗口拖窄后自动切换 Overlay，和移动端浏览器体验一致
- **WebAssembly**：Browser 项目的 `ISingleViewApplicationLifetime` 页面本身就是全窗口，SplitView 工作正常

## 六、注意事项

- ViewLocator 按全限定类型名做字符串替换匹配 View。子目录的命名空间必须含对应段：
  - `AvaViewUI.ViewModels.DemoPages.ButtonDemoViewModel`
  - `AvaViewUI.Views.DemoPages.ButtonDemoView`
- SplitView 的 `DisplayMode` 切换时机用窗口 `Bounds.Width` 判断，阈值 768px 是比较通用的分界点
- Overlay 模式下 Pane 默认不打开，需通过 `IsPaneOpen = true` 打开，点击 Pane 外区域或选中条目后关闭
- Browser 项目里的窗口 `Bounds` 实际是浏览器视口大小，响应式逻辑同样适用
