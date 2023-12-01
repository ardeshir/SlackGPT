using System.Threading;
using System.Threading.Tasks;
using CDSAIServices.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace CDSAIServices
{
    public interface IVMBotHelper
    {
        Task<CreateVMInformation> GetVMInformationFromState(ITurnContext turnContext, CancellationToken cancellationToken);
   
        Task ResetBot(ITurnContext turnContext, CancellationToken cancellationToken, DialogContext dialogContext = null);
        Task<DialogTurnResult> HandleMessage(ITurnContext turnContext, CancellationToken cancellationToken, DialogContext dialogContext = null);
        Task ResetOnClose(ITurnContext turnContext, CancellationToken cancellationToken, DialogContext dialogContext = null);
        Task<CreateVMFlow> GetVMFlowFromState(ITurnContext turnContext, CancellationToken cancellationToken);

    }
}
