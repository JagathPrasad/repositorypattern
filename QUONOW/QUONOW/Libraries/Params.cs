using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace QUONOW.Libraries
{
    public static class Params
    {
        public static readonly string stripeApiKey = ConfigurationManager.AppSettings["StripeApiKeySecretKey"] ?? "sk_test_9wnKd0ODnOXr00bysp5h7BD7";

        public static readonly string smsId = ConfigurationManager.AppSettings["SmsID"] ?? "";

        public static readonly string smsPwd = ConfigurationManager.AppSettings[""] ?? "";

    }
}