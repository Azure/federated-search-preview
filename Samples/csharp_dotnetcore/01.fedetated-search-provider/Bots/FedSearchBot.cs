// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.SearchProvider.Bots
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.SearchProvider.Bots.BotDialogs;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    public class FedSearchBot : ActivityHandler
    {
        /// <summary>
        /// The invoke name under the latest contract
        /// </summary>
        private const string InvokeName = Constants.ApplicationSearchVersion;

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Logger.LogInformation($"ServiceUrl: { turnContext.Activity.ServiceUrl}");
            await base.OnTurnAsync(turnContext, cancellationToken);
        }

        private ILogger Logger;
        public FedSearchBot(ILogger<FedSearchBot> logger)
        {
            this.Logger = logger;
        }

        //This is the function that will be called by the federated search provider.
        protected override Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            if (string.Equals(InvokeName, turnContext.Activity.Name))
            {
                return Task.FromResult(BotDialog.SendDialogInInvokeResponse(turnContext, cancellationToken));
            }
            else
            {
                return Task.FromResult<InvokeResponse>(null); 
            }
        }

        //This method is only for testing from Bot Emulator and Test in web chat (Azure Portal)
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await BotDialog.SendDialog(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = Constants.WelcomeText;
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
