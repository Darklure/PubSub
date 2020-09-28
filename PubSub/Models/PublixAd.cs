using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PubSub.Models.GraphQL.PublixAd
{
    public partial class Welcome
    {
        [JsonProperty("promotion")]
        public Promotion Promotion { get; set; }
    }

    public partial class Promotion
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonProperty("saleStartDateString")]
        public string SaleStartDateString { get; set; }

        [JsonProperty("saleEndDateString")]
        public string SaleEndDateString { get; set; }

        [JsonProperty("postStartDateString")]
        public string PostStartDateString { get; set; }

        [JsonProperty("previewPostStartDateString")]
        public string PreviewPostStartDateString { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("pages")]
        public Page[] Pages { get; set; }

        [JsonProperty("rollovers")]
        public Rollover[] Rollovers { get; set; }
    }

    public partial class Page
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int Id { get; set; }

        [JsonProperty("imageURL")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("order")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int Order { get; set; }
    }

    public class Rollover
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int ID { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("deal")]
        public string Deal { get; set; }
    }

    public partial class Welcome
    {
        public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(int) || t == typeof(int?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            int l;
            if (int.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type int");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (int)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
