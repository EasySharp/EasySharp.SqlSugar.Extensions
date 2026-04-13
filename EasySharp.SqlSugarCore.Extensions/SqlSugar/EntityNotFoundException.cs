using System;
using System.Runtime.Serialization;

namespace SqlSugar
{
    [Serializable]
    public class SqlSugarEntityNotFoundException : InvalidOperationException
    {
        public Type EntityType { get; }
        public string Predicate { get; }
        public string Sql { get; }

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

        protected SqlSugarEntityNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            EntityType = (Type)info.GetValue(nameof(EntityType), typeof(Type))!;
            Predicate = info.GetString(nameof(Predicate));
            Sql = info.GetString(nameof(Sql));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(EntityType), EntityType);
            info.AddValue(nameof(Predicate), Predicate);
            info.AddValue(nameof(Sql), Sql);
        }

        private static string BuildMessage(Type type, string? predicate, string? sql)
        {
            const int maxPredicateLength = 200;
            const int maxSqlLength = 500;

            var message = $"Entity '{type.FullName}' was not found.";

            if (!string.IsNullOrEmpty(predicate))
            {
                predicate = predicate.Length > maxPredicateLength
                    ? predicate[..maxPredicateLength] + "..."
                    : predicate;
                message += $"\nPredicate: {predicate}";
            }

            if (!string.IsNullOrEmpty(sql))
            {
                sql = sql.Length > maxSqlLength
                    ? sql[..maxSqlLength] + "..."
                    : sql;
                message += $"\nSQL: {sql}";
            }

            return message;
        }
    }
}
