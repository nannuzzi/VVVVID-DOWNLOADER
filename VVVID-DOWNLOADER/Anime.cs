using System.Collections.Generic;
namespace VVVID_DOWNLOADER {
    public class Episode {
        public int id { get; set; }
        public int season_id { get; set; }
        public int video_id { get; set; }
        public string number { get; set; }
        public string title { get; set; }
        public string thumbnail { get; set; }
        public string description { get; set; }
        public bool expired { get; set; }
        public bool seen { get; set; }
        public bool playable { get; set; }
        public int ondemand_type { get; set; }
        public int vod_mode { get; set; }
        public string videoLink { get; set; }
    }
    public class Anime {
        public int id { get; set; }
        public int show_id { get; set; }
        public int season_id { get; set; }
        public int show_type { get; set; }
        public int number { get; set; }
        public List<Episode> episodes { get; set; }
        public string name { get; set; }
        public string title { get; set; }

    }
    public class RootObject {
        public string result { get; set; }
        public string message { get; set; }
        public List<Anime> data { get; set; }
    }
}