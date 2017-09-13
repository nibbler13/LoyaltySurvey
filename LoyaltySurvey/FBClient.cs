using System;
using System.Data;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;

namespace LoyaltySurvey {
    class FBClient {
        private FbConnection connection;

		public FBClient(string ipAddress, string baseName, string user, string pass) {
			LoggingSystem.LogMessageToFile("Создание подключения к базе FB: " + 
				ipAddress + ":" + baseName);

			FbConnectionStringBuilder cs = new FbConnectionStringBuilder();
            cs.DataSource = ipAddress;
            cs.Database = baseName;
            cs.UserID = user;
            cs.Password = pass;
            cs.Charset = "NONE";
            cs.Pooling = false;

            connection = new FbConnection(cs.ToString());
		}

		public DataTable GetDataTable(string query) {
			DataTable dataTable = new DataTable();

			try {
				connection.Open();
				FbCommand command = new FbCommand(query, connection);

				FbDataAdapter fbDataAdapter = new FbDataAdapter(command);
				fbDataAdapter.Fill(dataTable);
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile("Не удалось получить данные, запрос: " + query + 
					Environment.NewLine + e.Message + " @ " + e.StackTrace);
			} finally {
				connection.Close();
			}

			return dataTable;
		}
    }
}
