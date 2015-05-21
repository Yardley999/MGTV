using SharedFx.Data;

namespace MGTV.ViewModels
{
    public class Channel : BindableBase
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { SetProperty<int>(ref id, value); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty<string>(ref name, value); }
        }

        private string iconUrl;

        public string IconUrl
        {
            get { return iconUrl; }
            set { SetProperty<string>(ref iconUrl, value); }
        }

        public static readonly Channel Home = new Channel()
        {
            IconUrl = "ms-appx:///Assets/NavListIcon/ico-nav-01.png",
            Name = "首页",
            Id = 0
        };

        public Channel()
        {
            id = -1;
            name = string.Empty;
            iconUrl = string.Empty;
        }
    }

}
