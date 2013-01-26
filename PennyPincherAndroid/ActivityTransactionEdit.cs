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
            txtTotal = FindViewById<TextView>(Resource.Id.txtTotal);
            txtComments.SetMinWidth(WindowManager.DefaultDisplay.Width);

            FindViewById<Button>(Resource.Id.btnSave).Click += btnSave_Click;
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
                txt.AfterTextChanged += Amount_Change;
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
        protected TextView txtTotal;
        public void btnSave_Click(object sender, EventArgs e)
        {
            var a = new TransactionMain();
            var l = new List<TransactionDetail>();
            a.transaction_id = transaction_id;
            a.transaction_comment = txtComments.Text;
            a.transaction_date = txtTransactionDate.Text == "" ? DateTime.Now : Convert.ToDateTime(txtTransactionDate.Text);
            a.transaction_title = txtTitle.Text;
            a.account_id = account_id;
            a.amount = 0;
            a.is_active = chkIsActive.Checked ? "1" : "0";
            {//gather transaction details, so we'll have the amount on the main transaction record, we'll save the details afteward
                foreach (EditText et in amounts)
                {
                    var td = new TransactionDetail();
                    td.account_id = account_id;
                    td.fund_id = Convert.ToString(et.Tag);
                    td.comment = et.Text;
                    var s = Misc.ValSum(et.Text);
                    td.amount = a.is_active == "1" ? s : 0;
                    a.amount += s;
                    l.Add(td);
                }
            }
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
            foreach (var li in l)
                li.transaction_id = transaction_id;
            Db.AddTransactionDetails(l);
            SetResult(Result.Ok);
            Finish();
        }

        public void Delete()
        {
            using(var b=new AlertDialog.Builder(this))
            {
                b.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                b.SetTitle("Confirm");
                b.SetMessage("Do you really want to delete?");
                b.SetPositiveButton("OK", delegate {
                    Db.DeleteTransaction(transaction_id);
                    SetResult(Result.Ok);
                    Finish();
                });
                b.SetNegativeButton("Cancel", delegate {});
                b.Show();
            }


        }
        
        public void Amount_Change(object sender, EventArgs e)
        {
            decimal sum = 0;
            foreach (EditText et in amounts)
            {
                sum+= Misc.ValSum(et.Text);
            }
            txtTotal.Text = String.Format("{0:C}", sum);
        }

        const int mnuDelete = 1;
        const int mnuDuplicate = 2;

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, mnuDelete, 0, "Delete");
            menu.Add(0, mnuDuplicate, 0, "Duplicate");
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == mnuDelete)
                Delete();
            return true;
        }

    }
}