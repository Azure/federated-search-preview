// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.SearchProvider.Bots
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class FedSearchBot : ActivityHandler
    {
        /// <summary>
        /// The invoke name under the latest contract
        /// </summary>
        private const string InvokeName = Constants.ApplicationSearchVersion;

        private ILogger Logger;

        public FedSearchBot(ILogger<FedSearchBot> logger)
        {
            this.Logger = logger;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation($"ServiceUrl: { turnContext.Activity.ServiceUrl}");
            await base.OnTurnAsync(turnContext, cancellationToken);
        }

        //This is the function that will be called by the federated search provider.
        protected override async Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken = default)
        {
            if (string.Equals(InvokeName, turnContext.Activity.Name))
            {
                return await SearchHelper.RunFederatedSearch(turnContext, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        //This method is only for testing from Bot Emulator and Test in web chat (Azure Portal)
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken = default)
        {
            await SearchHelper.RunSearchForUser(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken = default)
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
