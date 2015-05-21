using SharedFx.Data;

namespace MGTV.ViewModels
{
    public class PlayListItem : BindableBase
    {
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

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty<string>(ref name, value); }
        }

        public PlayListItem()
        {
            IsPlaying = false;
            name = string.Empty;
            videoId = -1;
        }
    }
}
