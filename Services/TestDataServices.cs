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
        private ObservableCollection<Playlists> lesPlaylists;

        public TestDataServices() {
            lesusers = new ObservableCollection<User>() {
                //new User("test123", "123456789", "test@test.com", 0,DateTime.Now),
                //new User("test12356", "123456789", "test123@test.com", 0,DateTime.Now)
            };
            LesTunes = new ObservableCollection<Tune>()
            {
                new Tune(1, 1, 1, 1, "Tune1", "Bob", "Rock", "file", 4, 1999),
                new Tune(1, 1, 1, 1, "Tune2", "Roger", "We", "test", 56, 2000)
                //new Tune("Music", "Bob", "Pop", 5, 1999),
                //new Tune("Chant", "Roger", "Rock", 73, 2010)
            };
            LesPlaylists = new ObservableCollection<Playlists>()
            {
                //new Playlists("George", "Pop", "TheTitle", 1800, 1, 1, "https://images.alphacoders.com/100/thumb-1920-1008709.jpg"),
                //new Playlists("bob", "Rock", "RockStar", 2000, 0, 4, "https://images3.alphacoders.com/165/thumb-1920-165265.jpg")
            };
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
        public ObservableCollection<Playlists> LesPlaylists
        {
            get => lesPlaylists;
            set { lesPlaylists = value; }
        }
    }
}
