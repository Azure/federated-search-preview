// <copyright file="SearchResult.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EchoBot.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// The request for answers, which goes in the Value field of the invoke activity
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// Gets or sets the query
        /// </summary>
        [JsonProperty("queryText")]
        public string QueryText { get; set; }

        /// <summary>
        /// Gets or sets the kind of query
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the query options
        /// </summary>
        [JsonProperty("queryOptions")]
        public SearchQueryOptions QueryOptions { get; set; }
    }
}
