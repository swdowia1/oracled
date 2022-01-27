using AnalizaDll;
using System.Configuration;

namespace AnalizaSold
{
    public class classConfig
    {
        
       
        //StatystkaKatalog
        static public string StatystkaKatalog
        {
            get
            {
                return ConfigurationManager.AppSettings["StatystkaKatalog"];
            }
        }
        //StatystkaKatalogMiesiac
        static public string StatystkaKatalogMiesiac
        {
            get
            {
                return ConfigurationManager.AppSettings["StatystkaKatalogMiesiac"];
            }
        }
        //SklepNotIn

        static public string SklepNotIn
        {
            get
            {
                return ConfigurationManager.AppSettings["SklepNotIn"];
            }
        }

        /// <summary>
        /// Zaszyfrowane encyrpt
        /// </summary>
        static public string Poloczenie
        {
            get
            {
                return Encrypt.OdSzyfrujHaslo(ConfigurationManager.AppSettings["Poloczenie"]);
            }
        }
       
    }
}
