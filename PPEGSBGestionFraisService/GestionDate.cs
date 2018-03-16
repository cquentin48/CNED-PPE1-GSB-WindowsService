using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionClotureFrais
{
    /// <summary>
    /// Classe abstraite de gestion des dates
    /// </summary>
    public abstract class GestionDate
    {
        /// <summary>
        /// Retourne le mois précédent
        /// </summary>
        /// <returns>Le mois précédent sous la forme MM</returns>
        public static String getMoisPrecedent()
        {
            string mois = DateTime.Now.ToString("MM");
            string annee = DateTime.Now.Year.ToString();
            //Récupération du mois précédent
            int moisPrecedent;
            int.TryParse(mois, out moisPrecedent);
            if(mois == "01") {
                moisPrecedent = 12;
            }else {
                int.TryParse(mois, out moisPrecedent);
                moisPrecedent--;
            }
            mois = (moisPrecedent < 10) ? "0"+moisPrecedent+annee : "" + moisPrecedent + annee;
            return mois;
        }

        /// <summary>
        /// Retourne le mois précédent
        /// </summary>
        /// <param name="mois">Le mois</param>
        /// <returns>Le mois précédent au format MM</returns>
        public static String getMoisPrecedent(String mois)
        {
            int moisPrecedent;
            int.TryParse(mois, out moisPrecedent);
            return (moisPrecedent <= 10) ? "0" : "" + (moisPrecedent - 1);
        }

        /// <summary>Retourne le mois suivant</summary>
        /// <returns>Le mois suivant</returns>
        public static String getMoisSuivant()
        {
            string mois = DateTime.Now.ToString("MM");
            //Récupération du mois suivant
            int moisSuivant;
            int.TryParse(mois, out moisSuivant);
            moisSuivant = (moisSuivant + 1) % 12;
            mois = (moisSuivant <= 10) ? "0" : "" + moisSuivant;
            return mois;
        }

        /// <summary>Retourne le mois suivant</summary>
        /// <param name="moisEnCours"> un mois</param>
        /// <returns>Le mois suivant</returns>
        public static String getMoisSuivant(String moisEnCours)
        {
            string mois = moisEnCours;
            int moisSuivant;
            int.TryParse(mois, out moisSuivant);
            mois = (moisSuivant <= 10) ? "0" : "" + moisSuivant;
            return mois;
        }

        /// <summary>
        /// Vérifie si le jour d'aujourd'hui est compris entre deux jours
        /// </summary>
        /// <param name="jour1">Un jour avant</param>
        /// <param name="jour2">Un jour après</param>
        /// <returns>Booléen : vrai si le jour d'ajourd'hui se trouve entre les deux jours, faux sinon</returns>
        public static Boolean entre(int jour1, int jour2)
        {
            return (jour1 <= DateTime.Now.Day && DateTime.Now.Day <= jour2) ? true : false;
        }

        /// <summary>
        /// Vérifie si le troisième jour passé en paramètre est compris entre deux jours
        /// </summary>
        /// <param name="jour1">Un jour avant</param>
        /// <param name="jour2">Un jour après</param>
        /// <param name="jour3">Un jour passé en paramètre pour vérifier si ce jour se trouve entre les deux premiers</param>
        /// <returns>Booléen : vrai si le jour d'ajourd'hui se trouve entre les deux jours, faux sinon</returns>
        public static Boolean entre(int jour1, int jour2, int jour3)
        {
            return (jour1 <= jour3 && jour3 <= jour2) ? true : false;
        }
    }
}
