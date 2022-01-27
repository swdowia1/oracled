using AnalizaDll;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaAppWpf
{
    public class DataShow
    {
        public string Data { get; set; }
        public string Show { get; set; }
        public DataShow(string data)
        {
            DateTime dt;
            /*
             *   DateTime StartDate = DateTime.ParseExact(lastReportDate,
                                        classConst.formatDate,
                                         CultureInfo.InvariantCulture);
             */
            bool check = DateTime.TryParseExact(data, classConst.formatDate, CultureInfo.InvariantCulture,
                         DateTimeStyles.None, out dt);
            if (check)
            {
                string[] dni = new string[] { "Poniedziałek"
                ,"Wtorek"
                ,"Środa"
                ,"Czwartek"
                ,"Piątek"
                ,"Sobota"
                ,"Niedziela"};
                int day = ((int)dt.DayOfWeek == 0) ? 7 : (int)dt.DayOfWeek;
                Data = data;
                Show =classFun.Data_MMDDYYYY(data)+" [" + dni[day-1] + "]";
            }
        }
    }
}
