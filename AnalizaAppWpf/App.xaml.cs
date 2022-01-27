using AnalizaDll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace AnalizaAppWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool checkGroup(string group, WindowsIdentity identity)
        {
           //
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(group);
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            string programName = "NposCheck";
            //WindowsIdentity identity = WindowsIdentity.GetCurrent();
            //if (checkGroup(classConfig.GrupaOdczyt,identity)==false)
            //{
            //    classWinUI.MessageError("Użytkownik "+identity.Name+" nie należy do grupy "+classConfig.GrupaOdczyt);
               
            //    System.Windows.Application.Current.Shutdown();
            //    return;
            //}
            var listProcess = Process.GetProcessesByName(programName).ToList();
            if (Process.GetProcessesByName(programName).Length > 1)
            {
                var proces = listProcess.First(k => k.MainWindowTitle != "");

                //classWinUI.MessageError("Aplikacja " + programName + "[id:" + proces.Id + "] jest już uruchomiona");
                classWinUI.MessageError("Aplikacja jest już uruchomiona".ToUpper());
                classLog.LogError("Aplikacja jest uruchomiona id: " + proces.Id.ToString());
                System.Windows.Application.Current.Shutdown();
                    return;
               
                
            }

            if (e.Args.Any())
            {
                Konfiguracja wKonf = new Konfiguracja();
                wKonf.Show();
            }
            else
            {
                MainWindow wnd = new MainWindow();
               
                wnd.Show();
            }
        }
    }
}
