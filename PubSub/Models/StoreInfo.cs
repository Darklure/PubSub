using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PubSub.Models.StoreInfo
{
    public partial class StoreList
    {
        [JsonProperty("Stores", NullValueHandling = NullValueHandling.Ignore)]
        public Store[] Stores { get; set; }
    }

    public partial class Store
    {
        [JsonProperty("KEY", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("NAME", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("ADDR", NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }

        [JsonProperty("CITY", NullValueHandling = NullValueHandling.Ignore)]
        public string City { get; set; }

        [JsonProperty("STATE", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        [JsonProperty("ZIP", NullValueHandling = NullValueHandling.Ignore)]
        public string ZIP { get; set; }

        [JsonProperty("PHONE", NullValueHandling = NullValueHandling.Ignore)]
        public string Phone { get; set; }

        [JsonProperty("PHMPHONE", NullValueHandling = NullValueHandling.Ignore)]
        public string PharmacyPhone { get; set; }

        [JsonProperty("LQRPHONE", NullValueHandling = NullValueHandling.Ignore)]
        public string Lqrphone { get; set; }

        [JsonProperty("PXFPHONE", NullValueHandling = NullValueHandling.Ignore)]
        public string Pxfphone { get; set; }

        [JsonProperty("FAX", NullValueHandling = NullValueHandling.Ignore)]
        public string Fax { get; set; }

        [JsonProperty("STRHOURS", NullValueHandling = NullValueHandling.Ignore)]
        public string StoreHours { get; set; }

        [JsonProperty("PHMHOURS", NullValueHandling = NullValueHandling.Ignore)]
        public string PharmacyHours { get; set; }

        [JsonProperty("LQRHOURS", NullValueHandling = NullValueHandling.Ignore)]
        public string Lqrhours { get; set; }

        [JsonProperty("PXFHOURS", NullValueHandling = NullValueHandling.Ignore)]
        public string Pxfhours { get; set; }

        [JsonProperty("OPTION", NullValueHandling = NullValueHandling.Ignore)]
        public string Option { get; set; }

        [JsonProperty("DEPTS", NullValueHandling = NullValueHandling.Ignore)]
        public string Depts { get; set; }

        [JsonProperty("SERVICES", NullValueHandling = NullValueHandling.Ignore)]
        public string Services { get; set; }

        [JsonProperty("TYPE", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("UNIQUE", NullValueHandling = NullValueHandling.Ignore)]
        public string Unique { get; set; }

        [JsonProperty("EPPH", NullValueHandling = NullValueHandling.Ignore)]
        public string Epph { get; set; }

        [JsonProperty("CSPH", NullValueHandling = NullValueHandling.Ignore)]
        public string Csph { get; set; }

        [JsonProperty("MAPH", NullValueHandling = NullValueHandling.Ignore)]
        public string Maph { get; set; }

        [JsonProperty("CLAT", NullValueHandling = NullValueHandling.Ignore)]
        public string Clat { get; set; }

        [JsonProperty("CLON", NullValueHandling = NullValueHandling.Ignore)]
        public string Clon { get; set; }

        [JsonProperty("DISTANCE", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Distance { get; set; }

        [JsonProperty("WABREAK")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Wabreak { get; set; }

        [JsonProperty("WASTORENUM", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Wastorenum { get; set; }

        [JsonProperty("OPENINGDATE")]
        public string Openingdate { get; set; }

        [JsonProperty("CLOSINGDATE")]
        public object Closingdate { get; set; }

        [JsonProperty("ISENABLED", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Isenabled { get; set; }

        [JsonProperty("STOREDATETIME", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Storedatetime { get; set; }

        [JsonProperty("STATUS", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("STOREMAPSID", NullValueHandling = NullValueHandling.Ignore)]
        public long? Storemapsid { get; set; }

        [JsonProperty("STOREMAPTOGGLE", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Storemaptoggle { get; set; }

        [JsonProperty("IMAGE", NullValueHandling = NullValueHandling.Ignore)]
        public Image Image { get; set; }

        [JsonProperty("SHORTNAME", NullValueHandling = NullValueHandling.Ignore)]
        public string Shortname { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("Thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public Uri[] Thumbnail { get; set; }

        [JsonProperty("Hero", NullValueHandling = NullValueHandling.Ignore)]
        public Uri[] Hero { get; set; }
    }

    public static class Converter
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
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}

