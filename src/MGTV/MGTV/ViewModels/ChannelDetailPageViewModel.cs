using SharedFx.Data;
using System.Collections.ObjectModel;

namespace MGTV.ViewModels
{
    public class ChannelDetailsPageViewModel : BindableBase
    {
        private string background;

        public string Background
        {
            get { return background; }
            set { SetProperty<string>(ref background, value); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty<string>(ref title, value); }
        }

        private int videoCount;

        public int VideoCount 
        {
            get { return videoCount; }
            set { SetProperty<int>(ref videoCount, value); }
        }


        public ObservableCollection<Filter> Filters { get; set; }

        public ChannelDetailsPageViewModel()
        {
            background = string.Empty;
            title = string.Empty;
            videoCount = 0;
            Filters = new ObservableCollection<Filter>();
        }

    }
}
