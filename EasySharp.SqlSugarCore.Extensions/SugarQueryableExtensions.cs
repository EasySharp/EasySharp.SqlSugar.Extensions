using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SqlSugar;

namespace EasySharp.SqlSugarCore.Extensions;

public static class SugarQueryableExtensions
{
    public static T FirstRequired<T>(this ISugarQueryable<T> queryable, string? businessKey = null)
        where T : class, new()
    {
        var entity = queryable.First();
        if (entity == null)
        {
            ThrowNotFound(queryable, businessKey);
        }
        return entity!;
    }

    public static async Task<T> FirstRequiredAsync<T>(this ISugarQueryable<T> queryable, string? businessKey = null)
        where T : class, new()
    {
        var entity = await queryable.FirstAsync();
        if (entity == null)
        {
            ThrowNotFound(queryable, businessKey);
        }
        return entity!;
    }

    public static T FirstRequired<T>(this ISugarQueryable<T> queryable, Expression<Func<T, bool>> expression)
        where T : class, new()
    {
        var entity = queryable.First(expression);
        if (entity == null)
        {
            ThrowNotFound(queryable, expression);
        }
        return entity!;
    }

    public static async Task<T> FirstRequiredAsync<T>(this ISugarQueryable<T> queryable, Expression<Func<T, bool>> expression)
        where T : class, new()
    {
        var entity = await queryable.FirstAsync(expression);
        if (entity == null)
        {
            ThrowNotFound(queryable, expression);
        }
        return entity!;
    }

    public static T SingleRequired<T>(this ISugarQueryable<T> queryable, string? businessKey = null)
        where T : class, new()
    {
        var entity = queryable.Single();
        if (entity == null)
        {
            ThrowNotFound(queryable, businessKey);
        }
        return entity!;
    }

    public static async Task<T> SingleRequiredAsync<T>(this ISugarQueryable<T> queryable, string? businessKey = null)
        where T : class, new()
    {
        var entity = await queryable.SingleAsync();
        if (entity == null)
        {
            ThrowNotFound(queryable, businessKey);
        }
        return entity!;
    }

    public static T SingleRequired<T>(this ISugarQueryable<T> queryable, Expression<Func<T, bool>> expression)
        where T : class, new()
    {
        var entity = queryable.Single(expression);
        if (entity == null)
        {
            ThrowNotFound(queryable, expression);
        }
        return entity!;
    }

    public static async Task<T> SingleRequiredAsync<T>(this ISugarQueryable<T> queryable, Expression<Func<T, bool>> expression)
        where T : class, new()
    {
        var entity = await queryable.SingleAsync(expression);
        if (entity == null)
        {
            ThrowNotFound(queryable, expression);
        }
        return entity!;
    }

    public static T InSingleRequired<T>(this ISugarQueryable<T> queryable, object pkValue)
        where T : class, new()
    {
        var entity = queryable.InSingle(pkValue);
        if (entity == null)
        {
            ThrowNotFound(queryable, pkValue.ToString());
        }
        return entity!;
    }

    public static async Task<T> InSingleRequiredAsync<T>(this ISugarQueryable<T> queryable, object pkValue)
        where T : class, new()
    {
        var entity = await queryable.InSingleAsync(pkValue);
        if (entity == null)
        {
            ThrowNotFound(queryable, pkValue.ToString());
        }
        return entity!;
    }

    private static void ThrowNotFound<T>(
        ISugarQueryable<T> query,
        string? businessKey)
        where T : class, new()
    {
        throw new SqlSugarEntityNotFoundException(
            typeof(T),
            predicate: businessKey ?? "No predicate",
            GetSqlString(query));
    }

    private static void ThrowNotFound<T>(
        ISugarQueryable<T> query,
        Expression<Func<T, bool>> predicate)
        where T : class, new()
    {
        throw new SqlSugarEntityNotFoundException(
            typeof(T),
            predicate.ToString(),
            GetSqlString(query));
    }

    private static string? GetSqlString<T>(ISugarQueryable<T> query)
    {
        string? sql = null;

        try
        {
            sql = query.ToSqlString();
        }
        catch
        {
            // 某些场景 ToSql 可能失败，忽略
        }

        return sql;
    }
}