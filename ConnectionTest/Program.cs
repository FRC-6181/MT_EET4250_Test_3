using System;
using System.Data;
using System.Data.SqlClient;
using NLog;

namespace ConnectionTest
{
	class Program
	{

		private static Logger logger = LogManager.GetCurrentClassLogger();

		static void Main(string[] args)
		{
			logger.Info("Testing Connection");
			if (!TestConnection()) { logger.Info("No Connection Made"); return; };

			getAverage(2);
			getmerge(3);
			getAverage(4);
			
			logger.Info("Completed Successfully");
		}

		private static void getAverage(int Question)
        {
			logger.Debug($"Calculating Average for Question {Question}");
			string sql = $"Select AVG(Temperature) as AVG_TEMP from ['DataSet1']";
			using SqlConnection conn = GetConnection();
			conn.Open();
			using SqlCommand cmd = new SqlCommand(sql, conn);
			SqlDataReader sdr = cmd.ExecuteReader();
			try
			{
				if (sdr.HasRows)
				{
					while (sdr.Read())
					{
						double temp = Util.DataReaderUtil.GetSafeDouble(sdr, "AVG_TEMP");
						string datarow = $"Average Temperature: {temp}";
						logger.Debug(datarow);
					}
				}
			}
			catch (Exception e)
			{
				logger.Error(e.StackTrace);
				return;
			}
			if (Question == 2) {
				logger.Debug($"Completed Question {Question}, Moving on to Question {Question + 1}");
			}
            else
            {
				logger.Debug($"Completed Question: {Question}");
			}
		}

		private static void getmerge(int Question)
        {
			int row = 0;
			try
			{
				logger.Debug($"Beginning Merge of Data Sets for Question: {Question}");
				string sql = @$"MERGE [Test_3].[dbo].['DataSet1'] as t 
									  USING [Test_3].[dbo].['DataSet2'] as s
									ON (s.RowId = t.RowId)
									WHEN MATCHED
									  THEN UPDATE SET 
										t.Temperature = s.Temperature, t.Density = s.Density
									WHEN NOT MATCHED BY TARGET 
									  THEN INSERT ([RowId]
											   ,[Temperature]
											   ,[Density])
										 VALUES (s.RowId,
												s.Temperature, 
												s.Density)
									When Not Matched by Source
										Then Delete;
                    ";
				using SqlConnection conn = GetConnection();
				conn.Open();
				using SqlCommand cmd = new SqlCommand(sql, conn);
				int rowsaffected = cmd.ExecuteNonQuery();
				row = row + rowsaffected;
				logger.Debug($"Rows Affected: {row}");
				logger.Debug($"Completed Question {Question}, Moving on to Question {Question + 1}");
			}
			catch (Exception e)
			{
				logger.Error(e.StackTrace);
			}
		}

		private static bool TestConnection()
		{
			try
			{
				using SqlConnection conn = GetConnection();
				conn.Open();
				logger.Debug("Connected");
			}
			catch(Exception e)
            {
				logger.Error($"Not Connected {e.StackTrace}");
				return false;
			}
			return true;
		}

		private static SqlConnection GetConnection()
		{
			string _connstr = "Server=LAPTOP-EFG50VT7\\SQLEXPRESS;Database=Test_3;Trusted_Connection=True;MultipleActiveResultSets=true;Connection Timeout=60";
			SqlConnection Connection = null;
			try
			{
				Connection = new SqlConnection(_connstr);
			}
			catch (Exception e)
			{
				logger.Error(e.StackTrace);
			}
			return Connection;
		}
	}
}
