using System.IO;
using System.Windows;
using System.Xml;

namespace BingedIt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

    }
    public static class Extension
    {
        public static void ToFileAsXaml(object xamlObject, string? fileName = null)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings() { Indent = true };
            FileStream fStream = new(string.Concat(fileName ?? "out", ".xaml"), FileMode.OpenOrCreate);
            using XmlWriter writer = XmlWriter.Create(fStream, xmlSettings);
            System.Windows.Markup.XamlWriter.Save(xamlObject, writer);
            writer.Flush();

        }
    }
}
