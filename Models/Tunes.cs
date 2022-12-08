using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tp_final.Models
{
    public class Tunes
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public int Length { get; set; }
        public int Year { get; set; }
        
        public Tunes(string title, string artist, string genre, int length, int year)
        {
            this.Title = title;
            this.Artist = artist;
            this.Genre = genre;
            this.Length = length;
            this.Year = year;
        }
    }
}
