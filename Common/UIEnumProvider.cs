using System;
using System.Windows;
using BingedIt.Models;

namespace BingedIt.Common
{
    public static class UIEnumProvider
    {
        private static UIEnumModel UIEnumModelConverter(string enumName)
        {
            string resourceName = string.Concat(enumName, "UI");
            string pathData = Application.Current.TryFindResource(resourceName) as string ?? string.Empty;
            return new UIEnumModel(enumName, pathData);
        }

        public static UIEnumModel[] GetNames(Type enumType)
        {
            string[] enumNames = Enum.GetNames(enumType);
            return Array.ConvertAll(enumNames, UIEnumModelConverter);
        }
    }
}
