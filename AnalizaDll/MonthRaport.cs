using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaDll
{
    public class MonthRaport
    {
        public string NumerSklep { get; set; }
        public string NumerStanowiska { get; set; }
        public string DataStart { get; set; }
        public string DataKoniec { get; set; }
        public decimal Kwota { get; set; }
        public string NazwaRaportu { get; set; }
       // public string Linia { get; set; }
        public MonthRaport()
        {

        }
        public MonthRaport(string linia)
        {
            //Linia = linia;
            string[] kolumn = linia.Split(';');
            NumerSklep = kolumn[0];
            NumerStanowiska = kolumn[1];
            DataStart = kolumn[2].Substring(0,6);
            DataKoniec = kolumn[2].Substring(6);
            Kwota = classFun.GetDecimal(kolumn[3]);
            NazwaRaportu = kolumn[4];
            
        }
    }
}
