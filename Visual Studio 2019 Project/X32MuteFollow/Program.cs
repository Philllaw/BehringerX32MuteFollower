using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static X32MuteFollow.SaveLoad;

namespace X32MuteFollow
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ProgramSettings settings = new ProgramSettings();
            SaveLoad.loadFromFile(settings);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GUI gui = new GUI(settings);
            Controller controller = new Controller(gui, settings);
            controller.Start();
            SaveLoad.saveToFile(settings);
            Application.Run(gui);
        }
    }
}
