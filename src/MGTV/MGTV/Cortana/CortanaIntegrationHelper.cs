using MGTV.Pages;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace MGTV.Cortana
{
    public class CortanaIntegrationHelper
    {
        public static async void CreateOrUpdateVoiceCommands()
        {
            Uri voiceCommandsDefiniationXml = new Uri("ms-appx:///Cortana/VoiceCommands.xml");
            var vhdFile = await StorageFile.GetFileFromApplicationUriAsync(voiceCommandsDefiniationXml);
            if (vhdFile != null)
            {
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vhdFile);
                try
                {
                    //await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vhdFile);
                    foreach (var item in VoiceCommandDefinitionManager.InstalledCommandDefinitions.Values)
                    {
                        Debug.WriteLine(item.Language);
                        Debug.WriteLine(item.Name);
                    }
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

            switch (voiceCommandName.ToLower())
            {
                case "opengroup":
                    string channelName = speechRecognitionResult.SemanticInterpretation.Properties["GROUPNAME"][0];
                    Debug.WriteLine("From Vioce Command: GROUPNAME={0}", channelName);
                    App.Instance.Frame.Navigate(typeof(ChannelDetailsPage));
                    break;
                default:
                    App.Instance.Frame.Navigate(typeof(MainPage));
                    break;
            }
        }

        private static string SemanticInterpretation(string key, SpeechRecognitionResult speechRecognitionResult)
        {
            if (speechRecognitionResult.SemanticInterpretation.Properties.ContainsKey(key))
            {
                return speechRecognitionResult.SemanticInterpretation.Properties[key][0];
            }
            else
            {
                return "unknown";
            }
        }

    }
}
