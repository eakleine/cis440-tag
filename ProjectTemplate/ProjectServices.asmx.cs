using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using Org.BouncyCastle.Bcpg;

namespace ProjectTemplate
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]

	public class ProjectServices : System.Web.Services.WebService
	{
		////////////////////////////////////////////////////////////////////////
		///replace the values of these variables with your database credentials
		////////////////////////////////////////////////////////////////////////
		private string dbID = "sprc2023team11";
		private string dbPass = "sprc2023team11";
		private string dbName = "sprc2023team11";
		////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////
		///call this method anywhere that you need the connection string!
		////////////////////////////////////////////////////////////////////////
		private string getConString() {
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName+"; UID=" + dbID + "; PASSWORD=" + dbPass;
		}
		////////////////////////////////////////////////////////////////////////

		public string table_value = "user_database";

		/////////////////////////////////////////////////////////////////////////
		//don't forget to include this decoration above each method that you want
		//to be exposed as a web service!
		[WebMethod(EnableSession = true)]
		/////////////////////////////////////////////////////////////////////////
		public string TestConnection()
		{
			try
			{
				string testQuery = "select * from user_database";

				////////////////////////////////////////////////////////////////////////
				///here's an example of using the getConString method!
				////////////////////////////////////////////////////////////////////////
				MySqlConnection con = new MySqlConnection(getConString());
				////////////////////////////////////////////////////////////////////////

				MySqlCommand cmd = new MySqlCommand(testQuery, con);
				MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
				DataTable table = new DataTable();
				adapter.Fill(table);
				return "Success!";
			}
			catch (Exception e)
			{
				return "Something went wrong, please check your credentials and db name and try again.  Error: "+e.Message;
			}
		}
		
		//EXAMPLE OF A SIMPLE SELECT QUERY (PARAMETERS PASSED IN FROM CLIENT)
		[WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
		public bool LogOn(string uid, string pass)
		{
			//we return this flag to tell them if they logged in or not
			bool success = false;

			//our connection string comes from our web.config file like we talked about earlier
			// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			//here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
			//NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
			string sqlSelect = "SELECT id, admin FROM user_database WHERE userid=@idValue and pass=@passValue";

			//set up our connection object to be ready to use our connection string
			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			//set up our command object to use our connection, and our query
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			//tell our command to replace the @parameters with real values
			//we decode them because they came to us via the web so they were encoded
			//for transmission (funky characters escaped, mostly)
			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

			//a data adapter acts like a bridge between our command object and 
			//the data we are trying to get back and put in a table object
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//here's the table we want to fill with the results from our query
			DataTable sqlDt = new DataTable();
			//here we go filling it!
			sqlDa.Fill(sqlDt);
			//check to see if any rows were returned.  If they were, it means it's 
			//a legit account
			if (sqlDt.Rows.Count > 0)
			{
				//if we found an account, store the id and admin status in the session
				//so we can check those values later on other method calls to see if they 
				//are 1) logged in at all, and 2) and admin or not
				Session["id"] = sqlDt.Rows[0]["id"];
				Session["admin"] = sqlDt.Rows[0]["admin"];
				success = true;
				// call a function that can connect to database again and store user login time or any details 
				// into the loginstatus table
				int userid = int.Parse(Session["id"].ToString());
				UpdateLoginStatus(userid, "logged in");
			}
			//return the result!
			return success;
		}

		[WebMethod(EnableSession = true)]
		public bool LogOff()
		{
			//if they log off, then we remove the session.  That way, if they access
			//again later they have to log back on in order for their ID to be back
			//in the session!
			Session.Abandon();
			int userid = int.Parse(Session["id"].ToString());
			UpdateLoginStatus(userid,"logged off");
			return true;
		}
		
		public void UpdateLoginStatus(int id, string log)
		{
			AllowChange();

			string sqlSelect = "insert into login_status (id, time, log) values(@idValue, now(), @logValue);";
			
			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			
			sqlCommand.Parameters.AddWithValue("@idValue", id);
			sqlCommand.Parameters.AddWithValue("@logValue", log);

			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			Console.WriteLine("Executing query...");
			try
			{
				sqlCommand.ExecuteNonQuery();
				Console.WriteLine("Query executed");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			sqlConnection.Close(); 
		}

		public void AllowChange()
		{
			string sqlSet = "set foreign_key_checks = 0;";
			
			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSet, sqlConnection);
			
			sqlConnection.Open();
			
			try
			{
				sqlCommand.ExecuteNonQuery();
				Console.WriteLine("Foreign key query executed");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			sqlConnection.Close(); 
		}
		
		//EXAMPLE OF A SELECT, AND RETURNING "COMPLEX" DATA TYPES
		[WebMethod(EnableSession = true)]
		public Account[] StoreAccounts()
		{
			DataTable sqlDt = new DataTable("accounts");

			// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			string sqlSelect = "select id, userid, pass, firstname, lastname, email from user_database";

			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			//gonna use this to fill a data table
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//filling the data table
			sqlDa.Fill(sqlDt);
			
			List<Account> accounts = new List<Account>();
			for (int i = 0; i < sqlDt.Rows.Count; i++)
			{
				accounts.Add(new Account
				{
					id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
					userId = sqlDt.Rows[i]["userid"].ToString(),
					password = sqlDt.Rows[i]["pass"].ToString(),
					firstName = sqlDt.Rows[i]["firstname"].ToString(),
					lastName = sqlDt.Rows[i]["lastname"].ToString(),
					email = sqlDt.Rows[i]["email"].ToString()
				});
			}
			//convert the list of accounts to an array and return!
			return accounts.ToArray();
		}

		//EXAMPLE OF AN INSERT QUERY WITH PARAMS PASSED IN.  BONUS GETTING THE INSERTED ID FROM THE DB!
		[WebMethod(EnableSession = true)]
		public void RequestAccount(string uid, string pass, string firstName, string lastName, string email)
		{
			// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			//the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
			//does is tell mySql server to return the primary key of the last inserted row.
			string sqlSelect = "insert into user_database (userid, pass, firstname, lastname, email, admin, charID) " +
				"values(@idValue, @passValue, @fnameValue, @lnameValue, @emailValue, 0, 1); SELECT LAST_INSERT_ID();";
			// ToDo: creating a request table and insert into request table instead of user_database
			
			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));
			sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(firstName));
			sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lastName));
			sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));

			//this time, we're not using a data adapter to fill a data table.  We're just
			//opening the connection, telling our command to "executescalar" which says basically
			//execute the query and just hand me back the number the query returns (the ID, remember?).
			//don't forget to close the connection!
			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			try
			{
				int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
				//here, you could use this accountID for additional queries regarding
				//the requested account.  Really this is just an example to show you
				//a query where you get the primary key of the inserted row back from
				//the database!
			}
			catch (Exception e) {
			}
			sqlConnection.Close();
		}

		//EXAMPLE OF A SELECT, AND RETURNING "COMPLEX" DATA TYPES
		[WebMethod(EnableSession = true)]
		public Account[] GetAccounts()
		{
			//check out the return type.  It's an array of Account objects.  You can look at our custom Account class in this solution to see that it's 
			//just a container for public class-level variables.  It's a simple container that asp.net will have no trouble converting into json.  When we return
			//sets of information, it's a good idea to create a custom container class to represent instances (or rows) of that information, and then return an array of those objects.  
			//Keeps everything simple.

			//WE ONLY SHARE ACCOUNTS WITH LOGGED IN USERS!
			if (Session["id"] != null)
			{
				DataTable sqlDt = new DataTable("accounts");

				// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
				string sqlSelect = "select id, userid, pass, firstname, lastname, email from table_value where active=1 order by lastname";

				MySqlConnection sqlConnection = new MySqlConnection(getConString());
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				//gonna use this to fill a data table
				MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
				//filling the data table
				sqlDa.Fill(sqlDt);

				//loop through each row in the dataset, creating instances
				//of our container class Account.  Fill each account with
				//data from the rows, then dump them in a list.
				List<Account> accounts = new List<Account>();
				for (int i = 0; i < sqlDt.Rows.Count; i++)
				{
					//only share user id and pass info with admins!
					if (Convert.ToInt32(Session["admin"]) == 1)
					{
						accounts.Add(new Account
						{
							id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
							userId = sqlDt.Rows[i]["userid"].ToString(),
							password = sqlDt.Rows[i]["pass"].ToString(),
							firstName = sqlDt.Rows[i]["firstname"].ToString(),
							lastName = sqlDt.Rows[i]["lastname"].ToString(),
							email = sqlDt.Rows[i]["email"].ToString()
						});
					}
					else
					{
						accounts.Add(new Account
						{
							id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
							firstName = sqlDt.Rows[i]["firstname"].ToString(),
							lastName = sqlDt.Rows[i]["lastname"].ToString(),
							email = sqlDt.Rows[i]["email"].ToString()
						});
					}
				}
				//convert the list of accounts to an array and return!
				return accounts.ToArray();
			}
			else
			{
				//if they're not logged in, return an empty array
				return new Account[0];
			}
		}

		//EXAMPLE OF AN UPDATE QUERY WITH PARAMS PASSED IN
		[WebMethod(EnableSession = true)]
		public void UpdateAccount(string id, string uid, string pass, string firstName, string lastName, string email)
		{
			//WRAPPING THE WHOLE THING IN AN IF STATEMENT TO CHECK IF THEY ARE AN ADMIN!
			if (Convert.ToInt32(Session["admin"]) == 1)
			{
				// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
				//this is a simple update, with parameters to pass in values
				string sqlSelect = "update table_value set userid=@uidValue, pass=@passValue, firstname=@fnameValue, lastname=@lnameValue, " +
					"email=@emailValue where id=@idValue";

				MySqlConnection sqlConnection = new MySqlConnection(getConString());
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				sqlCommand.Parameters.AddWithValue("@uidValue", HttpUtility.UrlDecode(uid));
				sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));
				sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(firstName));
				sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lastName));
				sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
				sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

				sqlConnection.Open();
				//we're using a try/catch so that if the query errors out we can handle it gracefully
				//by closing the connection and moving on
				try
				{
					sqlCommand.ExecuteNonQuery();
				}
				catch (Exception e)
				{
				}
				sqlConnection.Close();
			}
		}
		

		//EXAMPLE OF A SELECT, AND RETURNING "COMPLEX" DATA TYPES
		[WebMethod(EnableSession = true)]
		public Account[] GetAccountRequests()
		{//LOGIC: get all account requests and return them!
			if (Convert.ToInt32(Session["admin"]) == 1)
			{
				DataTable sqlDt = new DataTable("accountrequests");

				// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
				//requests just have active set to 0
				string sqlSelect = "select id, userid, pass, firstname, lastname, email from table_value where active=0 order by lastname";

				MySqlConnection sqlConnection = new MySqlConnection(getConString());
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
				sqlDa.Fill(sqlDt);

				List<Account> accountRequests = new List<Account>();
				for (int i = 0; i < sqlDt.Rows.Count; i++)
				{
					accountRequests.Add(new Account
					{
						id = Convert.ToInt32(sqlDt.Rows[i]["id"]),
						firstName = sqlDt.Rows[i]["firstname"].ToString(),
						lastName = sqlDt.Rows[i]["lastname"].ToString(),
						email = sqlDt.Rows[i]["email"].ToString()
					});
				}
				//convert the list of accounts to an array and return!
				return accountRequests.ToArray();
			}
			else {
				return new Account[0];
			}
		}

		//EXAMPLE OF A DELETE QUERY
		[WebMethod(EnableSession = true)]
		public void DeleteAccount(string id)
		{
			if (Convert.ToInt32(Session["admin"]) == 1)
			{
				// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
				//this is a simple update, with parameters to pass in values
				string sqlSelect = "delete from table_value where id=@idValue";

				MySqlConnection sqlConnection = new MySqlConnection(getConString());
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

				sqlConnection.Open();
				try
				{
					sqlCommand.ExecuteNonQuery();
				}
				catch (Exception e)
				{
				}
				sqlConnection.Close();
			}
		}

		//EXAMPLE OF AN UPDATE QUERY
		[WebMethod(EnableSession = true)]
		public void ActivateAccount(string id)
		{
			if (Convert.ToInt32(Session["admin"]) == 1)
			{
				// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
				//this is a simple update, with parameters to pass in values
				string sqlSelect = "update table_value set active=1 where id=@idValue";

				MySqlConnection sqlConnection = new MySqlConnection(getConString());
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

				sqlConnection.Open();
				try
				{
					sqlCommand.ExecuteNonQuery();
				}
				catch (Exception e)
				{
				}
				sqlConnection.Close();
			}
		}

		//EXAMPLE OF A DELETE QUERY
		[WebMethod(EnableSession = true)]
		public void RejectAccount(string id)
		{
			if (Convert.ToInt32(Session["admin"]) == 1)
			{
				// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB".ConnectionString;
				string sqlSelect = "delete from table_value where id=@idValue";

				MySqlConnection sqlConnection = new MySqlConnection(getConString());
				MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

				sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

				sqlConnection.Open();
				try
				{
					sqlCommand.ExecuteNonQuery();
				}
				catch (Exception e)
				{
				}

				sqlConnection.Close();
			}
		}
	}
}
