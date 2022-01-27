using AnalizaDll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaSold
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;
          

            string dataFile = "";

            string dataOracle = "";
            classLog.LogInfo("start ");
            if (args.Length == 0)
            {
                DateTime dt = DateTime.Today.AddDays(-1);
                dataFile = dt.ToString(classConst.formatDate);
                classLog.LogInfo("Generowanie danych dla " + dataFile);

                DateTime datastart = DateTime.Today.AddDays(-8);
                DateTime datakoniec = DateTime.Today.AddDays(-1);

                while (datastart < datakoniec)
                {
                    datastart = datastart.AddDays(1);
                    dataOracle = datastart.ToString(classConst.formatDateOracle);
                    dataFile = datastart.ToString(classConst.formatDate);
                    List<Sold> nposDataOld = classFun.SoldOracle(classConfig.Poloczenie, dataOracle,classConfig.SklepNotIn);


                    SaveOracleToFile(nposDataOld, dataFile);

                }
                if (startTime.Day==1)
                {
                    MiesieczneRaporty();
                }
                DateTime stopTime = DateTime.Now;
                TimeSpan roznica = stopTime - startTime;
                Console.WriteLine("Czas pracy:" + roznica.ToString(@"hh\:mm\:ss"));
                return;

                
            }
            if (args.Length == 1)
            {
                if (args[0].ToLower()=="m")
                {
                    MiesieczneRaporty();
                    return;
                }
                if (args[0].ToLower() == "h")
                {
                    DescribeProgram();
                    return;
                }
                else
                {
                    classLog.LogInfo("Generowanie danych dla " + args[0]);
                    dataFile = args[0];
                    DateTime dt = DateTime.ParseExact(dataFile, classConst.formatDate, System.Globalization.CultureInfo.InvariantCulture);

                    dataOracle = dt.ToString(classConst.formatDateOracle);

                    List<Sold> NposData1 = classFun.SoldOracle(classConfig.Poloczenie, dataOracle,classConfig.SklepNotIn);
                    SaveOracleToFile(NposData1, dataFile);

                }
                return;
            }
            if (args.Length == 2)
            {
                if (args[0] == "s")
                {
                    classLog.LogWarn("Szyfrowanie");
                    string dane = Encrypt.SzyfrujHaslo(args[1]);
                    Console.WriteLine(dane);
                    Console.WriteLine("Czy zapisac haslo w pliku config(nacisnąć t lub T(zapis) inny wyjście)");
                    ConsoleKeyInfo k = Console.ReadKey(true);
                    if (k.KeyChar == 't' || k.KeyChar == 'T')
                    {
                        classFun.UpdateAppSettings("Poloczenie", dane);
                        classLog.LogError("zapisano");
                    }
                    else
                    {
                        classLog.LogWarn("    tylko wygenerowano");
                    }
                    return;
                }
                else
                {
                    int wstecz = int.Parse(args[1]);
                    classLog.LogInfo("Generowanie plików dat");
                    DateTime dts = DateTime.Now.AddDays(-1 * wstecz);
                    while (dts < DateTime.Today)
                    {
                        Console.WriteLine(dts.ToString(classConst.formatDateOracle));
                        string dataGenFile = dts.ToString(classConst.formatDate);


                        List<Sold> NposData1 = classFun.SoldOracle(classConfig.Poloczenie, dts.ToString(classConst.formatDateOracle), ""); ;

                        SaveOracleToFile(NposData1, dataGenFile);

                        dts = dts.AddDays(1);

                    }
                }
            }

        }

        private static void MiesieczneRaporty()
        {

            DateTime startTime = DateTime.Now;
           string monthPrev = startTime.AddMonths(-1).ToString(classConst.formatYearMonth);
           // string monthPrev = startTime.ToString(classConst.formatYearMonth);
            string info = "Generowanie raport miesieczny za " + OpisMieieczny() + " na wsystkie daty";
            Console.WriteLine(info);
                classLog.LogInfo(info);
                classFun.GenerateMiesiac(classConfig.StatystkaKatalog, classConfig.StatystkaKatalogMiesiac, monthPrev);
           
           
        }

        private static void Testujemy()
        {
            Console.WriteLine("cos testujemy");
            var listSklepowSprzedaz = classFun.SoldOracle(classConfig.Poloczenie, "20210301", "");
            SaveTest(listSklepowSprzedaz, "listSklepowSprzedaz.txt");
            var listSklepow = classFun.SklepStanowisk(classConfig.Poloczenie, "20210301");
            SaveTest(listSklepow, "listSklepow.txt");
            int roznicaSklep = listSklepowSprzedaz.Count - listSklepow.Count;

            List<Sold> roznicaSold = (from n in listSklepow
                                      join f in listSklepowSprzedaz
                                      on new { n.NumerSklep, n.Stanowisko } equals new { f.NumerSklep, f.Stanowisko }
                                       into EmployeeAddressGroup
                                      from address in EmployeeAddressGroup.DefaultIfEmpty()
                                      where address is null
                                      select n).ToList();
            SaveTest(roznicaSold, "dane.txt");

            return;
        }

        private static void SaveTest(List<Sold> resultoRACLE, string v)
        {
            File.WriteAllText(v, "");
            foreach (var item in resultoRACLE)
            {
                File.AppendAllText(v,item.NumerSklep+";"+item.Stanowisko+Environment.NewLine);
            }
        }

        private static void UpdateShopData(ShopXml sklepXML, List<Sold> nposDataOld, DateTime dataOld)
        {
            var query1 = from x in sklepXML.Sklepy
                         join y in nposDataOld
                             on x.Numer equals y.NumerSklep
                         select new { x, y };
            foreach (var match in query1)
                match.x.DateUpdate = dataOld;
        }

        private static void SaveOracleToFile(List<Sold> nposData1, string dataFile)
        {
            string fileName = classConfig.StatystkaKatalog + "statystykaOracle_" + dataFile + ".csv";
            StringBuilder sb1 = new StringBuilder();
            string eol1 = Environment.NewLine;
            sb1.Append("Data;NumerSklep;Stanowisko;Brutto;NposTerminal;Terminal;NazwaSklep;Konwersja" + eol1);
            foreach (var item in nposData1)
            {
                //"++"
                sb1.Append(dataFile
                    + ";" + item.NumerSklep.PadLeft(4, '0')
                    + ";" + item.Stanowisko.PadLeft(2, '0')
                    + ";" + item.Brutto.ToString().Replace(",", ".")
                    + ";" + item.Karta.ToString().Replace(",", ".")
                    + ";" + item.Terminal.ToString().Replace(",", ".")
                    + ";" + item.ShopName
                     + ";" + item.Konwersja.ToString().Replace(",", ".")

                    + eol1);

            }
            File.WriteAllText(fileName, sb1.ToString());
            classLog.LogInfo("Zapis do katalogu " + fileName);
        }

        public static string GetConsoleVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version.ToString();

        }
        private static void DescribeProgram()
        {
            string eol = Environment.NewLine;

            string programName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string pom = "AnalizaDll.dll";
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(pom);
            StringBuilder sb = new StringBuilder();
            sb.Append(eol);
            sb.Append("Program " + programName + " [wersja:" + GetConsoleVersion() + "]"+  eol);
            int ii = classConfig.Poloczenie.IndexOf("PASSWORD");
            sb.Append("Odczytujemy dane z bazy:" + classConfig.Poloczenie.Substring(0,ii) + eol);
            sb.Append("Zapisujemy wynik katalogu "+ classConfig.StatystkaKatalog + eol);
            sb.Append("Program z parametrem h[H]\topis programu" + eol);

            sb.Append("Program z parametrem "+classConst.formatDate+"\tzapis do pliku "+ classConfig.StatystkaKatalog + "statystykaOracle_"+classConst.formatDate+".csv" + eol);
            sb.Append("Program z parametrem g 3\tgenerowanie plików na 3 dni wstecz" + eol);
            sb.Append("Program z parametrem m[M]\traport miesieczny za " + OpisMieieczny()+eol +"\t\t\tzapisujemy w "+classConfig.StatystkaKatalogMiesiac + eol);
            sb.Append("Program z parametrem s połączenie\tszyfrowanie hasla" + eol);
         
            sb.Append("\t parametr połączenie podajemy w cudzysłowiach np:" + eol);
            sb.Append("AnalizaSold.exe s \"DATA SOURCE=10.50.20.131:1521/CCCNPOS;USER ID=użytkownik;PASSWORD=haslo;PERSIST SECURITY INFO=True;\"" + eol);

            sb.Append("Historia zmian:" + eol);
            sb.Append(WersjaOPis(10) + eol);
            sb.Append("\t-HalfPrice" + eol);
            sb.Append(WersjaOPis(11) + eol);
            sb.Append("\t-Raporty miesieczne" + eol);
            sb.Append(WersjaOPis(12) + eol);
            sb.Append("\t-Powielone kwoty w raporcie miesiecznym" + eol);
            File.WriteAllText("help.txt", sb.ToString());
            classLog.LogWarn(sb.ToString());
            Console.WriteLine(sb.ToString());
        }

        private static string OpisMieieczny()
        {
            return DateTime.Now.AddMonths(-1).ToString(classConst.formatYearMonthName);
        }

        private static string WersjaOPis(int wersja)
        {
            return wersja + ".0.0.0:";
        }
    }
}
