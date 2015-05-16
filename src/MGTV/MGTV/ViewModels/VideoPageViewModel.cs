using SharedFx.Data;
using System.Collections.ObjectModel;

namespace MGTV.ViewModels
{
    public class VideoPlayerPageViewModel : BindableBase
    {
        public ObservableCollection<PlayListItem> PlayList { get; set; }

        private bool isPlaying;

        public bool IsPlaying
        {
            get { return isPlaying; }
            set { SetProperty<bool>(ref isPlaying, value); }
        }

        private int videoId;

        public int VideoId
        {
            get { return videoId; }
            set { SetProperty<int>(ref videoId, value); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty<string>(ref title, value); }
        }

        public ObservableCollection<VideoDefinationSource> VideoSources { get; set; }

        public VideoPlayerPageViewModel()
        {
            VideoSources = new ObservableCollection<VideoDefinationSource>();
            PlayList = new ObservableCollection<PlayListItem>();
            title = string.Empty;
        }
    }

    public class VideoDefinationSource : BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty<string>(ref name, value); }
        }

        private string url;

        public string Url
        {
            get { return url; }
            set { SetProperty<string>(ref url, value); }
        }


        public VideoDefinationSource()
        {
            name = string.Empty;
            url = string.Empty;
        }
    }


}
