using System;
using System.Net.Http;
using System.Text;

namespace MarthaService
{
    internal class StringContent
    {
        private string param;
        private Encoding uTF8;
        private string v;

        public StringContent(string param, Encoding uTF8, string v)
        {
            this.param = param;
            this.uTF8 = uTF8;
            this.v = v;
        }

        public static implicit operator HttpContent(StringContent v)
        {
            throw new NotImplementedException();
        }
    }
}