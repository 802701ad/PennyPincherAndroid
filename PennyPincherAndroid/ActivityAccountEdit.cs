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

namespace PennyPincher
{
    [Activity(Label = "PennyPincher - Manage Accounts", MainLauncher=false)]
    public class ActivityAccountEdit : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.AccountEdit);

            // Get our button from the layout resource,
            // and attach an event to it
            FindViewById<Button>(Resource.Id.btnSave).Click += btnSave_Click;
            FindViewById<Button>(Resource.Id.btnDelete).Click += btnDelete_Click;

            if (Intent.GetStringExtra("account_id") != "")
            {
                FindViewById<EditText>(Resource.Id.txtAccountName).Text = Db.getAccount(Intent.GetStringExtra("account_id")).account_name;
                FindViewById<Button>(Resource.Id.btnDelete).Visibility = ViewStates.Visible;
            }

            
        }

        public void btnSave_Click(object sender, EventArgs e)
        {
            var a = new Account();
            a.account_id = Intent.GetStringExtra("account_id");
            a.account_name = FindViewById<EditText>(Resource.Id.txtAccountName).Text;
            if (a.account_id == "")
            {
                a.account_id = Guid.NewGuid().ToString();
                Db.AddAccount(a);
            }
            else
            {
                Db.UpdateAccount(a);
            }
           
            SetResult(Result.Ok);
            Finish();
        }

        public void btnDelete_Click(object sender, EventArgs e)
        {

            Db.DeleteAccount(Intent.GetStringExtra("account_id"));
            SetResult(Result.Ok);
            Finish();
        }
    }
}