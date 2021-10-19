// <copyright file="SearchResult.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EchoBot.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// The search result data.
    /// A bot should replace this with its own data object.
    /// The fields of this object can be referenced in the corresponding display layout
    /// body. For example, reference data.searchResultText via {searchResultText}
    /// </summary>
    public class SearchResultData
    {
        /// <summary>
        /// Gets or sets the text
        /// </summary>
        [JsonProperty("searchResultText")]
        public string SearchResultText { get; set; }
    }
}
