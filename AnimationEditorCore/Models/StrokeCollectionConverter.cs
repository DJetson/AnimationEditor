using System.Text.Json;
using System.Windows.Ink;

namespace AnimationEditorCore.Models
{
    public class StrokeCollectionConverter : System.Text.Json.Serialization.JsonConverter<StrokeCollection>
    {
        public override StrokeCollection Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            using (var stream = new System.IO.MemoryStream(reader.GetBytesFromBase64()))
                return new StrokeCollection(stream);
        }

        public override void Write(Utf8JsonWriter writer, StrokeCollection value, JsonSerializerOptions options)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                value.Save(stream);
                writer.WriteBase64StringValue(stream.GetBuffer());
            }
        }
    }
}
