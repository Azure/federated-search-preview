// <copyright file="SearchResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EchoBot.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// The search response, which is returned in the body of
    /// the response to the invoke activity sent to a search results or answers provider
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "JSON class")]
    public class SearchResponse
    {
        /// <summary>
        /// Gets or sets the results, or null if none
        /// Results are used to specify the template variables used by the corresponding DisplayLayout
        /// </summary>
        [JsonProperty("results")]
        public List<SearchResult> Results { get; set; }

        /// <summary>
        /// Gets or sets the display layouts, or null if none
        /// Display layouts are used as templates for results.
        /// If the skill wants to specify several large adaptive cards all using the same format,
        /// it can specify the format once as a display layout and have the differences as variables.
        /// Then each result can specify the values of those variables.
        /// </summary>
        [JsonProperty("displayLayouts")]
        public List<DisplayLayout> DisplayLayouts { get; set; }

        /// <summary>
        /// Gets or sets the total results available, if pagination is supported, or null otherwise
        /// </summary>
        [JsonProperty("totalResultCount")]
        public int? TotalResultCount { get; set; }

        /// <summary>
        /// Gets or sets whether more results are available
        /// </summary>
        [JsonProperty("moreResultsAvailable")]
        public bool? MoreResultsAvailable { get; set; }
    }
}
