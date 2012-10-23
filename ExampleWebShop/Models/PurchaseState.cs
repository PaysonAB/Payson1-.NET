using System;
using System.Collections.Generic;

namespace ExampleWebShop.Models
{
    public class PurchaseState
    {
        public string OrderGuid { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserKey { get; set; }
        public Dictionary<DateTime, string> Updates { get; set; }
        public string LatestStatus { get; set; }
        public string ReceiverEmail { get; set; }
    }
}