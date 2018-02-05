using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using GestionClotureFrais;

namespace PPEGSBGestionFraisService
{
    public partial class PPEGSBApplicationFraisService : ServiceBase
    {
        //private System.ComponentModel.IContainer components;
        private System.Timers.Timer _timer;
        private DateTime _scheduleTime;
        private string operationType;

        //Les jours par défault de clôture et de validation
        private int jourCloture = 5;
        private int jourValidation = 20;

        /**
         * Constructeur du service windows
         */
        public PPEGSBApplicationFraisService(string[] args)
        {
            InitializeComponent();
            _timer = new System.Timers.Timer();
            string jourClotureVarWindows = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("jourClotureGSBFraisService"))?this.jourCloture.ToString():Environment.GetEnvironmentVariable("jourClotureGSBFraisService");
            string jourValidationVarWindows = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("jourValidationGSBFraisService")) ? this.jourValidation.ToString() : Environment.GetEnvironmentVariable("jourValidationGSBFraisService");
            
            //On récupère le type d'opération
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("typeOperationGSBFraisService")))
            {
                //Clôture
                if (DateTime.Now.Day <= 10)
                {
                    this.operationType = "cloture";
                }
                //Validation
                else if (DateTime.Now.Day >= 20)
                {
                    this.operationType = "validation";
                }
            }

            //Si aucune valeur n'a été définie dans les variables d'environement windows, on prends les valeurs par défault
            this.jourCloture = String.IsNullOrEmpty(jourClotureVarWindows)? jourCloture:-1;
            this.jourValidation = String.IsNullOrEmpty(jourClotureVarWindows) ? jourCloture : -1;

            int.TryParse(Environment.GetEnvironmentVariable("jourClotureGSBFraisService"), out this.jourCloture);
            int.TryParse(Environment.GetEnvironmentVariable("jourValidationGSBFraisService"), out this.jourValidation);
            
            
            _scheduleTime = DateTime.Today.AddDays(1).AddHours(23);
        }

        //Fonction de début du service
        protected override void OnStart(string[] args)
        {
            _timer.Enabled = true;
            _timer.Interval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);

            if (DateTime.Now.Day <= 10 && this.operationType == "cloture")
            {
                //On clôture les fiches
                MysqlDatabase db = new MysqlDatabase();
                db.clotureFiches();
                this.operationType = "validation";
            }
            else if (DateTime.Now.Day <= 20 && this.operationType == "validation")
            {
                //On valide les fiches
                MysqlDatabase db = new MysqlDatabase();
                db.validationFiches();
                this.operationType = "validation";
            }
        }

        protected void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //On récupère le nombre de jours dans le mois à partir du jour de clôture jusqu'au jour de la prochaine opération
            int annee = DateTime.Now.Year;
            int mois = DateTime.Now.Month;
            int intervale = 0;
            if(this.operationType == "validation") {
                intervale = DateTime.DaysInMonth(annee, mois) - this.jourValidation + this.jourValidation;
            }else {
                intervale = this.jourValidation - this.jourCloture;
            }

            // 2. If tick for the first time, reset next specific day
            if (_timer.Interval != intervale * 24 * 60 * 60 * 1000)
            {
                _timer.Interval = intervale * 24 * 60 * 60 * 1000;
            }
        }

        //Fonction d'arrêt du service
        protected override void OnStop()
        {
            //On écrit dans l'editeur de registre les variables d'environnement si elles n'existent pas
            if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("jourClotureGSBFraisService"))) {
                Environment.SetEnvironmentVariable("jourClotureGSBFraisService", this.jourCloture.ToString());
            }
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("jourValidationGSBFraisService")))
            {
                Environment.SetEnvironmentVariable("jourValidationGSBFraisService", this.jourValidation.ToString());
            }

            //On met à jour la variable d'environnement concernant le type d'opération à réaliser
            Environment.SetEnvironmentVariable("operationType", this.operationType);
        }

        public void firstTime() {
            Console.WriteLine("Premier lancement du service de gestion de frais PDO GSB.");
            Console.WriteLine("Merci de bien vouloir indiquer l'heure à laquelle le service va opérer.");
            string lineEntered = Console.ReadLine();
            int result;
            Boolean possible = int.TryParse(lineEntered, out result);
            if (possible == true && result>=0 && result<=23) {
                Environment.SetEnvironmentVariable("PDOGSBHOURREFRESH", result.ToString());
                if(DateTime.Now.Day < 20){ 
                    Environment.SetEnvironmentVariable("PDOGSBOPERATIONTYPE", "cloture");
                }else {
                    Environment.SetEnvironmentVariable("PDOGSBOPERATIONTYPE", "validation");
                }
            }
        }
    }
}
