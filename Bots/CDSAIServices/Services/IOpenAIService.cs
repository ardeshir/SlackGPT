//using CDSAIServices.Domain;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;

namespace CDSAIServices.Services
{
    public interface IOpenAIService
    {
        Task<string> PostOpenAIQuestion(string question, CancellationToken cancellationToken);
    }
}
