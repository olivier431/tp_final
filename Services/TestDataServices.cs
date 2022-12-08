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
        private ObservableCollection<Users> lesusers;
        private ObservableCollection<Tunes> lesTunes;

        public TestDataServices() {
            lesusers = new ObservableCollection<Users>() {
                new Users("test123", "123456789", "test@test.com", 0,DateTime.Now),
                new Users("test12356", "123456789", "test123@test.com", 0,DateTime.Now)
            };
            LesTunes = new ObservableCollection<Tunes>()
            {
                new Tunes("Music", "Bob", "Pop", 5, 1999),
                new Tunes("Chant", "Roger", "Rock", 73, 2010)
            };
        }

        public ObservableCollection<Users> Lesusers {
            get => lesusers;
            set { lesusers = value; }
        }
        public ObservableCollection<Tunes> LesTunes
        {
            get => lesTunes;
            set { lesTunes = value; }
        }
    }
}
