using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Data.Json;

namespace BackgroundService
{
    public sealed class MGTVVoiceCommandService : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;
        VoiceCommandServiceConnection voiceServiceConnection;
        List<Video> data;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Take a service deferral so the service isn't terminated
            this.serviceDeferral = taskInstance.GetDeferral();
            data = await GetRecommendation();

            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null && triggerDetails.Name == "MGTVVoiceCommandService")
            {
                try
                {
                    voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                    voiceServiceConnection.VoiceCommandCompleted += VoiceCommandCompleted;

                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                    switch (voiceCommand.CommandName)
                    {
                        case "ShowSomeMovies":
                            string content = string.Empty;
                            if (voiceCommand.Properties.ContainsKey("content"))
                            {
                                content = voiceCommand.Properties["content"][0];
                            }
                            SendCompletionMessageForSearchContent(content);
                            break;

                        // As a last resort launch the app in the foreground
                        default:
                            LaunchAppInForeground();
                            break;
                    }
                }
                finally
                {
                    if (this.serviceDeferral != null)
                    {
                        //Complete the service deferral
                        this.serviceDeferral.Complete();
                    }
                }
            }
        }

        private void VoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                // Insert your code here
                // Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }

        private async void SendCompletionMessageForSearchContent(string content)
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Which one do you wanna see?";
            userMessage.SpokenMessage = "Which one do you wanna see?";

            var contentItemTiles = new List<VoiceCommandContentTile>();

            List<Video> itemData = new List<Video>();

            if (content.Equals("movies"))
            {
                // 电影为 Recommendations 的第 3、4 条
                //
                itemData.Add(data[2]);
                itemData.Add(data[3]);
            }
            else
            {
                // 电视剧为 Recommendations 的第 1、2 条
                //
                itemData.Add(data[0]);
                itemData.Add(data[1]);
            }

            if (data != null)
            {
                foreach (var item in itemData)
                {
                    contentItemTiles.Add(new VoiceCommandContentTile()
                    {
                        ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                        AppLaunchArgument = item.Id.ToString(),
                        Title = item.Title,
                        TextLine1 = item.Desc
                    });
                }
            }

            var response = VoiceCommandResponse.CreateResponse(userMessage, contentItemTiles);
            response.AppLaunchArgument = "";

            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async Task<List<Video>> GetRecommendation()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(GetListURL);
            string content = await response.Content.ReadAsStringAsync();
            JsonObject json = JsonObject.Parse(content);
            string recommendationData = json["data"].GetArray()[0].Stringify();
            Recommendation data = Deserialize(recommendationData, typeof(Recommendation));

            List<Video> result = new List<Video>();

            if (data != null
                && data.Children.Recommendations.Length >= 4)
            {
                
                result.Add(data.Children.Recommendations[0]);
                result.Add(data.Children.Recommendations[1]);
                result.Add(data.Children.Recommendations[2]);
                result.Add(data.Children.Recommendations[3]);
            }

            return result;
        }

        private const string GetListURL = "http://win.api.hunantv.com/v1/channel/getList";

        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Opening Media Player";

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            // When launching the app in the foreground, pass an app 
            // specific launch parameter to indicate what page to show.
            response.AppLaunchArgument = "";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null)
            {
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }

        private Recommendation Deserialize(string json, Type type)
        {
            try
            {
                if (string.IsNullOrEmpty(json)) return null;

                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                    Recommendation value = serializer.ReadObject(ms) as Recommendation;
                    return value;
                }
            }
            catch
            {
                return null;
            }
        }
    }

    [DataContract]
    public sealed class Video
    {
        [DataMember(Name = "videoId")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "desc")]
        public string Desc { get; set; }

        public Video()
        {
            Id = -1;
            Title = string.Empty;
            Desc = string.Empty;
        }
    }

    [DataContract]
    public sealed class Recommendation
    {
        [DataMember(Name = "channelId")]
        public int Id { get; set; }

        [DataMember(Name = "channelName")]
        public string Name { get; set; }

        [DataMember(Name = "iconUrl")]
        public string IconUrl { get; set; }

        [DataMember(Name = "children")]
        public RecommendationDetail Children { get; set; }

        public Recommendation()
        {
            Id = -1;
            Name = string.Empty;
            IconUrl = string.Empty;
            Children = new RecommendationDetail();
        }
    }

    [DataContract]
    public sealed class RecommendationDetail
    {
        [DataMember(Name = "recommend")]
        public Video[] Recommendations { get; set; }

        public RecommendationDetail()
        {

        }
    }
}
