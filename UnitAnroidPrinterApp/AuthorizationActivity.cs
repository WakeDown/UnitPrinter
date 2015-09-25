
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SQLite;
using System;
using System.IO;

namespace UnitAnroidPrinterApp
{
    [Activity (Icon = "@drawable/icon")]		
	public class AuthorizationActivity : Activity
	{
		private EditText _name;
		private EditText _pass;
		private string _dbUnitAndroidPrinterApp = Path.Combine (
			System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
            "dbUnitAndroidPrinterApp.db3");

		private void Initialize()
		{
			_name = FindViewById<EditText> (Resource.Id.EnterName);
			_pass = FindViewById <EditText> (Resource.Id.EnterPass);
			var buttonLogIn = FindViewById<Button> (Resource.Id.LogInButton);
			buttonLogIn.Click += ButtonLogIn_Click;
			var buttonCreate = FindViewById<Button> (Resource.Id.CreateButton);
			buttonCreate.Click += ButtonCreate_Click;
            buttonCreate.Visibility = Android.Views.ViewStates.Gone;
        }

		void ButtonCreate_Click (object sender, EventArgs e)
		{
            AccountDB account = new AccountDB()
            { Login = _name.Text, Password = _pass.Text, Sid = "test", CurUserAdSid = null };

            using (var _dataBase = new SQLiteConnection(_dbUnitAndroidPrinterApp))
            {
                _dataBase.Insert(account);
            }
			Toast.MakeText(this, Resource.String.CompleteCreateEntry, ToastLength.Long).Show();
		}
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Authorization);

			Initialize ();
        }

        void ButtonLogIn_Click (object sender, EventArgs e)
		{
			using(var dataBase = new SQLiteConnection(_dbUnitAndroidPrinterApp))
			{
				try
				{
                    dataBase.Get<AccountDB>(x => x.Login == _name.Text && x.Password == _pass.Text);
                    GoMainActivity();
                }
				catch(Exception) {
					Toast.MakeText (this, Resource.String.ErrorAuth, ToastLength.Long).Show();
				}
			}
		}

        void GoMainActivity()
        {
            var activity2 = new Intent(this, typeof(MainActivity));
            activity2.PutExtra("Name", _name.Text);
            activity2.PutExtra("Pass", _pass.Text);
            StartActivity(activity2);
        }
	}
}

