// <copyright file="EchoBackDialog.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.SearchProvider.Bots
{
    using Newtonsoft.Json;

    /// <summary>Contains an authentication token to let the bot act on behalf of a user.</summary>
    public class SearchBotAuthenticationToken
    {
        /// <summary>Initializes a new instance of the <see cref="SearchBotAuthenticationToken"/> class.</summary>
        /// <param name="type">The authentication type.</param>
        /// <param name="token">The authentication token.</param>
        public SearchBotAuthenticationToken(AuthenticationTypes type, string token)
        {
            this.AuthType = type;
            this.Token = token;
        }

        /// <summary>
        /// Gets the authentication type.
        /// </summary>
        [JsonProperty("authType")]
        public AuthenticationTypes AuthType { get; }

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; }
    }
}
