using System;
using System.Collections.Generic;
using System.Globalization;
namespace VVVID_DOWNLOADER {
    class Program {
        public static VVVID vvvID;
        static void Main(string[] args) {
            vvvID = new VVVID();
            GetAnime();
            int episodioNumero = SelectEpisode();
            vvvID.Start(episodioNumero);
            Console.ReadKey();
        }
        static void GetAnime() {
            Console.Write("Inserisci l'iniziale dell'anime che vuoi guardare: ");
            char c = Console.ReadLine()[0];
            c = char.ToLower(c, CultureInfo.InvariantCulture);
            List<Anime> listAnime = vvvID.AnimeFilter(c);
            for (int k = 0; k < listAnime.Count; k++)
                Console.WriteLine((k + 1) + "- " + listAnime[k].title);
            Console.Write("Seleziona l'anime che vuoi guardare: ");
            int animeNumero = int.Parse(Console.ReadLine()) - 1;
            var animeData = vvvID.GetAnimeData(listAnime[animeNumero].show_id );
            int i = 0;
            if (animeData.data.Count > 1){
                foreach(Anime a in animeData.data)
                    Console.WriteLine(a.number + "- " + a.name);
                Console.Write("Scegli: ");
                i = int.Parse(Console.ReadLine()) - 1;
            }
            vvvID.anime = animeData.data[i];
            vvvID.anime.title = listAnime[animeNumero].title;
        }
        static int SelectEpisode() {
            Anime anime = vvvID.anime;
            for (int i = 0; i < anime.episodes.Count; i++)
                if(anime.episodes[i].playable)
                    Console.WriteLine($"{anime.episodes[i].number}  <-\t {anime.title} {anime.episodes[i].number}");
            Console.WriteLine("-1  <-\tPer scaricarli tutti");
            Console.Write("Inserisci il numero dell'episodio che vuoi scaricare: ");
            int episodioNumero = int.Parse(Console.ReadLine()) - 1;
            return episodioNumero;
        }
    }
}