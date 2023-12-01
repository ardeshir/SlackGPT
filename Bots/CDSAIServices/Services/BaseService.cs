using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CDSAIServices.Services
{
    public class BaseService(IHttpClientFactory httpClientFactory) : IBaseService
    {

        public async Task<string> PostReturnContent(string name, string url, string sendString, CancellationToken cancellationToken)
        {
            using (var httpClient = httpClientFactory.CreateClient(name))
            {
                var responseMessage = await httpClient.PostAsync(url, new StringContent(sendString), cancellationToken);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseStream = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

                    return responseStream;
                }
            }

            return string.Empty;
        }
    }
}
