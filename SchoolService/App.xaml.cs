using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "ZaporizhiaAcademicLyceum-SchoolService", out createdNew))
            {
                if (createdNew)
                {
                    SchoolService.App app = new SchoolService.App();
                    app.InitializeComponent();
                    app.Run();
                }
                else
                {
                    return;
                }
            }
        }
    }
}
