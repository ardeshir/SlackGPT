using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CDSAIServices.Dialogs.OpenAI;
using CDSAIServices.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace CDSAIServices.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly BotState _userState;
        private readonly BotState _conversationState;
        //private readonly IVMBotHelper _vmBotHelper;
        protected readonly ILogger _logger;
        protected const string ActionToPerformDialogName = "ActionToPerformDialog";
        private readonly BaseDataConfiguration _baseDataConfiguration;

        public MainDialog( OpenAIDialog openAIDialog, ConversationState conversationState, UserState userState, ILogger<MainDialog> logger, BaseDataConfiguration baseDataConfiguration): base(nameof(MainDialog))
        {
            _logger = logger;
            _conversationState = conversationState;
            _userState = userState;
            //_vmBotHelper = vmBotHelper;
            _baseDataConfiguration = baseDataConfiguration;

            
            AddDialog(openAIDialog);
            AddDialog(new ChoicePrompt(ActionToPerformDialogName));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                FireOffActionStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Ask a question?", InputHints.ExpectingInput),
                RetryPrompt = MessageFactory.Text($"Reask your question", InputHints.ExpectingInput),
                Style = ListStyle.HeroCard
            };

            return await stepContext.PromptAsync(ActionToPerformDialogName, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> FireOffActionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if (stepContext.Result != null)
            {
                switch (stepContext.Result)
                {

                    case FoundChoice foundChoice when foundChoice.Value == BaseData.TestOpenAI:
                        return await stepContext.BeginDialogAsync(nameof(OpenAIDialog), null, cancellationToken);
                }
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }
    }
}
