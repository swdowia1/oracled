using AnalizaDll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AnalizaAppWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<SummaryShop> Lista { get; set; }
        public bool IsLoad { get; set; }

        public List<SummaryShop> ListaFilter { get; set; }
        public List<string> ListaSklepow { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            IsLoad = false;

            this.MinWidth = 1200;
            txtLoad.Visibility = Visibility.Visible;
            dgSold.Visibility = Visibility.Hidden;
#if DEBUG
            this.Background = Brushes.LightGreen;
#endif
            var pom = classFun.GetStatFile(classConfig.StatystykaKatalog);
            if (pom.Count==0)
            {
                classWinUI.MessageError("Brak plik " + classConfig.StatystykaKatalog);
                return;
            }
            cbData.ItemsSource = pom.Select(k => new DataShow(k)).Take(classConfig.LastDay).ToList();

            this.WindowState = WindowState.Maximized;
            classLog.LogInfo("====================================================================");
            classLog.LogInfo("Odczyt z  katalogu " + classConfig.StatystykaKatalog);
            if (File.Exists(classConst.ShopFile))
            {
                rbSelect.IsChecked = true;

                ListaSklepow = File.ReadAllLines(classConst.ShopFile).ToList();

            }
            else
            {
                ListaSklepow = new List<string>();
                rbAll.IsChecked = true;
                RbEnable(false);


            }




        }

        private void RbEnable(bool state)
        {
            rbSelect.IsEnabled = state;
            rbAll.IsEnabled = state;
        }

        private void cbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string data = (cbData.SelectedValue as DataShow).Data;
            chkCC.IsChecked = true;
            chkHalf.IsChecked = true;
            GetData(data);
            if (rbSelect.IsChecked == true && ListaSklepow.Count > 0)
            {
                ListaFilter = ListaFilter.Where(k => ListaSklepow.Contains(k.NumerSklep)).ToList();
            }
            dgSold.ItemsSource = ListaFilter;

        }
        private void chkNotZero_Click(object sender, RoutedEventArgs e)
        {
            Filtruj();
        }

        private void Filtruj()
        {
            string szuka = txtFilter.Text;
            ListaFilter = Lista.Where(k => k.NumerSklep.Contains(szuka)).ToList();
            if (chkCC.IsChecked == false && chkHalf.IsChecked == false)
            {
                ListaFilter = new List<SummaryShop>();
            }

            if (chkNotZero.IsChecked == true)
            {
                ListaFilter = ListaFilter.Where(k => k.BruttoFiskalneNpos != 0).ToList();
            }
            if (chkNotSold.IsChecked == true)
            {
                ListaFilter = ListaFilter.Where(k => k.Status == Status.Gray).ToList();
            }
            if (chkCC.IsChecked == false || chkHalf.IsChecked == false)
            {
                if (chkCC.IsChecked == true && chkHalf.IsChecked == false)
                {
                    ListaFilter = ListaFilter.Where(k => k.ShopName.StartsWith("Half") == false).ToList();
                }
                if (chkCC.IsChecked == false && chkHalf.IsChecked == true)
                {
                    ListaFilter = ListaFilter.Where(k => k.ShopName.StartsWith("Half")).ToList();
                }
            }
           
            if (rbSelect.IsChecked == true && ListaSklepow.Count > 0)
            {
                ListaFilter = ListaFilter.Where(k => ListaSklepow.Contains(k.NumerSklep)).ToList();
            }
            dgSold.ItemsSource = ListaFilter;

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string data = (cbData.SelectedValue as DataShow).Data;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = path + "\\" + string.Format(classConfig.SaveXLS, data);
            classExcel.CreateExcel(ListaFilter, fileName);
            if (classWinUI.MessageQuestion("Wygenerowano plik " + fileName + "\nCzy otworzyć") == false)
            {
                System.Diagnostics.Process.Start(fileName);
            }
        }

        private void txtFilter_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Filtruj();
        }

        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            dgSold.SelectedIndex = -1;
        }

        private void btnHideRow_Click(object sender, RoutedEventArgs e)
        {
            dgSold.SelectedIndex = -1;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //  string data = DateTime.Now.AddDays(-1).ToString(classConst.formatDate);
            if (cbData.SelectedValue==null)
            {
                return;
            }
            string data = (cbData.SelectedValue as DataShow).Data;
            IsLoad = true;
            chkCC.IsChecked = true;
            chkHalf.IsChecked = true;
            GetData(data);
            if (rbSelect.IsChecked == true && ListaSklepow.Count > 0)
            {
                ListaFilter = ListaFilter.Where(k => ListaSklepow.Contains(k.NumerSklep)).ToList();
                dgSold.ItemsSource = ListaFilter;
            }
            txtLoad.Visibility = Visibility.Hidden;
            dgSold.Visibility = Visibility.Visible;
        }

        private void GetData(string data)
        {
            txtLoad.Visibility = Visibility.Visible;
            dgSold.Visibility = Visibility.Hidden;
            if (IsLoad == false)
            {
                return;
            }
            txtFilter.Text = "";
            chkNotZero.IsChecked = false;
            this.Title = "Dane z dnia: " + data + "         [wersja:" + Assembly.GetEntryAssembly().GetName().Version.ToString() + "]";
#if DEBUG
            this.Title += "      debug " + classConfig.StatystykaKatalog;
#endif
            Lista = classFun.SummaryAll(classConfig.StatystykaKatalog, data);
            CountShow();
            
            ListaFilter = Lista;
            dgSold.ItemsSource = Lista;

            txtLoad.Visibility = Visibility.Hidden;
           dgSold.Visibility = Visibility.Visible;
        }

        private void CountShow()
        {
            if (chkCC.IsChecked == true && chkHalf.IsChecked == true)
                lblIlosc.Content = "PL: " + Lista.Count;
            else
            {
                if (chkCC.IsChecked==true)
                {
                    //k.ShopName.StartsWith("Half") 
                    lblIlosc.Content = "PL: " + Lista.Where(k => !k.ShopName.StartsWith("Half")).Count();
                }
                if (chkHalf.IsChecked == true)
                {
                    //k.ShopName.StartsWith("Half") 
                    lblIlosc.Content = "PL: " + Lista.Where(k => k.ShopName.StartsWith("Half")).Count();
                }
                if (chkCC.IsChecked == false && chkHalf.IsChecked == false)
                    lblIlosc.Content = "PL: 0";

            }



        }

        private void chkShop_Click(object sender, RoutedEventArgs e)
        {
            Filtruj();
        }

        private void btnFilterShop_Click(object sender, RoutedEventArgs e)
        {
            ShopSelect g = new ShopSelect();

            var tt = g.ShowDialog();
            if (tt == true)
            {
                ListaSklepow = File.ReadAllLines(classConst.ShopFile).ToList();
                RbEnable(true);
                rbSelect.IsChecked = true;
                if (ListaSklepow.Count > 0)
                    ListaFilter = Lista.Where(k => ListaSklepow.Contains(k.NumerSklep)).ToList();
                else
                    ListaFilter = Lista;
                dgSold.ItemsSource = ListaFilter;
            }



        }

        private void rbAll_Click(object sender, RoutedEventArgs e)
        {
            Filtruj();
        }

        private void rbSelect_Click(object sender, RoutedEventArgs e)
        {
            Filtruj();
        }

        private void chkCC_Click(object sender, RoutedEventArgs e)
        {
            Filtruj();
            CountShow();
        }

        private void chkHalf_Click(object sender, RoutedEventArgs e)
        {
            Filtruj();
            CountShow();
        }

        private void chkNotSold_Click(object sender, RoutedEventArgs e)
        {
            Filtruj();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Title = e.NewSize.Width.ToString();
            File.WriteAllText("aa.txt", e.NewSize.Width.ToString());
        }

        private void btnMiesiac_Click(object sender, RoutedEventArgs e)
        {
            Miesiac p = new Miesiac();
            p.ShowDialog();
        }
    }
}
