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

namespace ListaPesama_SignalRClient.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", MainLauncher = true)]
    public class PreferencesActivity : Activity
    {
        Button btnSave, btnCancel;
        EditText etServerName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Preferences);

            SetControls();

            var listaPesamaPreferences = Application.Context.GetSharedPreferences("ListaPesama", FileCreationMode.Private);
            var serverNamePreference = listaPesamaPreferences.GetString("ServerName", null);

            etServerName.Text = serverNamePreference;
        }

        private void SetControls()
        {
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnCancel = FindViewById<Button>(Resource.Id.btnCancel);

            etServerName = FindViewById<EditText>(Resource.Id.etServerName);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            var listaPesamaPreferences = Application.Context.GetSharedPreferences("ListaPesama", FileCreationMode.Private);
            var serverNamePreference = listaPesamaPreferences.GetString("ServerName", null);

            if (serverNamePreference != null)
            {
                StartActivity(typeof(MainActivity));
            }
            else
            {
                Toast.MakeText(this, Resource.String.emptyServerName, ToastLength.Short).Show();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var listaPesamaPreferences = Application.Context.GetSharedPreferences("ListaPesama", FileCreationMode.Private);
            var preferencesEditor = listaPesamaPreferences.Edit();

            if (!string.IsNullOrWhiteSpace(etServerName.Text))
            {
                preferencesEditor.PutString("ServerName", etServerName.Text);
                preferencesEditor.Commit();
                StartActivity(typeof(MainActivity));
            }
            else
            {
                Toast.MakeText(this, Resource.String.emptyServerName, ToastLength.Short).Show();
            }
        }
    }
}