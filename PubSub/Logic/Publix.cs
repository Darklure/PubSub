using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PubSub.Models;
using PubSub.Models.GraphQL.PublixAd;
using PubSub.Models.StoreInfo;
using PubSub.Utils;
using System;
using System.Threading.Tasks;

namespace Logic.Publix
{
    public class Publix
    {
        private const string PublixAdQuery = @"
            query Promotion($promotionCode: ID, $previewHash: String, $promotionTypeID: Int, $require: String, $nuepOpen: Boolean) {
                promotion(code: $promotionCode, imageWidth: 1400, previewHash: $previewHash, promotionTypeID: $promotionTypeID, require: $require) {
                    id
                    title
                    displayOrder
                    saleStartDateString
                    saleEndDateString
                    postStartDateString
                    previewPostStartDateString
                    code
                    rollovers(previewHash: $previewHash, require: $require) {
                        id
                        title
                        deal
                        isCoupon
                        buyOnlineLinkURL
                        imageCommon: imageURL(imageWidth: 600, previewHash: $previewHash, require: $require)
                    }
                    pages(imageWidth: 1400, previewHash: $previewHash, require: $require, nuepOpen: $nuepOpen) {
                        id
                        imageURL(previewHash: $previewHash,require: $require)
                        order
                    }
                }
             }";

        public static async Task<StoreList> GetStoreListAsync(string zipCode)
        {
            var url = "https://services.publix.com/api/v1/storelocation?types=R,G,H,N,S&option=&count=15&includeOpenAndCloseDates=true&zipCode={0}";

            var storesResponse = await WebClient.GetString(string.Format(url, zipCode), "application/json");

            var storeList = JsonConvert.DeserializeObject<StoreList>(storesResponse, PubSub.Models.StoreInfo.Converter.Settings);

            return storeList;
        }

        public static async Task<Welcome> GetDealsByStoreAsync(string storeRef)
        {
            var graphQLServer = "https://graphql-cdn-slplatform.liquidus.net/";

            var graphQLClient = new GraphQLHttpClient(graphQLServer, new NewtonsoftJsonSerializer());

            graphQLClient.HttpClient.DefaultRequestHeaders.Add("campaignid", "80db0669da079dc6");
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("storeref", storeRef);
            graphQLClient.HttpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { NoCache = true };
            graphQLClient.HttpClient.DefaultRequestHeaders.Pragma.ParseAdd("No-Cache");
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("authority", new Uri(graphQLServer).Host);

            graphQLClient.HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(WebClient.UserAgent);

            var graphQLResponse = await graphQLClient.SendQueryAsync<Welcome>(new GraphQLRequest
            {
                Query = PublixAdQuery,
                OperationName = "Promotion",
                Variables = new
                {
                    sort = "",
                    preload = 3,
                    disablesneakpeekhero = false,
                    countryid = 1,
                    languageid = 1,
                    env = "undefined",
                    storeref = storeRef,
                    storeid = "undefined",
                    campaignid = "80db0669da079dc6",
                    require = "",
                    nuepOpen = false
                }
            });

            return graphQLResponse.Data;
        }
    }
}
