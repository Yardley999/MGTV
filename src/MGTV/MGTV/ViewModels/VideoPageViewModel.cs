using SharedFx.Data;
using System.Collections.ObjectModel;

namespace MGTV.ViewModels
{
    public class VideoPlayerPageViewModel : MGBindableBase
    {
        private ObservableCollection<PlayListItem> playList;
        public ObservableCollection<PlayListItem> PlayList
        {
            get
            {
                return playList;
            }
            set
            {
                SetProperty<ObservableCollection<PlayListItem>>(ref playList, value);
            }
        }

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

        private bool isFullScreen;

        public bool IsFullScreen
        {
            get { return isFullScreen; }
            set { SetProperty<bool>(ref isFullScreen, value); }
        }

        private bool isPlayButtonInPauseStatus;

        public bool IsPlayButtonInPauseStatus
        {
            get { return isPlayButtonInPauseStatus; }
            set { SetProperty<bool>(ref isPlayButtonInPauseStatus, value); }
        }


        public VideoPlayerPageViewModel()
        {
            VideoSources = new ObservableCollection<VideoDefinationSource>();
            PlayList = new ObservableCollection<PlayListItem>();
            title = string.Empty;
            isFullScreen = false;
            isPlayButtonInPauseStatus = false;
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

        private int id;

        public int Id
        {
            get { return id; }
            set { SetProperty<int>(ref id, value); }
        }

        public VideoDefinationSource()
        {
            name = string.Empty;
            url = string.Empty;
        }
    }


}
