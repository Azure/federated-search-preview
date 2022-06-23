//---------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Microsoft.SearchProvider.Bots
{
    public class Constants
    {
        public const string SearchVersion = "microsoft/search";

        public const string ApplicationSearchVersion = "application/search";

        public const string WelcomeText = @"This bot will introduce you to the third-party federated search provider. Type anything related with the triggering phrases file you uploaded to the search channel.";

        public const string Json = @"{
                          conversationFinished: true,
                           isErrorResponse: false,
                           version: '5.0'
                        }";

        /// <summary>
        /// Active Directory Authority
        /// </summary>
        public const string Authority = "https://login.microsoftonline.com/";

        /// <summary>
        /// Resource ID of target API which will be used to generate token which target API will allow to access
        /// </summary>
        public const string TargetApiResourceId = "https://MyApp.myCompany.com";

        /// <summary>
        /// Resource ID of target API which will be included in generated token.
        /// Target API should be configured to allow your app to generate token
        /// </summary>
        public const string ActiveDirectoryClientId = "MyApp AAD ID";

        /// <summary>
        /// Location where app certificate can be found
        /// </summary>
        public const string  ActiveDirectoryCertificateLocation = "CurrentUser";

        /// <summary>
        /// MUST CHANGE TO REAL VALUE
        /// Thumbprint for certificate owned by this application 
        /// This can also be a subject name but function to retrieve certificate need to change
        /// </summary>
        public const string WEBSITE_LOAD_CERTIFICATES = "App certificate thumbprint";

        public const int MinProps = 3;

        public const int MaxPerPage = 10;

        public const string ResponseSuccessfulType = "application/vnd.microsoft.search.searchResponse";

        public const string ResponseErrorType = "application/vnd.microsoft.error";

        public const int MaxLengthForDisplayUrl = 60;

        public const int SizeForPlaceHolderImg = 64;

        public const int SizeForPlaceHolderImgFont = 28;

        public const string Format = "yyyy-MM-ddTHH:mm:ssZ";

        public const string AdaptiveCardTemplateName = "MyUpdateCard.json";

        public const string AdaptiveCardTemplateNameVertical = "MyCardVertical.json";

        public const string DefaultOutput = "no data";

        public const int MaxFieldsPerColumn = 2;
    }
}
