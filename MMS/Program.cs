using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MMS.View;

namespace MMS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());

            FormView view = new FormView();
            MMS.Model.Model model = new Model.Model();
            MMS.Controller.Controller controller = new MMS.Controller.Controller(model, view);

            Application.Run(view);
        }
    }
}
