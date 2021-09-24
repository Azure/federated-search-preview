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
         public async Task<string> MyDataSourceSearch(string endpoint, string query, CancellationToken token = default)
        {
            // TODO Run the search to generate your answer.
            string responsePost = "This is a dummy response retrieved to this query: " + query + ", from the endpoint: " + endpoint + " using this search API " + Constants.SearchQueryApi;

            return responsePost;
        }
    }
}
