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