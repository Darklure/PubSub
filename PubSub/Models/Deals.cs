using PubSub.Models.GraphQL.PublixAd;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Models
{
    public struct Deals
    {
        public Rollover rollover { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }
    }
}
