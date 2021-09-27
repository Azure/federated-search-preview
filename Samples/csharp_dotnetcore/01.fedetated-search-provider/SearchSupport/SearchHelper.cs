// <copyright file="EchoBackDialog.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.SearchProvider.Bots
{
    using AdaptiveCards;
    using EchoBot.Models;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.SearchProvider.Bots.Clients;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using ErrorResponse = EchoBot.Models.ErrorResponse;

    public static class SearchHelper
    {
        /// <summary>
        /// The type of the invoke response corresponding to an error.
        /// </summary>
        private const string ErrorInvokeResponseType = Constants.ResponseErrorType;

        /// <summary>
        /// The type of the invoke response corresponding to search results.
        /// </summary>
        private const string SearchInvokeResponseType = Constants.ResponseSuccessfulType;

        /// <summary>
        /// Layout ID of the search results.
        /// </summary>
        private const string layoutId = "search_layout";

        /// <summary>
        /// ID of the search channel.
        /// </summary>
        private const string SearchChannelId = "searchassistant";

        //This method is only for testing from Bot Emulator and in Web Chat (in Azure portal).
        public static async Task RunSearchForUser(
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken = default)
        {

            string json = Constants.Json;
            JObject jObj = JObject.Parse(json);
            IMessageActivity activity = turnContext.Activity;

            var MyDataSourceClient = new Clients.MyDataSourceServiceClient();
            var responsePost = await MyDataSourceClient.MyDataSourceSearch(activity.Text);

            activity.ChannelData = jObj;

            activity.Speak = activity.Text;
            activity.InputHint = InputHints.ExpectingInput;
            activity.Attachments = new List<Attachment>
            {
                new Attachment
                {
                    Content = new AdaptiveCard("1.0")
                    {
                        Body = GetAdaptiveCard(responsePost, true)
                    },
                    ContentType = AdaptiveCard.ContentType,
                    Name = "SearchCard"
                }
            };

            IActivity a = (IActivity)activity;
            await turnContext.SendActivityAsync(a, cancellationToken);
            return;
        }

        private static SearchBotAuthorizationToken GetSearchOboToken(IInvokeActivity activity)
        {
            SearchBotAuthorizationToken token = null;
            if (activity.ChannelId == SearchChannelId)
            {
                dynamic data = activity.ChannelData;
                string traceId = null;

                List<SearchBotAuthorizationToken> listOfTokens = new();
                if (data?.authorizations != null)
                {
                    foreach (var a in data.authorizations)
                    {
                        listOfTokens.Add(JsonConvert.DeserializeObject<SearchBotAuthorizationToken>(JsonConvert.SerializeObject(a)));
                    }

                    token = listOfTokens?.Where(item => item.AuthType == AuthTypes.OBOToken).FirstOrDefault();
                    traceId = data?.traceId;
                }

                if (traceId != null)
                {
                    // The trace ID is a GUID that identifies the request.
                    // You can use this ID when you log debugging information.
                }
            }

            return token;
        }

        /// <summary>
        /// Returns an invoke response for the latest version of the search protocol
        /// </summary>
        /// <param name="turnContext">The turn context</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The invoke response</returns>
        /// <remarks>This method creates the search response for the federated search provider.</remarks>
        internal static async Task<InvokeResponse> RunFederatedSearch(
            ITurnContext<IInvokeActivity> turnContext,
            CancellationToken cancellationToken = default)
        {
            IInvokeActivity activity = turnContext.Activity;
            SearchRequest request = JObject.FromObject(activity.Value).ToObject<SearchRequest>();

            // Use this token when you need to get information that requires user authentication.
            var oboToken = GetSearchOboToken(activity);

            //Here is identified whether the search results will be displayed in a custom vertical tab.
            bool isVertical = string.Equals("search", request.Kind, StringComparison.OrdinalIgnoreCase);

            //Here is identified whether the search result will be displayed as an answer in the All tab.
            bool isAnswer = string.Equals("searchAnswer", request.Kind, StringComparison.OrdinalIgnoreCase);

            int lowerIndex = 0;
            int upperIndex = 0;

            if (!isVertical && !isAnswer)
            {
                return new InvokeResponse
                {
                    Status = 200,
                    Body = new InnerInvokeResponse()
                    {
                        StatusCode = 400,
                        Type = ErrorInvokeResponseType,
                        Value = JObject.FromObject(new ErrorResponse()
                        {
                            Code = "invalid_kind",
                            Message = $"Received unexpected kind {request.Kind}",
                        }),
                    },
                };
            }

            // Add here the logic to create the SearchResult with the data retrieved from your data source
            if (isVertical && (request.QueryOptions == null || request.QueryOptions.Top <= 0 || request.QueryOptions.Skip < 0))
            {
                return new InvokeResponse
                {
                    Status = 200,
                    Body = new InnerInvokeResponse()
                    {
                        StatusCode = 400,
                        Type = ErrorInvokeResponseType,
                        Value = JObject.FromObject(new ErrorResponse()
                        {
                            Code = "invalid_queryoptions",
                            Message = $"Received unexpected query options",
                        }),
                    },
                };
            }
            else if (isVertical)
            {
                lowerIndex = request.QueryOptions.Skip;
                upperIndex = request.QueryOptions.Skip + request.QueryOptions.Top - 1;
            }

            string queryString = request.QueryText;

            //Here the data from your data source is received.            
            var MyDataSourceClient = new Clients.MyDataSourceServiceClient();
            var responsePost = await MyDataSourceClient.MyDataSourceSearch(queryString, oboToken, cancellationToken);

            List<SearchResult> searchResults = new();

            //Here the search result for Answer is created
            if (isAnswer)
            {
                searchResults.Add(new SearchResult()
                {
                    Value = "1",
                    LayoutId = layoutId,
                    Data = new SearchResultData()
                    {
                        SearchResultText = responsePost.ResponseText,
                    },
                });
            }

            //Here the search result for Vertical is created
            // In this example we have only two results. Add each of them if the query options allow for them
            else if (isVertical)
            {
                if (lowerIndex <= 0 && 0 <= upperIndex)
                {
                    searchResults.Add(new SearchResult()
                    {
                        Value = "result1",
                        LayoutId = layoutId,
                        Data = new SearchResultData()
                        {
                            SearchResultText = responsePost + "_1",
                        },
                    });
                }
                if (lowerIndex <= 1 && 1 <= upperIndex)
                {
                    searchResults.Add(new SearchResult()
                    {
                        Value = "result2",
                        LayoutId = layoutId,
                        Data = new SearchResultData()
                        {
                            SearchResultText = responsePost + "_2",
                        },
                    });
                }
            }

            //Here the adaptive card for rendering the search results are created.
            AdaptiveCard adaptiveCard = new("1.0")
            {
                Body = GetAdaptiveCard(responsePost, isAnswer)
            };

            if (isVertical)
            {
                return new InvokeResponse
                {
                    Status = 200,
                    Body = new InnerInvokeResponse()
                    {
                        StatusCode = 200,
                        Type = SearchInvokeResponseType,
                        Value = JObject.FromObject(new SearchResponse()
                        {
                            // If we were asked only for result 0, we have one more result available. 
                            // Otherwise, we don't

                            MoreResultsAvailable = upperIndex == 0,
                            TotalResultCount = 2,
                            Results = searchResults,
                            DisplayLayouts = new List<DisplayLayout>()
                        {
                            new DisplayLayout()
                            {
                                LayoutId = layoutId,
                                LayoutBody = JsonConvert.SerializeObject(adaptiveCard),
                            },
                        },
                        }),
                    },
                };
            }
            else if (isAnswer)
            {
                return new InvokeResponse
                {
                    Status = 200,
                    Body = new InnerInvokeResponse()
                    {
                        StatusCode = 200,
                        Type = SearchInvokeResponseType,
                        Value = JObject.FromObject(new SearchResponse()
                        {
                            // Expected to be false for answers
                            MoreResultsAvailable = false,
                            Results = searchResults,
                            DisplayLayouts = new List<DisplayLayout>()
                        {
                            new DisplayLayout()
                            {
                                LayoutId = layoutId,
                                LayoutBody = JsonConvert.SerializeObject(adaptiveCard),
                            },
                        },
                        }),
                    }
                };
            }
            else
            {
                return new InvokeResponse
                {
                    Status = 200,
                    Body = new InnerInvokeResponse()
                    {
                        StatusCode = 400,
                        Type = ErrorInvokeResponseType,
                        Value = JObject.FromObject(new ErrorResponse()
                        {
                            Code = "invalid_kind",
                            Message = $"Received unexpected kind {request.Kind}",
                        }),
                    },
                };
            }
        }

        /// <summary>
        /// Create adaptive cards for rendering search results.
        /// </summary>
        /// <param name="response">Search result returned for the query.</param>
        /// <param name="isAnswer">The kind of search result to be rendered.</param>
        /// <returns>Returns Adaptive element</returns>
        /// <remarks>The response format for a vertical is different from that for an answer.
        /// You will need to format your query results based on the shape of your response object.</remarks>
        private static List<AdaptiveElement> GetAdaptiveCard(ResponseObject response, bool isAnswer)
        {
            //Adaptive card for verticals
            if (!isAnswer)
            {
                return new List<AdaptiveElement>
                {
                    new AdaptiveContainer
                    {
                        Spacing = AdaptiveSpacing.None,
                        Separator = false,
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock
                            {
                                Type = AdaptiveTextBlock.TypeName,
                                Text = response.ResponseText,
                                Size = AdaptiveTextSize.Medium,
                                Wrap = true
                            }
                        }
                    }
                };
            }
            //Adaptive card for answers
            else
            {
                return new List<AdaptiveElement>
                {
                    new AdaptiveContainer
                    {
                        Spacing = AdaptiveSpacing.Small,
                        Separator = false,
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock
                            {
                                Type = AdaptiveTextBlock.TypeName,
                                Text = "High confidence search result, aka Answer",
                                Size = AdaptiveTextSize.Large,
                                Color = AdaptiveTextColor.Accent,
                                Weight = AdaptiveTextWeight.Bolder,
                                Wrap = true
                            },

                            new AdaptiveImage
                            {
                                Type = AdaptiveImage.TypeName,
                                UrlString  = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU",
                                Size = AdaptiveImageSize.Medium,
                                Style = AdaptiveImageStyle.Person,
                                HorizontalAlignment = AdaptiveHorizontalAlignment.Left
                            },
                            new AdaptiveTextBlock
                            {
                                Type = AdaptiveTextBlock.TypeName,
                                Text = response.ResponseText,
                                Size = AdaptiveTextSize.Medium,
                                Color = AdaptiveTextColor.Dark,
                                Wrap = true
                            },
                        }
                    }
                };
            }
        }
    }
}
