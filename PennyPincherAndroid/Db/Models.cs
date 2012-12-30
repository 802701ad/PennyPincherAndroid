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
using SQLite;

public class Account
{
    [PrimaryKey]
    public string account_id { get; set; }
    public string account_name { get; set; }

}
public class Fund
{
    [PrimaryKey]
    public string fund_id { get; set; }
    public string fund_name { get; set; }
}

public class TransactionMain
{
    [Indexed]
    public string account_id { get; set; }
    [PrimaryKey]
    public string transaction_id { get; set; }
    public DateTime transaction_date { get; set; }
    public string transaction_title { get; set; }
    public string transaction_comment { get; set; }
    public string is_active { get; set; }
    public decimal amount { get; set; }
}

public class TransactionDetail
{
    [Indexed]
    public string transaction_id { get; set; }
    public string account_id { get; set; }
    public string fund_id { get; set; }
    public string comment { get; set; }
    public decimal amount { get; set; }
}