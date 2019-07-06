using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
namespace VVVID_DOWNLOADER {
    public  class VVVID
    {
        private static CookieContainer _cookieContainer = new CookieContainer();
        private static String _connectionId;
        public  Anime Anime;
        public VVVID()
        { _connectionId = GetConnectionId(); }
        private static string WebRequest(string url) {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0";
            webRequest.CookieContainer = _cookieContainer;
            using (Stream s = webRequest.GetResponse().GetResponseStream()) {
                using (StreamReader sr = new StreamReader(s))
                    return sr.ReadToEnd();
            }
        }
        private static string GetConnectionId() {
            string response = WebRequest("https://www.vvvvid.it/user/login");
            return response.Split(new[] { "\"conn_id\":\"" }, StringSplitOptions.None)[1].Split('\"')[0];
        }
        public List<Anime> AnimeFilter(char c) {
            String urlFirst15 = "https://www.vvvvid.it/vvvvid/ondemand/anime/channel/10003/last?filter=" + c + "&conn_id=" + _connectionId; //Questo link mostra solo i primi 15 anime
            String urlLast = "https://www.vvvvid.it/vvvvid/ondemand/anime/channel/10003?filter=" + c + "&conn_id=" + _connectionId; //Questo link mostra i restanti anime
            string response = WebRequest(urlFirst15);
            RootObject animeFirst15 = JsonConvert.DeserializeObject<RootObject>(response);
            response = WebRequest(urlLast);
            RootObject animeLast = JsonConvert.DeserializeObject<RootObject>(response);
            if(animeLast.data != null)
                return animeFirst15.data.ToArray().Union(animeLast.data.ToArray()).ToList();
            return animeFirst15.data.ToArray().ToList();
        }
        public RootObject GetAnimeData(int idAnime) {
            string url = $"https://www.vvvvid.it/vvvvid/ondemand/{idAnime}/seasons/?conn_id={_connectionId}";
            string response = WebRequest(url);
            RootObject animeData = JsonConvert.DeserializeObject<RootObject>(response);
            return animeData;
        }
        public void Start(int episodioNumero) {
            GetLinks(episodioNumero);
            foreach (Episode ep in Anime.episodes)
                if(!String.IsNullOrEmpty(ep.videoLink) && ep.playable)
                    Download(ep);
        }
        private void GetLinks(int episodioNumero) {
            if (episodioNumero < 0)
                foreach (Episode ep in Anime.episodes)
                    ep.videoLink = GetEpisodeVideoLink(ep);
            else Anime.episodes[episodioNumero].videoLink = GetEpisodeVideoLink(Anime.episodes[episodioNumero]);
        }
        private string GetEpisodeVideoLink(Episode ep) {
            if (ep.vod_mode == 2) return null; //vod_mode == 2 solo per utenti premium
            String url = $"https://www.vvvvid.it/#!show/{Anime.show_id}/text/{ep.season_id}/{ep.video_id}/text";
            string exe = @"youtube-dl -g " + url;
            Process p = new Process {
                StartInfo = {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
                FileName = "youtube-dl",
                Arguments = "-g " + url
                }
            };
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }
        private void Download(Episode ep) {
            var ffmpeg = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = $"ffmpeg",
                    Arguments = $"-i {ep.videoLink.Remove(ep.videoLink.Length - 1)} -c copy -bsf:a aac_adtstoasc \"{Anime.title} {ep.number}.mp4\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                }
            };
            ffmpeg.Start();
            Console.WriteLine($"Downloading { Anime.title} { ep.number}");
                while (!ffmpeg.StandardOutput.EndOfStream){ } //Runno in background
            Console.WriteLine("Download completed");
        }
    }
}