namespace Simple.Web.ContentTypeHandling
{
    using System;
    using System.IO;
    using System.Linq;

    [ContentTypes("application/x-www-form-urlencoded")]
    sealed class FormDeserializer : IContentTypeHandler
    {
        public object Read(Stream inputStream, Type inputType)
        {
            string text;
            using (var streamReader = new StreamReader(inputStream))
            {
                text = streamReader.ReadToEnd();
            }
            var pairs = text.Split('\n').Select(s => Tuple.Create(s.Split('=')[0], Uri.UnescapeDataString(s.Split('=')[1])));
            var obj = Activator.CreateInstance(inputType);
            foreach (var pair in pairs)
            {
                var property = inputType.GetProperty(pair.Item1);
                if (property != null)
                {
                    property.SetValue(obj, Convert.ChangeType(pair.Item2, property.PropertyType), null);
                }
            }
            return obj;
        }

        public void Write(IContent content, Stream outputStream)
        {
            throw new NotImplementedException();
        }
    }
}