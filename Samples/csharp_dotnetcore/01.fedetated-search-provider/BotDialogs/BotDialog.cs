// <copyright file="EchoBackDialog.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.SearchProvider.Bots.BotDialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using EchoBot.Models;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    

    using ErrorResponse = EchoBot.Models.ErrorResponse;
   

    public class BotDialog : ComponentDialog
    {
        /// <summary>
        /// The type of the invoke response corresponding to an error
        /// </summary>
        private const string ErrorInvokeResponseType = Constants.ResponseErrorType;

        /// <summary>
        /// The type of the invoke response corresponding to search results
        /// </summary>
        private const string SearchInvokeResponseType = Constants.ResponseSuccessfulType;

        /// <summary>
        /// Endpoint of the third-party data source, from where you want to get the search results. Replace it with the endpoint of your data source.
        /// </summary>
        private const string endpoint = Constants.Endpoint;

        /// <summary>
        /// Layout ID of the search results
        /// </summary>
        private const string layoutId = "search_layout";
        
        //private readonly HttpClient httpClient;
        
        //public BotDialog(HttpClient client)
        //{
        //    this.httpClient = client ?? throw new ArgumentNullException(nameof(client));
        //}

        public BotDialog() : base(nameof(BotDialog))
        {
        }

        //This method is only for testing from Bot Emulator and Test in web chat (Azure Portal)
        public static async Task SendDialog(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            string json = Constants.Json;
            JObject jObj = JObject.Parse(json);
            IMessageActivity activity = turnContext.Activity;

            var MyDataSourceClient = new Clients.MyDataSourceServiceClient();
            string responsePost = MyDataSourceClient.MyDataSourceSearch(endpoint, activity.Text);

            activity.ChannelData = jObj;
            
            activity.Speak = activity.Text;
            activity.InputHint = InputHints.ExpectingInput;
            activity.Attachments = new List<Attachment>();
            activity.Attachments.Add(
                new Attachment
                {
                    Content = new AdaptiveCard("1.0")
                    {
                        Body = GetAdaptiveCard(responsePost, true)
                    },
                    ContentType = AdaptiveCard.ContentType,
                    Name = "SearchCard"
                }
            );
            
            IActivity a = (IActivity)activity;
            await turnContext.SendActivityAsync(a, cancellationToken);
            return;
        }

        //This is the function creates the search response that will be send by the federated search provider.
        /// <summary>
        /// Returns an invoke response for the latest version of the search protocol
        /// </summary>
        /// <param name="turnContext">The turn context</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The invoke response</returns>
        internal static InvokeResponse SendDialogInInvokeResponse(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            IInvokeActivity activity = turnContext.Activity;
            SearchRequest request = JObject.FromObject(activity.Value).ToObject<SearchRequest>();

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

            // Add here the logic to create the SearchResult with the data retireved from your data source
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

            string query = request.QueryText;

            //Here the data from your data source is recieved.            
            var MyDataSourceClient = new Clients.MyDataSourceServiceClient();
            string responsePost = MyDataSourceClient.MyDataSourceSearch(endpoint, query);

            List<SearchResult> searchResults = new List<SearchResult>();
            
            //Here the search result for Answer is created
            if (isAnswer)
            {
                searchResults.Add(new SearchResult()
                {
                    Value = "1",
                    LayoutId = layoutId,
                    Data = new SearchResultData()
                    {
                        SearchResultText = responsePost,
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
            AdaptiveCard adaptiveCard = new AdaptiveCard("1.0")
            {
                Body = GetAdaptiveCard("Result text: {searchResultText}", isAnswer)               
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

        /// Create adaptive cards for redering search results.
        /// </summary>
        /// <param name="SearchResultText">Search result returned for the query.</param>
        /// <param name="isAnswer">The kind of search result to be rendered.</param>
        /// <returns>Returns Adaptive element</returns>
        private static List<AdaptiveElement> GetAdaptiveCard(string SearchResultText, bool isAnswer)
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
                                Text = SearchResultText,
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
                                Text = SearchResultText,
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
