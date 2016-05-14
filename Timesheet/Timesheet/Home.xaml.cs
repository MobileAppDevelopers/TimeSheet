using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

using Timesheet.Controller;

namespace Timesheet
{
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();

			CookieContainer cc = new CookieContainer ();
			HttpClientHandler handler = new HttpClientHandler ();
			handler.CookieContainer = cc;
			using (HttpClient client = new HttpClient (handler)) {
				client.DefaultRequestHeaders.Add ("Accept", "application/json;odata=verbose");
				var cookies = Authentication.GetAuthCookies ();
				string siteurl = Application.Current.Properties [Constants.SPOSITEURL].ToString();
				foreach(Cookie c in cookies)
				{					
					cc.Add (new Uri (siteurl), c);
				}
				var response = client.GetAsync (siteurl + "/_api/web").Result;
				if (response.IsSuccessStatusCode) {
					string result =response.Content.ReadAsStringAsync().Result;
					var obj = JObject.Parse (result);
					labelTitle.Text = obj ["d"] ["Title"].ToString();	
					labelUrl.Text = obj ["d"] ["Url"].ToString();
				}

			}
        }
    }
}
