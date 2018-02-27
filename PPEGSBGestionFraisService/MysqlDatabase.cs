using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.IO;
using System.ServiceProcess;

namespace GestionClotureFrais
{
    public class MySQLDatabase : ServiceBase
    {

        //Objet de connection à la base
        private MySqlConnection connection;
        private string server;
        private string database;
        private string usernameDatabase;
        private string password;

        //Variable pour les évenements dans le journal
        private System.Diagnostics.EventLog eventLog;

        //Constructeur
        public MySQLDatabase(System.Diagnostics.EventLog eventLog)
        {
            this.eventLog = eventLog;
            this.initConnection();
        }

        /**
         * Initialisation de la connection au serveur mysql
         */
        private void initConnection()
        {
            server = "localhost";
            database = "gsb_frais";
            usernameDatabase = "root";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + usernameDatabase + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
            eventLog.WriteEntry(this.setEntitleEventLog() + " || Opération MYSQL || " +
                                "Tentative de connection à la base de donnée avec la requête Suivante :" + connectionString);
        }

        /**
         * Cloture des fiches du mois précédent lorsque nous sommes entre le 1er et le 10 du mois
         */
        public void clotureFiches()
        {
            if (GestionDate.entre(1, 10))
            {
                eventLog.WriteEntry(this.setEntitleEventLog() +
                                    "Nous sommes le " + DateTime.Now.Day + " : opération clôture de fiche.");
                string mois = "201710";
                string query = "SELECT `idvisiteur` FROM `fichefrais` where `mois` = '" + mois + "';";
                eventLog.WriteEntry(this.setEntitleEventLog() + " SELECT " +
                                    "Sélection des fiches frais du mois "+mois+".");
                Dictionary<int, FicheMois> fichesMois = new Dictionary<int, FicheMois>();
                if (this.openConnection() == true)
                {
                    int i = 0;
                    string idEtat = "CL";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        eventLog.WriteEntry(this.setEntitleEventLog() + " Résultat || " +
                                           "Succès des sélection des fiches frais du mois " + mois + ".");
                        string idVisiteur = dataReader["idvisiteur"] + "";
                        FicheMois uneFiche = new FicheMois(idVisiteur, mois, idEtat);
                        fichesMois.Add(i, uneFiche);
                        i++;
                    }
                }
                this.closeConnection();

                for (int i = 0; i < fichesMois.Count; i++)
                {
                    eventLog.WriteEntry(this.setEntitleEventLog() + " Résultat || " +
                                       "Mise à jour des fiches frais du mois " + mois + ".");
                    query = "UPDATE fichefrais" +
                            " SET idetat='CL'" +
                            " WHERE idvisiteur='" + fichesMois[i].getIdVisiteur() + "'" +
                            " AND mois = '" + fichesMois[i].getMois() + "'";
                    Console.WriteLine(query);
                    //Open connection
                    if (this.openConnection() == true)
                    {
                        //create mysql command
                        MySqlCommand cmd = new MySqlCommand();
                        //Assign the query using CommandText
                        cmd.CommandText = query;
                        //Assign the connection using Connection
                        cmd.Connection = connection;

                        //Execute query
                        cmd.ExecuteNonQuery();

                        //close connection
                        this.closeConnection();
                    }
                }
            }
            else
            {
                eventLog.WriteEntry(this.setEntitleEventLog() + " Erreur || " +
                                   "Nous ne somme pas le bon jour pour la cloture.");
            }
        }

        //Close connection
        private bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /**
         * Fonction de validation des fiches ayant pour etat = 'Validé
         **/
        public void validationFiches()
        {
            if (GestionDate.entre(20, 31))
            {
                string mois = "201710";
                Console.WriteLine(mois);
                string query = "SELECT `idvisiteur` " +
                                "FROM `fichefrais` " +
                                "WHERE `mois` = '" + mois +
                                "' AND idetat = 'CL';";
                Dictionary<int, FicheMois> fichesMois = new Dictionary<int, FicheMois>();
                eventLog.WriteEntry(this.setEntitleEventLog() + " || Opération MYSQL || " +
                                    "Validation des fiches pour le mois " + mois);
                if (this.openConnection() == true)
                {
                    int i = 0;
                    string idEtat = "VA";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        eventLog.WriteEntry(this.setEntitleEventLog() + " || Opération MYSQL-SELECT || " +
                                            "Succès de l'opération SELECT " + mois);
                        string idVisiteur = dataReader["idvisiteur"] + "";
                        FicheMois uneFiche = new FicheMois(idVisiteur, mois, idEtat);
                        fichesMois.Add(i, uneFiche);
                        i++;
                    }
                }
                this.closeConnection();

                for (int i = 0; i < fichesMois.Count; i++)
                {
                    eventLog.WriteEntry(this.setEntitleEventLog() + " || Opération MYSQL-UPDATE || " +
                                        "Début de l'opération de validation des frais (UPDATE)" + mois);
                    query = "UPDATE fichefrais" +
                            " SET idetat='VA'" +
                            " WHERE idvisiteur='" + fichesMois[i].getIdVisiteur() + "'" +
                                " AND mois = '" + fichesMois[i].getMois() + "'" +
                                " AND idetat = 'CL'";
                    Console.WriteLine(query);
                    //Open connection
                    if (this.openConnection() == true)
                    {
                        //create mysql command
                        MySqlCommand cmd = new MySqlCommand();
                        //Assign the query using CommandText
                        cmd.CommandText = query;
                        //Assign the connection using Connection
                        cmd.Connection = connection;

                        //Execute query
                        cmd.ExecuteNonQuery();

                        //close connection
                        this.closeConnection();
                    }
                }
            }else
            {
                eventLog.WriteEntry(this.setEntitleEventLog() + " || Erreur || " +
                                    "Mauvais jours pour l'opération de validation des fiches de frais.");
            }
        }

        private bool openConnection()
        {
            try
            {
                connection.Open();
                eventLog.WriteEntry(this.setEntitleEventLog() + " Connection || " +
                                   "Succès de connection.");
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                eventLog.WriteEntry(this.setEntitleEventLog() + " Erreur || " +
                                   ex.ToString());
                return false;
            }
        }


        private string setEntitleEventLog()
        {
            return DateTime.Now.Year + "-" +
                   DateTime.Now.Month + "-" +
                   DateTime.Now.Day + "||" +
                   (DateTime.Now.Hour) + ":" +
                   (DateTime.Now.Minute) + "||";
        }

    }
}
