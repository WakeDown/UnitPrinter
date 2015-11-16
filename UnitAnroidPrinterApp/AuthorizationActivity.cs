using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using Android.Views.InputMethods;
using Android.Views;

namespace UnitAnroidPrinterApp
{
    [Activity(Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class AuthorizationActivity : Activity
    {
        private EditText m_name;
        private EditText m_pass;
        UnitAPIShellAuthorizator m_unitAPIShellAutorizator;

        private void Initialize()
        {
            m_name = FindViewById<EditText>(Resource.Id.EnterName);
            m_pass = FindViewById<EditText>(Resource.Id.EnterPass);
            var buttonLogIn = FindViewById<Button>(Resource.Id.LogInButton);
            buttonLogIn.Click += ButtonLogIn_Click;
            var login = "mobileUnit_Service";
            var pass = "1qazXSW@";
            m_unitAPIShellAutorizator = new UnitAPIShellAuthorizator(login, pass);
            AccountDB remember = m_unitAPIShellAutorizator.GetRememberAccount();
            if (remember != null)
            {
                m_name.Text = remember.Login;
                m_pass.Text = remember.Password;
                AccountDB account = new AccountDB() { CurUserAdSid = string.Empty, Password = m_pass.Text, Login = m_name.Text, Sid = string.Empty };
                if (m_unitAPIShellAutorizator.LogIn(account))
                    GoMainActivity();
            }
        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(Intent.ActionMain);
            intent.AddCategory(Intent.CategoryHome);
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
            Process.KillProcess(Process.MyPid());
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
            return true;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Authorization);

            Initialize();
        }

        void ButtonLogIn_Click(object sender, EventArgs e)
        {
            AccountDB account = new AccountDB() { CurUserAdSid = string.Empty, Password = m_pass.Text, Login = m_name.Text, Sid = string.Empty };
            if (m_unitAPIShellAutorizator.LogIn(account))
            {
                if (FindViewById<CheckBox>(Resource.Id.CheckRemember).Checked)
                {
                    m_unitAPIShellAutorizator.RememberMe(account);
                }
                GoMainActivity();
            }
            else
                Toast.MakeText(this, Resource.String.ErrorAuth, ToastLength.Long).Show();
        }

        void GoMainActivity()
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("Name", m_name.Text);
            intent.PutExtra("Pass", m_pass.Text);
            StartActivity(intent);
        }
    }
}

