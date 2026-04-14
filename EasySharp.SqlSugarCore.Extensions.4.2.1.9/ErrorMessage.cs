using System.Collections.Generic;

namespace EasySharp.SqlSugarCore.Extensions;

internal class ErrorMessage
{
    internal static string GetThrowMessage(
        string enMessage,
        string cnMessage,
        params string[] args)
    {
        var list = new List<string>
        {
            enMessage,
            cnMessage
        };
        list.AddRange(args);
        return string.Format("\r\n English Message : {0}\r\n Chinese Message : {1}", list.ToArray());
    }
}
