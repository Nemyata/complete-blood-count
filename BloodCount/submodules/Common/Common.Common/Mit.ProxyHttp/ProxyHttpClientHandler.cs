using System.Net;
using System.Net.Http;

namespace Common.ProxyHttp
{
    /// <summary>
    /// При подключении позволяет работать через porxy на сервере 
    /// </summary>
    public class ProxyHttpClientHandler : HttpClientHandler
    {
        public ProxyHttpClientHandler()
        {
            IWebProxy proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            Proxy = proxy;
        }
    }
}