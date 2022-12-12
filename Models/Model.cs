using MarthaService;

namespace tp_final.Models
{
    public abstract class Model
    {
        protected static MarthaProcessor Martha = MarthaProcessor.Instance;

        public abstract override string ToString();
    }
}
