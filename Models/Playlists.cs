using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tp_final.Models
{
    public class Playlists
    {
        string Artist { get; set; }
        string Genre { get; set; }
        string Title { get; set; }
        int Year { get; set; }
        int Is_public { get; set; }
        int User_id { get; set; }
        string Album_cover { get; set; }


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
