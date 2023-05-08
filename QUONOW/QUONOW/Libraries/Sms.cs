

namespace QUONOW.Libraries
{

    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Net.Http;
    #endregion


    public class Sms
    {
        public string Name { get; set; }
        public long PhoneNoTo { get; set; }
        public long PhoneNoFrom { get; set; }
        public string Message { get; set; }

        public bool SendSms()
        {
            try
            {
                using (HttpClient http = new HttpClient())
                {
                    //http.BaseAddress = new Uri("");
                    var response = http.GetAsync("http://mm.wallvesoft.com/WebSms.aspx?ID=" + Params.smsId + "&PWD=" + Params.smsPwd + "&TEXT=" + this.Message + "&MobNo=" + this.PhoneNoTo + "&TYPE=2&Pri=3&SID=WALLVE").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return false;
        }

    }
}