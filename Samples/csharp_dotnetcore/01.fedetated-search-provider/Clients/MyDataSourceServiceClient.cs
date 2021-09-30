//---------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicsServiceClient.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Microsoft.SearchProvider.Bots.Clients
{
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
        /// Performs a query based on the user's request.
        /// </summary>
        /// <param name="query">The user's query text.</param>
        /// <param name="oboToken">The on-behalf-of user token; or null.</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>The response information to the user's query.</returns>
         public async Task<ResponseObject> MyDataSourceSearch(
             string query,
             SearchBotAuthenticationToken oboToken = null,
             CancellationToken token = default)
        {
            // TODO Run the search to generate your answer.
            return new ResponseObject { ResponseText = "This is a dummy response retrieved to this query: " + query };
        }
    }
}
