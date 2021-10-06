// <copyright file="DisplayLayout.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EchoBot.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// The display layout, which serves as a template for a search result
    /// </summary>
    public class DisplayLayout
    {
        /// <summary>
        /// Gets or sets the layout id
        /// </summary>
        [JsonProperty("layoutId")]
        public string LayoutId { get; set; }

        /// <summary>
        /// Gets or sets the layout body
        /// </summary>
        [JsonProperty("layoutBody")]
        public string LayoutBody { get; set; }
    }
}
