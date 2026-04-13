using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SqlSugar.Extensions
{
    public static class SugarQueryableExtensions
    {
        public static async Task<T> FirstRequiredAsync<T>(this ISugarQueryable<T> queryable, string businessKey = null)
            where T : class, new()
        {
            var entity = await queryable.FirstAsync();
            if (entity == null)
            {
                ThrowNotFound(queryable, businessKey);
            }
            return entity;
        }

        public static async Task<T> FirstRequiredAsync<T>(this ISugarQueryable<T> queryable, Expression<Func<T, bool>> expression)
            where T : class, new()
        {
            var entity = await queryable.FirstAsync(expression);
            if (entity == null)
            {
                ThrowNotFound(queryable, expression);
            }
            return entity;
        }


        public static T InSingleRequired<T>(this ISugarQueryable<T> queryable, object pkValue)
            where T : class, new()
        {
            var entity = queryable.InSingle(pkValue);
            if (entity == null)
            {
                ThrowNotFound(queryable, pkValue.ToString());
            }
            return entity;
        }

        public static async Task<T> InSingleRequiredAsync<T>(this ISugarQueryable<T> queryable, object pkValue)
            where T : class, new()
        {
            var entity = await queryable.InSingleAsync(pkValue);
            if (entity == null)
            {
                ThrowNotFound(queryable, pkValue.ToString());
            }
            return entity;
        }

        public static async Task<T> InSingleAsync<T>(this ISugarQueryable<T> queryable, object pkValue)
        {
            Check.Exception(queryable.QueryBuilder.SelectValue.HasValue(), "'InSingle' and' Select' can't be used together,You can use .Select(it=>...).Single(it.id==1)");
            List<T> list = await queryable.In<object>(pkValue).ToListAsync();
            T obj = list != null ? list.SingleOrDefault<T>() : default(T);
            list = (List<T>)null;
            return obj;
        }

        private static void ThrowNotFound<T>(
            ISugarQueryable<T> query,
            string businessKey)
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

        private static string GetSqlString<T>(ISugarQueryable<T> query)
        {
            string sql = null;

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
    }
}

