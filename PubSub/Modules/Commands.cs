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
        public async Task GetWinnersByYear([Remainder] string args = null)
        {
            args = (args != null && args.Contains("-")) ? args.Split("-")[0] : args;

            if (args == null || !Regex.IsMatch(args, @"^\d+$"))
            {                
                await Context.Channel.SendMessageAsync($"{Context.User.Username}... how about a valid 5 digit zip code?");
                return;
            }

            var storeList = Publix.GetStoreListAsync(args).Result;

            if (storeList["Stores"] == null || storeList["Stores"].Count() == 0)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}... No stores found for your zip code.");
                return;
            }

            var storeRef = storeList["Stores"][0]["KEY"].ToString();

            var ad = Publix.GetDealsByStoreAsync(storeRef).Result;
            
            if (ad.Promotion == null)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Username}... No deals available for " + args + ", there likely isn't a Publix in that Zip Code.");
                return;
            }

            var startDate = DateTime.ParseExact(ad.Promotion.SaleStartDateString, "MMM dd, yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);
            var endDate = DateTime.ParseExact(ad.Promotion.SaleEndDateString, "MMM dd, yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);

            var subs = ad.Promotion.Rollovers.Where(a => a.Title.Contains("Whole Sub")).ToList();

            await Context.Channel.SendMessageAsync("", false, Deals(subs, args, startDate, endDate).Build());
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
