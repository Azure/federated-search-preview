// <copyright file="EchoBackDialog.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.SearchProvider.Bots
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;

    /// <summary>
    /// Token helper component for validating and return required claim value from token.
    /// </summary>
    public class TokenHelper
    {
        private readonly IList<string> acceptedAudiences;

        private readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenHelper"/> class.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        public TokenHelper(IList<string> acceptedAudiences, ILogger logger)
        {
            this.acceptedAudiences = acceptedAudiences;
            this.Logger = logger;
        }

        /// <summary>
        /// Method used to validate the token
        /// </summary>
        /// <param name="token">The token value.</param>
        /// <returns>Token validated status in <see cref="bool"/></returns>
        public bool IsValidToken(string token)
        {

            if (string.IsNullOrWhiteSpace(token))
            {
                this.Logger.LogError($"Error Details:: [TokenHelper::IsValidToken] Received user token contains null or empty value.");
                return false;
            }
            else
            {
                var jwtToken = new JwtSecurityToken(token);

                // Validate if token got expired.
                if (jwtToken?.ValidTo < DateTime.UtcNow)
                {
                    this.Logger.LogError($"Error Details:: [TokenHelper::IsValidToken] Received token got expired at {jwtToken?.ValidTo}");
                    return false;
                }
            }

            string audience = GetClaimValueFromAuthToken(token, "aud");
            if (!acceptedAudiences.Contains(audience))
            {
                this.Logger.LogError($"Error Details:: [TokenHelper::IsValidToken] Received token's resource ID do not match with My bot resource ID {audience}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Helper method to get hashed user id.
        /// </summary>
        /// <param name="token">The incoming OBO token.</param>
        /// <returns>Returns hashed user id</returns>
        public string GetUserId(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return null;
                }
                JwtSecurityToken securityToken = new JwtSecurityToken(token);
                string oid = GetClaimValueFromAuthToken(token, "oid");
                return oid;
            }
            catch (Exception)
            {
                // Add logs
                return null;
            }
        }

        /// <summary>
        /// Helper method to get required claim value from access token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="claimName">The claim type name.</param>
        /// <returns>Claim value from access token.</returns>
        public string GetClaimValueFromAuthToken(string accessToken, string claimName)
        {
            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(claimName))
            {
                return string.Empty;
            }
            var jwtToken = new JwtSecurityToken(accessToken);
            return jwtToken.Claims.FirstOrDefault(claim => claim.Type == claimName)?.Value;
        }
    }

}
