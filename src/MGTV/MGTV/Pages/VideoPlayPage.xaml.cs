using MGTV.Common;
using MGTV.MG.API;
using MGTV.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SharedFx.Extensions;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Animation;
using Windows.Storage;

namespace MGTV.Pages
{
    public sealed partial class VideoPlayPage : Page
    {
        #region Page Parameters

        public class PageParams
        {
            public int VideoId { get; set; }

            public bool IsLanuchFromSerivice { get; set; }

            public TimeSpan StartPosition { get; set; }

            public PageParams()
            {
                VideoId = -1;
                IsLanuchFromSerivice = false;
                StartPosition = TimeSpan.Zero;
            }
        }

        #endregion

        #region Field && Property

        private VideoPlayerPageViewModel viewModel;

        private PageParams pageParams;
        private DispatcherTimer progressTimer;
        private bool isPlaying = false;
        TimeSpan playingPosition = TimeSpan.Zero;

        #endregion

        #region Life Cycle

        public VideoPlayPage()
        {
            this.InitializeComponent();
            Init();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            pageParams = e.Parameter as PageParams;

            PlayerSetup();
            if(pageParams != null)
            {
                LoadVideoDataAsync(pageParams.VideoId);
                HideControlPanelAnimation();
            }
        }

        #endregion

        #region Data 

        private async Task LoadVideoDataAsync(int videoId)
        {
            if(pageParams == null)
            {
                return;
            }

            playingPosition = pageParams.StartPosition;

            await VideoAPI.GetById(videoId, videoInfo => {

                viewModel.Title = videoInfo.Title;
                viewModel.VideoSources.Clear();
                viewModel.IsPlaying = false;
                viewModel.VideoId = videoId;

                if (videoInfo.VideoSources != null)
                {
                    foreach (var item in videoInfo.VideoSources)
                    {
                        viewModel.VideoSources.Add(new VideoDefinationSource() {
                            Name = item.Name,
                            Url = item.Url,
                            Id = item.Definition
                        });
                    }
                }

                viewModel.PlayList.Clear();

                if (videoInfo.RelatedVideos != null)
                {
                    foreach (var item in videoInfo.RelatedVideos)
                    {
                        viewModel.PlayList.Add(new PlayListItem() {
                            IsPlaying = false,
                            Name = item.Title,
                            VideoId = item.Id
                        });
                    }
                }

                //insert current playing to list
                //
                int index = viewModel.PlayList.Count > 2 ? 1 : 0;
                viewModel.PlayList.Insert(index, new PlayListItem() {
                    IsPlaying = true,
                    Name = viewModel.Title,
                    VideoId = viewModel.VideoId
                });

                SelectVideoRate(2, viewModel.VideoSources);
                
            }, error => { });

        }

        #endregion

        #region Init

        private void Init()
        {
            viewModel = new VideoPlayerPageViewModel();
            this.root.DataContext = viewModel;
            this.playlistBox.ItemsSource = viewModel.PlayList;

            this.player.MediaOpened += Player_MediaOpened;
            this.progress.ThumbToolTipValueConverter = new PlayerTimeSliderTooltipValueConverter();
            this.volumeSlider.ThumbToolTipValueConverter = new VolumePercentageConverter();

            GetControlPanelStoryBoard();
        }

        #endregion

        #region  Setup

        private void PlayerSetup()
        {
            ResetVideoText();
            this.player.Volume = 0.5;
            this.volumeSlider.Value = this.player.Volume * 100;
        }

        private void ResetVideoText()
        {
            this.currentPosition.Text = TimeSpan.Zero.ToShortFromatString();
            this.duration.Text = TimeSpan.Zero.ToShortFromatString();
        }

        #endregion

        #region Progress Bar

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.player.Position = playingPosition;
            StatusbarSetup();
            progressTimer.Start();
        }

        private void StatusbarSetup()
        {
            ProgressBarSetup();
        }

        private void ProgressBarSetup()
        {
            this.progress.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
            this.progress.Minimum = 0;

            this.duration.Text = player.NaturalDuration.TimeSpan.ToShortFromatString();

            if (progressTimer == null)
            {
                progressTimer = new DispatcherTimer();
                progressTimer.Interval = TimeSpan.FromSeconds(1);
                progressTimer.Tick += ProgressTimer_Tick;
            }
        }

        private void ProgressTimer_Tick(object sender, object e)
        {
            progress.Value = player.Position.TotalSeconds;
            progress.DownloadProgressValue = player.DownloadProgress;
            currentPosition.Text = player.Position.ToShortFromatString();

            if (this.player.BufferingProgress >= 1)
            {
                viewModel.IsPlayButtonInPauseStatus = isPlaying;
            }
            else
            {
                viewModel.IsPlayButtonInPauseStatus = false;
            }

            SavePlayingInfo();
        }

        #endregion

        #region Player Function

        private void SelectVideoRate(int definitionId, IEnumerable<VideoDefinationSource> videoSources)
        {
            // select rate 
            //
            var toPlayVideoRate = videoSources.FirstOrDefault(v => v.Id == definitionId);
            if (toPlayVideoRate == null)
            {
                toPlayVideoRate = videoSources.FirstOrDefault();
            }

            // update menu item state
            //
            foreach(var menuItem in videoRateFlyoutMenu.Items)
            {
                var tag = menuItem.Tag.ToString();
                menuItem.IsEnabled = videoSources.Any(v => v.Id.ToString() == tag);
            }

            // play
            //
            if (toPlayVideoRate != null)
            {
                this.videoRateButton.Content = toPlayVideoRate.Name;
                SetUrlAndTryPlay(toPlayVideoRate.Url);
            }
        }

        private async void SetUrlAndTryPlay(string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                return;
            }

            string playUrl = await VideoAPI.GetRealVideoAddress(url);

            if(!string.IsNullOrEmpty(playUrl))
            {
                var hlslUrl = new Uri(playUrl, UriKind.RelativeOrAbsolute);
                player.Source = hlslUrl;
                Play();
            }
        }

        private void Next()
        {
            var currentPlaying = viewModel.PlayList.FirstOrDefault(v => v.IsPlaying);
            int index = viewModel.PlayList.IndexOf(currentPlaying);
            if(viewModel.PlayList.Count > index)
            {
                LoadVideoDataAsync(viewModel.PlayList[index + 1].VideoId);
            }
        }

        public void Play()
        {
            this.player.Play();
            isPlaying = true;
            viewModel.IsPlayButtonInPauseStatus = isPlaying;
        }

        public void Stop()
        {
            this.player.Stop();
            this.player.Position = TimeSpan.Zero;
            playingPosition = TimeSpan.Zero;
            progress.Value = 0;

            if (progressTimer != null)
            {
                progressTimer.Stop();
            }
            isPlaying = false;
            ResetVideoText();
        }

        public void Pause()
        {
            this.player.Pause();
            isPlaying = false;
            viewModel.IsPlayButtonInPauseStatus = isPlaying;
        }

        private void SetWindowFull(bool isFull)
        {
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            if (appView == null)
            {
                return;
            }

            if(isFull)
            {
                appView.TryEnterFullScreenMode();
                this.contentGrid.Margin = new Thickness(0);
                this.contentGrid.ColumnDefinitions.Last().MaxWidth = 0;
                this.contentGrid.RowDefinitions.First().MaxHeight = 0;
                this.contentGrid.RowDefinitions.Last().MaxHeight = 0;
                this.topNavigationBar.SetValue(Grid.RowProperty, 1);
            }
            else
            {
                appView.ExitFullScreenMode();
                this.contentGrid.Margin = new Thickness(50,20,30,100);
                this.contentGrid.ColumnDefinitions.Last().MaxWidth = double.MaxValue;
                this.contentGrid.RowDefinitions.First().MaxHeight = double.MaxValue;
                this.contentGrid.RowDefinitions.Last().MaxHeight = double.MaxValue;
                this.topNavigationBar.SetValue(Grid.RowProperty, 0);
                if(hideTitlePanel != null)
                {
                    hideTitlePanel.Stop();
                    this.topNavigationBar.Opacity = 1;
                }
            }
        }

        private void SavePlayingInfo()
        {
            try
            {
                if (pageParams == null)
                {
                    return;
                }

                var settings = ApplicationData.Current.LocalSettings.Values;
                string infoData = string.Format("{0}_{1}", pageParams.VideoId, player.Position);

                if (!settings.ContainsKey(Constants.CurrentPlayingVideoInfo))
                {
                    settings.Add(Constants.CurrentPlayingVideoInfo, infoData);
                }
                else
                {
                    settings[Constants.CurrentPlayingVideoInfo] = infoData;
                }
            }
            catch
            { }
        }

        #endregion

        #region Play List

        private void topAppBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //if(topAppBar.IsOpen)
            //{
            //    topAppBar.IsOpen = false;
            //}
        }

        #endregion

        #region Navigation

        public void BackToLastPage()
        {
            viewModel.IsFullScreen = false;
            SetWindowFull(viewModel.IsFullScreen);
            progressTimer.Stop();

            if(pageParams != null && pageParams.IsLanuchFromSerivice)
            {
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                Frame.GoBack(); 
            }
        }

        #endregion

        #region Event

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Escape:
                    BackToLastPage();
                    break;
                default:
                    break;
            }

            base.OnKeyDown(e);
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private async void PlayListItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dataContext = sender.GetDataContext<PlayListItem>();
            await LoadVideoDataAsync(dataContext.VideoId);
        }

        private void Progress_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            this.player.Position = TimeSpan.FromSeconds(this.progress.Value);
            playingPosition = this.player.Position;
        }

        private void videoRateTypeFlayoutMentu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string tag = (sender as FrameworkElement).Tag.ToString();
            if(!string.IsNullOrEmpty(tag))
            {
                int definitionId = 0;
                if(int.TryParse(tag, out definitionId))
                {
                    playingPosition = this.player.Position;
                    SelectVideoRate(definitionId, viewModel.VideoSources);
                }
            }
        }

        private void volumeSliderValue_Changed(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider volumeSlider = sender as Slider;
            this.player.Volume = volumeSlider.Value / 100.0;
        }

        #endregion

        #region Control Panel

        Storyboard hideControlPanel;
        Storyboard hideTitlePanel;

        private void GetControlPanelStoryBoard()
        {
            hideControlPanel = this.Resources["HideControlPanel"] as Storyboard;
            hideTitlePanel = this.Resources["HideTitlePanel"] as Storyboard;
        }

        private void HideControlPanelAnimation()
        {
            if(hideControlPanel != null)
            {
                hideControlPanel.Stop();
                hideControlPanel.Begin();
            }
            if(viewModel.IsFullScreen && hideTitlePanel != null)
            {
                hideTitlePanel.Stop();
                hideTitlePanel.Begin();
            }
        }

        private void controlPanel_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            HideControlPanelAnimation();
        }

        private void StatusBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void NavigationBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackToLastPage();
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsFullScreen = !viewModel.IsFullScreen;
            SetWindowFull(viewModel.IsFullScreen);
        }
        
        #endregion
        
    }
}
