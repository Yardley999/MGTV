using MGTV.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MGTV.Controls
{
    public sealed partial class HotRecommendation : UserControl
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(ObservableCollection<Video>), typeof(HotRecommendation), new PropertyMetadata(null));

        public ObservableCollection<Video> Data
        {
            get { return (ObservableCollection<Video>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public HotRecommendation()
        {
            this.InitializeComponent();
            this.root.DataContext = this;
        }
    }
}
