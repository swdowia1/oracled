using AnalizaDll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for Konfiguracja.xaml
    /// </summary>
    public partial class Konfiguracja : Window
    {
        public Konfiguracja()
        {
            InitializeComponent();
            this.Title = "Konfiruracja aplikacji         [wersja:" + Assembly.GetEntryAssembly().GetName().Version.ToString() + "]";


            txtSaveXLS.Text = classConfig.SaveXLS;
            txtStatystykaFile.Text = classConfig.StatystykaKatalog;
          

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                classFun.UpdateAppSettings("SaveXLS", txtSaveXLS.Text);
                classFun.UpdateAppSettings("StatystykaKatalog", txtStatystykaFile.Text);
              
                classWinUI.MessageInfo("zapiso w pliku AnalizaAppWpf.exe.config");
            }
            catch (Exception ex)
            {
                classWinUI.MessageError(ex.Message);
                classLog.LogException(ex);
            }
        }

      
        

       
    }
}
