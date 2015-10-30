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
        private Button m_exit;
        private Button m_createAnother;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CompleteSaveEntry);

            Initialize();
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

        private void Initialize()
        {
            m_exit = FindViewById<Button>(Resource.Id.Exit);
            m_exit.Visibility = Android.Views.ViewStates.Invisible;
            m_exit.Click += Exit_Click;
            m_createAnother = FindViewById<Button>(Resource.Id.CreateAnother);
            m_createAnother.Click += CreateAnother_Click;
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