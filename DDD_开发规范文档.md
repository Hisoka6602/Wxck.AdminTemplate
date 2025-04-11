# 📘 Wxck 授权管理平台 - 开发规范文档

本开发文档用于指导团队成员在 Wxck 授权管理平台项目中，按照统一的标准进行开发，确保项目结构清晰、职责明确、代码易于维护和扩展。

---

## 🏗 架构分层说明

本项目采用 **DDD 分层架构**，划分为以下层级，各层职责如下：

- **Domain**：领域层，负责定义业务实体、接口和领域服务，不依赖任何外部资源。
- **Application**：应用层，协调业务流程和用例，封装服务逻辑，调用领域服务或仓储接口。
- **Infrastructure**：基础设施层，实现数据持久化、文件系统访问、第三方服务调用等。
- **EntityConfigurations**：数据库结构定义层，包括实体与数据库的映射、迁移配置等。
- **Client / UI**：用户界面层或服务入口，可为 WPF、Web、API 等不同形态。

---

## 📂 项目结构与命名规范

| 模块    | 功能描述        | 文件夹路径                                        | 命名规范                     |
| ----- | ----------- | -------------------------------------------- | ------------------------ |
| 实体模型  | 定义数据库实体     | `Domain/Entities`                            | 类名统一以 `xxxModel` 结尾      |
| 数据上下文 | 配置实体映射与关系   | `EntityConfigurations/xxxContext.cs`         | 映射写在 `OnModelCreating` 中 |
| 数据迁移  | 添加和管理数据迁移文件 | `EntityConfigurations/Code-First/Migrations` | 使用 `dotnet ef` 工具生成      |
| 应用服务  | 编写业务逻辑入口类   | `Application/Services`                       | 类名以 `xxxAppService` 结尾   |
| 仓储接口  | 定义仓储访问接口    | `Domain/Repositories`                        | 类名以 `IxxxRepository` 结尾  |
| 仓储实现  | 实现数据库访问逻辑   | `Infrastructure/Repositories`                | 类名以 `xxxRepository` 结尾   |
| 控制入口  | UI 界面或服务接口层 | `Client` 或 `Host`                            | 遵循具体平台的命名规范              |

> 所有模型类、配置类、接口、服务、仓储、DTO 以及其他核心结构应统一存放在对应的 `Objects` 文件夹下，所有类型名称应使用统一后缀名进行识别与分类。

---

## ✅ 实体映射规范（EntityConfigurations）

在 `xxxContext.cs` 中重写 `OnModelCreating` 方法，配置所有实体的映射关系，需遵循以下规范：

- 每个实体必须设置主键（`.HasKey()`）
- 所有用于查询的字段应建立索引（`.HasIndex()`）
- 实体间关系（如一对一、一对多）必须显式配置（`.HasOne()` / `.WithMany()`）
- 配置中应包含是否唯一、排序方式等详细信息（如 `IsUnique(true)`、`IsDescending(true)`）
- 必须使用 `builder.Entity<xxx>().ToTable("xxx")` 指定表名，禁止默认表名

### 示例：

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    {
        // 用户相关实体配置
        modelBuilder.Entity<UserInfoModel>()
            .ToTable("UserInfo")
            .HasKey(c => new { c.Id });
        modelBuilder.Entity<UserInfoModel>()
            .HasIndex(b => b.UserCode).IsUnique(true).IsDescending(true);
        modelBuilder.Entity<UserInfoModel>()
            .HasIndex(b => b.CreatedTime).IsUnique(false).IsDescending(true);

        modelBuilder.Entity<UserVipInfoModel>()
            .ToTable("UserVipInfo")
            .HasKey(c => new { c.Id });

        modelBuilder.Entity<UserInfoModel>()
            .HasOne(b => b.UserVipInfo)
            .WithOne(n => n.UserInfo)
            .HasForeignKey<UserVipInfoModel>(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    {
        // 日志相关实体配置
        modelBuilder.Entity<OperationLogInfoModel>()
            .ToTable("OperationLogInfo")
            .HasKey(c => new { c.Id });
        modelBuilder.Entity<OperationLogInfoModel>()
            .HasIndex(b => b.CreatedTime).IsUnique(false).IsDescending(true);
        modelBuilder.Entity<OperationLogInfoModel>()
            .HasIndex(b => b.OperationTime).IsUnique(false).IsDescending(true);
        modelBuilder.Entity<OperationLogInfoModel>()
            .HasIndex(b => b.OperatorId).IsUnique(false);
        modelBuilder.Entity<OperationLogInfoModel>()
            .HasIndex(b => b.RequestMethod).IsUnique(false);
    }

    base.OnModelCreating(modelBuilder);
}
```

---

## 🔁 添加数据迁移规范（Code-First）

迁移操作集中在 `EntityConfigurations/Code-First/Migrations` 目录下，禁止散乱保存。

### 添加迁移步骤：

```bash
dotnet ef migrations add YourMigrationName --output-dir EntityConfigurations/Code-First/Migrations
dotnet ef database update
```

### 要求：

- 每次数据库结构修改后，必须立即创建对应迁移文件
- 迁移文件需提交至版本控制（Git）
- 禁止手动修改数据库结构（避免与代码配置不一致）

---

## 🚀 新业务功能开发流程（全体成员必须遵循）

1. **定义实体类**：在 `Domain/Entities` 中新增实体类 `xxxModel.cs`
2. **配置实体映射**：在 `xxxContext.cs` 的 `OnModelCreating` 中添加实体关系配置，并显式使用 `ToTable()` 指定表名
3. **添加数据迁移**：在 `EntityConfigurations/Code-First/Migrations` 中通过命令添加并更新数据库
4. **定义仓储接口**：在 `Domain/Repositories` 中定义 `IxxxRepository` 接口
5. **实现数据访问逻辑**：在 `Infrastructure/Repositories` 中使用 EF 实现 `xxxRepository`
6. **编写应用服务类**：在 `Application/Services` 中添加 `xxxAppService`
7. **集成至界面或接口**：在 `Client` 或 `Host` 中调用 Application 层服务
8. **提交代码前审查**：确保命名规范、目录结构正确，注释清晰，日志完整，并提交 PR

---

## 🚫 禁止事项

为确保系统架构一致性，请勿违反以下规范：

- ❌ 禁止绕过 Application 和 Repository 层直接访问数据库
- ❌ 禁止在 Controller 或前端直接编写业务逻辑
- ❌ 禁止在 Domain 层引用 EF Core 或基础设施类库
- ❌ 禁止散乱保存迁移文件
- ❌ 禁止提交包含调试路径或绝对路径的代码
- ❌ 禁止省略 `ToTable()` 显式配置表名（避免默认表名影响迁移兼容性）

---

## 🛠 推荐补充措施

为进一步提升开发协作效率，建议项目包含以下工具与文件：

- `CONTRIBUTING.md`：编写贡献说明，放置于项目根目录
- `.editorconfig`：统一代码风格配置
- Git Hooks：通过 hook 自动阻止不符合规范的提交
- 自动脚本：提供一键构建、迁移、初始化数据库的脚本（PowerShell 或 .bat）

---

## ✅ 最终说明

本规范是所有项目成员必须遵循的标准，旨在提高协作效率、降低维护成本、保障系统质量。每位开发者在编码前务必熟悉并严格执行本规范，如有异议请提前沟通确认。

让我们共同打造高质量、可维护、可扩展的授权管理平台。

