using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.IO;

namespace GestionClotureFrais
{
    public class MysqlDatabase
    {

        //Objet de connection à la base
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructeur
        public MysqlDatabase()
        {
            this.InitConnection();
        }

        /**
         * Initialisation de la connection au serveur mysql
         */
        private void InitConnection()
        {
            server = "localhost";
            database = "gsb_frais";
            uid = "root";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        /**
         * Cloture des fiches du mois précédent lorsque nous sommes entre le 1er et le 10 du mois
         */
        public void clotureFiches()
        {
            if (GestionDate.entre(1, 10))
            {
                string mois = "201710";
                string query = "SELECT `idvisiteur` FROM `fichefrais` where `mois` = '" + mois + "';";
                Dictionary<int, FicheMois> fichesMois = new Dictionary<int, FicheMois>();
                if (this.OpenConnection() == true)
                {
                    int i = 0;
                    string idEtat = "CL";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        string idVisiteur = dataReader["idvisiteur"] + "";
                        FicheMois uneFiche = new FicheMois(idVisiteur, mois, idEtat);
                        fichesMois.Add(i, uneFiche);
                        i++;
                    }
                }
                this.CloseConnection();

                for (int i = 0; i < fichesMois.Count; i++)
                {
                    query = "UPDATE fichefrais" +
                            " SET idetat='CL'" +
                            " WHERE idvisiteur='" + fichesMois[i].getIdVisiteur() + "'" +
                            " AND mois = '" + fichesMois[i].getMois() + "'";
                    Console.WriteLine(query);
                    //Open connection
                    if (this.OpenConnection() == true)
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
                        this.CloseConnection();
                    }
                }
            }
            else
            {
                Console.WriteLine("La cloture est déjà terminée!");
            }
        }

        //Close connection
        private bool CloseConnection()
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
                if (this.OpenConnection() == true)
                {
                    int i = 0;
                    string idEtat = "VA";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        string idVisiteur = dataReader["idvisiteur"] + "";
                        FicheMois uneFiche = new FicheMois(idVisiteur, mois, idEtat);
                        fichesMois.Add(i, uneFiche);
                        i++;
                    }
                }
                this.CloseConnection();

                for (int i = 0; i < fichesMois.Count; i++)
                {
                    query = "UPDATE fichefrais" +
                            " SET idetat='VA'" +
                            " WHERE idvisiteur='" + fichesMois[i].getIdVisiteur() + "'" +
                                " AND mois = '" + fichesMois[i].getMois() + "'" +
                                " AND idetat = 'CL'";
                    Console.WriteLine(query);
                    //Open connection
                    if (this.OpenConnection() == true)
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
                        this.CloseConnection();
                    }
                }
            }else {
                Console.WriteLine("Pas encore la validation!");
            }
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
    }
}
