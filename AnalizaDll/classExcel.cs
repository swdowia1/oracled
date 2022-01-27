
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaDll
{
    public class classExcel
    {
        public static void CreateExcel(List<MonthSklep> Sklepy, string fileName)
        {
            try
            {
                string monthName = DateTime.Now.AddMonths(-1).ToString("MMMM yyyy");
                monthName = monthName.Substring(0, 1).ToUpper() + monthName.Substring(1);
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var sklepArkusz = excel.Workbook.Worksheets.Add("sklep");
                    sklepArkusz.Cells[1, 3].Value = monthName;
               
                    sklepArkusz.Cells[1, 3].Style.Font.Bold = true;
                    sklepArkusz.Cells[1, 3].Style.Font.Size = 14;

                    var stanowiskoArkusz = excel.Workbook.Worksheets.Add("sklep stanowisko");
                    stanowiskoArkusz.Cells[1, 3].Value = monthName;
                    stanowiskoArkusz.Cells[1, 3].Style.Font.Bold = true;
                    stanowiskoArkusz.Cells[1, 3].Style.Font.Size = 14;
                    int k = 2;
                    HeaderPos(sklepArkusz,k,2,"Salon","Fiskalne","nPOS","Fiskalne vs nPOS");

                    foreach (var item in Sklepy)
                    {
                        k++;
                        sklepArkusz.Cells[k, 2].Value = item.NumerSklep;
                        sklepArkusz.Cells[k, 3].Value = item.BruttoFiskalne;
                        sklepArkusz.Cells[k, 4].Value = item.BruttoNpos;
                        sklepArkusz.Cells[k, 5].Value = item.BruttoFiskalneNpos;
                       
                    }
                    //dalsze wiesrz dla skepow
                    FormatTable(sklepArkusz, "B", 2, "E", k - 1);
                    sklepArkusz.Cells.AutoFitColumns();
                    k = 2;

                    foreach (var item in Sklepy)
                    {
                        int start = k;
                       
                        HeaderPos(stanowiskoArkusz, k, 2, "Salon", "Fiskalne", "nPOS", "Fiskalne vs nPOS");
                        k++;
                        stanowiskoArkusz.Cells[k, 2].Value = item.NumerSklep;
                        stanowiskoArkusz.Cells[k, 2].Style.Font.Bold = true;
                        stanowiskoArkusz.Cells[k, 3].Value = item.BruttoFiskalne;
                        stanowiskoArkusz.Cells[k, 4].Value = item.BruttoNpos;
                        stanowiskoArkusz.Cells[k, 5].Value = item.BruttoFiskalneNpos;
                        k++;
                        foreach (var itemPos in item.Pos)
                        {
                            stanowiskoArkusz.Cells[k, 2].Value = "POS " + itemPos.Stanowisko;
                            stanowiskoArkusz.Cells[k, 3].Value = itemPos.BruttoFiskalne;
                            stanowiskoArkusz.Cells[k, 4].Value = itemPos.BruttoNpos;
                            stanowiskoArkusz.Cells[k, 5].Value = itemPos.BruttoFiskalneNpos;
                           
                            k++;
                        }
                        //z 3 kolumnami bylo H
                        FormatTable(stanowiskoArkusz, "B", start, "E", k - 1);
                        k++;

                    }
                    stanowiskoArkusz.Cells.AutoFitColumns();
                    excel.SaveAs(new FileInfo(fileName));
                }
            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
            }
        }
            /// <summary>
            /// Wersja bze 3 ostatnich kolumn
            /// </summary>
            /// <param name="Sklepy"></param>
            /// <param name="fileName"></param>
            public static void CreateExcel(List<SummaryShop> Sklepy, string fileName)
        {
            
            try
            {
                

                
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var sklepArkusz = excel.Workbook.Worksheets.Add("sklep");
                    var stanowiskoArkusz = excel.Workbook.Worksheets.Add("sklep stanowisko");
                     int k = 2;
                    sklepArkusz.Cells[k, 2].Value = "Salon";
                    sklepArkusz.Cells[k, 3].Value = "Fiskalne";
                    sklepArkusz.Cells[k, 4].Value = "Data";
                    sklepArkusz.Cells[k, 5].Value = "nPOS";
                    sklepArkusz.Cells[k, 6].Value = "Fiskalne vs nPOS";
                    sklepArkusz.Cells[k, 7].Value = "Status";

                    sklepArkusz.Cells[k, 8].Value = "Npos Karta";
                    sklepArkusz.Cells[k, 9].Value = "Konwersja";
                    sklepArkusz.Cells[k, 10].Value = "Terminal";
                    sklepArkusz.Cells[k, 11].Value = "Terminal vs Npos Karta";

                    k++;
                    foreach (var item in Sklepy)
                    {
                        sklepArkusz.Cells[k, 2].Value = item.NumerSklep;
                        sklepArkusz.Cells[k, 3].Value = item.BruttoFiskalne;
                        sklepArkusz.Cells[k, 4].Value = item.Data;
                        sklepArkusz.Cells[k, 5].Value = item.BruttoNpos;
                        sklepArkusz.Cells[k, 6].Value = item.BruttoFiskalneNpos;
                   
                        StatusShow(sklepArkusz, k, 7, item.BruttoFiskalne, item.BruttoNpos, "B","K");
                        sklepArkusz.Cells[k, 8].Value = item.Karta;
                        sklepArkusz.Cells[k, 9].Value = item.Konwersja;
                        sklepArkusz.Cells[k, 10].Value = item.Terminal;
                        sklepArkusz.Cells[k, 11].Value = item.KartaTerminal;
                        if (item.KartaTerminal != 0)
                        {
                            string zakres = "K" + k.ToString() + ":" + "K" + k.ToString();
                            sklepArkusz.Cells[zakres].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        k++;
                    }
                    //z 3 kolumnami bylo G
                    FormatTable(sklepArkusz, "B", 2, "K", k - 1);
                    sklepArkusz.Cells.AutoFitColumns();
                    k = 2;
                  
                    foreach (var item in Sklepy)
                    {
                        int start = k;
                        HeaderPos(stanowiskoArkusz, k, 2, "Salon", "Fiskalne", "Data", "Sprzedaż fiskalna", "nPOS", "Fiskalne vs nPOS", "Status", "Npos Karta", "Konwersja","Terminal", "Terminal vs Npos Karta");

                        k++;
                        stanowiskoArkusz.Cells[k, 2].Value = item.NumerSklep;
                        stanowiskoArkusz.Cells[k, 2].Style.Font.Bold = true;
                        stanowiskoArkusz.Cells[k, 3].Value = item.BruttoFiskalne;
                        stanowiskoArkusz.Cells[k, 6].Value = item.BruttoNpos;
                        stanowiskoArkusz.Cells[k, 7].Value = item.BruttoFiskalneNpos;
                        StatusShow(stanowiskoArkusz, k, 8, item.BruttoFiskalne, item.BruttoNpos,"B","K");
                        k++;
                        foreach (var itemPos in item.Drukarka)
                        {
                            stanowiskoArkusz.Cells[k, 2].Value = "POS "+itemPos.Stanowisko;
                            stanowiskoArkusz.Cells[k, 3].Value = itemPos.BruttoFiskalne;
                            stanowiskoArkusz.Cells[k, 4].Value = itemPos.Data;
                            stanowiskoArkusz.Cells[k, 5].Value = item.BruttoFiskalne > 0 ? "tak":"nie" ;
                            stanowiskoArkusz.Cells[k, 6].Value = itemPos.BruttoNpos;
                            stanowiskoArkusz.Cells[k, 7].Value = itemPos.BruttoFiskalneNpos;
                           
                            StatusShow(stanowiskoArkusz, k, 8, item.BruttoFiskalne, item.BruttoNpos);
                            stanowiskoArkusz.Cells[k, 9].Value = itemPos.Karta;
                            stanowiskoArkusz.Cells[k, 10].Value = itemPos.Konwersja;
                            stanowiskoArkusz.Cells[k, 11].Value = itemPos.Terminal;
                            stanowiskoArkusz.Cells[k, 12].Value = itemPos.KartaTerminal;
                            if (itemPos.KartaTerminal != 0)
                            {
                                string zakres = "J" + k.ToString() + ":" + "J" + k.ToString();
                                stanowiskoArkusz.Cells[zakres].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            }

                            k++;
                        }
                        //z 3 kolumnami bylo H
                        FormatTable(stanowiskoArkusz, "B",start,"L", k-1);
                        k++;
                      
                    }
                    stanowiskoArkusz.Cells.AutoFitColumns();
                    FileInfo excelFile = new FileInfo(fileName);
                    excel.SaveAs(excelFile);
                }
            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
            }
        }

        private static string Data_MMDDYYYY(string data)
        {
            return data.Substring(6, 2) + "." + data.Substring(4, 2) + "." + data.Substring(0, 4);
        }

        private static void StatusShow(ExcelWorksheet arkusz, int wiersz, int kolumna, decimal bruttoFiskalne, decimal bruttoNpos,string startKolumna="",string koniecKolumna="")
        {
            string litera = ((char)(64 + kolumna)).ToString();
            string zakres = litera + wiersz.ToString() + ":" + litera + wiersz.ToString();
            string zakresWiersz = "";
            if(startKolumna!="")
            {
                zakresWiersz= startKolumna + wiersz.ToString() + ":" + koniecKolumna + wiersz.ToString();
            }
            if (bruttoFiskalne==0&&bruttoNpos==0)
            {
                //inne awarie
                arkusz.Cells[wiersz, kolumna].Value = "inne awarie";
                arkusz.Cells[zakres].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                // 
                if (zakresWiersz != "")
                {
                    arkusz.Cells[zakresWiersz].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    arkusz.Cells[zakresWiersz].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                }
                return;
            }
            if (bruttoFiskalne==bruttoNpos)
            {
                arkusz.Cells[wiersz, kolumna].Value = "poprawne dane";
                arkusz.Cells[zakres].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                return;

            }
            if (bruttoFiskalne == 0)
            {
                arkusz.Cells[wiersz, kolumna].Value = "brak danych";
                arkusz.Cells[zakres].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                return;

            }
            if (bruttoFiskalne != bruttoNpos)
            {
                arkusz.Cells[wiersz, kolumna].Value = "dane nie  kompletne";
                arkusz.Cells[zakres].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                return;

            }
        }

        private static void FormatTable(ExcelWorksheet Arkusz,string literaStart,int wierszStart,string literaKoniec,int wierszKoniec)
        {
            string modelRange = literaStart+wierszStart.ToString()+":"+literaKoniec + wierszKoniec.ToString();
            var modelTable = Arkusz.Cells[modelRange];
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            modelTable.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
           
            string modelRangeHead = literaStart + wierszStart.ToString() + ":" + literaKoniec + wierszStart.ToString();
            var modelTableHead = Arkusz.Cells[modelRangeHead];
            modelTableHead.Style.Font.Bold = true;

        }

        private static void HeaderPos(ExcelWorksheet Arkusz,int wiersz,int kolumnaStart, params string[] list)
        {
           
            foreach (var item in list)
            {
                Arkusz.Cells[wiersz, kolumnaStart].Value =item;
                kolumnaStart++;
            }




        }
    }

}

