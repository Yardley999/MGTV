using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace MGTV.Controls
{
    public class ProgressSilder : Slider
    {
        private double downloadProgressValue;

        public double DownloadProgressValue
        {
            get { return downloadProgressValue; }
            set
            {
                downloadProgressValue = value;
                ProgressTrackRect.Width = HorizontalTrackRect.ActualWidth * downloadProgressValue;
            }
        }

        private Rectangle ProgressTrackRect;
        private Rectangle HorizontalTrackRect;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ProgressTrackRect = GetTemplateChild("ProgressTrackRect") as Rectangle;
            HorizontalTrackRect = GetTemplateChild("HorizontalTrackRect") as Rectangle;
        }
    }
}
