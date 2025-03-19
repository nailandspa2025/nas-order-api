using System;
using System.ComponentModel;

namespace BuildingBlocks.Common.Extensions;

//https://stackoverflow.com/a/19621488/581476
public static class EnumExtensions
{
    public static T GetAttribute<T>(this Enum value) where T : Attribute
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return attributes.Length > 0
            ? (T)attributes[0]
            : null;
    }

    public static string ToName(this Enum value)
    {
        var attribute = value.GetAttribute<DescriptionAttribute>();
        return attribute == null ? value.ToString() : attribute.Description;
    }

}
