using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Database
{
    /// <summary>
    /// Class which helps with SQLite table interaction.
    /// </summary>
    public class DatabaseInteraction
    {
        private SQLiteConnection connection;
        private SQLiteCommand cmd;
        
        /// <summary>
        /// Create a connection to specified SQLite table.
        /// </summary>
        /// <param name="path">path to SQLite table</param>
        public DatabaseInteraction(string path)
        {
            connection = new SQLiteConnection("Data Source=" + path + ";Version = 3;");
            connection.Open();
            cmd = new SQLiteCommand(connection);
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
            cmd.CommandText = @"select signature from viruses";
            using SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                signatureList.Add(Convert.ToString(reader["signature"]));
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