using SqlSugar;

namespace ClassLibrary1
{
    public class Class1
    {
        public void Aaa()
        {
            var client = new SqlSugarClient(new ConnectionConfig());
            var sql = client.Queryable<object>().ToSql();
            // EasySharp.SqlSugarCore.Extensions.4.2.1.9
        }
    }
}
