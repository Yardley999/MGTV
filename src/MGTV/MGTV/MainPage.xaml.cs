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

        private Dictionary<int, string> AppBarIconMap = new Dictionary<int, string>() {
            { 1, "ms-appx:///Assets/NavListIcon/ico-nav-02.png"},
            { 2, "ms-appx:///Assets/NavListIcon/ico-nav-03.png"},
            { 3, "ms-appx:///Assets/NavListIcon/ico-nav-04.png"},
            { 4, "ms-appx:///Assets/NavListIcon/ico-nav-05.png"},
            { 5, "ms-appx:///Assets/NavListIcon/ico-nav-06.png"},
            { 7, "ms-appx:///Assets/NavListIcon/ico-nav-07.png"},
            { 8, "ms-appx:///Assets/NavListIcon/ico-nav-08.png"},
            { 10, "ms-appx:///Assets/NavListIcon/ico-nav-09.png"},
            { 9, "ms-appx:///Assets/NavListIcon/ico-nav-10.png"},
        };

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
                        string iconUrl = item.IconUrl;
                        if(string.IsNullOrEmpty(iconUrl))
                        {
                            if(AppBarIconMap.ContainsKey(item.Id))
                            {
                                iconUrl = AppBarIconMap[item.Id];
                            }
                        }

                        viewModel.ChannelNavigationItems.Add(new Channel() {
                            Id = item.Id,
                            IconUrl = iconUrl,
                            Name = item.Name
                        });
                    }
                }


            }, error => {

            });
        }

        private void TopAppBarItem_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var dataContext = sender.GetDataContext<Channel>();

            if(dataContext.Id > 0)
            {
                ChannelDetailsPage.PageParams para = new ChannelDetailsPage.PageParams();
                para.Background = currentBackground;
                para.ChannelId = dataContext.Id;
                para.ChannelName = dataContext.Name;

                Frame.Navigate(typeof(ChannelDetailsPage), para);
            }
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
            "ms-appx:///Assets/Background/1.jpg",
            "ms-appx:///Assets/Background/2.jpg",
            "ms-appx:///Assets/Background/3.jpg",
            "ms-appx:///Assets/Background/4.jpg"
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
