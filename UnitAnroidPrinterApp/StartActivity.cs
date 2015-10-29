using System;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace UnitAnroidPrinterApp
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
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
            //this.OnBackButtonPressed();
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    DependencyService.Get<IAndroidMethods>().CloseApp();
        //    return base.OnBackButtonPressed();
        //}

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
            var activity2 = new Intent(this, typeof(AuthorizationActivity));
            StartActivity(activity2);
        }
    }
}