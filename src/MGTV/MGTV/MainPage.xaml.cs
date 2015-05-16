using MGTV.MG.API;
using MGTV.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MGTV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel viewModel = new MainPageViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            this.root.DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadDataAysnc();
        }

        public async void LoadDataAysnc()
        {
            await ChannelAPI.GetList(9, channels => {
                if (channels == null)
                {
                    return;
                }

                viewModel.Categoyies.Clear();

                if(channels.Recommendation != null)
                {
                    viewModel.Recommendation.ChannelId = channels.Recommendation.Id;
                    viewModel.Recommendation.Name = channels.Recommendation.Name;
                    if(channels.Recommendation.Children != null)
                    {
                        if(channels.Recommendation.Children.FlashItems != null
                            &&channels.Recommendation.Children.FlashItems.Length > 0)
                        {
                            var videoData = channels.Recommendation.Children.FlashItems[0];
                            viewModel.Recommendation.Videos.Add(CopyVideoData(videoData));
                        }

                        if(channels.Recommendation.Children.Recommendations != null)
                        {
                            foreach (var item in channels.Recommendation.Children.Recommendations)
                            {
                                viewModel.Recommendation.Videos.Add(CopyVideoData(item));
                            }
                        }
                    }
                }

                if(channels.Channels != null)
                {
                    foreach (var channel in channels.Channels)
                    {
                        Category category = new Category();
                        category.ChannelId = channel.Id;
                        category.Name = channel.Name;

                        if(channel.Children != null)
                        {
                            foreach (var videoItem in channel.Children)
                            {
                                category.Videos.Add(CopyVideoData(videoItem));
                            }
                        }

                        viewModel.Categoyies.Add(category);
                    }
                }


            }, error => {

            });
        }

        private Video CopyVideoData(MG.DataModels.Video source)
        {
            Video video = new Video();
            video.Name = source.Title;
            video.VideoId = source.Id;
            video.ImageUrl = source.ImageUrl;

            return video;
        }
    }
}
