using System.Threading;
using System.Threading.Tasks;
using CDSAIServices.Services;
using Microsoft.Bot.Builder.Dialogs;

namespace CDSAIServices.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        public CancelAndHelpDialog(string id): base(id)
        {
        }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            return base.BeginDialogAsync(outerDc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            return await Task.FromResult<DialogTurnResult>(null);
        }
    }
}
