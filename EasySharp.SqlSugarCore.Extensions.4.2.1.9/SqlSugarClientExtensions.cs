namespace SqlSugar.Extensions
{
    internal static class SqlSugarClientExtensions
    {
        public static SqlSugarClient CopyContext(this SqlSugarClient client, ConnectionConfig config)
        {
            var newClient = new SqlSugarClient(config);
            newClient.MappingColumns = client.Context.MappingColumns;
            newClient.MappingTables = client.Context.MappingTables;
            newClient.IgnoreColumns = client.Context.IgnoreColumns;
            return newClient;
        }
    }
}
