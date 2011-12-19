using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace D3DDetour
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Pulse.Initialize(D3DVersion.Direct3D9);
            D3DHook.OnFrame += new EventHandler(OnFrame);            

            Application.EnableVisualStyles();
            Application.Run(new Form1());
        }

        static void OnFrame(object sender, EventArgs e)
        {
            Console.WriteLine("Pulse!");
        }
    }
}
