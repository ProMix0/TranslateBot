using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreTranslate.Net;

namespace TranslateBot
{
    public interface ITranslator
    {
        public Task<string> Translate(string text, string emoji);
    }
}
