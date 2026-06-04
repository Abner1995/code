---
name: git
description: 自动生成 conventional commit 提交信息并推送到 Gitee
---

# Git 自动提交并推送

自动分析 git diff 和 staged changes，按项目规范生成提交信息，提交并推送到 Gitee。

## 执行步骤

### 1. 查看当前状态

```bash
git status
```

```bash
git diff --stat
```

```bash
git diff --cached --stat
```

### 2. 分析变更生成提交信息

根据 diff 内容，遵循项目已有的 commit 风格：

- 格式：`<type>(<scope>): <描述>`
- type: `feat` / `fix` / `refactor` / `docs` / `style` / `chore` / `build`
- scope: 变更涉及的项目或模块
- 描述：一句话说明做了什么

**项目 scope 参考：**

| scope | 对应项目 |
|-------|---------|
| `api-gateway` | YARP 网关 (EShopMicroservices) |
| `catalog` | Catalog.API 产品服务 |
| `basket` | Basket.API 购物车服务 |
| `discount` | Discount.Grpc 折扣服务 |
| `ordering` | Ordering.API 订单服务 |
| `identity` | Identity.API 身份认证服务 |
| `shopping-web` | Shopping.Web 前端 |
| `messaging` | 消息传递模块 (MassTransit/RabbitMQ) |
| `avaviewui` | Avalonia UI 跨平台应用 |
| `stock` | Python 股票数据分析 |
| `clean-architecture` | CleanArchitecture 项目 |
| `contact` | Contact 通讯录项目 |
| `contactsms` | ContactSMS WebAPI 项目 |
| `todomicroservices` | TodoMicroservices 项目 |

**已有提交参考：**

```
feat(api-gateway): 添加用户身份信息到请求头
feat(stock): 新增股票数据分析项目并配置环境
feat(basket): 添加购物车API v2版本端点
refactor(ordering-api): 移除v2端点中未使用的命名空间导入
feat(identity): 添加身份认证服务和相关配置
feat(shopping-web): 添加购物车功能和商品页面
build(Ordering.Application): 添加 Microsoft.EntityFrameworkCore 包引用
```

### 3. 提交前验证

**如果变更包含 C# 项目文件，检查 build：**

```bash
dotnet build <sln-path> 2>&1 | tail -5
```

build 失败时停止提交流程，先修复问题。

### 4. 提交

```bash
git add <files>
```

```bash
git commit -m "$(cat <<'EOF'
<type>(<scope>): <描述>
EOF
)"
```

### 5. 推送到 Gitee

```bash
git push origin master
```

推送前确认远程地址：`origin  https://gitee.com/abner1995/code.git`

## 注意事项

- 不要提交 `.env`、`bin/`、`obj/`、`node_modules/`、`.vs/`、`envs/` 等文件
- 不要 `git add -A` 或 `git add .`，始终明确指定文件
- 推送前确认用户意图（force push 绝不执行）
- 如果有未跟踪文件，询问用户是否纳入提交
- commit message 遵循 conventional commits 规范
- 不要提交 `CLAUDE.md` 和 `.claude/` 目录下的文件（这些由 Claude 管理）
