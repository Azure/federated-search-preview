// <copyright file="IAadTokenResolver.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.SearchProvider.Bots
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for implementing AAD authentication token resolver for the requested resource.
    /// </summary>
    public interface IAadTokenResolver
    {

        /// <summary>
        /// Asynchronously gets the on behalf of token value.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="token">The scope.</param>
        /// <returns>The token value.</returns> 
        Task<string> GetOnBehalfOfTokenAsync(string authority, string resource, string token);
    }
}