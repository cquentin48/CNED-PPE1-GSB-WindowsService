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

/// <summary>
/// Service Windows de gestion de cloture de frais
/// </summary>
namespace PPEGSBGestionFraisService
{
    /// <summary>
    /// Classe gérant le service windows
    /// </summary>
    public partial class PPEGSBApplicationFraisService : ServiceBase
    {
        //private System.ComponentModel.IContainer components;
        private System.Timers.Timer _timer;
        private DateTime _scheduleTime;
        private string operationType;

        //Les jours par défault de clôture et de validation
        private int jourCloture = 1;
        private int jourValidation = 20;

        //Variable pour les évenements dans le journal
        private System.Diagnostics.EventLog eventLog;

        /// <summary>
        /// Constructeur du service windows
        /// </summary>
        /// <param name="args">Non utilisé</param>
        public PPEGSBApplicationFraisService(string[] args)
        {
            //Initialisation des composant
            InitializeComponent();
            //Initialisation du timer
            _timer = new System.Timers.Timer();
            
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
            
            
            _scheduleTime = DateTime.Today.AddDays(1).AddHours(23);
        }

        /// <summary>
        /// Fonction de début de services
        /// </summary>
        /// <param name="args">Non utilisé ici</param>
        protected override void OnStart(string[] args)
        {
            _timer.Enabled = true;
            _timer.Interval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            MySQLDatabase db = new MySQLDatabase(eventLog);

            if (DateTime.Now.Day <= 10 && this.operationType == "cloture")
            {
                //On clôture les fiches
                eventLog.WriteEntry("Nous somme le "+DateTime.Now.Day+" : Clôturation de fiche");
                db.clotureFiches();
                eventLog.WriteEntry("Fiches clôturées");
                this.operationType = "validation";
            }
            else if (DateTime.Now.Day <= 20 && this.operationType == "validation")
            {
                //On valide les fiches
                eventLog.WriteEntry("Nous somme le " + DateTime.Now.Day + " : Validation de fiche");
                db.validationFiches();
                this.operationType = "validation";
                eventLog.WriteEntry("Fiches validées");
            }
            eventLog.WriteEntry("Fin du service");
        }


        /// <summary>
        /// Fonction d'écoulement de temps entre deux utilisations du service
        /// </summary>
        /// <param name="sender">Non utilisé</param>
        /// <param name="e">Non utilisé</param>
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

        /// <summary>
        /// Fonction d'arrêt du service
        /// </summary>
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

        /// <summary>
        /// Fonction de premier lancement || NON UTILISE ICI
        /// </summary>
        public void firstTime() {
        }
    }
}
