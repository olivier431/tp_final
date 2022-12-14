using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tp_final.Models
{
    public class Playlists
    {
        public string Artist { get; set; }
        public string Genre { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public int Is_public { get; set; }
        public int User_id { get; set; }
        public string Album_cover { get; set; }

        public Playlists() { }

        public Playlists(string artist, string genre, string title, int year, int isPublic, int userId, string albumCover)
        {
            this.Artist = artist;
            this.Genre = genre;
            this.Title = title;
            this.Year = year;
            this.Is_public = isPublic;
            this.User_id = userId;
            this.Album_cover = albumCover;
        }
    }
}
