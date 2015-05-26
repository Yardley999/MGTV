using SharedFx.Data;
using System.Collections.ObjectModel;

namespace MGTV.ViewModels
{
    public class Category : BindableBase
    {
        private int channelId;

        public int ChannelId
        {
            get { return channelId; }
            set { SetProperty<int>(ref channelId, value); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty<string>(ref name, value); }
        }

        public ObservableCollection<Video> FlashVideos { get; set; }

        public ObservableCollection<Video> Videos { get; set; }

        public Category()
        {
            channelId = -1;
            Name = string.Empty;
            Videos = new ObservableCollection<Video>();
            FlashVideos = new ObservableCollection<Video>();
        }
    }
}
