using MGTV.MG.API;
using MGTV.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace MGTV
{
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel viewModel = new MainPageViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            this.root.DataContext = viewModel;
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.NavigationMode == NavigationMode.Back)
            {
                return;
            }

            LoadDataAysnc();
        }

        public async Task LoadDataAysnc()
        {
            await ChannelAPI.GetList(9, channels => {
                if (channels == null)
                {
                    return;
                }

                viewModel.Categories.Clear();

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

                        viewModel.Categories.Add(category);
                    }
                }

                // lazy init
                //
                this.FindName("contentScrollViewer");

            }, error => {

            });
        }

        private Video CopyVideoData(MG.DataModels.Video source)
        {
            Video video = new Video();
            video.Name = source.Title;
            video.VideoId = source.Id;
            video.ImageUrl = source.ImageUrl;
            video.Intro = source.Title;

            return video;
        }
    }
}
