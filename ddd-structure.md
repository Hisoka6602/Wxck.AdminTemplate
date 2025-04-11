Wxck.AdminTemplate 项目结构

## 项目结构说明

├── Wxck.AdminTemplate.Analyzer  
│   ├── Wxck.AdminTemplate.Analyzer  # 代码分析器核心（可选）  
│   ├── Wxck.AdminTemplate.Analyzer.CodeFixes  # 代码修复工具（可选）  

├── Wxck.AdminTemplate.Api  
│   ├── Controllers  # Web API 控制器，负责暴露 API 接口（DDD 必需）  
│   ├── Filters  # 用于 API 请求的过滤器（DDD 可选）  
│   ├── Warmup  # 预热功能，通常用于初始化（DDD 可选）  
│   ├── wwwroot  # 静态文件的存放位置（DDD 可选）  

├── Wxck.AdminTemplate.AppHost  
│   ├── Properties  # 启动配置和属性（可选）  

├── Wxck.AdminTemplate.Application  
│   ├── DTOs  # 数据传输对象（DTOs），用于数据传递（DDD 必需）  
│   │   ├── RequestModels  # 请求数据模型（DDD 必需）  
│   │   │   ├── User       # 示例  
│   │   ├── ResponseModels  # 响应数据模型（DDD 必需）  
│   ├── Interfaces  # 服务接口，定义应用层服务（DDD 必需）  
│   ├── Services  # 应用服务，包含业务逻辑（DDD 必需）  
│   │   ├── User  # 例如：用户相关的服务（DDD 必需）  
│   ├── Validators  # 验证器，用于验证数据（DDD 可选）  
│   │   ├── Attributes  # 自定义验证属性（DDD 可选）  
│   │   │   ├── User    # 示例  

├── Wxck.AdminTemplate.CommsCore  
│   ├── DataFormatting  # 数据格式化（DDD 可选）  
│   ├── Encryption  # 数据加密（DDD 可选）  
│   ├── Enums  # 枚举类型（DDD 可选）  
│   │   ├── User # 示例  
│   ├── ErrorHandling  # 错误处理机制（DDD 可选）  
│   ├── Implementations  # 通用实现（DDD 可选）  
│   ├── Interfaces  # 通用接口（DDD 可选）  
│   ├── MessageHandling  # 消息处理（DDD 可选）  
│   ├── SignalR  # SignalR 通信功能（DDD 可选）  
│   ├── Sms  # 短信服务（DDD 可选）  

├── Wxck.AdminTemplate.CrossCutting  
│   ├── Attributes  # 跨领域的自定义属性（DDD 可选）  
│   ├── Auth  # 认证相关功能（DDD 必需）  
│   │   ├── Jwt  # JWT 认证功能（DDD 必需）  
│   ├── Converters  # 数据转换功能（DDD 可选）  
│   ├── EventBus  # 事件总线，用于跨应用通信（DDD 可选）  
│   │   ├── Events  # 事件类（DDD 可选）  
│   ├── Extensions  # 扩展方法（DDD 可选）  
│   ├── Security  # 安全性相关功能（DDD 可选）  
│   │   ├── Signing  # 签名相关（DDD 可选）  
│   ├── Utils  # 实用工具（DDD 可选）  

├── Wxck.AdminTemplate.Domain  
│   ├── Aggregates  # 聚合根，用于定义业务实体的聚合（DDD 必需）  
│   ├── Attributes  # 领域模型相关的自定义属性（DDD 可选）  
│   ├── Entities  # 领域实体，核心业务模型（DDD 必需）  
│   │   ├── Logs  # 示例  
│   │   ├── User  # 示例  
│   ├── Events  # 领域事件，记录领域层的状态变化（DDD 必需）  
│   │   ├── User # 示例  
│   ├── Repositories  # 仓储，持久化领域接口（DDD 必需）  
│   │   ├── Logs  # 示例  
│   │   ├── User  # 示例  
│   ├── Strategies  # 领域策略，定义领域逻辑（DDD 可选）  
│   ├── ValueObjects  # 值对象，表示不可变的数据模型（DDD 必需）  

├── Wxck.AdminTemplate.Infrastructure  
│   ├── Auth  # 认证相关基础设施（DDD 可选）  
│   │   ├── Jwt  # JWT 实现（DDD 可选）  
│   ├── DependencyInjection  # 依赖注入配置（DDD 可选）  
│   ├── EntityConfigurations  # 数据库实体配置（DDD 可选）  
│   │   ├── Code-First  # 代码优先数据库配置（DDD 可选）  
│   ├── ExternalServices  # 外部服务连接（DDD 可选）  
│   ├── Migrations  # 数据库迁移（DDD 可选）  
│   ├── Repositories  # 仓储实现（DDD 必需）  
│   │   ├── Logs # 示例  
│   │   ├── User # 示例  
│   ├── Security  # 安全相关基础设施（DDD 可选）  
│   │   ├── Signing  # 签名实现（DDD 可选）  
│   ├── SignalR  # SignalR 实现（DDD 可选）  
│   ├── Sms  # 短信服务实现（DDD 可选）  

├── Wxck.AdminTemplate.ServiceCluster  
│   ├── BackgroundServices  # 后台服务（DDD 可选）  

├── Wxck.AdminTemplate.ServiceDefaults  # 默认服务配置（DDD 可选）  

---


# Wxck.AdminTemplate 项目开发流程

## 1. 定义实体和领域对象（Domain 层）

- 在开发过程中，首先需要定义**实体**和**领域对象**，这些是系统的核心业务模型。

### 实体(Wxck.AdminTemplate.Domain-Entities)

- 实体类应该代表系统中的核心业务对象，并且通常具有一个唯一标识符（ID）。例如，`User` 类可以包含 `Username`、`Email` 和 `Password` 等字段。

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
```

### 聚合根(Wxck.AdminTemplate.Domain-Aggregates)
- 聚合根是在领域模型中，代表一个业务聚合的主要对象。例如，Order 可以是一个聚合根，包含多个 OrderItem。
```csharp
public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; }
}
```
### 创建领域事件(Wxck.AdminTemplate.Domain-Events)
- 在业务操作时，领域事件用于记录领域对象的状态变化并通知相关的服务或者其他系统。
# 领域事件
- 领域事件通常位于 Wxck.AdminTemplate.Domain.Events 中。例如，当 User 被创建时，可能会生成一个用户创建事件。

```csharp
public class UserCreatedEvent : DomainEvent
{
    public User User { get; set; }
}
```
### 添加实体映射

- 在项目的 EntityConfigurations 目录下，通常会创建一个或多个上下文配置类，负责定义实体到数据库表的映射关系。这些配置通过继承 DbContext 的 OnModelCreating 方法实现。以下是如何在该方法中配置和添加实体的映射、索引、关系和约束等
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // 用户相关
    // 用户信息
    modelBuilder.Entity<UserInfoModel>()
        .HasKey(c => new { c.Id });  // 定义主键

    modelBuilder.Entity<UserInfoModel>()
        .HasIndex(b => b.UserCode)  // 设置索引
        .IsUnique(true)  // 唯一索引
        .IsDescending(true);  // 降序排列

    modelBuilder.Entity<UserInfoModel>()
        .HasIndex(b => b.CreatedTime)
        .IsUnique(false)  // 非唯一索引
        .IsDescending(true);  // 降序排列

    // 用户Vip信息
    modelBuilder.Entity<UserVipInfoModel>()
        .HasKey(c => new { c.Id });  // 定义主键

    // 用户与Vip信息关系
    modelBuilder.Entity<UserInfoModel>()
        .HasOne(b => b.UserVipInfo)  // 关联UserInfoModel与UserVipInfoModel
        .WithOne(n => n.UserInfo)  // 反向关联
        .HasForeignKey<UserVipInfoModel>(n => n.UserId)  // 外键
        .OnDelete(DeleteBehavior.Cascade);  // 删除时级联删除

    // 日志相关
    // 操作日志
    modelBuilder.Entity<OperationLogInfoModel>()
        .HasKey(c => new { c.Id });  // 定义主键

    modelBuilder.Entity<OperationLogInfoModel>()
        .HasIndex(b => b.CreatedTime)  // 设置索引
        .IsUnique(false)  // 非唯一索引
        .IsDescending(true);  // 降序排列

    modelBuilder.Entity<OperationLogInfoModel>()
        .HasIndex(b => b.OperationTime)  // 设置索引
        .IsUnique(false)  // 非唯一索引
        .IsDescending(true);  // 降序排列

    modelBuilder.Entity<OperationLogInfoModel>()
        .HasIndex(b => b.OperatorId)  // 设置索引
        .IsUnique(false);  // 非唯一索引

    modelBuilder.Entity<OperationLogInfoModel>()
        .HasIndex(b => b.RequestMethod)  // 设置索引
        .IsUnique(false);  // 非唯一索引

    // 调用基类方法，确保其他配置生效
    base.OnModelCreating(modelBuilder);
}
```
### 添加数据迁移
- 在项目的 EntityConfigurations/Code-First 目录中，我们遵循 Code-First 方式进行数据库的结构设计与变更管理。通过 EF Core 的迁移机制，可以将实体模型的- 变更自动转换为数据库的结构变更脚本，实现结构同步。
--具体操作在Code-First 目录中的MigrationInstructions.md文件中有详细说明

### 定义仓储接口(Wxck.AdminTemplate.Domain-Repositories)

仓储接口定义了对领域对象的基本操作，如创建、更新、删除和查询。仓储通常位于Wxck.AdminTemplate.Domain.Repositories 文件夹中

--本身已经默认实现基本的增删改查操作,如果需要额外的操作或者事务处理则需要补充定义
```csharp
public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
```
### 实现仓储接口(Wxck.AdminTemplate.Infrastructure.Repositories)

- 仓储接口的实现通常位于 Wxck.AdminTemplate.Infrastructure.Repositories 文件夹中，使用数据库来存储和查询数据
```csharp
public class UserRepository : IUserRepository
{
    private readonly DbContext _context;

    public UserRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
```

### 设计应用服务（Application 层）

- 应用服务是连接领域层和外部世界（如 API）之间的桥梁。它们定义了业务操作的具体实现，并返回数据

# DTOs
- 数据传输对象（DTOs）用于表示从应用服务层传递到 API 层的数据，通常包含请求和响应的模型。
```csharp
public class CreateUserRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
}

public class CreateUserResponse
{
    public int UserId { get; set; }
}
```
# 应用服务

- 应用服务位于 Wxck.AdminTemplate.Application.Services 文件夹中，包含核心的业务逻辑。

```csharp
public class UserAppService : IUserAppService
{
    private readonly IUserService _userService;
    
    public UserAppService(IUserService userService)
    {
        _userService = userService;
    }
    
    public CreateUserResponse CreateUser(CreateUserRequest request)
    {
        var user = new User { Username = request.Username, Email = request.Email };
        _userService.CreateUser(user);
        return new CreateUserResponse { UserId = user.Id };
    }
}
```
### 定义接口（Api 层）
- API 层负责将外部请求转发到应用服务。我们使用控制器来暴露这些接口。
# API 控制器
```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserAppService _userAppService;
    
    public UsersController(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }
    
    [HttpPost]
    public ActionResult<CreateUserResponse> CreateUser([FromBody] CreateUserRequest request)
    {
        var response = _userAppService.CreateUser(request);
        return Ok(response);
    }
}
```