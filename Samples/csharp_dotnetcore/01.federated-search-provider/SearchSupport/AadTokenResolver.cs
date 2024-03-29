// <copyright file="AadTokenResolver.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.SearchProvider.Bots
{
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.VisualStudio.Threading;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    public class AadTokenResolver: IAadTokenResolver
    {
        private const string CacheDelimiter = ";";
        private const int TokenExpirationBufferSeconds = 30;

        private readonly string certThumbPrint;
        private readonly string clientId;
        private readonly StoreLocation storeLocation;

        /// <summary>
        /// Caches tokens in memory to be used for next call
        /// Key for cache should have a unique value representing user as token are user specific
        /// </summary>
        private readonly ConcurrentDictionary<string, AuthModel> tokenCache = new ConcurrentDictionary<string, AuthModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AadTokenResolver" /> class.
        /// Builds a new AadToken resolver requires the following config values:
        /// AADAppId: The Id of the AAD application to use
        /// AzureSubscriptionId: The Id of the Azure subscription to use
        /// AADTenant: The Id of the AAD Tenant to use
        /// AADCertName: The Name of the AAD authentication cert to use
        /// </summary>
        /// <param name="clientId">Application Principal Client Id</param>
        /// <param name="certThumbPrint">Certificate thumbprint associated with client app</param>
        /// <param name="certLocation">Certificate location</param>
        public AadTokenResolver(string clientId, string certThumbPrint, string certLocation)
        {
            this.clientId = clientId;
            this.certThumbPrint = certThumbPrint;
            this.storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), certLocation, true);
        }

        /// <inheritdoc/>
        public async Task<string> GetOnBehalfOfTokenAsync(string authority, string resource, string token, CancellationToken cancelationToken=default)
        {
            var authResult = await this.GetOrRefreshOnBehalfOfToken(authority, resource, token, cancelationToken).ConfigureAwait(false);
            return authResult?.Token;
        }

        /// <summary>
        /// Gets the cache key. Key formation can vary but idea should be each user should get unique key.
        /// If process need to get more than 1 token to complete workflow adding target token resourceID will 
        /// make token unique for different downstream APIs.
        /// </summary>
        /// <param name="resourceId">The target token resource ID.</param>
        /// <param name="uniqUserId">The unique id representing user.</param>
        /// <returns>Cache key.</returns>
        private static string GetCacheKey(string resourceId, string uniqUserId)
        {
            return resourceId + CacheDelimiter + uniqUserId;
        }

        /// <summary>
        /// Get the refreshed token.
        /// </summary>
        /// <param name="authority">The token issuing authority.</param>
        /// <param name="resource">The target token resource ID.</param>
        /// <param name="userToken">The user token.</param>
        /// <param name="cancelationToken">Cancelation Token</param>
        /// <returns>Authentication result.</returns>
        /// <exception cref="Exception">Error exception</exception>
        private async Task<AuthModel> GetOrRefreshOnBehalfOfToken(string authority, string resource, string userToken, CancellationToken cancelationToken=default)
        {
            try
            {
                var tokenHelper = new TokenHelper(null,null);
                var upn = tokenHelper.GetClaimValueFromAuthToken(userToken, "upn");
                if (string.IsNullOrWhiteSpace(upn))
                {
                    upn = tokenHelper.GetClaimValueFromAuthToken(userToken, "preferred_username")?.Split("@").FirstOrDefault();
                }
                if (!string.IsNullOrWhiteSpace(upn))
                {
                    AuthModel token = this.CheckExisting(resource, upn);
                    if (token != null)
                    {
                        return token;
                    }
                }
                AuthenticationResult output;
                var cert = this.GetCert();
                var authContext = new AuthenticationContext(authority);
                var certCred = new ClientAssertionCertificate(this.clientId, cert);
                var userAssertion = new UserAssertion(userToken, "urn:ietf:params:oauth:grant-type:jwt-bearer", upn);
                output = await authContext.AcquireTokenAsync(resource, certCred, userAssertion).WithCancellation(cancelationToken).ConfigureAwait(false);

                var authModel = new AuthModel(output.AccessToken, resource, output.ExpiresOn);
                if (!string.IsNullOrWhiteSpace(upn))
                {
                    this.tokenCache[GetCacheKey(resource, upn)] = authModel;
                }
                return authModel;
            }
            catch (Exception ex)
            {
                throw new Exception("Error acquiring the AAD authentication token" + ex.Message);
            }
        }

        /// <summary>
        /// Checks existing token for authentication.
        /// </summary>
        /// <param name="resourceId">The target token resource ID.</param>
        /// <param name="uniqUserId">The unique id representing user.</param>
        /// <returns>Authentication result.</returns>
        private AuthModel CheckExisting(string resourceId, string uniqUserId)
        {
            // Check if token is valid for sometime in future, this example uses token only if it is valid for next 30 seconds
            if (!this.tokenCache.TryGetValue(GetCacheKey(resourceId, uniqUserId), out AuthModel output) || output.ExpiresOn.AddSeconds(-TokenExpirationBufferSeconds) < DateTime.UtcNow)
            {
                return null;
            }
            return output;
        }

        /// <summary>
        /// Gets the certificate for this AAD process.
        /// </summary>
        /// <returns>An X.509 certificate.</returns>
        /// <exception cref="CertificateRetrievalException">Error retrieving the AAD authentication certificate</exception>
        private X509Certificate2 GetCert()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.certThumbPrint))
                {
                    return X509CertificateManager.GetCertificateFromPersonalStoreByThumbprint(this.storeLocation, this.certThumbPrint);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving the AAD authentication certificate", ex);
            }
        }
    }
}
