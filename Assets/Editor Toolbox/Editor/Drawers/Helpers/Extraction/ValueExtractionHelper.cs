using System.Collections.Generic;

namespace Toolbox.Editor.Drawers
{
    public static class ValueExtractionHelper
    {
        private static readonly List<IValueExtractor> extractors = new List<IValueExtractor>()
        {
            new FieldValueExtractor(),
            new PropertyValueExtractor(),
            new MethodValueExtractor()
        };


        public static bool TryGetValue(string source, object declaringObject, out object value)
        {
            for (var i = 0; i < extractors.Count; i++)
            {
                if (extractors[i].TryGetValue(source, declaringObject, out value))
                {
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}