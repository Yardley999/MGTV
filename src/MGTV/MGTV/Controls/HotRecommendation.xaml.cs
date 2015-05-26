using MGTV.ViewModels;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MGTV.Controls
{
    public sealed partial class HotRecommendation : UserControl
    {
        #region Dependency Property

        public static readonly DependencyProperty FlashDataProperty = DependencyProperty.Register("FlashData", typeof(ObservableCollection<Video>), typeof(HotRecommendation), new PropertyMetadata(null));
        public static readonly DependencyProperty NonFlashDataProperty = DependencyProperty.Register("NonFlashData", typeof(ObservableCollection<Video>), typeof(HotRecommendation), new PropertyMetadata(null));

        #endregion

        #region CLR Property Wrapper

        public ObservableCollection<Video> NonFlashData
        {
            get { return (ObservableCollection<Video>)GetValue(NonFlashDataProperty); }
            set { SetValue(NonFlashDataProperty, value); }
        }

        public ObservableCollection<Video> FlashData
        {
            get { return (ObservableCollection<Video>)GetValue(FlashDataProperty); }
            set { SetValue(FlashDataProperty, value); }
        }

        #endregion

        #region Life Cycle

        public HotRecommendation()
        {
            this.InitializeComponent();
            this.root.DataContext = this;
        }

        #endregion

        #region Flip View

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StopScrollAnimation();
            StartScrollAnimation();
        }

        DispatcherTimer scrollTimer;

        public void StartScrollAnimation()
        {
            if(scrollTimer == null)
            {
                scrollTimer = new DispatcherTimer();
                scrollTimer.Interval = TimeSpan.FromSeconds(3.6);
                scrollTimer.Tick += ScrollTimer_Tick;
            }

            StopScrollAnimation();
            scrollTimer.Start();
        }

        public void StopScrollAnimation()
        {
            if(scrollTimer != null)
            {
                scrollTimer.Stop();
            }
        }

        private void ScrollTimer_Tick(object sender, object e)
        {
            if(flipView != null)
            {
                int index = (this.flipView.SelectedIndex + 1) % (this.flipView.Items.Count);
                this.flipView.SelectedIndex = index;
            }
        }

        #endregion
    }
}
