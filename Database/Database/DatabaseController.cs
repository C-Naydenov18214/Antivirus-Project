using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Database
{
    /// <summary>
    /// Class which helps with SQLite table interaction.
    /// </summary>
    public class DatabaseController
    {
        private          SQLiteConnection connection;
        private readonly SQLiteCommand    cmd;
        private readonly string           path;

        /// <summary>
        /// Create a Database Controller class.
        /// </summary>
        /// <param name="path">path to SQLite table</param>
        public DatabaseController(string path)
        {
            connection = new SQLiteConnection();
            cmd        = new SQLiteCommand();
            this.path  = path;
        }

        /// <summary>
        /// Create a connection to existing SQLite table.
        /// </summary>
        public void Connect()
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Database not created.");
            }

            try
            {
                connection = new SQLiteConnection("Data Source=" + path + ";Version = 3;");
                connection.Open();
                cmd.Connection = connection;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Got an exception: " + e.Message);
            }
        }

        /// <summary>
        /// Create a database and viruses table.
        /// </summary>
        public void Create()
        {
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
            }

            try
            {
                connection = new SQLiteConnection("Data Source=" + path + ";Version = 3;");
                connection.Open();
                cmd.Connection  = connection;
                cmd.CommandText = "create table if not exists viruses (signature text)";
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Got an exception: " + e.Message);
            }
        }

        /// <summary>
        /// Insert signature to the SQLite table.
        /// </summary>
        /// <param name="signature">virus signature</param>
        public void Insert(string signature)
        {
            cmd.CommandText = "insert into viruses (signature) values (@signature)";
            cmd.Parameters.AddWithValue("@signature", signature);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Delete signature from a SQLite table.
        /// </summary>
        /// <param name="signature">virus signature</param>
        public void Delete(string signature)
        {
            cmd.CommandText = "delete from viruses where signature = @signature";
            cmd.Parameters.AddWithValue("@signature", signature);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Get signatures list from SQLite table.
        /// </summary>
        /// <returns>signatures list</returns>
        public IEnumerable<string> GetSignatureList()
        {
            List<string> signatureList = new List<string>();
            try
            {
                cmd.CommandText = @"select signature from viruses";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    signatureList.Add(Convert.ToString(reader["signature"]));
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Got an exception: " + e.Message);
            }

            return signatureList;
        }

        /// <summary>
        /// Close connection to the SQLite table. 
        /// </summary>
        public void CloseConnection()
        {
            connection.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}