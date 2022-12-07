using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tp_final.Models
{
    public class Users
    {
        public Users() { 
        
        }
        public string Username { get; set; }
        public string Pwd { get; set; }
        public string Email { get; set; }
        public int Is_admin { get; set; }
        public DateTime Last_connection { get; set; }

    }
}
