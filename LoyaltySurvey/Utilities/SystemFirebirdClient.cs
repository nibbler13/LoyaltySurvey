using System;
using System.Data;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;

namespace LoyaltySurvey {
    class SystemFirebirdClient : IDisposable {
        private readonly FbConnection connection;

		public SystemFirebirdClient(string ipAddress, string baseName, string user, string pass) {
			SystemLogging.ToLog("Создание подключения к базе FB: " + 
				ipAddress + ":" + baseName);

			FbConnectionStringBuilder cs = new FbConnectionStringBuilder();
			try {
				cs.DataSource = ipAddress;
				cs.Database = baseName;
				cs.UserID = user;
				cs.Password = pass;
				cs.Charset = "NONE";
				cs.Pooling = false;

				connection = new FbConnection(cs.ToString());
			} catch (Exception e) {
				SystemLogging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
			}
		}

		public DataTable GetDataTable(string query) {
			DataTable dataTable = new DataTable();

			try {
				connection.Open();
				using (FbCommand command = new FbCommand(query, connection))
				using (FbDataAdapter fbDataAdapter = new FbDataAdapter(command))
					fbDataAdapter.Fill(dataTable);
			} catch (Exception e) {
				SystemLogging.ToLog("GetDataTable exception: " + query + 
					Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
			} finally {
				connection.Close();
			}

			return dataTable;
		}

		public bool ExecuteUpdateQuery(string query, Dictionary<string, object> parameters) {
			bool updated = false;

			try {
				connection.Open();
				using (FbCommand update = new FbCommand(query, connection)) {
					if (parameters != null && parameters.Count > 0)
						foreach (KeyValuePair<string, object> parameter in parameters)
							update.Parameters.AddWithValue(parameter.Key, parameter.Value);
					
					updated = update.ExecuteNonQuery() > 0 ? true : false;
				}
			} catch (Exception e) {
				SystemLogging.ToLog("ExecuteUpdateQuery exception: " + query +
					Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
			} finally {
				connection.Close();
			}

			return updated;
		}

		public void Dispose() {
			connection.Dispose();
		}
	}
}
