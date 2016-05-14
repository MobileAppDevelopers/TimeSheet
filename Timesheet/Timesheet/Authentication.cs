using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace Timesheet
{
    public class Authentication
    {

        public async static Task<bool> Authenticate(string username, string password)
        {
            if (!HasDefaultUrl)
                return false;
            var issuccess = false;
            Uri siteUrl = new Uri(DefaultUrl);
            using (HttpClient client = new HttpClient())
            {
                var samlRequest = _requestSaml
                    .Replace("[Username]", username)
                    .Replace("[Password]", password)
                    .Replace("[To]", _stsUrl)
                    .Replace("[applyTo]", siteUrl.AbsoluteUri);
                using (StringContent content = new StringContent(samlRequest))
				{
					//client.DefaultRequestHeaders.Host = "sharepoint.com";
                    var response = await client.PostAsync(_stsUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        XDocument xml = XDocument.Parse(result);
                        var element = xml.Root.Descendants(XName.Get(_securityTokenTag, _securityTokenTagNS)).FirstOrDefault();
                        if (element != null)
                        {
                            var token = element.Value;
                            CookieContainer cc = new CookieContainer();
                            HttpClientHandler webRequestHandler = new HttpClientHandler() { AllowAutoRedirect = false };
                            webRequestHandler.CookieContainer = cc;
                            webRequestHandler.UseCookies = true;
                            using (HttpClient client2 = new HttpClient(webRequestHandler))
                            using (StringContent content2 = new StringContent(token))
                            {
                                string signinUrl = siteUrl.Scheme + "://" + siteUrl.Host + _spoSigninUrl;
                                var response2 = await client2.PostAsync(signinUrl, content2);
                                result = await response2.Content.ReadAsStringAsync();
                                var cookies = cc.GetCookies(siteUrl).Cast<Cookie>();

                                Cookie cookie1 = cookies.Where(i => i.Name == _cookieFedAuth).FirstOrDefault();
                                Cookie cookie2 = cookies.Where(i => i.Name == _cookiertFA).FirstOrDefault();
                                if (cookie1 != null && cookie2 != null)
                                {
                                    if (Application.Current.Properties.ContainsKey(_cookieFedAuth))
                                    {
                                        Application.Current.Properties[_cookieFedAuth] = cookie1.Value;
                                        Application.Current.Properties[_cookiertFA] = cookie2.Value;
                                    }
                                    else
                                    {
                                        Application.Current.Properties.Add(_cookieFedAuth, cookie1.Value);
                                        Application.Current.Properties.Add(_cookiertFA, cookie2.Value);
                                    }

                                    issuccess = true;
                                }

                            }
                        }
                    }
                }

            }
            return issuccess;

        }

        public static List<Cookie> GetAuthCookies()
        {
            return new List<Cookie>() {
                new Cookie (_cookieFedAuth, Application.Current.Properties [_cookieFedAuth].ToString ()),
                new Cookie (_cookiertFA, Application.Current.Properties [_cookiertFA].ToString ())
            };
        }

        public static bool HasDefaultUrl
        {
            get
            {
                return !string.IsNullOrWhiteSpace(DefaultUrl);
            }

        }
        public static string DefaultUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultUrl) && Application.Current.Properties.ContainsKey(Constants.SPOSITEURL))
                {
                    _defaultUrl = Application.Current.Properties[Constants.SPOSITEURL].ToString();
                }
                return _defaultUrl;
            }

        }
        private static string _defaultUrl;
        private const string _requestSaml = @"<?xml version='1.0' encoding='utf-8' ?>
<s:Envelope xmlns:s='http://www.w3.org/2003/05/soap-envelope' xmlns:a='http://www.w3.org/2005/08/addressing' xmlns:u='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>
    <s:Header>
        <a:Action s:mustUnderstand='1'>http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</a:Action>
        <a:ReplyTo>
            <a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>
        </a:ReplyTo>
        <a:To s:mustUnderstand='1'>[To]</a:To>
        <o:Security s:mustUnderstand='1' xmlns:o='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'>
            <o:UsernameToken>
                <o:Username>[Username]</o:Username>
                <o:Password>[Password]</o:Password>
            </o:UsernameToken>
        </o:Security>
    </s:Header>
    <s:Body>
        <t:RequestSecurityToken xmlns:t='http://schemas.xmlsoap.org/ws/2005/02/trust'>
            <wsp:AppliesTo xmlns:wsp='http://schemas.xmlsoap.org/ws/2004/09/policy'>
                <a:EndpointReference>
                    <a:Address>[applyTo]</a:Address>
                </a:EndpointReference>
            </wsp:AppliesTo>
            <t:KeyType>http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey</t:KeyType>
            <t:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</t:RequestType>
            <t:TokenType>urn:oasis:names:tc:SAML:1.0:assertion</t:TokenType>
        </t:RequestSecurityToken>
    </s:Body>
</s:Envelope>";

        private const string _stsUrl = "https://login.microsoftonline.com/extSTS.srf";
        private const string _securityTokenTag = "BinarySecurityToken";
        private const string _securityTokenTagNS = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        private const string _spoSigninUrl = "/_forms/default.aspx?wa=wsignin1.0";
        private const string _cookieFedAuth = "FedAuth";
        private const string _cookiertFA = "rtFa";
    }
}
