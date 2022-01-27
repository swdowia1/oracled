using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaAppWpf
{
    public class DataSelect
    {
        public string Data { get; set; }
        /// <summary>
        /// -1 bez zmian 1 zmieniono
        /// </summary>
        public int Status { get; set; }
        public DataSelect()
        {
            
        }
        public DataSelect(string dane)
        {
            Data = dane;
            Status = 1;
        }
    }
}
