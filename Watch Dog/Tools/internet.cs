using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Data;

namespace Watch_Dog.Tools
{
	class Internet
	{
		public class HistoryItem
		{
			public string URL { get; set; }

			public string Title { get; set; }

			public DateTime VisitedTime { get; set; }
		}

		public static List<HistoryItem> GetInternetHistoryEdge()
        	{
			string edgedatafile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Microsoft\Edge\User Data\Default\History";
			List<HistoryItem> allHistoryItems = new List<HistoryItem>();

            if (File.Exists(edgedatafile))
            {
				SQLiteConnection connection = new SQLiteConnection($"Data Source={edgedatafile};Version=3;New=False;Compress=True;");
				connection.Open();

				DataSet dataset = new DataSet();

				SQLiteDataAdapter adapter = new SQLiteDataAdapter("select * from urls order by last_visit_time desc", connection);
				adapter.Fill(dataset);
				if (dataset != null && dataset.Tables.Count > 0 & dataset.Tables[0] != null)
				{
					DataTable dt = dataset.Tables[0];

					foreach (DataRow historyRow in dt.Rows)
					{
						HistoryItem historyItem = new HistoryItem();

						historyItem.Title = Convert.ToString(historyRow["url"]);
						historyItem.URL = Convert.ToString(historyRow["title"]);

						// Chrome stores time elapsed since Jan 1, 1601 (UTC format) in microseconds
						long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

						// Windows file time UTC is in nanoseconds, so multiplying by 10
						DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

						// Converting to local time
						DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
						historyItem.VisitedTime = localTime;

						allHistoryItems.Add(historyItem);
					}
				}
			}
			return allHistoryItems;
		}

		public static List<HistoryItem> GetInternetHistoryChrome()
		{
			List<HistoryItem> allHistoryItems = new List<HistoryItem>();
			string chromeHistoryFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\History";
			if (File.Exists(chromeHistoryFile))
			{
				SQLiteConnection connection = new SQLiteConnection("Data Source=" + chromeHistoryFile + ";Version=3;New=False;Compress=True;");

				connection.Open();

				DataSet dataset = new DataSet();

				SQLiteDataAdapter adapter = new SQLiteDataAdapter("select * from urls order by last_visit_time desc", connection);
				adapter.Fill(dataset);
				if (dataset != null && dataset.Tables.Count > 0 & dataset.Tables[0] != null)
				{
					DataTable dt = dataset.Tables[0];

					foreach (DataRow historyRow in dt.Rows)
					{
						HistoryItem historyItem = new HistoryItem();

						historyItem.Title = Convert.ToString(historyRow["url"]);
						historyItem.URL = Convert.ToString(historyRow["title"]);

						// Chrome stores time elapsed since Jan 1, 1601 (UTC format) in microseconds
						long utcMicroSeconds = Convert.ToInt64(historyRow["last_visit_time"]);

						// Windows file time UTC is in nanoseconds, so multiplying by 10
						DateTime gmtTime = DateTime.FromFileTimeUtc(10 * utcMicroSeconds);

						// Converting to local time
						DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);
						historyItem.VisitedTime = localTime;

						allHistoryItems.Add(historyItem);
					}
				}
			}
			return allHistoryItems;
		}

		public static List<string> GetSearchTermsEdge()
        {
			List<string> searches = new List<string>();
			string edgedatafile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Microsoft\Edge\User Data\Default\History";

			SQLiteConnection connection = new SQLiteConnection("Data Source=" + edgedatafile + ";Version=3;New=False;Compress=True;");

			connection.Open();

			DataSet dataset = new DataSet();

			SQLiteDataAdapter adapter = new SQLiteDataAdapter("select * from keyword_search_terms", connection);
			adapter.Fill(dataset);

			if (dataset != null && dataset.Tables.Count > 0 & dataset.Tables[0] != null)
            		{
				DataTable dt = dataset.Tables[0];
				foreach(DataRow row in dt.Rows){
					searches.Add(Convert.ToString(row["terms"]));
                }
			}
			return searches;
		}
    }
}
