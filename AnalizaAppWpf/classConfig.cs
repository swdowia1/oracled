using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaAppWpf
{
    public class classConfig
    {
        static public int LastDay
        {

            get
            {
                return int.Parse(ConfigurationManager.AppSettings["LastDay"]);
            }
        }

        static public string RaportMiesiac
        {
            get
            {
                return ConfigurationManager.AppSettings["RaportMiesiac"];
            }
        }
        static public string StatystykaKatalog
        {

            get
            {
                return ConfigurationManager.AppSettings["StatystykaKatalog"];
            }
        }
        
       
        /// <summary>
        /// Zapis do pliku porównanie danych fiskalnych vs Npos.xlsx
        /// </summary>
        static public string SaveXLS
        {
            get
            {
                return ConfigurationManager.AppSettings["SaveXLS"];
            }
        }
     }
}
