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
			string sqlSelect = "SELECT id FROM user_database WHERE userid=@idValue and pass=@passValue";

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
				// Session["admin"] = sqlDt.Rows[0]["admin"];
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
		
		[WebMethod(EnableSession = true)]
		public void StoreUserCharacter(string avatar, string name, string planet)
		{
			// AllowChange();
			
			string sqlSelect = "insert into characters (avatar, name, planet) values (@avatarValue, @nameValue, @planetValue);";
			
			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			
			sqlCommand.Parameters.AddWithValue("@avatarValue", HttpUtility.UrlDecode(avatar));
			sqlCommand.Parameters.AddWithValue("@nameValue", HttpUtility.UrlDecode(name));
			sqlCommand.Parameters.AddWithValue("@planetValue", HttpUtility.UrlDecode(planet));
			
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
			
			// update user characterID in user_database table
			updateUserCharacter();

		}
		
		//EXAMPLE OF AN UPDATE QUERY WITH PARAMS PASSED IN
		[WebMethod(EnableSession = true)]
		public void updateUserCharacter()
		{
			int charId = LoadCurrentCharacterID();
			int userId = LoadCurrentAccount();
			
			Console.WriteLine(charId + " " + userId);

			string sqlSelect = "update user_database set charID=@charIDValue where id=@userIDValue;";
			                  

			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@charIDValue", charId);
			sqlCommand.Parameters.AddWithValue("@userIDValue", userId);
			

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
		
		//EXAMPLE OF AN INSERT QUERY WITH PARAMS PASSED IN.  BONUS GETTING THE INSERTED ID FROM THE DB!
		[WebMethod(EnableSession = true)]
		public void RequestAccount(string uid, string pass, string firstName, string lastName, string email)
		{
			// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			//the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
			//does is tell mySql server to return the primary key of the last inserted row.
			string sqlSelect = "insert into user_database (userid, pass, firstname, lastname, email, charID, choices) " +
			                   "values(@idValue, @passValue, @fnameValue, @lnameValue, @emailValue, 1, null); SELECT LAST_INSERT_ID();";
			
			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));
			sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(firstName));
			sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lastName));
			sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
			
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
		
		
		[WebMethod(EnableSession = true)]
		public int LoadCurrentAccount()
		{
			DataTable sqlDt = new DataTable("accounts");

			// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			// select latest login id
			string sqlSelect = "select id from login_status ORDER BY time DESC LIMIT 1;";

			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			//gonna use this to fill a data table
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//filling the data table
			sqlDa.Fill(sqlDt);

			int id = 0;
			for (int i = 0; i < sqlDt.Rows.Count; i++)
			{
				id = Convert.ToInt32(sqlDt.Rows[i]["id"]);
				Console.WriteLine(id);
			}
			//return id
			return id;
		}
		
		[WebMethod(EnableSession = true)]
		public int LoadCurrentCharacterID()
		{
			DataTable sqlDt = new DataTable("accounts");

			// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			// select latest login id
			string sqlSelect = "select charID from characters ORDER BY charID DESC LIMIT 1;";

			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			//gonna use this to fill a data table
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//filling the data table
			sqlDa.Fill(sqlDt);

			int charid = 0;
			for (int i = 0; i < sqlDt.Rows.Count; i++)
			{
				charid = Convert.ToInt32(sqlDt.Rows[i]["charID"]);
				Console.WriteLine(charid);
			}
			//return charid
			return charid;
		}
		
		[WebMethod(EnableSession = true)]
		public string LoadCharacter(int charID)
		{
			DataTable sqlDt = new DataTable("accounts");

			// string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			// select latest login id
			string sqlSelect = "select name from characters where charID=@charIDValue;";

			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			
			sqlCommand.Parameters.AddWithValue("@charIDValue", charID);


			//gonna use this to fill a data table
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//filling the data table
			sqlDa.Fill(sqlDt);

			string character_result = "";
			
			for (int i = 0; i < sqlDt.Rows.Count; i++)
			{
				string name = sqlDt.Rows[i]["name"].ToString();
				character_result = name;
				Console.WriteLine(character_result);
			}
			//return character_result
			return character_result;
		}

		[WebMethod(EnableSession = true)]
		public void UpdateChoices(string choices)
		{
			AllowChange();
			
			int id = int.Parse(Session["id"].ToString());
			
			string sqlUpdate = "update user_database set choices=@choiceValue where id=@idValue";
			
			MySqlConnection sqlConnection = new MySqlConnection(getConString());
			MySqlCommand sqlCommand = new MySqlCommand(sqlUpdate, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", id);
			Console.WriteLine(id);
			sqlCommand.Parameters.AddWithValue("@choiceValue", HttpUtility.UrlDecode(choices));
			Console.WriteLine(choices);
			
			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
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
			string sqlSelect = "select id, userid, pass, firstname, lastname, email, charID from user_database;";

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
					email = sqlDt.Rows[i]["email"].ToString(),
					charID = Convert.ToInt32(sqlDt.Rows[i]["charID"])
				});
			}
			//convert the list of accounts to an array and return!
			return accounts.ToArray();
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
	}
}
