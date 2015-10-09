using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace UnitAnroidPrinterApp
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class StartActivity : Activity
    {
        private async void Initialize(Action callback)
        {
            var login = "mobileUnit_Service";
            var pass = "1qazXSW@";
            UnitAPIShellSaver unitAPIShellSaver = new UnitAPIShellSaver(login, pass);

            UnitAPIShellUpdater unitAPIShellUpdater = new UnitAPIShellUpdater(login, pass);
            if (unitAPIShellUpdater.CheckEmptyDB())
            {
                try
                {
                    await unitAPIShellUpdater.UpdateAsync();
                }
                catch (WebException)
                {
                    FindViewById<TextView>(Resource.Id.TitleWait).Text = Resources.GetString(Resource.String.NoConnectionServer);
                    return;
                }
            }
            callback();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Start);
            FindViewById<TextView>(Resource.Id.TitleWait).Text = Resources.GetString(Resource.String.Wait);

            Initialize(() => GoToAutorizationActivity());
        }

        private void GoToAutorizationActivity()
        {
            var activity2 = new Intent(this, typeof(AuthorizationActivity));
            StartActivity(activity2);
        }
    }
}