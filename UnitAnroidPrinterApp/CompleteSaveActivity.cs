using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace UnitAnroidPrinterApp
{
    [Activity(Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class CompleteSaveActivity : Activity
    {
        private Button Exit;
        private Button CreateAnother;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CompleteSaveEntry);

            Initialize();
        }

        private void Initialize()
        {
            Exit = FindViewById<Button>(Resource.Id.Exit);
            Exit.Visibility = Android.Views.ViewStates.Invisible;
            Exit.Click += Exit_Click;
            CreateAnother = FindViewById<Button>(Resource.Id.CreateAnother);
            CreateAnother.Click += CreateAnother_Click;
        }

        void GoMainActivity()
        {
            var activity = new Intent(this, typeof(MainActivity));
            activity.PutExtra("Name", Intent.GetStringExtra("Name"));
            activity.PutExtra("Pass", Intent.GetStringExtra("Pass"));
            StartActivity(activity);
        }

        private void CreateAnother_Click(object sender, EventArgs e)
        {
            GoMainActivity();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Process.KillProcess(Process.MyPid());
        }
    }
}