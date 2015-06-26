using MGTV.Common;
using MGTV.Pages;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Xaml;

namespace MGTV.Cortana
{
    public class VoiceCommand
    {
        public string Content { get; set; }

        public VoiceCommand()
        {
            Content = string.Empty;
        }
    }

    public class CortanaIntegrationHelper
    {
        public static async void CreateOrUpdateVoiceCommands()
        {
            Uri voiceCommandsDefiniationXml = new Uri("ms-appx:///Cortana/VoiceCommands.xml");
            var vhdFile = await StorageFile.GetFileFromApplicationUriAsync(voiceCommandsDefiniationXml);
            if (vhdFile != null)
            {
                try
                {
                    await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vhdFile);
#if DEBUG
                    foreach (var item in VoiceCommandDefinitionManager.InstalledCommandDefinitions.Values)
                    {
                        Debug.WriteLine(item.Language);
                        Debug.WriteLine(item.Name);
                    }
#endif
                }
                catch
                {
                }
            }
        }

        public static void HandleCommands(VoiceCommandActivatedEventArgs argus)
        {
            SpeechRecognitionResult speechRecognitionResult = argus.Result;

            string voiceCommandName = speechRecognitionResult.RulePath[0];
            Type NavigationPageType = null;
            VoiceCommand voiceCommand = new VoiceCommand();

            switch (voiceCommandName.ToLower())
            {
                case "closeapp":
                    Application.Current.Exit();
                    break;
                case "openapp":
                    if(App.Instance.Frame.Content == null)
                    {
                        NavigationPageType = typeof(MainPage);
                        App.Instance.Frame.Navigate(NavigationPageType, voiceCommand);
                    }
                    Window.Current.Activate();
                    break;
                case "continuevideo":

                    var settings = ApplicationData.Current.LocalSettings.Values;
                    object videoInfo = string.Empty;
                    if(settings.TryGetValue(Constants.CurrentPlayingVideoInfo, out videoInfo))
                    {
                        string[] temp = videoInfo.ToString().Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        if(temp.Length == 2)
                        {
                            VideoPlayPage.PageParams paras = new VideoPlayPage.PageParams();
                            paras.VideoId = Int32.Parse(temp[0]);
                            paras.StartPosition = TimeSpan.Parse(temp[1]);
                            paras.IsLanuchFromSerivice = true;

                            App.Instance.Frame.Navigate(typeof(VideoPlayPage), paras);
                            Window.Current.Activate();
                        }
                    }

                    break;
                case "playvideo":
                    if(IsInVideoPlayerPage())
                    {
                        var videoPlayer = App.Instance.Frame.Content as VideoPlayPage;
                        videoPlayer.Play();
                    }
                    break;
                case "pausevideo":
                    if(IsInVideoPlayerPage())
                    {
                        var videoPlayer = App.Instance.Frame.Content as VideoPlayPage;
                        videoPlayer.Pause();
                    }
                    break;
                case "stopvideo":
                    if(IsInVideoPlayerPage())
                    {
                        var videoPlayer = App.Instance.Frame.Content as VideoPlayPage;
                        videoPlayer.Stop();
                    }
                    break;
                default:
                    App.Instance.Frame.Navigate(typeof(MainPage), voiceCommand);
                    Window.Current.Activate();
                    break;
            }
        }

        private static bool IsInVideoPlayerPage()
        {
            var content = App.Instance.Frame.Content;
            if(content == null)
            {
                return false;
            }

            return content is VideoPlayPage;
        }

        private static string SemanticInterpretation(string key, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[key].FirstOrDefault();
        }

    }
}
