using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionClotureFrais
{
    public class FicheMois
    {
        private string idVisiteur;
        private string mois;
        private string idEtat;

        public void setIdEtat(string newIdEtat) {
            if (newIdEtat == "CL" || newIdEtat == "CR" || newIdEtat == "RB" || newIdEtat == "VA")
            {
                this.idEtat = newIdEtat;
            }
        }

        public FicheMois(string idVisiteur, string mois, string idEtat)
        {
            this.idVisiteur = idVisiteur;
            this.mois = mois;
            this.setIdEtat(idEtat);
        }

        public string getIdVisiteur() { return this.idVisiteur; }

        public string getMois() { return this.mois; }
    }
}
