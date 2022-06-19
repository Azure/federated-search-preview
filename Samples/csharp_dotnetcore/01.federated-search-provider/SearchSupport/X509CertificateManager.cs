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
        /// Gets the certificate fromstring.
        /// </summary>
        /// <param name="value">base64 certificate string</param>
        /// <returns>X509Certificate2</returns>
        public static X509Certificate2 GetCertificate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return new X509Certificate2(Convert.FromBase64String(value));
        }

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
        /// Gets the certificate by friendly name.
        /// </summary>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="certFriendlyName">Freindly name of the cert.</param>
        /// <returns>
        /// X509Certificate2 object
        /// </returns>
        public static X509Certificate2 GetCertificateByFriendlyName(StoreLocation storeLocation, string certFriendlyName)
        {
            var certCollection = GetCertificateCollection(storeLocation, StoreName.My, X509FindType.FindBySubjectName, certFriendlyName);
            return certCollection.Cast<X509Certificate2>().Single(_ => _.FriendlyName.Equals(certFriendlyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get certificate from current user personal store based on passed parameters
        /// </summary>
        /// <param name="findType">X509FindType object</param>
        /// <param name="findValue">object to find</param>
        /// <returns>X509Certificate2 object</returns>
        public static X509Certificate2 GetCertificateFromCurrentUserPersonalStore(X509FindType findType, object findValue)
        {
            return GetCertificate(StoreLocation.CurrentUser, StoreName.My, findType, findValue);
        }

        /// <summary>
        /// Get certificate from current user personal store and thumbprint value
        /// </summary>
        /// <param name="findValue">string value to find</param>
        /// <returns>X509Certificate2 object</returns>
        public static X509Certificate2 GetCertificateFromCurrentUserPersonalStoreThumbprint(string findValue)
        {
            return GetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindByThumbprint, findValue);
        }

        /// <summary>
        /// Get certificate from Local Machine personal store based on passed parameters
        /// </summary>
        /// <param name="findType">X509FindType object</param>
        /// <param name="findValue">object to find</param>
        /// <returns>X509Certificate2 object</returns>
        public static X509Certificate2 GetCertificateFromLocalMachinePersonalStore(X509FindType findType, object findValue)
        {
            return GetCertificate(StoreLocation.LocalMachine, StoreName.My, findType, findValue);
        }

        /// <summary>
        /// Get certificate from local machine personal store and thumbprint value
        /// </summary>
        /// <param name="findValue">string value to find</param>
        /// <returns>X509Certificate2 object</returns>
        public static X509Certificate2 GetCertificateFromLocalMachinePersonalStoreThumbprint(string findValue)
        {
            return GetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, findValue);
        }

        /// <summary>
        /// Get certificate from personal store  and thumbprint value
        /// </summary>
        /// <param name="storeLocation">Store Location</param>
        /// <param name="findValue">string value to find</param>
        /// <returns>X509Certificate2 object</returns>
        public static X509Certificate2 GetCertificateFromPersonalStoreByThumbprint(StoreLocation storeLocation, string findValue)
        {
            return GetCertificate(storeLocation, StoreName.My, X509FindType.FindByThumbprint, findValue);
        }

        /// <summary>
        /// Gets the certificate tostring.
        /// </summary>
        /// <param name="cert">X509Certificate2</param>
        /// <returns>base64 certificate string</returns>
        public static string GetCertificateString(X509Certificate2 cert)
        {
            if (cert == null)
            {
                return string.Empty;
            }

            byte[] certificate = cert.HasPrivateKey
                         ? cert.Export(X509ContentType.Pfx)
                         : cert.Export(X509ContentType.Cert);
            return Convert.ToBase64String(certificate);
        }

        /// <summary>
        /// Gets the name of the certificate subject.
        /// </summary>
        /// <param name="certificate">X509Certificate</param>
        /// <param name="trimPrefix">[Optional] Trim prefix value with default value as true.</param>
        /// <returns>string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.StartsWith(System.String)", Justification = "X509 dependency")]
        public static string GetCertificateSubjectName(X509Certificate certificate, bool trimPrefix = true)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            var certificateSubjectName = certificate.Subject;

            const string prefix = "CN=";
            if (trimPrefix && certificateSubjectName.StartsWith(prefix))
            {
                return certificateSubjectName.Substring(prefix.Length);
            }

            return certificateSubjectName;
        }

        /// <summary>
        /// Removes the certificate.
        /// </summary>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="findType">Type of the find.</param>
        /// <param name="findValue">The find value.</param>
        public static void RemoveCertificate(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findValue)
        {
            var certificateToRemove = GetCertificate(storeLocation, storeName, findType, findValue);
            var store = new X509Store(
                storeName,
                storeLocation);
            store.Open(
                OpenFlags.OpenExistingOnly |
                OpenFlags.ReadWrite);
            if (store.Certificates.Count < 1)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Unable to find any certificates in store {0}/{1}", storeLocation, storeName));
            }

            store.Remove(certificateToRemove);
        }

        /// <summary>
        /// Remove certificate from local machine personal store  and thumbprint value
        /// </summary>
        /// <param name="findValue">string value to find</param>
        public static void RemoveCertificateFromLocalMachinePersonalStoreThumbprint(string findValue)
        {
            RemoveCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, findValue);
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
