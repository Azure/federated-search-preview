//---------------------------------------------------------------------------------------------------------------------
// <copyright file="MyDataSourceServiceClient.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Microsoft.SearchProvider.Bots.Clients
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    //The following is a dummy example of data from an external data source.
    //Replace it with the logic to get data from your data source.
    public class MyDataSourceServiceClient 
    {
        /// <summary>
        /// Active directory authority URL which is responsible to issue token
        /// </summary>
        private const string activeDirAuthority = "https://login.windows.net/microsoft.onmicrosoft.com";

        private const string targateApiResourceId = "https://MyApp.myCompany.com";

        /// <summary>
        /// Performs a query based on the user's request.
        /// </summary>
        /// <param name="query">The user's query text.</param>
        /// <param name="oboToken">The on-behalf-of user token; or null.</param>
        /// <param name="logger">Logger which can be used to log messages</param>
        /// <param name="aadTokenResolver">AAD token resolver which will be used to get API token</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>The response information to the user's query.</returns>
        public async Task<ResponseObject> MyDataSourceSearch(
            string query,
            SearchBotAuthenticationToken oboToken = null,
            ILogger logger = null,
            IAadTokenResolver aadTokenResolver = null,
            CancellationToken token = default)
        {
            // TODO Run the search to generate your answer.
            string wasTokenFound = oboToken == null ? "not found" : "found";
            string apiToken = null;
            if (!string.IsNullOrWhiteSpace(oboToken.Token))
            {
                // Exchange OBO token to this app OBO token
                apiToken = aadTokenResolver.GetOnBehalfOfTokenAsync(activeDirAuthority, targateApiResourceId, oboToken.Token).ConfigureAwait(true).GetAwaiter().GetResult();
            }
            return new ResponseObject { ResponseText = "This is a dummy response retrieved to this query: " + query + " and the token was " + wasTokenFound };
        }
    }
}
