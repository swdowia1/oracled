using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaDll
{
    public class Sold
    {
        public string NumerSklep { get; set; }
        public string Opis { get; set; }
        public string Stanowisko { get; set; }
        public decimal Brutto { get; set; }
      

        public decimal Karta { get; set; }
        public decimal Terminal { get; set; }
        public decimal KartaTerminal { get; set; }
        public string ShopName { get; set; }
        public decimal Konwersja { get; set; }

    }
}
