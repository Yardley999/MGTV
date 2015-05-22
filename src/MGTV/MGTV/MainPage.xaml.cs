using MGTV.MG.API;
using MGTV.ViewModels;
using SharedFx.UI.Animations;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.StartScreen;
using MGTV.Common;
using MGTV.LiveTile;
using MGTV.Pages;
using SharedFx.Extensions;
using SharedFx.Data;

namespace MGTV
{
    public sealed partial class MainPage : Page
    {
        #region Field && Property

        private MainPageViewModel viewModel = new MainPageViewModel();

        #endregion

        #region Life Cycle

        public MainPage()
        {
            this.InitializeComponent();
            this.root.DataContext = viewModel;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            BackgroundInit();
            CreateTile();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            topAppBar.IsOpen = false;

            if (e.NavigationMode == NavigationMode.Back)
            {
                this.FindName("contentScrollViewer");
                return;
            }

            LoadDataAysnc();
            navigationBar.LoadTopAppBarItemsAsync();
        }

        #endregion

        #region Content Data

        private async Task LoadDataAysnc()
        {
            indicator.IsActive = true;
            await ChannelAPI.GetList(12, channels => {
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
                
                // update live tile
                //
                if(viewModel.Recommendation != null
                    && viewModel.Recommendation.Videos.Count > 0)
                {
                    var video = viewModel.Recommendation.Videos[0];
                    ImageHelper imageHelper = new ImageHelper();
                    string fileName = MD5Core.GetHashString(video.ImageUrl) + ".jpg";
                    imageHelper.Download(video.ImageUrl, "Images", fileName, localFile =>
                    {
                        LiveTileHelper.UpdateSecondaryTileAsync(
                            Constants.TileId, 
                            video.Name, 
                            video.Intro, 
                            TimeSpan.FromMinutes(1), 
                            "ms-appdata:///local/Images/" + fileName);
                    });
                }

                // lazy init
                //
                this.FindName("contentScrollViewer");
                indicator.IsActive = false;

            }, error => {
                //indicator.IsActive = false;
            });
        }

        private Video CopyVideoData(MG.DataModels.Video source)
        {
            Video video = new Video();
            video.Name = source.Title;
            video.VideoId = source.Id;
            video.ImageUrl = source.ImageUrl;
            video.Intro = source.Desc;

            return video;
        }

        #endregion

        #region App Bar

        private void TopAppBar_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled = true;

            if (this.topAppBar.IsOpen)
            {
                this.topAppBar.IsOpen = false;
            }
        }

        #endregion

        #region Change Background

        private void ChangeBackground_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ChangeBackground();
        }

        private List<string> backgourndImages = new List<string>()
        {
            "ms-appx:///Assets/Background/0.jpg",
            "ms-appx:///Assets/Background/1.jpg",
            "ms-appx:///Assets/Background/2.jpg",
            "ms-appx:///Assets/Background/3.jpg",
            "ms-appx:///Assets/Background/4.jpg"
        };

        bool isBackgroundInChanging = false;

        private void BackgroundInit()
        {
            App.Instance.BackgroundImage = backgourndImages.FirstOrDefault();
            background1.Opacity = 1;
            background1.Visibility = Visibility.Visible;
            background1.Background = new ImageBrush() {
                ImageSource = new BitmapImage(new Uri(App.Instance.BackgroundImage)),
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,
                Stretch = Stretch.UniformToFill
            };

            background2.Opacity = 0;
            background2.Visibility = Visibility.Collapsed;
        }

        private string RandomSelectBackground()
        {
            var list = backgourndImages.Where(img => !img.Equals(App.Instance.BackgroundImage, StringComparison.OrdinalIgnoreCase)).ToList();
            return list[(new Random()).Next(0, list.Count())];
        }

        public void ChangeBackground()
        {
            if (isBackgroundInChanging)
            {
                return;
            }

            Border borderToShow;
            Border borderToHide;

            if(background1.Visibility == Visibility.Collapsed)
            {
                borderToShow = background1;
                borderToHide = background2;
            }
            else
            {
                borderToShow = background2;
                borderToHide = background1;
            }

            isBackgroundInChanging = true;
            TimeSpan duration = TimeSpan.FromSeconds(2.8);
            borderToShow.Visibility = Visibility.Visible;
            string imageToShow = RandomSelectBackground();
            borderToShow.Background = new ImageBrush() {
                ImageSource = new BitmapImage(new Uri(imageToShow, UriKind.RelativeOrAbsolute)),
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,
                Stretch = Stretch.UniformToFill
            };

            FadeAnimation.Fade(borderToHide, 1, 0, duration, null);
            FadeAnimation.Fade(borderToShow, 0, 1, duration, fe =>
            {
                borderToHide.Visibility = Visibility.Collapsed;
                isBackgroundInChanging = false;
                App.Instance.BackgroundImage = imageToShow;
            });
        }

        #endregion

        #region Live Tile

        private async Task CreateTile()
        {
            await LiveTileHelper.PinSecondaryTileAsync(Constants.TileId, Constants.TileDisplayName, string.Empty, TileSize.Wide310x150);
        }
        #endregion

       
    }
}
