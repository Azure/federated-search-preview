// <copyright file="SearchResult.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EchoBot.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// The search result
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Gets or sets a unique identifier or value of the search result
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the layout id of the search result
        /// </summary>
        [JsonProperty("layoutId")]
        public string LayoutId { get; set; }

        /// <summary>
        /// Gets or sets the data used to fill in variables in the corresponding layout
        /// This represents an arbitrary object defined by the bot.
        /// The fields of this object can be referenced in the corresponding display layout
        /// body. For example, reference data.searchResultText via {searchResultText}
        /// </summary>
        [JsonProperty("data")]
        public SearchResultData Data { get; set; }
    }
}
