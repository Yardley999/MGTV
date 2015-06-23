using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace BackgroundService
{
    public sealed class MGTVVoiceCommandService : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;
        VoiceCommandServiceConnection voiceServiceConnection;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Take a service deferral so the service isn't terminated
            this.serviceDeferral = taskInstance.GetDeferral();

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
                        case "ShowSomeMovices":
                            string content = string.Empty;
                            if(voiceCommand.Properties.ContainsKey("content"))
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

            var destinationsContentTiles = new List<VoiceCommandContentTile>();

            if (content.Equals("movies"))
            {
                var destinationTile1 = new VoiceCommandContentTile();
                destinationTile1.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                destinationTile1.Title = "洛杉矶之战";
                destinationTile1.AppLaunchArgument = "1708618";
                destinationTile1.TextLine1 = "别辜负你的肾上腺";
                destinationsContentTiles.Add(destinationTile1);

                var destinationTile2 = new VoiceCommandContentTile();
                destinationTile2.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                destinationTile2.Title = "绝地战警";
                destinationTile2.AppLaunchArgument = "1708599";
                destinationTile2.TextLine1 = "好莱坞的坏男孩";
                destinationsContentTiles.Add(destinationTile2);
            }
            else
            {
                var destinationTile1 = new VoiceCommandContentTile();
                destinationTile1.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                destinationTile1.Title = "茗天闪亮";
                destinationTile1.AppLaunchArgument = "1711175";
                destinationTile1.TextLine1 = "正浩与成峰同床共枕";
                destinationsContentTiles.Add(destinationTile1);

                var destinationTile2 = new VoiceCommandContentTile();
                destinationTile2.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                destinationTile2.Title = "花千骨"; 
                destinationTile2.AppLaunchArgument = "1710137";
                destinationTile2.TextLine1 = "糖宝爆料画骨师徒八卦";
                destinationsContentTiles.Add(destinationTile2);
            }

            var response = VoiceCommandResponse.CreateResponse(userMessage, destinationsContentTiles);
            response.AppLaunchArgument = "";

            await voiceServiceConnection.ReportSuccessAsync(response);
        }

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

        /// <summary>
        /// When the background task is cancelled, clean up/cancel any ongoing long-running operations.
        /// This cancellation notice may not be due to Cortana directly. The voice command connection will
        /// typically already be destroyed by this point and should not be expected to be active.
        /// </summary>
        /// <param name="sender">This background task instance</param>
        /// <param name="reason">Contains an enumeration with the reason for task cancellation</param>
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null)
            {
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }
    }
}
