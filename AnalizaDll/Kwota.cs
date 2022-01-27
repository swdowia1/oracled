using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaDll
{
    public abstract class Kwota
    {
        public decimal BruttoNpos { get; set; }
        public decimal BruttoFiskalne { get; set; }
        public decimal Karta { get; set; }
        public decimal Terminal { get; set; }
        public decimal BruttoFiskalneNpos { get; set; }
        public Status Status { get; set; }
        public string StatusKarta { get; set; }
    }
}
