namespace BingedIt.Models
{
    public class UIEnumModel
    {
        private readonly string _enumName;
        private readonly string _geometryData;

        public string EnumName => _enumName;
        public string GeometryData => _geometryData;

        public UIEnumModel(string enumName, string geometryData)
        {
            _enumName = enumName;
            _geometryData = geometryData;
        }
    }
}
