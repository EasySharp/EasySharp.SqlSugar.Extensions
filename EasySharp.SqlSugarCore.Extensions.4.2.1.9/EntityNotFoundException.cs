using System;

namespace SqlSugar.Extensions
{
    [Serializable]
    public class SqlSugarEntityNotFoundException : InvalidOperationException
    {
        public Type EntityType { get; }
        public string? Predicate { get; }
        public string? Sql { get; }

        public SqlSugarEntityNotFoundException(
            Type entityType,
            string? predicate = null,
            string? sql = null)
            : base(BuildMessage(entityType, predicate, sql))
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            Predicate = predicate;
            Sql = sql;
        }

        public SqlSugarEntityNotFoundException(
            Type entityType,
            string message,
            Exception? innerException = null)
            : base(message, innerException)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            Predicate = null;
            Sql = null;
        }

        private static string BuildMessage(Type type, string? predicate, string? sql)
        {
            const int maxPredicateLength = 200;
            const int maxSqlLength = 500;

            var message = $"Entity '{type.FullName}' was not found.";

            if (!string.IsNullOrEmpty(predicate))
            {
                predicate = predicate.Length > maxPredicateLength
                    ? predicate.Substring(predicate.Length - maxPredicateLength - 1) + "..."
                    : predicate;
                message += $"\nPredicate: {predicate}";
            }

            if (!string.IsNullOrEmpty(sql))
            {
                sql = sql.Length > maxSqlLength
                    ? sql.Substring(sql.Length - maxPredicateLength - 1) + "..."
                    : sql;
                message += $"\nSQL: {sql}";
            }

            return message;
        }
    }
}