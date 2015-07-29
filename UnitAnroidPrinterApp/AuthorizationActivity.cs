
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using SQLite;
using Android.Net;

namespace UnitAnroidPrinterApp
{
	[Activity (Label = "AuthorizationActivity", MainLauncher = true, Icon = "@drawable/icon")]		
	public class AuthorizationActivity : Activity
	{
		private EditText Name;
		private EditText Pass;
		private string _urlApi;
		private string _dbAccountLocal = Path.Combine (
			System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
			"dbAccountLocal.db3");
		private string _dbAccountGlobal = Path.Combine (
			System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
			"dbAccountGlobal.db3");

		private void Initialize()
		{
			Name = FindViewById<EditText> (Resource.Id.EnterName);
			Pass = FindViewById <EditText> (Resource.Id.EnterPass);
			var buttonLogIn = FindViewById<Button> (Resource.Id.LogInButton);
			buttonLogIn.Click += ButtonLogIn_Click;
			var buttonCreate = FindViewById<Button> (Resource.Id.CreateButton);
			buttonCreate.Click += ButtonCreate_Click;
			using (var dataBase = new SQLiteConnection (_dbAccountGlobal)) {
				dataBase.CreateTable<AccountDB> ();
			}
			using (var dataBase = new SQLiteConnection (_dbAccountLocal)) {
				dataBase.CreateTable<AccountDB> ();
			}
			_urlApi = Resources.GetString (Resource.String.URLApi);
		}

		void ButtonCreate_Click (object sender, EventArgs e)
		{
			try
			{
				var httpRequest = (HttpWebRequest)WebRequest.Create(_urlApi);
				using(var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
				{
					string jsonObj = JsonConvert.SerializeObject(new {Name = Name.Text, Pass = Pass.Text});
					streamWriter.WriteLine(jsonObj);
				}
				using(var httpResponse = httpRequest.GetResponse())
				{
					using(var streamReader = new StreamReader(httpResponse.GetResponseStream()))
					{
						string strResponse = streamReader.ReadToEnd();
						if(strResponse == "OK")
							Toast.MakeText(this, Resource.String.CompleteCreateEntry, ToastLength.Long).Show();
						else
							Toast.MakeText(this, Resource.String.ErrorCreateEntry, ToastLength.Long).Show();
					}
				}
			}
			catch(Exception) {
				var account = new AccountDB (Name.Text, Pass.Text);
				using (var _dataBase = new SQLiteConnection (_dbAccountLocal)) {
					_dataBase.Insert (account);
				}
				using (var _dataBase = new SQLiteConnection (_dbAccountGlobal)) {
					_dataBase.Insert (account);
				}
				Toast.MakeText(this, Resource.String.CompleteCreateEntry, ToastLength.Long).Show();
			}
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Authorization);

			Initialize ();

//			var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
//			var activeConnection = connectivityManager.ActiveNetworkInfo;
//			if ((activeConnection != null)  && activeConnection.IsConnected)
//			{
//				try{
//					using (var dataBase = new SQLiteConnection (_dbAccountLocal)) {
//						var dispatchInfoDBAll = dataBase.Table<DispatchPrinterInfoDB> ().ToArray();
//						string jsonObj = JsonConvert.SerializeObject(dispatchInfoDBAll);
//						HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlApi);
//						using(StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
//						{
//							streamWriter.WriteLine(jsonObj);
//						}
//						request.Accept = "Application/Json";
//						request.Method = "Post";
//						using(var responseInfo = request.GetResponse()) { }
//
//						dataBase.DeleteAll<DispatchPrinterInfoDB>();
//					}
//				}
//				catch(Exception) {
//				}
//
//				using(var dataBase = new SQLiteConnection(_dbAccountGlobal)) {
//					//закачать всю базу с сервера
//				}
//			}
		}

		void ButtonLogIn_Click (object sender, EventArgs e)
		{
			try
			{
				HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(_urlApi);
				using(var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
				{
					string jsonObj = JsonConvert.SerializeObject(new AccountDB(Name.Text, Pass.Text));
					streamWriter.WriteLine(jsonObj);
				}
				using(var httpResponse = httpRequest.GetResponse())
				{
					using(var streamReader = new StreamReader(httpResponse.GetResponseStream()))
					{
						string strResponse = streamReader.ReadToEnd();
						if (strResponse == "OK")
						{
							var activity2 = new Intent (this, typeof(MainActivity));
							activity2.PutExtra ("Name", Name.Text);
							activity2.PutExtra ("Pass", Pass.Text);
							StartActivity (activity2);
						}
						else
							Toast.MakeText (this, Resource.String.ErrorAuth, ToastLength.Long).Show();
					}
				}
			}
			catch(Exception)
			{
				using(var dataBase = new SQLiteConnection(_dbAccountGlobal))
				{
					try
					{
						var account = dataBase.Get<AccountDB> (x => x.Name == Name.Text && x.Pass == Pass.Text);
						if (account != null) {
							var activity2 = new Intent (this, typeof(MainActivity));
							activity2.PutExtra ("Name", Name.Text);
							activity2.PutExtra ("Pass", Pass.Text);
							StartActivity (activity2);
						}
					}
					catch(Exception) {
						Toast.MakeText (this, Resource.String.ErrorAuth, ToastLength.Long).Show();
					}
				}
			}
		}
	}
}

