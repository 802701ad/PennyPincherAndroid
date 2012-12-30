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
    [Activity(Label = "Transaction", MainLauncher=false)]
    public class ActivityTransactionEdit : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.TransactionEdit);
            hScroll = FindViewById<HorizontalScrollView>(Resource.Id.hScroll);
            txtTitle = FindViewById<EditText>(Resource.Id.txtTitle);
            txtTransactionDate = FindViewById<EditText>(Resource.Id.txtTransactionDate);
            txtComments = FindViewById<EditText>(Resource.Id.txtComments);
            chkIsActive = FindViewById<CheckBox>(Resource.Id.chkIsActive);
            FindViewById<Button>(Resource.Id.btnSave).Click += btnSave_Click;
            FindViewById<Button>(Resource.Id.btnDelete).Click += btnDelete_Click;
            transaction_id = Intent.GetStringExtra("transaction_id");
            if (Convert.ToString(Intent.GetStringExtra("account_id")) == "")
                throw new Exception("cannot create or edit a transaction without an account");
            //create amount inputs
            var t = new TableLayout(this);
            t.StretchAllColumns = true;
            foreach (Fund f in Db.getFunds())
            {
                var tr = new TableRow(this);
                var lbl = new TextView(this);
                lbl.Text = f.fund_name;
                var txt = new EditText(this);
                amounts.Add(txt);
                txt.Tag = f.fund_id;
                tr.AddView(lbl);
                tr.AddView(txt);
                t.AddView(tr);
            }
            hScroll.AddView(t);
            //load fields from database
            if (transaction_id == "")
            {
                account_id = Intent.GetStringExtra("account_id");
                txtTransactionDate.Text=DateTime.Now.ToShortDateString();
            }
            else
            {
                var transaction = Db.getTransaction(transaction_id);
                chkIsActive.Checked = transaction.is_active == "1";
                txtComments.Text = transaction.transaction_comment;
                txtTitle.Text = transaction.transaction_title;
                txtTransactionDate.Text = transaction.transaction_date.ToShortDateString();
                account_id = transaction.account_id;
                var details = Db.getTransactionDetails(transaction_id);
                foreach (TransactionDetail td in details)
                {
                    foreach (EditText et in amounts)
                    {
                        if (Convert.ToString(et.Tag) == td.fund_id)
                            et.Text = td.comment;
                    }
                }


            }

            Android.Util.Log.Info("account_id", account_id);
        }
        protected string transaction_id;
        protected string account_id;
        protected HorizontalScrollView hScroll;
        protected EditText txtTitle;
        protected EditText txtTransactionDate;
        protected EditText txtComments;
        protected CheckBox chkIsActive;
        protected List<EditText> amounts=new List<EditText>();

        public void btnSave_Click(object sender, EventArgs e)
        {
            var a = new TransactionMain();
            a.transaction_id = transaction_id;
            a.transaction_comment = txtComments.Text;
            a.transaction_date = txtTransactionDate.Text==""?DateTime.Now:Convert.ToDateTime(txtTransactionDate.Text);
            a.transaction_title = txtTitle.Text;
            a.account_id = account_id;
            a.is_active = chkIsActive.Checked ? "1" : "0";
            if (a.transaction_id == "")
            {
                a.transaction_id = Guid.NewGuid().ToString();
                transaction_id = a.transaction_id;
                Db.AddTransaction(a);
            }
            else
            {
                Db.UpdateTransaction(a);
            }
            var l = new List<TransactionDetail>();
            foreach (EditText et in amounts)
            {
                var td = new TransactionDetail();
                td.transaction_id = transaction_id;
                td.account_id = account_id;
                td.fund_id = Convert.ToString(et.Tag);
                td.comment = et.Text;
                td.amount = Misc.Val(et.Text);
                l.Add(td);
            }
            Db.AddTransactionDetails(l);
            SetResult(Result.Ok);
            Finish();
        }

        public void btnDelete_Click(object sender, EventArgs e)
        {

            Db.DeleteTransaction(transaction_id);
            SetResult(Result.Ok);
            Finish();
        }
    }
}