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
    [Activity(Label = "PennyPincher - Manage Funds")]
    public class ActivityFundEdit : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.FundEdit);

            // Get our button from the layout resource,
            // and attach an event to it
            FindViewById<Button>(Resource.Id.btnSave).Click += btnSave_Click;
            FindViewById<Button>(Resource.Id.btnDelete).Click += btnDelete_Click;

            if (Intent.GetStringExtra("fund_id") != "")
            {
                FindViewById<EditText>(Resource.Id.txtFundName).Text = Db.getFund(Intent.GetStringExtra("fund_id")).fund_name;
                FindViewById<Button>(Resource.Id.btnDelete).Visibility = ViewStates.Visible;
            }

            
        }

        public void btnSave_Click(object sender, EventArgs e)
        {
            var a = new Fund();
            a.fund_id = Intent.GetStringExtra("fund_id");
            a.fund_name = FindViewById<EditText>(Resource.Id.txtFundName).Text;
            if (a.fund_id == "")
            {
                a.fund_id = Guid.NewGuid().ToString();
                Db.AddFund(a);
            }
            else
            {
                Db.UpdateFund(a);
            }
           
            SetResult(Result.Ok);
            Finish();
        }

        public void btnDelete_Click(object sender, EventArgs e)
        {

            Db.DeleteFund(Intent.GetStringExtra("fund_id"));
            SetResult(Result.Ok);
            Finish();
        }
    }
}