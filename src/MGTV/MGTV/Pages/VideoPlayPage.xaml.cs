using MGTV.Common;
using System;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            public string Url { get; set; }

            public PageParams()
            {
                Url = string.Empty;
            }
        }

        #endregion

        #region Field && Property

        private PageParams pageParams;
        private DispatcherTimer progressTimer;

        #endregion

        #region Life Cycle

        public VideoPlayPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            pageParams = e.Parameter as PageParams;
            SetUrlAndTryPlay();
        }

        #endregion

        private async void SetUrlAndTryPlay()
        {
            if(pageParams != null && !string.IsNullOrEmpty(pageParams.Url))
            {
                var hlslUrl = new Uri(pageParams.Url, UriKind.RelativeOrAbsolute);
                var hlsSource = await AdaptiveMediaSource.CreateFromUriAsync(hlslUrl);

                if (hlsSource.Status == AdaptiveMediaSourceCreationStatus.Success)
                {
                    player.SetMediaStreamSource(hlsSource.MediaSource);
                    ProgressBarSetup();
                    Play();
                }
            }
        }
        
        private void ProgressBarSetup()
        {
            this.progress.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
            this.progress.Minimum = 0;

            if(progressTimer == null)
            {
                progressTimer = new DispatcherTimer();
                progressTimer.Interval = TimeSpan.FromSeconds(1);
                progressTimer.Tick += ProgressTimer_Tick;
            }
        }

        private void ProgressTimer_Tick(object sender, object e)
        {
            progress.Value = player.Position.TotalSeconds;
            currentPosition.Text = player.Position.ToShortFromatString(); ;
        }

        private void Play()
        {
            this.player.Play();
            progressTimer.Start();
        }

        private void Stop()
        {
            this.player.Stop();
            progressTimer.Stop();
        }

        private void Pause()
        {
            this.player.Pause();
            progressTimer.Stop();
        }

        private void SetFullScreen(bool isFullScreen)
        {
            this.player.IsFullWindow = isFullScreen;
        }

        private void fullScreenSwitch_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreen(!player.IsFullWindow);
        }
    }
}
