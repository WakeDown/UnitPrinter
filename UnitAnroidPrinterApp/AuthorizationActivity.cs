using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace UnitAnroidPrinterApp
{
    [Activity (Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]		
    public class AuthorizationActivity : Activity
	{
		private EditText _name;
		private EditText _pass;
        UnitAPIShellAutorizator unitAPIShellAutorizator;

        private void Initialize()
		{
			_name = FindViewById<EditText> (Resource.Id.EnterName);
			_pass = FindViewById <EditText> (Resource.Id.EnterPass);
			var buttonLogIn = FindViewById<Button> (Resource.Id.LogInButton);
			buttonLogIn.Click += ButtonLogIn_Click;
            var login = "mobileUnit_Service";
            var pass = "1qazXSW@";
            unitAPIShellAutorizator = new UnitAPIShellAutorizator(login, pass);
            var remember = unitAPIShellAutorizator.GetRememberAccount();
            _name.Text = remember.Login;
            _pass.Text = remember.Password;
        }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Authorization);

			Initialize ();
        }

        void ButtonLogIn_Click (object sender, EventArgs e)
		{
            AccountDB account = new AccountDB() { CurUserAdSid = string.Empty, Password = _pass.Text, Login = _name.Text, Sid = string.Empty };
            if(unitAPIShellAutorizator.LogIn(account))
            {
                if (FindViewById<CheckBox>(Resource.Id.CheckRemember).Checked)
                {
                    unitAPIShellAutorizator.RememberMe(account);    
                }
                GoMainActivity();
            }
            else
                Toast.MakeText(this, Resource.String.ErrorAuth, ToastLength.Long).Show();
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

