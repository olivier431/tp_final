using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tp_final.Services;

namespace tp_final.Models
{
    public abstract class Model
    {
        protected static MarthaProcessor Martha = MarthaProcessor.Instance;

        public abstract override string ToString();
    }
}
