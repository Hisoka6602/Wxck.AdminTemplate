# 数据迁移操作指南

本文档旨在帮助团队成员执行 Entity Framework Core 的数据库迁移操作，确保数据库结构与实体模型同步。

---

## 1. 前置准备

确保你已完成以下配置：

- 安装 .NET SDK（建议使用与项目一致的版本）
- 安装 EF Core 工具（如未安装，请运行以下命令）：


dotnet tool install --global dotnet-ef

---
## 2. 常用命令

# 添加迁移
dotnet ef migrations add <迁移名称>
# 示例
dotnet ef migrations add AddUserTable

# 更新数据库
dotnet ef database update

-  调用完添加以后需要执行更新数据库的命令才会生效
	- 任何的修改都需要执行更新数据库的命令
# 删除最近一次迁移
dotnet ef migrations remove

- 适用于未执行update

# 指定上下文和项目

dotnet ef migrations add <迁移名称> --context <上下文名称> --project <项目名称>
# 示例
dotnet ef migrations add AddUserTable --context SqlServerContext --project MyProject

dotnet ef database update --context SqlServerContext --project MyProject

dotnet ef migrations remove --context SqlServerContext --project MyProject



