using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionClotureFrais
{
    public abstract class GestionDate
    {

        /* Retourne le mois précédent
         * @return string mois le précédent
         */
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

        /* Retourne le mois précédent
         * @return string mois le précédent
         * @param mois
         */
        public static void getMoisPrecedent(String mois)
        {
            int moisPrecedent;
            int.TryParse(mois, out moisPrecedent);
            mois = (moisPrecedent <= 10) ? "0" : "" + (moisPrecedent - 1);
        }

        /* Retourne le mois suivant
         * @return string mois le suivant
         */
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

        /* Retourne le mois suivant
         * @return string mois le suivant
         * @param mois
         */
        public static void getMoisSuivant(String mois)
        {
            int moisSuivant;
            int.TryParse(mois, out moisSuivant);
            mois = (moisSuivant <= 10) ? "0" : "" + (moisSuivant + 1);
        }

        /* Vérifie si le jour d'aujourd'hui est compris entre deux jours
         * @return boolean
         */
        public static Boolean entre(int jour1, int jour2)
        {
            return (jour1 <= DateTime.Now.Day && DateTime.Now.Day <= jour2) ? true : false;
        }

        /* Vérifie si le jour d'aujourd'hui est compris entre deux jours
         * @return boolean
         */
        public static Boolean entre(int jour1, int jour2, int jour3)
        {
            return (jour1 <= jour3 && jour3 <= jour2) ? true : false;
        }
    }
}
