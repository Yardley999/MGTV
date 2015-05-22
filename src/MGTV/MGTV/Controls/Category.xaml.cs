using MGTV.Pages;
using MGTV.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MGTV.Controls
{
    public sealed partial class Category : UserControl
    {

        #region Dependency Property

        public static readonly DependencyProperty CategoryNameProperty = DependencyProperty.Register("CategoryName", typeof(string), typeof(Category), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty VideoDataProperty = DependencyProperty.Register("VideoData", typeof(ObservableCollection<Video>), typeof(Category), new PropertyMetadata(null));
        public static readonly DependencyProperty CategoryIdProperty = DependencyProperty.Register("CategoryId", typeof(int), typeof(Category), new PropertyMetadata(-1));

        #endregion

        #region CLR Wrapper

        public string CategoryName
        {
            get { return (string)GetValue(CategoryNameProperty); }
            set { SetValue(CategoryNameProperty, value); }
        }

        public ObservableCollection<Video> VideoData
        {
            get { return (ObservableCollection<Video>)GetValue(VideoDataProperty); }
            set { SetValue(VideoDataProperty, value); }
        }

        public int CategoryId
        {
            get { return (int)GetValue(CategoryIdProperty); }
            set { SetValue(CategoryIdProperty, value); }
        }

        #endregion

        public Category()
        {
            this.InitializeComponent();
            this.root.DataContext = this;
        }

        private void Header_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ChannelDetailsPage.PageParams para = new ChannelDetailsPage.PageParams();
            para.ChannelId = CategoryId;
            para.ChannelName = CategoryName;

            App.Instance.Frame.Navigate(typeof(ChannelDetailsPage), para);
            
        }
    }
}
