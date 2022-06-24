// <copyright file="AuthModel.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.SearchProvider.Bots
{
    using Newtonsoft.Json;
    using System;


    /// <summary>
    /// Data model for authentication.
    /// </summary>
    public class AuthModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthModel"/> class.
        /// </summary>
        /// <param name="token">The token value.</param>
        /// <param name="resourceId">The resource identifier.</param>
        /// <param name="expiration">Token expiry date time.</param>
        public AuthModel(string token, string resourceId, DateTimeOffset expiration)
        {
            this.Token = token;
            this.ResourceId = resourceId;
            this.ExpiresOn = expiration;
        }

        /// <summary>
        /// Gets or sets the token expiry date time.
        /// </summary>
        public DateTimeOffset ExpiresOn { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        [JsonProperty("ResourceID")]
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }
    }
}
