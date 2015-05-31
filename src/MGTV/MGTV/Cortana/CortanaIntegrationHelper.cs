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

#if !DEBUG
                try
                {
#endif
                    await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vhdFile);
                    foreach (var item in VoiceCommandDefinitionManager.InstalledCommandDefinitions.Values)
                    {
                        Debug.WriteLine(item.Language);
                        Debug.WriteLine(item.Name);
                    }
#if !DEBUG
                }
                catch
                {
                }
#endif
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
                default:
                    App.Instance.Frame.Navigate(typeof(MainPage), voiceCommand);
                    Window.Current.Activate();
                    break;
            }
        }

        private static string SemanticInterpretation(string key, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[key].FirstOrDefault();
        }

    }
}
