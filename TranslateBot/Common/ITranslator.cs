using LibreTranslate.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateBot.Common
{
    internal interface ITranslator
    {
        public Task<string> Translate(string text, string emoji);
    }
}
