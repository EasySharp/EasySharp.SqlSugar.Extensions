using System;

namespace EasySharp.SqlSugarCore.Extensions;

internal static class ValidateExtensions
{
    public static bool HasValue(this object? thisValue)
    {
        return thisValue != null && thisValue != DBNull.Value && thisValue.ToString() != "";
    }

    public static bool IsNullOrEmpty(this object? thisValue)
    {
        return thisValue == null || thisValue == DBNull.Value || thisValue.ToString() == "";
    }
}