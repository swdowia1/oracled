
using Dapper;
using Oracle.ManagedDataAccess.Client;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AnalizaDll
{
    public static class classFun
    {
         public static List<Sold> SoldOracle(string connstring, string data, string sklepNotIn)
        {
            try
            {

                Console.WriteLine("Raport dla daty " + data);
                using (OracleConnection conn = new OracleConnection(connstring))
                {

                    conn.Open();
                    var strQuery = @"with terminalTR as(select k.id KasaId,
sum(
CASE
WHEN (dt.symbol='FVKOR' or dt.symbol='PARKOR') THEN -1*nvl2(t.amountinterminalcurrency,t.amountinterminalcurrency,t.amount)

ELSE nvl2(t.amountinterminalcurrency,t.amountinterminalcurrency,t.amount)
END) as Terminal

from POS.nposterminaltransakcja t
INNER JOIN pos.NPosDokument d on t.dokumentid=d.id and  t.issuccess=1
inner join pos.nposdzien dz on d.dzienid=dz.id
inner join POS.nposdokumenttyp dt on d.dokumenttypid=dt.id
inner join POS.nsysstanowisko st on d.stanowiskoid=st.id 
inner join POS.nposkasa k on k.stanowiskoid=st.id
where  (dz.Data='" + data + "') " +
@"and
 t.issuccess=1
 group by k.id),
 ksiegowa as (SELECT  
  l.Symbol AS NumerSklep,

  st.Nr as Stanowisko,
  formaPlat.id as formaPlatid,
  sum(tot.Netto) as netto, 
  sum(tot.Brutto) as brutto, 
  sum(tot.Vat) as vat,    
  sum(tot.Ilosc) as ilosc,
 dz.id dzienid,
 kasa.Id kasid 
FROM pos.NSysLokal l
INNER JOIN pos.NPosDzien dz ON dz.LokalID= l.ID
INNER JOIN pos.NPosDokument d ON d.DzienID= dz.ID AND d.FlgImportowany= 0
inner join POS.nsysstanowisko st on d.stanowiskoid=st.id
inner join POS.nposkasa kasa on st.id=kasa.stanowiskoid
INNER JOIN pos.NPosDokumentTyp dt ON dt.ID= d.DokumentTypID AND  dt.FlgTyp= 1 AND dt.FlgRodzajSpr= 0 AND dt.FlgKorekta= 0
INNER JOIN pos.NSysOperator oper ON oper.ID= d.OperatorID
inner join POS.nposplatnoscforma formaPlat on formaPlat.flgtyp=1
LEFT JOIN pos.NPosDokumentTotalizer tot ON tot.DokumentID= d.ID

left join pos.nsyskraj kr on l.krajid = kr.id

WHERE

    (kr.symbol = 'PL')  and
   --l.nr=1146 and
   l.opis not like ('Gino%') and
    (dz.Data='" + data + "') " +
    @"group by   l.Symbol,st.Nr,dz.id,kasa.id,formaPlat.id)select 
    ksiegowa.numersklep,ksiegowa.stanowisko,max(ksiegowa.brutto) Brutto,sum(plat.utarg) Karta,max(
    CASE
WHEN tg.terminal is  NULL THEN 0.0

ELSE tg.terminal
END) Terminal  from ksiegowa
    left join terminalTR tg on ksiegowa.kasid=tg.kasaid
    left join POS.nposzmianatotalizer plat on plat.kasaid=ksiegowa.kasid and plat.dzienid=ksiegowa.dzienid
  and plat.platnoscformaid=ksiegowa.formaPlatid

group by ksiegowa.numersklep,ksiegowa.stanowisko";
                    classLog.LogInfo(classConst.Podzial, "Sklepy sprzedaz", strQuery);
                    string sklepyQuer = @"SELECT Lokal.symbol NumerSklep,s.Nr Stanowisko,lokal.opis as ShopName
FROM POS.NSYSLOKAL Lokal
join POS.nsysstanowisko s on s.lokalid=Lokal.id
INNER JOIN POS.NSYSKRAJ Kraj on kraj.id = lokal.krajid
WHERE lokal.opis not like ('%Gino%')
and lokal.dataotwarcia <'" + data + "' and (lokal.datazamkniecia >'" + data + "' or trunc(lokal.datazamkniecia) = '1900-01-01' )" +
@"and kraj.symbol ='PL' and lokal.Nr not in(" + sklepNotIn + ")"
+ @" and s.nazwa like 'POS%'
order by 1,2";
                    var listaSklep = conn.Query<Sold>(sklepyQuer).ToList(); ;

                    classLog.LogInfo(classConst.Podzial, "Sklepy", sklepyQuer);

                    var lista = conn.Query<Sold>(strQuery).ToList(); ;
                  
                    List<Sold> roznicaSold = (from n in listaSklep
                                              join f in lista
                                              on new { n.NumerSklep, n.Stanowisko } equals new { f.NumerSklep, f.Stanowisko }
                                               into EmployeeAddressGroup
                                              from address in EmployeeAddressGroup.DefaultIfEmpty()
                                              where address is null
                                              select n).ToList();
                    lista.AddRange(roznicaSold);
                    lista.ForEach(k =>
                    {
                        var listaAktualizacja = listaSklep.FirstOrDefault(s => s.NumerSklep == k.NumerSklep);
                        if (listaAktualizacja != null)
                            k.ShopName = listaAktualizacja.ShopName;
                       
                    });
                    lista.ForEach(k => k.Stanowisko = k.Stanowisko.PadLeft(2, '0'));
                    lista.ForEach(k => k.KartaTerminal = k.Terminal - k.Karta);
                    lista.ForEach(k => k.Konwersja = 12.34M);
                    return lista.OrderBy(k => k.NumerSklep).ThenBy(j => j.Stanowisko).ToList();

                }
            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
                return null;
            }
            //string connstring = "DATA SOURCE=10.50.20.131:1521/CCCNPOS;USER ID="+user+";PASSWORD=Piaseczno!;PERSIST SECURITY INFO=True;";

        }

        public static string LiniaCSV(params string[] list)
        {
            return string.Join(";", list);
        }
        public static string LiniaCSV(string sklep,string stanowisko,params decimal[] list)
        {//.Replace(",", ".")

            string pom1 = sklep + ";" + stanowisko;
            foreach (var item in list)
            {
                pom1 += ";" + item.ToString().Replace(",", ".");
            }

            return pom1;
        }
        public static void GenerateMiesiac(string source,string dest,string monthPrev)
        {
            string liniaInfo = "";
            try
            {
                classLog.LogInfo(source, dest, monthPrev);
                List<MonthStanowisko> listaNpos = new List<MonthStanowisko>();
                List<MonthStanowisko> listaFiskal = new List<MonthStanowisko>();
                string nameFileFiskal =string.Format(classConst.nameFileFiskal, monthPrev);
                string nameFileNpos = string.Format(classConst.nameFileNpos, monthPrev); ;
                string filtr = $"statystyka*_{monthPrev}*.csv";
                string[] pliki = Directory.GetFiles(source, filtr);
                if (pliki.Length == 0)
                {
                    classLog.LogWarn("Brak plików za " + monthPrev + " w katalogu " + source);
                    return;
                }
                foreach (var item in pliki)
                {

                    string[] linie = File.ReadAllLines(item);

                    if (item.Contains("Oracle"))
                    {
                        foreach (var linia in linie.Skip(1))
                        {
                            string[] kolumn = linia.Split(';');
                            MonthStanowisko s = new MonthStanowisko();

                            s.NumerSklep = kolumn[1].Trim();
                            s.Stanowisko = kolumn[2].Trim();
                            s.BruttoNpos = GetDecimal(kolumn[3]);
                            s.Karta = GetDecimal(kolumn[4]);
                            s.Terminal = GetDecimal(kolumn[4]);
                            listaNpos.Add(s);
                        }
                    }
                    else
                    {
                        foreach (var linia in linie)
                        {
                            string[] kolumn = linia.Split(';');
                            MonthStanowisko s = new MonthStanowisko();

                            s.NumerSklep = kolumn[2].Trim();
                            s.Stanowisko = kolumn[3].Trim();
                            s.NumerRaport = kolumn[4].Trim();
                            liniaInfo = item + " " +linia;
                            s.BruttoFiskalne = GetDecimal(kolumn[6],liniaInfo);
                            listaFiskal.Add(s);


                        }
                    }
                }
                listaFiskal = (from r in listaFiskal
                         group r by new { r.NumerSklep, r.Stanowisko, r.NumerRaport } into rGrupa
                         select new MonthStanowisko
                         {
                             NumerSklep = rGrupa.Key.NumerSklep,
                             Stanowisko = rGrupa.Key.Stanowisko,
                             NumerRaport = rGrupa.Key.NumerRaport,
                             BruttoFiskalne = rGrupa.Max(j => j.BruttoFiskalne)
                         }).ToList();
                var grupaFiskal = (from l in listaFiskal
                                   group l by new { Sklep = l.NumerSklep, Pos = l.Stanowisko } into lGroup
                                   select new MonthStanowisko
                                   {
                                       NumerSklep = lGroup.Key.Sklep,
                                       Stanowisko = lGroup.Key.Pos,
                                       //BruttoNpos = lGroup.Sum(k => k.BruttoNpos),
                                       BruttoFiskalne = lGroup.Sum(k => k.BruttoFiskalne),
                                       //Karta = lGroup.Sum(k => k.Karta),
                                      // Terminal = lGroup.Sum(k => k.Terminal),
                                   }).ToList();
                var grupaOracle = (from l in listaNpos
                                   group l by new { Sklep = l.NumerSklep, Pos = l.Stanowisko } into lGroup
                                   select new MonthStanowisko
                                   {
                                       NumerSklep = lGroup.Key.Sklep,
                                       Stanowisko = lGroup.Key.Pos,
                                       BruttoNpos = lGroup.Sum(k => k.BruttoNpos),
                                       BruttoFiskalne = lGroup.Sum(k => k.BruttoFiskalne),
                                       Karta = lGroup.Sum(k => k.Karta),
                                       Terminal = lGroup.Sum(k => k.Terminal),
                                   }).ToList();

                SaveGrupa(grupaFiskal, dest + nameFileFiskal);
                SaveGrupa(grupaOracle, dest + nameFileNpos);

            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
            }
        }

        private static void SaveGrupa(List<MonthStanowisko> grupaFiskal, string pathSave)
        {/*
          *  public decimal BruttoNpos { get; set; }
        public decimal BruttoFiskalne { get; set; }
        public decimal Karta { get; set; }
        public decimal Terminal { get; set; }
        public decimal BruttoFiskalneNpos { get; set; }
        public Status Status { get; set; }
        public string StatusKarta { get; set; }
            public string Stanowisko { get; set; }
        public string NumerSklep { get; set; }
        public Status Status { get; set; }

          */
            try
            {
                StringBuilder sb = new StringBuilder();
               
                string eol = classConst.Eol;
                string header = LiniaCSV("NumerSklep", "Stanowikos", "BruttoFiskalne", "BruttoNpos", "Karta", "Terminal"+eol);
                sb.Append(header);
                foreach (var item in grupaFiskal.OrderBy(k=>k.NumerSklep).ThenBy(j=>j.Stanowisko))
                {
                    string linia = LiniaCSV(item.NumerSklep, item.Stanowisko, item.BruttoFiskalne, item.BruttoNpos, item.Karta, item.Terminal) + eol;
                    sb.Append(linia);
                }
                File.WriteAllText(pathSave, sb.ToString());
            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
            }
        }

        public static List<MonthSklep> RaportMiesiac(string monthPrev,string statystykaMiesiac)
        {
            List<MonthSklep> result = new List<MonthSklep>();
            List<MonthStanowisko> listaNpos = new List<MonthStanowisko>();
            List<MonthStanowisko> listaFiskal = new List<MonthStanowisko>();

            List<MonthStanowisko> listSuma = new List<MonthStanowisko>();
            try
            {
                string fileNpos = statystykaMiesiac + string.Format(classConst.nameFileNpos, monthPrev);
                string[] linieNpos = File.ReadAllLines(fileNpos).Skip(1).ToArray();

                foreach (var linia in linieNpos)
                {
                    string[] kolumn = linia.Split(';');
                    MonthStanowisko s = new MonthStanowisko();

                    s.NumerSklep = kolumn[0].Trim();
                    s.Stanowisko = kolumn[1].Trim();
                    s.BruttoNpos = GetDecimal(kolumn[3]);
                    s.Karta = GetDecimal(kolumn[4]);
                    s.Terminal = GetDecimal(kolumn[5]);
                    /*
                     0 NumerSklep;
                    1 Stanowikos;
                   
                    3 BruttoNpos;
                    4 Karta;
                    5 Terminal
                     */
                    listaNpos.Add(s);
                }

                string[] linieFiskal = File.ReadAllLines(statystykaMiesiac + string.Format(classConst.nameFileFiskal, monthPrev)).Skip(1).ToArray();

                foreach (var linia in linieFiskal)
                {
                    string[] kolumn = linia.Split(';');
                    MonthStanowisko s = new MonthStanowisko();

                    s.NumerSklep = kolumn[0].Trim();
                    s.Stanowisko = kolumn[1].Trim();
                    s.BruttoFiskalne = GetDecimal(kolumn[2]);
                   
                    listaFiskal.Add(s);
                }


                var grupaFiskal = (from l in listaFiskal
                                   group l by new { Sklep = l.NumerSklep, Pos = l.Stanowisko } into lGroup
                                   select new MonthStanowisko
                                   {
                                       NumerSklep = lGroup.Key.Sklep,
                Stanowisko = lGroup.Key.Pos,
                BruttoNpos = lGroup.Sum(k=>k.BruttoNpos),
                                       BruttoFiskalne = lGroup.Sum(k => k.BruttoFiskalne),
                                       Karta = lGroup.Sum(k => k.Karta),
                                      Terminal= lGroup.Sum(k => k.Terminal),
                                   }).ToList();
                var grupaOracle = (from l in listaNpos
                                   group l by new { Sklep = l.NumerSklep, Pos = l.Stanowisko } into lGroup
                                   select new MonthStanowisko
                                   {
                                       NumerSklep = lGroup.Key.Sklep,
                                       Stanowisko = lGroup.Key.Pos,
                                       BruttoNpos = lGroup.Sum(k => k.BruttoNpos),
                                       BruttoFiskalne = lGroup.Sum(k => k.BruttoFiskalne),
                                       Karta = lGroup.Sum(k => k.Karta),
                                       Terminal = lGroup.Sum(k => k.Terminal),
                                   }).ToList();
                var listaAll=(from o in grupaOracle
                             join f in grupaFiskal
                             on new { o.NumerSklep, o.Stanowisko } equals new { f.NumerSklep, f.Stanowisko } into lNull
                             from lAll in lNull.DefaultIfEmpty()
                             select new MonthStanowisko
                             {
                                 NumerSklep = o.NumerSklep,
                                 Stanowisko = o.Stanowisko,
                                 BruttoNpos = o.BruttoNpos,
                                 BruttoFiskalne = lAll!=null?lAll.BruttoFiskalne:0,
                                 Karta = o.Karta,
                                 Terminal = o.Terminal,
                             }).ToList();

                result = (from m in listaAll
                          group m by m.NumerSklep into mGroup
                          select new MonthSklep
                          {
                              NumerSklep = mGroup.Key,
                              BruttoFiskalne = mGroup.Sum(k => k.BruttoFiskalne),
                              BruttoNpos = mGroup.Sum(k => k.BruttoNpos),
                              Karta = mGroup.Sum(k => k.Karta),
                             
                              Terminal = mGroup.Sum(k => k.Terminal),
                              Status=GetStatus(mGroup.Sum(k => k.BruttoFiskalne), mGroup.Sum(k => k.BruttoNpos)),                             
                              Pos =(from p in mGroup
                                    group p by p.Stanowisko into pGroup
                                    select new MonthStanowisko
                                    {
                                        Stanowisko= pGroup.Key,
                                        BruttoFiskalne = pGroup.Sum(k => k.BruttoFiskalne),
                                        BruttoNpos = pGroup.Sum(k => k.BruttoNpos),
                                        Karta = pGroup.Sum(k => k.Karta),
                                        Terminal = pGroup.Sum(k => k.Terminal),
                                        Status = GetStatus(pGroup.Sum(k => k.BruttoFiskalne), pGroup.Sum(k => k.BruttoNpos))
                                    }).OrderBy(p=>p.Stanowisko).ToList()

                          }).OrderBy(j=>j.NumerSklep).ToList();

                result.ForEach(k => k.BruttoFiskalneNpos = k.BruttoNpos - k.BruttoFiskalne);
                foreach (var item in result)
                {
                    item.Pos.ForEach(j => j.BruttoFiskalneNpos = j.BruttoNpos - j.BruttoFiskalne);
                }
              
            }


            catch (Exception ex)
            {
                classLog.LogException(ex);
            }
            return result;

        }

        public static List<MonthSklep> RaportMiesiac_prev(string monthPrev, string katalogSatytyka)
        {
            List<MonthSklep> result = new List<MonthSklep>();
            List<MonthStanowisko> listaOracle = new List<MonthStanowisko>();
            List<MonthStanowisko> listaFiskal = new List<MonthStanowisko>();

            List<MonthStanowisko> listSuma = new List<MonthStanowisko>();
            try
            {
                string filtr = $"statystyka*_{monthPrev}*.csv";

                string[] pliki = Directory.GetFiles(katalogSatytyka, filtr);
                if (pliki.Length == 0)
                {
                    classLog.LogWarn("Brak plików za " + monthPrev + " w katalogu " + katalogSatytyka);
                    return result;
                }
                classLog.LogInfo("Raporty Miesieczne za " + monthPrev);
                classLog.LogInfo("Ilosc sklepow " + pliki.Length);
                foreach (var item in pliki)
                {

                    string[] linie = File.ReadAllLines(item);

                    if (item.Contains("Oracle"))
                    {
                        foreach (var linia in linie.Skip(1))
                        {
                            string[] kolumn = linia.Split(';');
                            MonthStanowisko s = new MonthStanowisko();

                            s.NumerSklep = kolumn[1].Trim();
                            s.Stanowisko = kolumn[2].Trim();
                            s.BruttoNpos = GetDecimal(kolumn[3]);
                            s.Karta = GetDecimal(kolumn[4]);
                            s.Terminal = GetDecimal(kolumn[4]);
                            listaOracle.Add(s);
                        }
                    }
                    else
                    {
                        foreach (var linia in linie)
                        {
                            string[] kolumn = linia.Split(';');
                            MonthStanowisko s = new MonthStanowisko();

                            s.NumerSklep = kolumn[2].Trim();
                            s.Stanowisko = kolumn[3].Trim();
                            s.BruttoFiskalne = GetDecimal(kolumn[6]);
                            listaFiskal.Add(s);


                        }
                    }
                }
                classLog.LogInfo("Grupowanie fiskal:" + listaFiskal.Count + "         oracle:" + listaOracle.Count);

                var grupaFiskal = (from l in listaFiskal
                                   group l by new { Sklep = l.NumerSklep, Pos = l.Stanowisko } into lGroup
                                   select new MonthStanowisko
                                   {
                                       NumerSklep = lGroup.Key.Sklep,
                                       Stanowisko = lGroup.Key.Pos,
                                       BruttoNpos = lGroup.Sum(k => k.BruttoNpos),
                                       BruttoFiskalne = lGroup.Sum(k => k.BruttoFiskalne),
                                       Karta = lGroup.Sum(k => k.Karta),
                                       Terminal = lGroup.Sum(k => k.Terminal),
                                   }).ToList();
                var grupaOracle = (from l in listaOracle
                                   group l by new { Sklep = l.NumerSklep, Pos = l.Stanowisko } into lGroup
                                   select new MonthStanowisko
                                   {
                                       NumerSklep = lGroup.Key.Sklep,
                                       Stanowisko = lGroup.Key.Pos,
                                       BruttoNpos = lGroup.Sum(k => k.BruttoNpos),
                                       BruttoFiskalne = lGroup.Sum(k => k.BruttoFiskalne),
                                       Karta = lGroup.Sum(k => k.Karta),
                                       Terminal = lGroup.Sum(k => k.Terminal),
                                   }).ToList();
                var listaAll = (from o in grupaOracle
                                join f in grupaFiskal
                                on new { o.NumerSklep, o.Stanowisko } equals new { f.NumerSklep, f.Stanowisko } into lNull
                                from lAll in lNull.DefaultIfEmpty()
                                select new MonthStanowisko
                                {
                                    NumerSklep = o.NumerSklep,
                                    Stanowisko = o.Stanowisko,
                                    BruttoNpos = o.BruttoNpos,
                                    BruttoFiskalne = lAll != null ? lAll.BruttoFiskalne : 0,
                                    Karta = o.Karta,
                                    Terminal = o.Terminal,
                                }).ToList();

                result = (from m in listaAll
                          group m by m.NumerSklep into mGroup
                          select new MonthSklep
                          {
                              NumerSklep = mGroup.Key,
                              BruttoFiskalne = mGroup.Sum(k => k.BruttoFiskalne),
                              BruttoNpos = mGroup.Sum(k => k.BruttoNpos),
                              Karta = mGroup.Sum(k => k.Karta),

                              Terminal = mGroup.Sum(k => k.Terminal),
                              Status = GetStatus(mGroup.Sum(k => k.BruttoFiskalne), mGroup.Sum(k => k.BruttoNpos)),
                              Pos = (from p in mGroup
                                     group p by p.Stanowisko into pGroup
                                     select new MonthStanowisko
                                     {
                                         Stanowisko = pGroup.Key,
                                         BruttoFiskalne = pGroup.Sum(k => k.BruttoFiskalne),
                                         BruttoNpos = pGroup.Sum(k => k.BruttoNpos),
                                         Karta = pGroup.Sum(k => k.Karta),
                                         Terminal = pGroup.Sum(k => k.Terminal),
                                         Status = GetStatus(pGroup.Sum(k => k.BruttoFiskalne), pGroup.Sum(k => k.BruttoNpos))
                                     }).OrderBy(p => p.Stanowisko).ToList()

                          }).OrderBy(j => j.NumerSklep).ToList();

                result.ForEach(k => k.BruttoFiskalneNpos = k.BruttoNpos - k.BruttoFiskalne);
                foreach (var item in result)
                {
                    item.Pos.ForEach(j => j.BruttoFiskalneNpos = j.BruttoNpos - j.BruttoFiskalne);
                }

            }


            catch (Exception ex)
            {
                classLog.LogException(ex);
            }
            return result;

        }


        public static List<Sold> SklepStanowisk(string poloczenie, string v)
        {

            using (OracleConnection conn = new OracleConnection(poloczenie))
            {

                conn.Open();
                var strQuery = @"SELECT Lokal.symbol NumerSklep,s.Nr Stanowisko
FROM POS.NSYSLOKAL Lokal
join POS.nsysstanowisko s on s.lokalid=Lokal.id
INNER JOIN POS.NSYSKRAJ Kraj on kraj.id = lokal.krajid
WHERE lokal.opis not like ('%Gino%')
and trunc(lokal.dataotwarcia) <TRUNC(SYSDATE)
and (trunc(lokal.datazamkniecia) >TRUNC(SYSDATE) or trunc(lokal.datazamkniecia) = '1900-01-01' )
and kraj.symbol ='PL' and lokal.symbol <> ('0000') and lokal.symbol <> ('4000') and lokal.symbol <> ('199999')
and s.nazwa like 'POS%'
order by 1,2";
                //  classLog.LogInfo(classConst.Podzial, "glowne", strQuery);

                var lista = conn.Query<Sold>(strQuery).ToList(); ;
                lista.ForEach(k => k.Stanowisko = k.Stanowisko.PadLeft(2, '0'));
                lista.ForEach(k => k.KartaTerminal = k.Terminal - k.Karta);

                return lista.OrderBy(k => k.NumerSklep).ThenBy(j => j.Stanowisko).ToList();

            }
        }

        public static DateTime DateFromString(string dataFile)
        {
            return DateTime.ParseExact(dataFile,
                                  classConst.formatDate,
                                   CultureInfo.InvariantCulture);
        }

        public static List<Sold> SoldOracle_popredzni(string connstring, string data, string sklepNotIn)
        {
            try
            {


                using (OracleConnection conn = new OracleConnection(connstring))
                {

                    conn.Open();
                    var strQuery = @"with ksiegowa as (SELECT  
  l.Symbol AS NumerSklep,

  st.Nr as Stanowisko,
  formaPlat.id as formaPlatid,
  sum(tot.Netto) as netto, 
  sum(tot.Brutto) as brutto, 
  sum(CASE
WHEN tr.amount is  NULL THEN 0.0

ELSE tr.amount
END) as Terminal,
  sum(tot.Vat) as vat,    
  sum(tot.Ilosc) as ilosc,
 dz.id dzienid,
 kasa.Id kasid 
FROM pos.NSysLokal l
INNER JOIN pos.NPosDzien dz ON dz.LokalID= l.ID
INNER JOIN pos.NPosDokument d ON d.DzienID= dz.ID AND d.FlgImportowany= 0 AND d.FlgFiskalizacja= 1
inner join POS.nsysstanowisko st on d.stanowiskoid=st.id
inner join POS.nposkasa kasa on st.id=kasa.stanowiskoid
INNER JOIN pos.NPosDokumentTyp dt ON dt.ID= d.DokumentTypID AND  dt.FlgTyp= 1 AND dt.FlgRodzajSpr= 0 AND dt.FlgKorekta= 0
INNER JOIN pos.NSysOperator oper ON oper.ID= d.OperatorID
inner join POS.nposplatnoscforma formaPlat on formaPlat.flgtyp=1
LEFT JOIN pos.NPosDokumentTotalizer tot ON tot.DokumentID= d.ID
LEFT JOIN POS.Nposterminaltransakcja tr on tr.DokumentID = d.ID
left join pos.nsyskraj kr on l.krajid = kr.id

WHERE

    (kr.symbol = 'PL')  and
   l.opis not like ('Gino%') and
    (dz.Data='" + data + "') group by   l.Symbol,st.Nr,dz.id,kasa.id,formaPlat.id)" +
    @"select 
    ksiegowa.numersklep,ksiegowa.stanowisko,max(ksiegowa.brutto) Brutto,sum(plat.utarg) Karta,max(ksiegowa.terminal) Terminal  from ksiegowa
    left join POS.nposzmianatotalizer plat on plat.kasaid=ksiegowa.kasid and plat.dzienid=ksiegowa.dzienid
  and plat.platnoscformaid=ksiegowa.formaPlatid

group by ksiegowa.numersklep,ksiegowa.stanowisko";
                    classLog.LogInfo(classConst.Podzial, "glowne", strQuery);

                    var lista = conn.Query<Sold>(strQuery).ToList(); ;
                    string sklepyQuer = @"SELECT Lokal.symbol NumerSklep,s.Nr Stanowisko,lokal.opis as ShopName
FROM POS.NSYSLOKAL Lokal
join POS.nsysstanowisko s on s.lokalid=Lokal.id
INNER JOIN POS.NSYSKRAJ Kraj on kraj.id = lokal.krajid
WHERE lokal.opis not like ('%Gino%')
and lokal.dataotwarcia <'" + data + "' and (lokal.datazamkniecia >'" + data + "' or trunc(lokal.datazamkniecia) = '1900-01-01' )" +
@"and kraj.symbol ='PL' and lokal.Nr not in(" + sklepNotIn + ")"
+ @" and s.nazwa like 'POS%'
order by 1,2";
                    var listaSklep = conn.Query<Sold>(sklepyQuer).ToList(); ;
                    List<Sold> roznicaSold = (from n in listaSklep
                                              join f in lista
                                              on new { n.NumerSklep, n.Stanowisko } equals new { f.NumerSklep, f.Stanowisko }
                                               into EmployeeAddressGroup
                                              from address in EmployeeAddressGroup.DefaultIfEmpty()
                                              where address is null
                                              select n).ToList();
                    lista.AddRange(roznicaSold);
                    lista.ForEach(k => k.Stanowisko = k.Stanowisko.PadLeft(2, '0'));
                    lista.ForEach(k => k.KartaTerminal = k.Terminal - k.Karta);
                    return lista.OrderBy(k => k.NumerSklep).ThenBy(j => j.Stanowisko).ToList();

                }
            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
                return null;
            }
            //string connstring = "DATA SOURCE=10.50.20.131:1521/CCCNPOS;USER ID="+user+";PASSWORD=Piaseczno!;PERSIST SECURITY INFO=True;";

        }
        public static string Data_MMDDYYYY(string data)
        {
            return data.Substring(6, 2) + "." + data.Substring(4, 2) + "." + data.Substring(0, 4);
        }

        /// <summary>
        /// Zapis d
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void UpdateAppSettings(string key, string value)
        {
            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    configuration.AppSettings.Settings[key].Value = value;
                }
                else
                {
                    classLog.LogError("Brak Klucza:" + key);
                    return;
                }

                configuration.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("appSettings");


            }
            catch (Exception ex)
            {
                classLog.LogException(ex);

            }
        }



        public static List<FiskalData> FiskalFile(string path)
        {
            List<FiskalData> result = new List<FiskalData>();
            if (!File.Exists(path))
            {
                classLog.LogWarn("Nie wygenerowano pliku " + path);
                return result;
            }
            try
            {
                var linia = File.ReadAllLines(path);
                foreach (var item in linia)
                {

                   
                    string[] kolumn = item.Split(';');
                    FiskalData s = new FiskalData();
                    s.NumerSklep = kolumn[2];
                    s.Stanowisko = kolumn[3].PadLeft(2, '0');
                    s.NumerRaport = kolumn[4];
                    s.Data = kolumn[5];
                    s.Brutto = GetDecimal(kolumn[6]);

                   
                    result.Add(s);
                }

               SklepStanowiskoRaportaunikalny(result,path);
                result = (from r in result
                             group r by new { r.NumerSklep, r.Stanowisko, r.NumerRaport } into rGrupa
                             select new FiskalData
                             {
                                 NumerSklep = rGrupa.Key.NumerSklep,
                                 Stanowisko = rGrupa.Key.Stanowisko,
                                 NumerRaport = rGrupa.Key.NumerRaport,
                                 Brutto = rGrupa.Max(j => j.Brutto)
                             }).ToList();
               
               
                result = (from r in result
                          group r by new { r.NumerSklep, r.Stanowisko } into gg
                          select new FiskalData
                          {
                              NumerSklep = gg.Key.NumerSklep,
                              Stanowisko = gg.Key.Stanowisko,
                              Brutto = gg.Sum(j => j.Brutto),
                              TestIlosc=gg.Count()
                          }).ToList();

               
                
            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
                throw;
            }
            return result;
        }

        private static void SklepStanowiskoRaportaunikalny(List<FiskalData> result,string pathFile)
        {
            var test = (from r in result
                      group r by new { r.NumerSklep, r.Stanowisko, r.NumerRaport } into rGrupa
                      select new 
                      {
                          NumerSklep = rGrupa.Key.NumerSklep,
                          Stanowisko = rGrupa.Key.Stanowisko,
                          NumerRaport = rGrupa.Key.NumerRaport,
                          Ilosc = rGrupa.Count()
                      }).ToList();
            var test2 = test.Where(k => k.Ilosc > 1).ToList();
            if (test2.Count == 0)
            {
                classLog.LogWarn("Brak powielonych w " + pathFile);
            }
            else
            {
                classLog.LogWarn("W pliku " + pathFile);
                foreach (var item in test2)
                {
                    //0008;02;0271;
                    classLog.LogWarn("\t"+item.NumerSklep+";"+item.Stanowisko+";"+item.NumerRaport);
                }
            }
           
        }

        /*
         * using (var stream = new MemoryStream())
	using (var writer = new StreamWriter(stream))
	using (var reader = new StreamReader(stream))
	using (var csv = new CsvReader(reader))
	{
		writer.WriteLine("FirstName,LastName");
		writer.WriteLine("\"Jon\"hn\"\",\"Doe\"");
		writer.WriteLine("\"Jane\",\"Doe\"");
		writer.Flush();
		stream.Position = 0;

		var good = new List<Test>();
		var bad = new List<string>();
		var isRecordBad = false;
		csv.Configuration.BadDataFound = context =>
		{
			isRecordBad = true;
			bad.Add(context.RawRecord);
		};
		while (csv.Read())
		{
			var record = csv.GetRecord<Test>();
			if (!isRecordBad)
			{
				good.Add(record);
			}

			isRecordBad = false;
		}

		good.Dump();
		bad.Dump();
	}
         */

        private static List<Summary> GetLineSummary(string path)
        {
            List<Summary> result = new List<Summary>();
            try
            {
                var linia = File.ReadAllLines(path).Skip(1);
                foreach (var item in linia)
                {

                    string[] kolumn = item.Split(';');
                    Summary s = new Summary();
                    s.NumerSklep = kolumn[0];
                    s.Stanowisko = kolumn[1];
                    s.NumerRaport = kolumn[2];
                    s.BruttoNpos = GetDecimal(kolumn[3]);
                    s.BruttoFiskalne = GetDecimal(kolumn[4]);
                    s.BruttoFiskalneNpos = GetDecimal(kolumn[5]);

                    result.Add(s);
                }
            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
                throw;
            }
            return result;
        }

        public static decimal GetDecimal(string v,string liniaInfo="")
        {
            try
            {
                return decimal.Parse(v, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                classLog.LogError(liniaInfo);
                return 0;
            }
            //v = v.Replace(",", ".");
           
        }
        public static string GetLast(this string source, int tail_length, int take = 0)
        {

            if (tail_length >= source.Length)
                return source;
            if (take == 0)
                return source.Substring(source.Length - tail_length);
            else
                return source.Substring(source.Length - tail_length, take);
        }
        public static List<string> GetStatFile_org(string katalog, string StatystykaUser, string StatystykaPassword)
        {
            List<string> wynik = new List<string>();
            try
            {
                ClassImpesonality zab = new ClassImpesonality(StatystykaUser, StatystykaPassword);
                if (zab.impersonateValidUser())
                {

                    wynik = Directory.GetFiles(katalog, "*statystyka_*").Select(k => k.GetLast(12, 8)).OrderByDescending(k => k).ToList();
                    zab.undoImpersonation();
                    return wynik;
                }
            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
            }
            return wynik;
        }
        public static List<string> GetStatFile(string katalog)
        {
            List<string> wynik = new List<string>();
            try
            {

                wynik = Directory.GetFiles(katalog, "*statystykaOracle_*").Select(k => k.GetLast(12, 8)).OrderByDescending(k => k).ToList();



            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
            }
            return wynik;
        }
        /// <summary>
        /// Deserializes an XML file to the specified type of object.
        /// </summary>
        /// <typeparam name="T">The Type of object to deserialize</typeparam>
        /// <param name="xmlFilePath">The path to the XML file to deserialize</param>
        /// <returns>An object instance</returns>
        public static T DeserializeFromFile<T>(string xmlFilePath) where T : class
        {
            using (var reader = XmlReader.Create(xmlFilePath))
            {
                var serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(reader);
            }
        }
        public static void SerializeToFile<T>(string xmlFilePath, T objectToSerialize) where T : class
        {
            using (var writer = new StreamWriter(xmlFilePath))
            {
                // Do this to avoid the serializer inserting default XML namespaces.
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                var serializer = new XmlSerializer(objectToSerialize.GetType());
                serializer.Serialize(writer, objectToSerialize, namespaces);
            }
        }

        public static List<SummaryShop> SummaryAll(string katalog, string data)
        {

            List<SummaryShop> resultSummary = new List<SummaryShop>();
            List<Summary> resultAll = null;
            List<FiskalData> FiskalData = null;

            List<Sold> NposData = null;
            try
            {




                string pomPlik = katalog + "statystyka_" + data + ".csv";
                string oracleFile = katalog + "statystykaOracle_" + data + ".csv";
                FiskalData = classFun.FiskalFile(pomPlik);

                NposData = classFun.OracleFile(oracleFile);
                var ggg = NposData.Select(k => k.NumerSklep).Distinct().ToList();
                resultAll = (from n in NposData
                             join f in FiskalData
                             on new { n.NumerSklep, n.Stanowisko } equals new { f.NumerSklep, f.Stanowisko }
                             select new Summary
                             {
                                 NumerSklep = n.NumerSklep,
                                 ShopName=n.ShopName,
                                 Stanowisko = n.Stanowisko,
                                 NumerRaport = f.NumerRaport,
                                 
                                 BruttoNpos = n.Brutto,
                                 BruttoFiskalne = f.Brutto,
                                 BruttoFiskalneNpos = f.Brutto - n.Brutto,
                                 SapBrutto = 0,
                                 Data = data,
                                 Karta = n.Karta,
                                 Terminal = n.Terminal,
                                 KartaTerminal = n.Terminal - n.Karta+n.Konwersja,
                                 Konwersja=n.Konwersja,


                             }).ToList();

                List<Summary> resultoRACLE = (from n in NposData
                                              join f in FiskalData
                                              on new { n.NumerSklep, n.Stanowisko } equals new { f.NumerSklep, f.Stanowisko }
                                               into EmployeeAddressGroup
                                              from address in EmployeeAddressGroup.DefaultIfEmpty()
                                              where address is null
                                              select new Summary
                                              {
                                                  NumerSklep = n.NumerSklep,
                                                  ShopName=n.ShopName,
                                                  Stanowisko = n.Stanowisko,
                                                  NumerRaport = "",
                                                  BruttoNpos = n.Brutto,
                                                  BruttoFiskalne = 0,
                                                  BruttoFiskalneNpos = 0 - n.Brutto,
                                                  SapBrutto = 0,
                                                  Data = data,
                                                  Karta = n.Karta,
                                                  Terminal = n.Terminal,
                                                  Konwersja=n.Konwersja,
                                                  KartaTerminal = n.Terminal - n.Karta+n.Konwersja


                                              }).ToList();
                List<Summary> resultFiskal = (from f in FiskalData
                                              join n in NposData
                                              on new { f.NumerSklep, f.Stanowisko } equals new { n.NumerSklep, n.Stanowisko }
                                               into EmployeeAddressGroup
                                              from address in EmployeeAddressGroup.DefaultIfEmpty()
                                              where address is null
                                              select new Summary
                                              {
                                                  NumerSklep = f.NumerSklep,
                                                  Stanowisko = f.Stanowisko,
                                                  ShopName= EmployeeAddressGroup.Count()>0?EmployeeAddressGroup.FirstOrDefault().ShopName:"",
                                                  NumerRaport = f.NumerRaport,
                                                  BruttoNpos = 0,
                                                  BruttoFiskalne = f.Brutto,
                                                  BruttoFiskalneNpos = f.Brutto,
                                                  Konwersja=0,
                                                  SapBrutto = 0,
                                                  Data = data
                                              }).ToList();

                resultAll.AddRange(resultoRACLE);
                resultAll.AddRange(resultFiskal);
                resultSummary = (from l in resultAll
                                 group l by l.NumerSklep into Sklep
                                 select new SummaryShop
                                 {
                                     NumerSklep = Sklep.Key,
                                     ShopName=Sklep.FirstOrDefault().ShopName,
                                     BruttoFiskalne = Sklep.Sum(j => j.BruttoFiskalne),
                                     Data = classFun.Data_MMDDYYYY(Sklep.First().Data),
                                     BruttoNpos = Sklep.Sum(j => j.BruttoNpos),
                                     BruttoFiskalneNpos = Sklep.Sum(j => j.BruttoFiskalneNpos),
                                     Status = GetStatus(Sklep.Sum(j => j.BruttoFiskalne), Sklep.Sum(j => j.BruttoNpos)),
                                     Karta = Sklep.Sum(j => j.Karta),
                                     Terminal = Sklep.Sum(j => j.Terminal),
                                     KartaTerminal = Sklep.Sum(j => j.KartaTerminal),
                                     Konwersja=Sklep.Sum(j=>j.Konwersja),
                                     StatusKarta = GetStatusKarta(Sklep.Sum(j => j.Karta), Sklep.Sum(j => j.Terminal)),
                                     Drukarka = (from d in Sklep
                                                 select new Summary
                                                 {
                                                     NumerSklep = d.NumerSklep,
                                                     Stanowisko = d.Stanowisko,
                                                     NumerRaport = d.NumerRaport,
                                                     BruttoNpos = d.BruttoNpos,
                                                     BruttoFiskalne = d.BruttoFiskalne,
                                                     BruttoFiskalneNpos = d.BruttoFiskalneNpos,
                                                     SapBrutto = d.SapBrutto,
                                                     Status = GetStatus(d.BruttoFiskalne, d.BruttoNpos),
                                                     Data = classFun.Data_MMDDYYYY(d.Data),
                                                     Karta = d.Karta,
                                                     Terminal = d.Terminal,
                                                     Konwersja=d.Konwersja,
                                                     KartaTerminal = d.Terminal - d.Karta+d.Konwersja,
                                                     StatusKarta = GetStatusKarta(d.Karta, d.Terminal)


                                                 }).OrderBy(j => j.Stanowisko).ToList()


                                 }).OrderBy(s => s.NumerSklep).ToList();


            }
            catch (Exception ex)
            {

                classLog.LogException(ex);
            }

            return resultSummary;
        }

        private static string GetStatusKarta(decimal karta, decimal terminal)
        {
            if (karta != terminal)
                return "Red";
            else
                return "";


        }

        private static List<Sold> OracleFile(string oracleFile)
        {
            List<Sold> result = new List<Sold>();
            try
            {
                string ss = File.ReadAllText(oracleFile);
                if (ss.Length<2)
                {
                    classLog.LogError("Brak danych w " + oracleFile);
                    return result;
                }
                var linia = File.ReadAllLines(oracleFile).Skip(1);
                foreach (var item in linia)
                {

                    string[] kolumn = item.Split(';');
                    Sold s = new Sold();
                    s.NumerSklep = kolumn[1];
                    s.Stanowisko = kolumn[2];
                    s.Brutto = GetDecimal(kolumn[3]);
                    s.Karta = GetDecimal(kolumn[4]);
                    s.Terminal = GetDecimal(kolumn[5]);
                    s.ShopName = kolumn[6];
                    s.Konwersja = GetDecimal(kolumn[7]);

                    result.Add(s);
                }
            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
                throw;
            }
            return result;

        }

        public static List<SummaryShop> SummaryCVS(string path, string user, string password)
        {
            try
            {

                ClassImpesonality zab = new ClassImpesonality(user, password);
                if (zab.impersonateValidUser())
                {

                    string dataFilter = DateTime.Now.AddDays(-1).ToString(classConst.formatDate);



                    var records = GetLineSummary(path);

                    var Sklepy = (from l in records
                                  group l by l.NumerSklep into Sklep
                                  select new SummaryShop
                                  {
                                      NumerSklep = Sklep.Key,
                                      BruttoFiskalne = Sklep.Sum(j => j.BruttoFiskalne),
                                      Data = Sklep.First().Data,
                                      BruttoNpos = Sklep.Sum(j => j.BruttoNpos),
                                      BruttoFiskalneNpos = Sklep.Sum(j => j.BruttoFiskalneNpos),
                                      Status = GetStatus(Sklep.Sum(j => j.BruttoFiskalne), Sklep.Sum(j => j.BruttoNpos)),
                                      Drukarka = (from d in Sklep
                                                  select new Summary
                                                  {
                                                      NumerSklep = d.NumerSklep,
                                                      Stanowisko = d.Stanowisko,
                                                      NumerRaport = d.NumerRaport,
                                                      BruttoNpos = d.BruttoNpos,
                                                      BruttoFiskalne = d.BruttoFiskalne,
                                                      BruttoFiskalneNpos = d.BruttoFiskalneNpos,
                                                      SapBrutto = d.SapBrutto,
                                                      Status = GetStatus(d.BruttoFiskalne, d.BruttoNpos),
                                                      Data = d.Data
                                                  }).ToList()


                                  }).ToList();
                    classLog.LogError(Sklepy.Count.ToString());
                    zab.undoImpersonation();
                    return Sklepy;


                }
                else
                {

                    return null;
                }
            }
            catch (Exception ex)
            {

                return null;
            }


        }

        private static Status GetStatus(decimal fiskalne, decimal npos)
        {
            if (fiskalne == 0 && npos == 0)
            {
                return Status.Gray;
            }
            if (fiskalne == npos)
            {
                return Status.Green;
            }
            if (fiskalne == 0)
            {
                return Status.Red;
            }

            if (fiskalne != npos)
            {
                return Status.Blue;
            }
            return Status.Black;


        }

        public static void SerializeToXml<T>(T anyobject, string fileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(anyobject.GetType());
            /*
             *  XmlSerializer ser = new XmlSerializer(typeof(List<Foo>),
             new XmlRootAttribute("Flibble"));
             */
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                xmlSerializer.Serialize(writer, anyobject);
            }
        }
    }


}
