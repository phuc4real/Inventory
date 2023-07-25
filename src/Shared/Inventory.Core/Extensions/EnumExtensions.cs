using System.ComponentModel;
using System.Reflection;

namespace Inventory.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string? ToDescriptionString<TEnum>(this TEnum _enum)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            FieldInfo info = _enum!.GetType().GetField(_enum.ToString()!);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            var attributes = (DescriptionAttribute[])info!.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Any() ? attributes[0].Description : _enum.ToString();
        }
    }
}
