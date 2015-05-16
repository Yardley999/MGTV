using MGTV.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MGTV.Controls
{
    public sealed partial class Category : UserControl
    {

        #region Dependency Property

        public static readonly DependencyProperty CategoryNameProperty = DependencyProperty.Register("CategoryName", typeof(string), typeof(Category), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty VideoDataProperty = DependencyProperty.Register("VideoData", typeof(ObservableCollection<Video>), typeof(Category), new PropertyMetadata(null));

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

        #endregion

        public Category()
        {
            this.InitializeComponent();
            this.root.DataContext = this;
        }
    }
}
