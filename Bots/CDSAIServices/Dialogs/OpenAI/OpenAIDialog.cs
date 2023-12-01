using CDSAIServices.Domain;
using CDSAIServices.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CDSAIServices.Dialogs.OpenAI
{
    public class OpenAIDialog : CancelAndHelpDialog
    {
        private readonly BotState _userState;
        private readonly BotState _conversationState;
        protected readonly ILogger _logger;
        private readonly IOpenAIService _openAIService;

        public OpenAIDialog(ConversationState conversationState, UserState userState, IOpenAIService openAIService,
            ILogger<OpenAIDialog> logger) : base(nameof(OpenAIDialog))
        {
            _logger = logger;
            _conversationState = conversationState;
            _userState = userState;
            _openAIService = openAIService;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                PromptForQuestion,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> PromptForQuestion(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Prompting user for question");
            var promptMessage = MessageFactory.Text("How can AI assist you today? ", InputHints.ExpectingInput);
            var repromptMessage = MessageFactory.Text("Please provide a valid question", InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = promptMessage,
                RetryPrompt = repromptMessage
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting the virtual machine name entered");

            var vmsToDelete = new List<string>();
            if (stepContext.Result is string question)
            {
                var result = await _openAIService.PostOpenAIQuestion(question, cancellationToken);
                _logger.LogInformation("Displaying question results");
                await stepContext.Context.SendActivityAsync(result, null, null, cancellationToken);
            }

            _logger.LogInformation("Ending dialog");
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
