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
    /// Interaction logic for HashForm.xaml
    /// </summary>
    public partial class HashForm : Window
    {
        public HashForm()
        {
            InitializeComponent();
        }
        public string Answer
        {
            get { return txtPaswordHash.Text; }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtPasword_KeyUp(object sender, KeyEventArgs e)
        {
            txtPaswordHash.Text = Encrypt.SzyfrujHaslo(txtPasword.Text);
        }
    }
}
