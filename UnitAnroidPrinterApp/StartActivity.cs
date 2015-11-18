using System;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace UnitAnroidPrinterApp
{
    [Activity(MainLauncher = true, Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class StartActivity : Activity
    {
        private TextView m_titleWait;
        private async void Initialize(Action callback)
        {
            var login = "mobileUnit_Service";
            var pass = "1qazXSW@";
            UnitAPIShellSaver unitAPIShellSaver = new UnitAPIShellSaver(login, pass);

            UnitAPIShellUpdater unitAPIShellUpdater = new UnitAPIShellUpdater(login, pass);
            if (unitAPIShellUpdater.CheckEmptyDB())
            {
                StartConnection:
                try
                {
                    RunOnUiThread(() =>
                        m_titleWait.Text = Resources.GetString(Resource.String.Wait));
                    await unitAPIShellUpdater.UpdateAsync();
                }
                catch (WebException)
                {
                    RunOnUiThread(() =>
                        m_titleWait.Text = Resources.GetString(Resource.String.NoConnectionServer));
                    await Task.Delay(3 * 1000);
                    goto StartConnection;
                }
            }
            callback();
        }

        public override void OnBackPressed()
        {
            Intent intent = new Intent(Intent.ActionMain);
            intent.AddCategory(Intent.CategoryHome);
            intent.SetFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
            Process.KillProcess(Process.MyPid());
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Start);
            m_titleWait = FindViewById<TextView>(Resource.Id.TitleWait);
            m_titleWait.Text = Resources.GetString(Resource.String.Wait);

            Initialize(() => GoToAutorizationActivity());
        }

        private void GoToAutorizationActivity()
        {
            var intent = new Intent(this, typeof(AuthorizationActivity));
            StartActivity(intent);
        }
    }
}