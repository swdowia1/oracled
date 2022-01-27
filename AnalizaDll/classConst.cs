using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaDll
{
    public class classConst
    {
        /// <summary>
        /// data z yyyyMMdd
        /// </summary>
        public static string formatDate = "yyyyMMdd";
        public static string formatYearMonth = "yyyyMM";
        public static string formatYearMonthName = "yyyy MMMM";
        /// <summary>
        /// yyyy'-'MM'-'dd
        /// </summary>
        public static string formatDateOracle = "yyyy'-'MM'-'dd";
        public static string formatDateFull = "yyyy'-'MM'-'dd HH':'mm':'ss";
        public static string Eol = Environment.NewLine;
        public static string ShopFile = "shop.txt";
        public static string ShopFileXml = "sklepy.xml";

        public static string Podzial
        {
            get
            {
                return "".PadLeft(100, '=');
            }
        }

        public static string nameFileFiskal = "miesiacFiskal_{0}.csv";
        public static string nameFileNpos = "miesiacNpos_{0}.csv";
    }
}
