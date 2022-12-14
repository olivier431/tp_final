using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using tp_final.Models;

namespace tp_final.Services
{
    public class TestDataServices
    {
        private ObservableCollection<User> lesusers;
        private ObservableCollection<Tune> lesTunes;
        private ObservableCollection<Playlist> lesPlaylists;

        public TestDataServices() {
            lesusers = new ObservableCollection<User>() {
                //new User("test123", "123456789", "test@test.com", 0,DateTime.Now),
                //new User("test12356", "123456789", "test123@test.com", 0,DateTime.Now)
            };
            LesTunes = new ObservableCollection<Tune>()
            {
                new Tune(1, 1, 1, 1, "Tune1", "Bob", "Rock", "file", 4, 1999),
                new Tune(2, 1, 1, 2, "Tune2", "Roger", "We", "test", 56, 2000),
                new Tune(3, 1, 0, 1, "Tune3", "Roger", "We", "test", 56, 2000),
                new Tune(4, 1, 0, 2, "Tune4", "Roger", "We", "test", 56, 2000),
                new Tune(5, 1, 22, 1, "Tune5", "b", "We", "test", 56, 2000),
                new Tune(6, 1, 22, 2, "Tune6", "b", "We", "test", 56, 2000)
                //new Tune("Music", "Bob", "Pop", 5, 1999),
                //new Tune("Chant", "Roger", "Rock", 73, 2010)
            };
            LesPlaylists = new ObservableCollection<Playlist>()
            {
                new Playlist(0, 1, 0, "Unknown", 1, 1, 0,"", "", "", 54),
                new Playlist(1, 1, 0, "Playlist1", 1, 1, 0,"", "", "", 54),
                new Playlist(22, 1, 1, "Album22", 3, 6, 1, "Bob", "Rock", "", 1999)
            };

            int id = 1;
            foreach(var p in LesPlaylists)
            {
                p.MusicPlaylist.Add(new Tune(id, 1, p.id, id, "Tune1", "Bob", "Rock", "file", 4, 1999));
            }
        }

        public ObservableCollection<User> Lesusers {
            get => lesusers;
            set { lesusers = value; }
        }
        public ObservableCollection<Tune> LesTunes
        {
            get => lesTunes;
            set { lesTunes = value; }
        }
        public ObservableCollection<Playlist> LesPlaylists
        {
            get => lesPlaylists;
            set { lesPlaylists = value; }
        }
    }
}
