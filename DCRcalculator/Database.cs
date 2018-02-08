using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DCRcalculator
{
    class Database
    {
        private string filePath, connectionString;

        public Database()
        {
            filePath = $"DCR_{DateTime.Now.Ticks}.sqlite";
            connectionString = $"Data Source={filePath};Version=3;";
            CreateDB();
            CreateTable();
        }

        private void CreateDB()
        {
            SQLiteConnection.CreateFile(filePath);
        }

        private void CreateTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS DCR(" +
            "APP TEXT," +
            "COMMIT_SHA TEXT, " +
            "AUTHOR_EMAIL TEXT, " +
            "AUTHOR_CURRENT_COMMIT_COUNT REAL, " +
            "APP_CURRENT_COMMIT_COUNT REAL, " +
            "AUTHOR_CURRENT_FILES_COUNT REAL, " +
            "APP_CURRENT_FILES_COUNT REAL, " +
            "AUTHOR_DCR REAL " +
            ");";

            using (var dbConnection = new SQLiteConnection(connectionString))
            {
                dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    using (var transaction = dbConnection.BeginTransaction())
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }

                dbConnection.Close();
            }
        }

        public void BatchInsert(List<AuthorDCR> dcrList)
        {
            string query = "INSERT INTO DCR " +
                                "(APP, COMMIT_SHA, AUTHOR_EMAIL, AUTHOR_CURRENT_COMMIT_COUNT, APP_CURRENT_COMMIT_COUNT, AUTHOR_CURRENT_FILES_COUNT, APP_CURRENT_FILES_COUNT, AUTHOR_DCR) " +
                                "VALUES (@APP, @COMMIT_SHA, @AUTHOR_EMAIL, @AUTHOR_CURRENT_COMMIT_COUNT, @APP_CURRENT_COMMIT_COUNT, @AUTHOR_CURRENT_FILES_COUNT, @APP_CURRENT_FILES_COUNT, @AUTHOR_DCR);";

            using (var dbConnection = new SQLiteConnection(connectionString))
            {
                dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    using (var transaction = dbConnection.BeginTransaction())
                    {
                        foreach (var dcrItem in dcrList)
                        {
                            command.Parameters.AddWithValue("@APP", dcrItem.App);
                            command.Parameters.AddWithValue("@COMMIT_SHA", dcrItem.CommitSHA);
                            command.Parameters.AddWithValue("@AUTHOR_EMAIL", dcrItem.AuthorEmail);
                            command.Parameters.AddWithValue("@AUTHOR_CURRENT_COMMIT_COUNT", dcrItem.AuthorCommits);
                            command.Parameters.AddWithValue("@APP_CURRENT_COMMIT_COUNT", dcrItem.AppCommits);
                            command.Parameters.AddWithValue("@AUTHOR_CURRENT_FILES_COUNT", dcrItem.AuthorFiles);
                            command.Parameters.AddWithValue("@APP_CURRENT_FILES_COUNT", dcrItem.AppFiles);
                            command.Parameters.AddWithValue("@AUTHOR_DCR", dcrItem.Dcr);                        

                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                dbConnection.Close();
            }
        }
    }
}
