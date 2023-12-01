using System.Threading;
using System.Threading.Tasks;
using CDSAIServices.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CDSAIServices
{
    public class VMBotHelper : IVMBotHelper
    {
        private readonly UserState _userState;
        private const string HelpMsgText = "HELP:\n\n" +
                                            "- Invalid Characters for VM Name `~!@#$%^&*()=+_[]{}\\|;:.'\",<>/?.\n\n" +
                                            "- Type Reset, Cancel, or Quit to clear your session in the bot\n\n" +
                                            "- Type Help or ? to get to the help section";
        private const string ResettingStateMsgText = "Clearing State...\n\nCancelling Dialogs...";
        private const string ResetCompletedMsgText = "Finished clearing state and cancelling dialogs.";

        public VMBotHelper(UserState userState)
        {
            _userState = userState;
        }

        public Task<CreateVMFlow> GetVMFlowFromState(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var userStateAccessors = _userState.CreateProperty<CreateVMFlow>(nameof(CreateVMFlow));
            return userStateAccessors.GetAsync(turnContext, () => new CreateVMFlow(), cancellationToken);
        }


        public async Task<DialogTurnResult> HandleMessage(ITurnContext turnContext, CancellationToken cancellationToken, DialogContext dialogContext = null)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var text = turnContext.Activity.Text.ToLowerInvariant();

                switch (text)
                {
                    case "help":
                    case "?":
                        var helpMessage = MessageFactory.Text(HelpMsgText, HelpMsgText, InputHints.ExpectingInput);
                        await turnContext.SendActivityAsync(helpMessage, cancellationToken);
                        return new DialogTurnResult(DialogTurnStatus.Waiting);

                    case "reset":
                    case "cancel":
                    case "quit":
                        await ResetOnClose(turnContext, cancellationToken, dialogContext);
                        return new DialogTurnResult(DialogTurnStatus.Cancelled);
                }
            }

            return null;
        }

        public async Task ResetBot(ITurnContext turnContext, CancellationToken cancellationToken, DialogContext dialogContext = null)
        {
            var conversationStateAccessorsVMInfo = _userState.CreateProperty<CreateVMInformation>(nameof(CreateVMInformation));
            await conversationStateAccessorsVMInfo.SetAsync(turnContext, new CreateVMInformation(), cancellationToken);

            var conversationStateAccessors = _userState.CreateProperty<CreateVMFlow>(nameof(CreateVMFlow));
            await conversationStateAccessors.SetAsync(turnContext, new CreateVMFlow(), cancellationToken);

            var dialogStateAccessors = _userState.CreateProperty<DialogState>(nameof(DialogState));

            if (dialogContext != null)
            {
                await dialogContext.CancelAllDialogsAsync();
            }

            await dialogStateAccessors.SetAsync(turnContext, new DialogState(), cancellationToken);
        }

        public async Task ResetOnClose(ITurnContext turnContext, CancellationToken cancellationToken, DialogContext dialogContext = null)
        {
            var resetMessage = MessageFactory.Text(ResettingStateMsgText, ResettingStateMsgText, InputHints.IgnoringInput);
            var resetCompleteMessage = MessageFactory.Text(ResetCompletedMsgText, ResetCompletedMsgText, InputHints.IgnoringInput);
            await turnContext.SendActivityAsync(resetMessage, cancellationToken);
            await ResetBot(turnContext, cancellationToken, dialogContext);
            await turnContext.SendActivityAsync(resetCompleteMessage, cancellationToken);
        }

        public Task<CreateVMInformation> GetVMInformationFromState(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
