using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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

    public static string ToSqlString<T>(this ISugarQueryable<T> query)
    {
        return query.ToSql().Key;
    }

    public static async Task<T?> InSingleAsync<T>(this ISugarQueryable<T> queryable, object pkValue)
    {
        Check.Exception(queryable.SqlBuilder.QueryBuilder.SelectValue.HasValue(), "'InSingle' and' Select' can't be used together,You can use .Select(it=>...).Single(it.id==1)");
        var list = await queryable.In(pkValue).ToListAsync();
        return list == null ? default : list.SingleOrDefault();
    }

    public static async Task<List<T>?> ToListAsync<T>(this ISugarQueryable<T> queryable)
    {
        var result = new Task<List<T>>(() =>
        {
            var asyncQueryable = CopyQueryable(queryable);
            return asyncQueryable.ToList();
        });
        result.Start();
        return await result;
    }

    private static ISugarQueryable<T> CopyQueryable<T>(ISugarQueryable<T> queryable)
    {
        var asyncContext = queryable.Context.CopyContext(queryable.Context.RewritableMethods.TranslateCopy(queryable.Context.CurrentConnectionConfig));
        asyncContext.CurrentConnectionConfig.IsAutoCloseConnection = true;
        asyncContext.Ado.IsEnableLogEvent = queryable.Context.Ado.IsEnableLogEvent;
        asyncContext.Ado.LogEventStarting = queryable.Context.Ado.LogEventStarting;
        asyncContext.Ado.LogEventCompleted = queryable.Context.Ado.LogEventCompleted;
        asyncContext.Ado.ProcessingEventStartingSQL = queryable.Context.Ado.ProcessingEventStartingSQL;

        var asyncQueryable = asyncContext.Queryable<ExpandoObject>().Select<T>(string.Empty);
        var asyncQueryableBuilder = asyncQueryable.SqlBuilder.QueryBuilder;
        asyncQueryableBuilder.Take = queryable.SqlBuilder.QueryBuilder.Take;
        asyncQueryableBuilder.Skip = queryable.SqlBuilder.QueryBuilder.Skip;
        asyncQueryableBuilder.SelectValue = queryable.SqlBuilder.QueryBuilder.SelectValue;
        asyncQueryableBuilder.WhereInfos = queryable.SqlBuilder.QueryBuilder.WhereInfos;
        asyncQueryableBuilder.EasyJoinInfos = queryable.SqlBuilder.QueryBuilder.EasyJoinInfos;
        asyncQueryableBuilder.JoinQueryInfos = queryable.SqlBuilder.QueryBuilder.JoinQueryInfos;
        asyncQueryableBuilder.WhereIndex = queryable.SqlBuilder.QueryBuilder.WhereIndex;
        asyncQueryableBuilder.EntityType = queryable.SqlBuilder.QueryBuilder.EntityType;
        asyncQueryableBuilder.EntityName = queryable.SqlBuilder.QueryBuilder.EntityName;
        asyncQueryableBuilder.Parameters = queryable.SqlBuilder.QueryBuilder.Parameters;
        asyncQueryableBuilder.TableShortName = queryable.SqlBuilder.QueryBuilder.TableShortName;
        asyncQueryableBuilder.TableWithString = queryable.SqlBuilder.QueryBuilder.TableWithString;
        return asyncQueryable;
    }

    public static async Task<T?> FirstAsync<T>(this ISugarQueryable<T> queryable, Expression<Func<T, bool>> expression)
    {
        return await queryable.Where(expression).FirstAsync();
    }

    public static async Task<T?> FirstAsync<T>(this ISugarQueryable<T> queryable)
    {
        if (queryable.SqlBuilder.QueryBuilder.OrderByValue.IsNullOrEmpty())
            queryable.SqlBuilder.QueryBuilder.OrderByValue = queryable.SqlBuilder.QueryBuilder.DefaultOrderByTemplate;
        queryable.SqlBuilder.QueryBuilder.Skip ??= 0;
        queryable.SqlBuilder.QueryBuilder.Take = 1;
        var list = await queryable.ToListAsync();
        return list != null ? list.FirstOrDefault() : default;
    }

    public static async Task<T?> SingleAsync<T>(this ISugarQueryable<T> queryable, Expression<Func<T, bool>> expression)
    {
        return await queryable.Where(expression).SingleAsync();
    }

    public static async Task<T?> SingleAsync<T>(this ISugarQueryable<T> queryable)
    {
        if (queryable.SqlBuilder.QueryBuilder.OrderByValue.IsNullOrEmpty())
            queryable.SqlBuilder.QueryBuilder.OrderByValue = queryable.SqlBuilder.QueryBuilder.DefaultOrderByTemplate;
        var skip = queryable.SqlBuilder.QueryBuilder.Skip;
        var take = queryable.SqlBuilder.QueryBuilder.Take;
        var orderByValue = queryable.SqlBuilder.QueryBuilder.OrderByValue;
        queryable.SqlBuilder.QueryBuilder.Skip = null;
        queryable.SqlBuilder.QueryBuilder.Take = null;
        queryable.SqlBuilder.QueryBuilder.OrderByValue = null;
        var list = await queryable.ToListAsync();
        queryable.SqlBuilder.QueryBuilder.Skip = skip;
        queryable.SqlBuilder.QueryBuilder.Take = take;
        queryable.SqlBuilder.QueryBuilder.OrderByValue = orderByValue;
        if (list == null || list.Count == 0)
            return default;
        if (list.Count < 2)
            return list.SingleOrDefault();
        Check.Exception(true, ErrorMessage.GetThrowMessage(".SingleAsync()  result must not exceed one . You can use.First()", "使用single查询结果集不能大于1，适合主键查询，如果大于1你可以使用Queryable.First"));
        return default;
    }
}