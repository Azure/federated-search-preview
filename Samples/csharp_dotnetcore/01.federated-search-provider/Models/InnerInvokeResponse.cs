// <copyright file="InnerInvokeResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EchoBot.Models
{
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The invoke response, which is returned in the body of
    /// the response to the invoke activity
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "JSON class")]
    public class InnerInvokeResponse
    {
        /// <summary>
        /// Gets or sets the status code for the operation of the bot
        /// These correspond to http status codes but for the internal operation of the bot rather than the network connection
        /// </summary>
        [JsonProperty("statusCode")]
        public int? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the type of the invoke response, defining the format of the value field
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the invoke response, whose schema corresponds to the value of the type field
        /// </summary>
        [JsonProperty("value")]
        public JObject Value { get; set; }
    }
}
