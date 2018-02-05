using PPEGSBGestionFraisService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Configuration;

namespace GestionClotureFrais
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] { new PPEGSBApplicationFraisService(args) };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
