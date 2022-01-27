using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaDll
{
    public class Summary
    {
        public string NumerSklep { get; set; }
        public string ShopName { get; set; }
        public string Stanowisko { get; set; }
        public string NumerRaport { get; set; }
        public decimal BruttoNpos { get; set; }
        public decimal BruttoFiskalne { get; set; }
        public decimal BruttoFiskalneNpos { get; set; }
        public decimal SapBrutto { get; set; }
        public string Data { get; set; }
        public decimal Karta { get; set; }
        public decimal Terminal { get; set; }
        public decimal KartaTerminal { get; set; }
        public decimal Konwersja { get; set; }
        /// <summary>
        /// 0 ok -1 FiskalneWiecej 1 nposWiecej
        /// </summary>
        public Status Status { get; set; }
        public string StatusKarta { get; set; }

    }
}
