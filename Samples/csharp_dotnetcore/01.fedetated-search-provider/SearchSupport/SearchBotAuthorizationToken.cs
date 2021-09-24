// <copyright file="EchoBackDialog.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.SearchProvider.Bots
{
    using Newtonsoft.Json;

    /// <summary>Contains an authorization token to let the bot act on behalf of a user.</summary>
    public class SearchBotAuthorizationToken
    {
        /// <summary>Initializes a new instance of the <see cref="SearchBotAuthorizationToken"/> class.</summary>
        /// <param name="type">The authentication type.</param>
        /// <param name="token">The token.</param>
        public SearchBotAuthorizationToken(
            AuthTypes type,
            string token)
        {
            this.AuthType = type;
            this.Token = token;
        }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        [JsonProperty("authType")]
        public AuthTypes AuthType { get; }

        /// <summary>
        /// Gets the intent.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; }
    }
}
