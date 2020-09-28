using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PubSub.Models.GraphQL.PublixAd;
using System.Globalization;
using System.Text.RegularExpressions;
using Discord;
using Logic.Publix;
using System.Reflection.Metadata.Ecma335;
using PubSub.Models;

namespace PubSub.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        public const string EMBED_URL = @"https://www.publix.com/shop-online/in-store-pickup/subs/d3a5f2da-3002-4c6d-949c-db5304577efb";
        public const string EMBED_THUMBNAIL_URL = @"https://play-lh.googleusercontent.com/LsRv7xASMmDKWWnMtk-KF9AVCYM9jXRqTbvt_vWS5iwHgx6nBx8ACEShoCAMtH0qFQ";

        private List<String> _validColors = new List<String>();
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        public Commands(IServiceProvider services)
        {
            _services = services;
            _config = services.GetRequiredService<IConfiguration>();
        }

        [Command("info")]
        [Summary("Displays Bot Details")]
        public Task SayAsync()
        {
            string info = "Looks for current deals on Publix Subs ";
            return ReplyAsync(info);
        }
        
        [Command("deals")]
        [Summary("Returns current deals for pub subs by zip code")]
        public async Task GetSubDealsByZipCode([Remainder] string args = null)
        {
            // create array if contains a space if not make a default array
            var zips = (args != null && args.Contains(" ")) ? args.Trim().Split(" ").Distinct().ToList() : new string[]{ args }.ToList();

            // store data for subs to be published nicely to Discord
            var fields = new List<EmbedFieldBuilder>();

            // fix any 
            for (int i = 0; i < zips.Count; i++)
            {
                string zip = zips[i];

                // remove any 9 digit zips and use the 5 digit zip
                zip = (zip != null && zip.Contains("-")) ? zip.Split("-")[0] : zip;

                // ensure 5 digit zip is a valid number
                if (zip == null || !Regex.IsMatch(zip, @"^\d+$"))
                {
                    continue;
                }

                fields = GetSubDeals(zip, fields);
            }

            if (fields.Count > 0)
            {
                await Context.Channel.SendMessageAsync("", false, EmbedBuilderBot("PubSub Deals For Zip Code(s) Supplied", fields).Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}... Unable to find any deals with the data supplied.");
            }
        }

        public List<EmbedFieldBuilder> GetSubDeals(string zipCode, List<EmbedFieldBuilder> fields)
        {
            var storeList = Publix.GetStoreListAsync(zipCode).Result;

            var deals = new List<Deals>();

            if (storeList == null || !storeList.Stores.Any())
            {

                deals.Add(new Models.Deals()
                {
                    endDate = new DateTime(),
                    startDate = new DateTime(),
                    rollover = new Rollover()
                    {
                        Title = $"Deals for Zip Code {zipCode}",
                        Deal = $"No deals available for {zipCode}, there likely isn't a Publix in that Zip Code",
                        ID = -1
                    }
                });

                fields.Add(new EmbedFieldBuilder()
                {
                    IsInline = false,
                    Name = $"Deals for Zip Code {zipCode}",
                    Value = $"No deals available for {zipCode}, there likely isn't a Publix in that Zip Code."
                });

                return fields;
            }

            var storeRef = storeList.Stores.First(a => string.IsNullOrWhiteSpace(a.Status)).Key;

            var ad = Publix.GetDealsByStoreAsync(storeRef).Result;

            if (ad.Promotion == null)
            {
                fields.Add(new EmbedFieldBuilder()
                {
                    IsInline = false,
                    Name = $"Deals for Zip Code {zipCode}",
                    Value = $"No deals available for {zipCode}, there likely isn't a Publix in that Zip Code."
                });

                return fields;
            }

            var startDate = DateTime.ParseExact(ad.Promotion.SaleStartDateString, "MMM dd, yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);
            var endDate = DateTime.ParseExact(ad.Promotion.SaleEndDateString, "MMM dd, yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);

            var subs = ad.Promotion.Rollovers.Where(a => a.Title.Contains("Whole Sub"));



            var headerField = new EmbedFieldBuilder()
            {
                IsInline = false,
                Name = $"Deals for Zip Code {zipCode}",
                Value = $"From {startDate.ToShortDateString()} To {endDate.ToShortDateString()}"
            };

            fields.Add(headerField);

            foreach (var sub in subs)
            {
                fields.Add(new EmbedFieldBuilder()
                {
                    IsInline = false,
                    Name = sub.Title,
                    Value = sub.Deal
                });
            }

            return fields;
        }

        private Discord.EmbedBuilder Deals(List<Rollover> subs, string zipCode, DateTime startDate, DateTime endDate)
        {
            var fields = new List<EmbedFieldBuilder>();

            foreach (var sub in subs)
            {
                fields.Add(new EmbedFieldBuilder()
                {
                    IsInline = true,
                    Name = sub.Title,
                    Value = sub.Deal
                });
            }

            return EmbedBuilderBot($"Sub For Sale in {zipCode} From {startDate.ToShortDateString()} To {endDate.ToShortDateString()}" , fields);
        }

        public Discord.EmbedBuilder EmbedBuilderBot(string title, List<EmbedFieldBuilder> fields)
        {
            var eb = new EmbedBuilder()
            {
                Color = Color.Green,
                Title = title,
                Url = EMBED_URL,
                ThumbnailUrl = EMBED_THUMBNAIL_URL,
                Fields = fields
            };

            return eb;
        }
    }
}
