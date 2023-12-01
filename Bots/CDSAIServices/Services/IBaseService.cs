using System.Threading;
using System.Threading.Tasks;
//using FSDI_CreateVMBot.Domain;

namespace CDSAIServices.Services
{
    public interface IBaseService
    {

        Task<string> PostReturnContent(string name, string url, string sendString, CancellationToken cancellationToken);
   }
}
