// <copyright file="X509CertificateManager.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.SearchProvider.Bots
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Class for Certificate operations.
    /// </summary>
    public static class X509CertificateManager
    {

        /// <summary>
        /// Get certificate based on passed parameters
        /// </summary>
        /// <param name="storeLocation">Store Location</param>
        /// <param name="storeName">Store Name</param>
        /// <param name="findType">X509FindType object</param>
        /// <param name="findValue">object to find</param>
        /// <returns>X509Certificate2 object</returns>
        public static X509Certificate2 GetCertificate(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findValue)
        {
            var certificateCollection = GetCertificateCollection(storeLocation, storeName, findType, findValue);
            return GetFirstValidCertificateFromCollection(certificateCollection);
        }

        /// <summary>
        /// Get certificate from personal store  and thumbprint value
        /// </summary>
        /// <param name="storeLocation">Store Location</param>
        /// <param name="findValue">string value to find</param>
        /// <returns>X509Certificate2 object</returns>
        public static X509Certificate2 GetCertificateFromPersonalStoreByThumbprint(StoreLocation storeLocation, string findValue)
        {
            return GetCertificate(storeLocation, StoreName.My, X509FindType.FindByThumbprint /*or X509FindType.FindBySubjectName*/ , findValue);
        }

        /// <summary>
        /// GetCertificateCollection
        /// </summary>
        /// <param name="storeLocation">store Location</param>
        /// <param name="storeName">store Name</param>
        /// <param name="findType">X509FindType object</param>
        /// <param name="findValue">object to find</param>
        /// <returns>X509Certificate2Collection object</returns>
        private static X509Certificate2Collection GetCertificateCollection(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findValue)
        {
            var store = new X509Store(
                storeName,
                storeLocation);
            store.Open(
                OpenFlags.OpenExistingOnly |
                OpenFlags.ReadOnly);
            if (store.Certificates.Count == 0)
            {
                throw new Exception($"Unable to find any certificates in store {storeLocation}/{storeName}");
            }

            var certificateCollection = store.Certificates.Find(
                    findType,
                    findValue,
                    false);

            if (certificateCollection.Count < 1)
            {
                throw new Exception($"Unable to find certificate of {findValue} in store {storeLocation}/{storeName}");
            }

            return (certificateCollection.Count < 1) ? null : certificateCollection;
        }

        /// <summary>
        /// Get first Valid Certificate from certificate collection
        /// </summary>
        /// <param name="certificateCollection">X509Certificate2Collection</param>
        /// <returns>X509Certificate2 object</returns>
        private static X509Certificate2 GetFirstValidCertificateFromCollection(X509Certificate2Collection certificateCollection)
        {
            if (certificateCollection == null)
            {
                return null;
            }

            return certificateCollection.Cast<X509Certificate2>().FirstOrDefault();
        }
    }
}
