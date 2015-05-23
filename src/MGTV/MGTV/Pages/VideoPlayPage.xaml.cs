using MGTV.Common;
using MGTV.MG.API;
using MGTV.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SharedFx.Extensions;

namespace MGTV.Pages
{
    public sealed partial class VideoPlayPage : Page
    {
        #region Page Parameters

        public class PageParams
        {
            public int VideoId { get; set; }

            public PageParams()
            {
                VideoId = -1;
            }
        }

        #endregion

        #region Field && Property

        private VideoPlayerPageViewModel viewModel;

        private PageParams pageParams;
        private DispatcherTimer progressTimer;
        private bool isPlaying = false;

        #endregion

        #region Life Cycle

        public VideoPlayPage()
        {
            this.InitializeComponent();
            Init();
            this.player.MediaFailed += (s, o) =>
            {
                string error = o.ErrorMessage;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            pageParams = e.Parameter as PageParams;

            PlayerSetup();
            if(pageParams != null)
            {
                LoadVideoDataAsync(pageParams.VideoId);
            }
        }

        #endregion

        #region Data 

        private async Task LoadVideoDataAsync(int videoId)
        {
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
                            Url = item.Url
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


                string fetchUrl = viewModel.VideoSources[viewModel.VideoSources.Count - 1].Url;
                SetUrlAndTryPlay(fetchUrl);

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
            currentPosition.Text = player.Position.ToShortFromatString();
        }

        #endregion

        #region Player Function

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
        }

        private void Play()
        {
            this.player.Play();
            isPlaying = true;
        }

        private void Stop()
        {
            this.player.Stop();
            this.player.Position = TimeSpan.Zero;

            if (progressTimer != null)
            {
                progressTimer.Stop();
            }
            isPlaying = false;
            ResetVideoText();
        }

        private void Pause()
        {
            this.player.Pause();
            isPlaying = false;
        }

        private void SetFullScreen(bool isFullScreen)
        {

        }

        #endregion

        #region Play List

        #endregion

        #region Navigation

        public void BackToLastPage()
        {
            Frame.GoBack();
            viewModel.IsFullScreen = false;
            SetWindowFull(viewModel.IsFullScreen);
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
        }

        private async void PlayListItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dataContext = sender.GetDataContext<PlayListItem>();
            await LoadVideoDataAsync(dataContext.VideoId);
        }

        private void Progress_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            this.player.Position = TimeSpan.FromSeconds(this.progress.Value);
        }

        private void videoRateTypeFlayoutMentu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.videoRateButton.Content = (sender as MenuFlyoutItem).Text;
        }

        private void volumeSliderValue_Changed(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider volumeSlider = sender as Slider;
            this.player.Volume = volumeSlider.Value / 100.0;
        }

        #endregion

        #region Control Panel

        private void StatusBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void NavigationBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void controlPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(this.controlPanel.Opacity == 0)
            {
                this.controlPanel.Opacity = 1;
            }
            else
            {
                this.controlPanel.Opacity = 0;
            }
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
            }
            else
            {
                appView.ExitFullScreenMode();
            }
        }

        #endregion
    }
}
