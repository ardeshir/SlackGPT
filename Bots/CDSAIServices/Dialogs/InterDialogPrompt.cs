using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace CDSAIServices.Dialogs
{
    public class IntegerDialogPromptProperties
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public string PromptMessageText { get; set; }
        public string RepromptMessageText { get; set; }
    }
    public class IntegerDialogPrompt : ComponentDialog
    {
        private readonly IntegerDialogPromptProperties _integerDialogPromptProperties;
        private readonly ILogger<IntegerDialogPrompt> _logger;
        public IntegerDialogPrompt(IntegerDialogPromptProperties integerDialogPromptProperties, ILogger<IntegerDialogPrompt> logger, string id = nameof(IntegerDialogPrompt)) : base(id)
        {
            _logger = logger;
            _integerDialogPromptProperties = integerDialogPromptProperties;
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), IntegerDialogPromptValidator));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initial Step");
            var value = (int)stepContext.Options;

            var promptMessage = MessageFactory.Text(_integerDialogPromptProperties.PromptMessageText, InputHints.ExpectingInput);
            var repromptMessage = MessageFactory.Text(_integerDialogPromptProperties.RepromptMessageText, InputHints.ExpectingInput);

            _logger.LogInformation("Checking if there is a value");
            if (value == 0)
            {
                _logger.LogInformation("Prompting user since there is no value");
                // We were not given a valid number
                return await stepContext.PromptAsync(nameof(NumberPrompt<int>),
                        new PromptOptions
                        {
                            Prompt = promptMessage,
                            RetryPrompt = repromptMessage,
                        }, cancellationToken);
            }

            _logger.LogInformation("Existing value moving to next step");
            return await stepContext.NextAsync(value, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Value is confirmed and moving out of the integer dialog.");
            var number = (int)stepContext.Result;
            return await stepContext.EndDialogAsync(number, cancellationToken);
        }

        private Task<bool> IntegerDialogPromptValidator(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if the value is an integer");
            if (promptContext.Recognized.Succeeded)
            {
                var value = promptContext.Recognized.Value;
                _logger.LogInformation($"Checking if {value} is between {_integerDialogPromptProperties.Minimum} and {_integerDialogPromptProperties.Maximum}");
                return Task.FromResult(value >= _integerDialogPromptProperties.Minimum && value <= _integerDialogPromptProperties.Maximum);
            }

            return Task.FromResult(false);
        }
    }
}
