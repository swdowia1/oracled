using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaDll
{
    public class MonthSklep:Kwota
    {
        public string NumerSklep { get; set; }
        public List<MonthStanowisko> Pos { get; set; }
        public Status Status { get; set; }
    }
}
