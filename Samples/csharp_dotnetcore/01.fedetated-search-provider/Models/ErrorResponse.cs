// <copyright file="ErrorResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EchoBot.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// The error response, which is returned in the value field of
    /// the response to the invoke activity if there's an error
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the error code defined by the bot, or null if not specified
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the error message, or null if not specified
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
