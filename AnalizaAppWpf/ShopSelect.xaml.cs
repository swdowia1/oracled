using AnalizaDll;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ShopSelect.xaml
    /// </summary>
    public partial class ShopSelect : Window
    {
        private List<DataSelect> listaSelect;
        private List<DataSelect> listaOracle;
        public ShopSelect()
        {
            InitializeComponent();
            try
            {
                if (File.Exists(classConst.ShopFile))
                    listaSelect = File.ReadAllLines(classConst.ShopFile).Select(k => new DataSelect() { Data = k, Status = 0 }).ToList();
                else
                    listaSelect = new List<DataSelect>();
                listaSelect = listaSelect.OrderBy(k => k.Data).ToList();
                lbSelect.ItemsSource = listaSelect;

                string data = DateTime.Now.AddDays(-1).ToString(classConst.formatDate);

                listaOracle = GetData();
                listaOracle = listaOracle.Except(listaSelect).ToList();
                listaOracle = listaOracle.OrderBy(k => k.Data).ToList();
                lbOracle.ItemsSource = listaOracle;

                TitleLabel(listaOracle, listaSelect);


            }
            catch (Exception ex)
            {

                classLog.LogException(ex);
            }
        }

        private void TitleLabel(List<DataSelect> listaOracle, List<DataSelect> listaSelect)
        {

            lbNoSelect.Content= "Lista salonów nie wybranych ("+listaOracle.Count+")";
            lblYesSelect.Content = "Lista salonów  wybranych (" + listaSelect.Count + ")";
        }

        private List<DataSelect> GetData()
        {
            List<DataSelect> result = new List<DataSelect>();
            try
            {
                string[] pliki = Directory.GetFiles(classConfig.StatystykaKatalog, "statystykaOracle_*");
                List<string> pom= new List<string>();
                foreach (var item in pliki)
                {
                    pom.AddRange(File.ReadAllLines(item).Skip(1)
                        .Select(g => g.Substring(9, 4)).Distinct().ToList());
                }
                result=pom.Distinct().Select(j => new DataSelect() { Data = j, Status = 0 }).ToList();

            }
            catch (Exception ex)
            {
                classLog.LogException(ex);
            }
            return result;
            }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in listaSelect)
            {
                sb.AppendLine(item.Data);
            }
            File.WriteAllText(classConst.ShopFile, sb.ToString());
            this.DialogResult = true;
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            Label lb = sender as Label;
            DataSelect dt = lb.DataContext as DataSelect;
            AddList(dt);
        }

        private void AddList(DataSelect dt)
        {
            if (dt is null)
            {
                return;
            }
            lbOracle.ItemsSource = null;
            lbSelect.ItemsSource = null;
            listaOracle.RemoveAll(s => s.Data == dt.Data);
            if (dt.Status == 1)
            {
                dt.Status = 0;
            }
            else
                dt.Status = 1;
            listaSelect.Add(dt);
            SortList();
            lbOracle.ItemsSource = listaOracle;

            lbSelect.ItemsSource = listaSelect;
            TitleLabel(listaOracle, listaSelect);
        }



        private void SortList()
        {
            listaOracle = listaOracle.OrderBy(k => k.Data).ToList();
            listaSelect = listaSelect.OrderBy(k => k.Data).ToList();

        }




        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRenove_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Test");
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {


            Label lb = sender as Label;
            DataSelect dt = lb.DataContext as DataSelect;
            RemoveList(dt);
        }

        private void RemoveList(DataSelect dt)
        {
            if (dt is null)
            {
                return;
            }
            lbOracle.ItemsSource = null;
            lbSelect.ItemsSource = null;
            listaSelect.RemoveAll(s => s.Data == dt.Data);
            if (dt.Status == 1)
            {
                dt.Status = 0;
            }
            else
                dt.Status = 1;
            listaOracle.Add(dt);
            SortList();
            lbOracle.ItemsSource = listaOracle;

            lbSelect.ItemsSource = listaSelect;
            TitleLabel(listaOracle, listaSelect);
        }
    }
}
