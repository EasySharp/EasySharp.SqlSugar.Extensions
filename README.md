# EasySharp.SqlSugar.Extensions

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

`EasySharp.SqlSugar.Extensions` 是 [SqlSugar](https://github.com/donet5/SqlSugar) ORM 的扩展库，提供了一组强类型的查询扩展方法，用于简化数据库查询操作并增强错误处理。

## 功能特性

- **强类型查询**：提供 `FirstRequiredAsync` 、 `SingleRequiredAsync` 和 `InSingleRequiredAsync` 等扩展方法，确保查询结果存在
- **详细的异常信息**：当实体未找到时，抛出包含实体类型、查询条件 SQL 语句的详细异常
- **支持同步和异步操作**：所有方法都提供同步和异步版本
- **多版本支持**：针对不同 SqlSugar 版本提供兼容包
- 

## 安装

### NuGet 包管理器

```bash
Install-Package EasySharp.SqlSugarCore.Extensions
```

### .NET CLI

```bash
dotnet add package EasySharp.SqlSugarCore.Extensions
```

## 版本兼容性

| 项目名 | SqlSugarCore 版本 | 目标框架 |
|------|--------------|----------|
| EasySharp.SqlSugarCore.Extensions | 5.0.8.2~更高版本 | netstandard2.1 |
| EasySharp.SqlSugarCore.Extensions.5.0.0.5 | 5.0.0.5~5.0.8.1 | netstandard2.1 |
| EasySharp.SqlSugarCore.Extensions.4.5.1 | 4.5.1~5.0.0.5 | netstandard2.0 |
| EasySharp.SqlSugarCore.Extensions.4.3.2.4 | 4.3.2.4~4.5.0.2 | netstandard2.0 |
| EasySharp.SqlSugarCore.Extensions.4.2.1.9 | 4.2.1.9~4.2.3.3 | netstandard1.6 |
| EasySharp.SqlSugarCore.Extensions.4.0.0.3 | 4.0.0.3~4.2.1.8 | netstandard1.6 |

## 使用方法

### 1. 确保查询结果存在

使用 `FirstRequiredAsync` 方法查询单条记录，如果记录不存在则抛出 `SqlSugarEntityNotFoundException`：

```csharp
using SqlSugar.Extensions;

// 根据条件查询
var user = await db.Queryable<User>()
    .Where(u => u.Id == 1)
    .FirstRequiredAsync();
    
// 根据条件查询
var user = await db.Queryable<User>()
    .Where(u => u.Id == 1)
    .SingleRequiredAsync();

// 如果记录不存在则抛出带有业务键的异常
var order = await db.Queryable<Order>()
    .FirstRequiredAsync("Order-2024-001");
```

### 2. 根据主键查询

使用 `InSingleRequired` 或 `InSingleRequiredAsync` 根据主键查询：

```csharp
// 同步版本
var user = db.Queryable<User>().InSingleRequired(1);

// 异步版本
var user = await db.Queryable<User>().InSingleRequiredAsync(1);
```

### 3. 异常处理

当实体未找到时，会抛出 `SqlSugarEntityNotFoundException`，包含以下信息：

- **EntityType**: 实体类型
- **Predicate**: 查询条件
- **Sql**: 执行的 SQL 语句

```csharp
try
{
    var user = await db.Queryable<User>()
        .FirstRequiredAsync(u => u.Id == 999);
}
catch (SqlSugarEntityNotFoundException ex)
{
    Console.WriteLine($"实体类型: {ex.EntityType}");
    Console.WriteLine($"查询条件: {ex.Predicate}");
    Console.WriteLine($"SQL: {ex.Sql}");
}
```

## API 参考

### SugarQueryableExtensions

| 方法 | 描述 |
|------|------|
| `FirstRequired<T>()` | 同步获取第一条记录，不存在则抛出 `SqlSugarEntityNotFoundException` |
| `FirstRequiredAsync<T>()` | 异步获取第一条记录，不存在则抛出 `SqlSugarEntityNotFoundException` |
| `FirstRequired<T>(Expression<Func<T, bool>>)` | 根据条件同步获取第一条记录，不存在则抛出 `SqlSugarEntityNotFoundException` |
| `FirstRequiredAsync<T>(Expression<Func<T, bool>>)` | 根据条件异步获取第一条记录，不存在则抛出 `SqlSugarEntityNotFoundException` |
| `SingleRequired<T>()` | 同步获取一条记录，如果记录超过一条则抛出 `SqlSugarException`，不存在则抛出 `SqlSugarEntityNotFoundException` |
| `SingleRequiredAsync<T>()` | 异步获取一条记录，如果记录超过一条则抛出 `SqlSugarException`，不存在则抛出 `SqlSugarEntityNotFoundException` |
| `SingleRequired<T>(Expression<Func<T, bool>>)` | 根据条件同步获取一条记录，如果记录超过一条抛出 `SqlSugarException`，不存在则抛出 `SqlSugarEntityNotFoundException` |
| `SingleRequiredAsync<T>(Expression<Func<T, bool>>)` | 根据条件异步获取一条记录，如果记录超过一条抛出 `SqlSugarException`，不存在则抛出 `SqlSugarEntityNotFoundException` |
| `InSingleRequired<T>(object pkValue)` | 根据主键获取记录，不存在则抛出 `SqlSugarEntityNotFoundException` |
| `InSingleRequiredAsync<T>(object pkValue)` | 异步根据主键获取记录，不存在则抛出 `SqlSugarEntityNotFoundException` |

### SqlSugarEntityNotFoundException

| 属性 | 类型 | 描述 |
|------|------|------|
| `EntityType` | `Type` | 实体类型 |
| `Predicate` | `string?` | 查询条件 |
| `Sql` | `string?` | SQL 语句 |

## 依赖项

- [SqlSugarCore](https://www.nuget.org/packages/SqlSugarCore/) (4.0.0.3 或更高版本)

## 许可证

本项目采用 [MIT](LICENSE) 许可证开源。