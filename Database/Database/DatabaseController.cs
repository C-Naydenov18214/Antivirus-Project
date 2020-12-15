using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Dapper;
using Dapper.Contrib.Extensions;

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
            SqlMapper.AddTypeHandler(new ListTypeHandler());
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
                Console.WriteLine("Database not created. Please create it.");
                return;
            }

            try
            {
                connection = new SQLiteConnection("Data Source=" + path + ";Version = 3;");
                connection.Open();
                cmd.Connection = connection;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Got an exception: {0}", e.Message);
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
                Console.WriteLine("Database created!");
            }

            try
            {
                connection = new SQLiteConnection("Data Source=" + path + ";Version = 3;");
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText =
                    "create table if not exists viruses (id INTEGER PRIMARY KEY, virusType text, signature text, metadata text)";
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Got an exception: {0}", e.Message);
            }
        }

        #region insert
        /// <summary>
        /// Insert virus to the SQLite table.
        /// </summary>
        /// <param name="signature">virus signature</param>
        /// <param name="virus">virus</param>
        /// <returns></returns>
        public long Insert(Virus virus)
        {
            try
            {
                return connection.Insert(virus);
            }
            catch (SQLiteException e)
            {
                Console.Write("Got an exception: {0}", e.Message);
                return -1;
            }
        }

        /// <summary>
        /// Insert viruses list to the SQLite table.
        /// </summary>
        /// <param name="viruses">viruses</param>
        /// <returns></returns>
        public long InsertList(List<Virus> viruses)
        {
            try
            {
                return connection.Insert(viruses);
            }
            catch (SQLiteException e)
            {
                Console.Write("Got an exception: {0}", e.Message);
                return -1;
            }
        }
        #endregion

        #region update
        /// <summary>
        /// Update specific virus in SQLite table.
        /// </summary>
        /// <param name="virus">virus</param>
        /// <returns></returns>
        public bool Update(Virus virus)
        {
            try
            {
                return connection.Update<Virus>(virus);
            }
            catch (SQLiteException e)
            {
                Console.Write("Got an exception: {0}", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Update a list of viruses in SQLite table.
        /// </summary>
        /// <param name="viruses">viruses</param>
        /// <returns></returns>
        public bool UpdateList(List<Virus> viruses)
        {
            try
            {
                return SqlMapperExtensions.Update(connection, viruses);
            }
            catch (SQLiteException e)
            {
                Console.Write("Got an exception: {0}", e.Message);
                return false;
            }
        }
        #endregion
        
        #region delete
        /// <summary>
        /// Delete specific virus in SQLite table.
        /// </summary>
        /// <param name="virus">virus</param>
        /// <returns></returns>
        public bool Delete(Virus virus)
        {
            try
            {
                return connection.Delete(virus);
            }
            catch (SQLiteException e)
            {
                Console.Write("Got an exception: {0}", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Delete viruses list in SQLite table.
        /// </summary>
        /// <param name="viruses">viruses</param>
        /// <returns></returns>
        public bool DeleteList(List<Virus> viruses)
        {
            try
            {
                return connection.Delete(viruses);
            }
            catch (SQLiteException e)
            {
                Console.Write("Got an exception: {0}", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Delete all viruses in SQLite table.
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll()
        {
            try
            {
                return connection.DeleteAll<Virus>();
            }
            catch (SQLiteException e)
            {
                Console.Write("Got an exception: {0}", e.Message);
                return false;
            }
        }
        #endregion
        
        #region get
        /// <summary>
        /// Get specific virus by id.
        /// </summary>
        /// <param name="id">virus id</param>
        /// <returns>virus</returns>
        public Virus GetVirus(int id)
        {
            try
            {
                return connection.Get<Virus>(id);
            }
            catch (SQLiteException e)
            {
                Console.Write("Got an exception: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get all viruses list from SQLite table.
        /// </summary>
        /// <returns>signatures list</returns>
        public IEnumerable<Virus> GetVirusesList()
        {
            try
            {
                return connection.GetAll<Virus>();
            }
            catch (SQLiteException e)
            {
                Console.Write("Got an exception: {0}", e.Message);
                return null;
            }
        }
        #endregion
        
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