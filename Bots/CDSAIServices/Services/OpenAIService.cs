using System.Threading;
using System.Threading.Tasks;

namespace CDSAIServices.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IBaseService _baseService;

        public OpenAIService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<string> PostOpenAIQuestion(string question, CancellationToken cancellationToken)
        {
            return await _baseService.PostReturnContent(nameof(OpenAIService), "OpenAIFunction", question,
                                                            cancellationToken);
        }
    }
}