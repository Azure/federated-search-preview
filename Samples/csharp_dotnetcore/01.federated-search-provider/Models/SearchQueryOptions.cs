// <copyright file="SearchResult.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EchoBot.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// The query options for a search request
    /// </summary>
    public class SearchQueryOptions
    {
        /// <summary>
        /// Gets or sets the index of the first result to send
        /// </summary>
        [JsonProperty("skip")]
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of results to send
        /// </summary>
        [JsonProperty("top")]
        public int Top { get; set; }
    }
}
