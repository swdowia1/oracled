using AnalizaDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AnalizaAppWpf
{
    /// <summary>
    /// Interaction logic for Miesiac.xaml
    /// </summary>
    public partial class Miesiac : Window
    {
        private List<MonthSklep> raportMiesiac;
        private List<MonthSklep> raportMiesiacFiltr;
        string monthPrev = "";
        public Miesiac()
        {
            InitializeComponent();
            DateTime datastart = DateTime.Now.AddMonths(-1);
            monthPrev = datastart.ToString(classConst.formatYearMonth);
            this.Title = "Podsumowanie za " + datastart.ToString("MMMM yyyy");
            lblTitle.Content= "Podsumowanie za " + datastart.ToString("MMMM yyyy");
            raportMiesiac = classFun.RaportMiesiac(monthPrev,classConfig.RaportMiesiac);
            raportMiesiacFiltr = raportMiesiac;
            dgMonth.ItemsSource = raportMiesiac;
          
            
        }

        private void btnHideRow_Click(object sender, RoutedEventArgs e)
        {
            dgMonth.SelectedIndex = -1;
        }

        private void txtFilter_KeyUp(object sender, KeyEventArgs e)
        {
            Filtruj();
        }
        private void Filtruj()
        {
            string szuka = txtFilter.Text;
            raportMiesiacFiltr = raportMiesiac.Where(k => k.NumerSklep.Contains(szuka)).ToList();
            
            dgMonth.ItemsSource = raportMiesiacFiltr;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string fileName = path + "\\miesiac_"+monthPrev+".xlsx";
            classExcel.CreateExcel(raportMiesiac, fileName);
            if (classWinUI.MessageQuestion("Wygenerowano plik " + fileName + "\nCzy otworzyć") == false)
            {
                System.Diagnostics.Process.Start(fileName);
            }
        }
    }
}
