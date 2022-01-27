using AnalizaDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaAppWpf
{
    public class classWinUI
    {
        public static void MessageInfo(string body, string title = "Informacja")
        {
            System.Windows.Forms.MessageBox.Show(body, title, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
        public static void MessageError(string body, string title = "Error")
        {
            classLog.LogError(body);
            System.Windows.Forms.MessageBox.Show(body, title, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }
        public static bool MessageQuestion(string body, string title = "Pytanie")
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(body, title, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (dialogResult == (System.Windows.Forms.DialogResult.OK))
            {
                return false;
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                return true;
            }
            return false;
        }
    }
}
