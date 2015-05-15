using MGTV.Common;
using MGTV.ViewModels;
using SharedFx.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MGTV.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoPlayPage : Page
    {
        #region Page Parameters

        public class PageParams
        {
            public int PlayIndex { get; set; }

            public List<PlayListItem> PlayList { get; set; }

            public PageParams()
            {
                PlayIndex = 0;
                PlayList = new List<PlayListItem>();
            }
        }

        #endregion

        #region Field && Property

        public ObservableCollection<PlayListItem> PlayLists { get; set; }

        private PageParams pageParams;
        private DispatcherTimer progressTimer;
        private bool isPlaying = false;
        private bool isFullScreenMode = false;

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
            PlayListSetup();
            PlayerSetup();
            SetUrlAndTryPlay();
        }

        #endregion

        #region Init

        private void Init()
        {
            PlayLists = new ObservableCollection<PlayListItem>();
            playListBox.ItemsSource = PlayLists;

            this.player.MediaOpened += Player_MediaOpened;
            this.progress.ThumbToolTipValueConverter = new PlayerTimeSliderTooltipValueConverter();
            this.volumeSlider.ThumbToolTipValueConverter = new VolumePercentageConverter();
        }

        #endregion

        #region  Setup

        private void PlayListSetup()
        {
            if (pageParams != null)
            {
                PlayLists.Clear();
                foreach (var item in pageParams.PlayList)
                {
                    PlayLists.Add(new PlayListItem()
                    {
                        IsPlaying = item.IsPlaying,
                        Name = item.Name,
                        Url = item.Url
                    });
                }
            }
        }

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
            StatebarSetup();
            progressTimer.Start();
        }

        private void StatebarSetup()
        {
            ProgressBarSetup();

            this.PlayPauseButton.IsChecked = true;
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

        private async void SetUrlAndTryPlay()
        {
            if (pageParams != null)
            {
                string playUrl = GetPlayingUrl();
                
                if (string.IsNullOrEmpty(playUrl))
                {
                    return;
                }

                var hlslUrl = new Uri(playUrl, UriKind.RelativeOrAbsolute);
                var hlsSource = await AdaptiveMediaSource.CreateFromUriAsync(hlslUrl);

                if (hlsSource.Status == AdaptiveMediaSourceCreationStatus.Success)
                {
                    player.SetMediaStreamSource(hlsSource.MediaSource);
                    Play();
                }
            }
        }

        private void Next()
        {
            int index = -1;

            var playingItem = PlayLists.FirstOrDefault(p => p.IsPlaying);
            if (playingItem != null)
            {
                index = PlayLists.IndexOf(playingItem);
            }

            if (index >= 0 && index < PlayLists.Count - 1)
            {
                PlayLists[index].IsPlaying = false;
                PlayLists[index + 1].IsPlaying = true;
                SetUrlAndTryPlay();
            }
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
            isFullScreenMode = isFullScreen;
            this.player.Margin = new Thickness(0, 0, 0, isFullScreenMode ? 0 : this.StatusBar.ActualHeight);
        }

        #endregion

        #region Play List

        private string GetPlayingUrl()
        {
            var item = PlayLists.FirstOrDefault(p => p.IsPlaying);
            if (item != null)
            {
                return item.Url;
            }
            return string.Empty;
        }

        public void ShowPlaylit(bool show)
        {
            this.header.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Event

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

        private void fullScreenToggle_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreen(!isFullScreenMode);
            AppBarButton button = sender as AppBarButton;
            if (button != null)
            {
                button.Icon = isFullScreenMode ? new SymbolIcon(Symbol.BackToWindow) : new SymbolIcon(Symbol.FullScreen);
            }
        }

        private void PlayListItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Stop();

            var dataContext = sender.GetDataContext<PlayListItem>();
            foreach (var item in PlayLists)
            {
                if(item.IsPlaying )
                {
                   item.IsPlaying = false;
                }
            }

            dataContext.IsPlaying = true;
            SetUrlAndTryPlay();
        }

        private void RootGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            bool isShow = this.header.Visibility == Visibility.Visible ? false : true;
            ShowPlaylit(isShow);
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

        
    }
}
