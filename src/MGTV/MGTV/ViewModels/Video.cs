﻿using SharedFx.Data;

namespace MGTV.ViewModels
{
    public class Video : BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty<string>(ref name, value); }
        }

        private string imgUrl;

        public string ImageUrl
        {
            get { return imgUrl; }
            set { SetProperty<string>(ref imgUrl, value); }
        }

        private string intro;

        public string Intro
        {
            get { return intro; }
            set { SetProperty<string>(ref intro, value); }
        }

        private int videoId;

        public int VideoId
        {
            get { return videoId; }
            set { SetProperty<int>(ref videoId, value); }
        }

        public Video()
        {
            name = string.Empty;
            imgUrl = string.Empty;
            intro = string.Empty;
            videoId = -1;
        }
    }

}
