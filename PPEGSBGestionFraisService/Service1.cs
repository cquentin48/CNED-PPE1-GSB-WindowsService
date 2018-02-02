using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PPEGSBGestionFraisService
{
    public partial class PPEGSBApplicationFraisService : ServiceBase
    {
        //private System.ComponentModel.IContainer components;
        private System.Diagnostics.EventLog eventLog1;
        private string mois = "";
        private static OperatingSystem OS;

        //
        public PPEGSBApplicationFraisService()
        {
            //Recherche quel Système d'exploitation
            String operatingSystem = OS.Version.ToString();
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            string cloture = Environment.GetEnvironmentVariable("cloture");
            string validation = Environment.GetEnvironmentVariable("validation");

            //Jour de clôture
            if (DateTime.Now.Day == 1 && cloture == "pas ok" && validation == "ok")
            {
                Environment.SetEnvironmentVariable("cloture", "Ok");
                Environment.SetEnvironmentVariable("validation", "Pas Ok");
                if (!System.Diagnostics.EventLog.SourceExists("Clôture du mois de "+mois))
                {
                    System.Diagnostics.EventLog.CreateEventSource(
                        "Clôture du mois de " + mois, "Début de clôture des fiches");
                        //Insérer ici la fonction de clôture des fiches de mois
                }
            }else if(DateTime.Now.Day == 2 && cloture == "ok" && validation == "pas ok")
            {
                Environment.SetEnvironmentVariable("validation", "Ok");
                Environment.SetEnvironmentVariable("cloture", "Pas Ok");

                //Insérer ici la fonction de validation des fiches de moi

            }
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Début de la clôtures de fiches de frais du mois de " + mois);
        }

        protected override void OnStop()
        {
        }

        private void popupResult(int returnType, string message){
            
        }

        private void resultNotification(int returnType, string message){

        }
    }
}
