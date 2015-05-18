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
            TopAppBarItemListDataBinding();
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
            LoadTopAppBarItemsAsync();
        }

        #endregion

        #region Content Data

        private async Task LoadDataAysnc()
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

        #endregion

        #region App Bar

        private void TopAppBarItemListDataBinding()
        {
            topAppBarItemList.ItemsSource = viewModel.ChannelNavigationItems;
        }

        private async Task LoadTopAppBarItemsAsync()
        {
            await ChannelAPI.GetMajorList(channels => {
                viewModel.ChannelNavigationItems.Clear();
                viewModel.ChannelNavigationItems.Add(Channel.Home);

                if (channels != null)
                {
                    foreach (var item in channels)
                    {
                        viewModel.ChannelNavigationItems.Add(new Channel() {
                            Id = item.Id,
                            IconUrl = item.IconUrl,
                            Name = item.Name
                        });
                    }
                }


            }, error => {

            });
        }

        #endregion

        #region Change Background

        private void ChangeBackground_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ChangeBackground();
            LiveTileHelper.UpdateBadgeNumber(Constants.TileId, (uint)(new Random()).Next(1, 99));
            LiveTileHelper.UpdateSecondaryTileAsync(Constants.TileId, "热播新剧", TimeSpan.FromSeconds(20));
        }

        private string currentBackground = string.Empty;

        private List<string> backgourndImages = new List<string>()
        {
            "ms-appx:///Assets/Background/night.png",
            "ms-appx:///Assets/Background/ice.jpg",
            "ms-appx:///Assets/Background/ice2.jpg",
            "ms-appx:///Assets/Background/forest.jpg"
        };

        bool isBackgroundInChanging = false;

        private void BackgroundInit()
        {
            currentBackground = backgourndImages.FirstOrDefault();
            background1.Opacity = 1;
            background1.Visibility = Visibility.Visible;
            background1.Background = new ImageBrush() {
                ImageSource = new BitmapImage(new Uri(currentBackground)),
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,
                Stretch = Stretch.UniformToFill
            };

            background2.Opacity = 0;
            background2.Visibility = Visibility.Collapsed;
        }

        private string RandomSelectBackground()
        {
            var list = backgourndImages.Where(img => !img.Equals(currentBackground, StringComparison.OrdinalIgnoreCase)).ToList();
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
            TimeSpan duration = TimeSpan.FromSeconds(1.8);
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
                currentBackground = imageToShow;
            });
        }

        #endregion

        #region Live Tile

        private async Task CreateTile()
        {
            await LiveTileHelper.PinSecondaryTileAsync(Constants.TileId, Constants.TileDisplayName, string.Empty, TileSize.Wide310x150);
            LiveTileHelper.UpdateBadgeNumber(Constants.TileId, 10);
        }
        #endregion
    }
}
