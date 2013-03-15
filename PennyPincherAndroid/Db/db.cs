using System.Collections.Generic;

using SQLite;
using System.IO;
using System;
using System.Linq;

namespace PennyPincher
{
    class Db
    {

        private static SQLiteConnection db = null;
        private static void getConnection()
        {
            if (db == null)
            {
                string path = "";
                path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "PennyPincher.sqlite");
                path = Android.OS.Environment.ExternalStorageDirectory + "/PennyPincher/";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = Path.Combine(path, "db.sqlite");                
                db = new SQLiteConnection(path);
                db.CreateTable<Account>();
                db.CreateTable<Fund>();
                db.CreateTable<TransactionMain>();
                db.CreateTable<TransactionDetail>();
                var m = db.TableMappings;
            }
        }
        #region Accounts
        public static List<Account> getAccounts()
        {
            getConnection();
            return db.Query<Account>("select * from ACCOUNT order by account_name");
        }


        public static Account getAccount(string account_id)
        {
            getConnection();
            var a=db.Query<Account>("select * from ACCOUNT where account_id=?", account_id);
            if (a.Count == 0) return null; else return a[0];
        }

        public static void AddAccount(Account a)
        {
            getConnection();
            db.Insert(a);
        }

        public static void UpdateAccount(Account a)
        {
            getConnection();
            db.Update(a);
        }

        public static void DeleteAccount(string account_id)
        {
            getConnection();
            db.Delete<Account>(account_id);
        }
        #endregion

        #region Fund
        public static List<Fund> getFunds()
        {
            getConnection();
            return db.Query<Fund>("select * from Fund order by fund_name");
        }

        public static Fund getFund(string fund_id)
        {
            getConnection();
            var a = db.Query<Fund>("select * from Fund where fund_id=?", fund_id);
            if (a.Count == 0) return null; else return a[0];
        }

        public static void AddFund(Fund a)
        {
            getConnection();
            db.Insert(a);
        }

        public static void UpdateFund(Fund a)
        {
            getConnection();
            db.Update(a);
        }

        public static void DeleteFund(string fund_id)
        {
            getConnection();
            db.Delete<Fund>(fund_id);
        }
        #endregion

        #region Transaction
        public static List<TransactionMain> getTransactions(string account_id, string start, string end)
        {
            getConnection();
            string qry = @"select * from TransactionMain where account_id=? and transaction_date between ? and ? order by transaction_date desc limit 100";
            DateTime s;
            DateTime e;
            if (start == "")
                s = DateTime.Now.AddYears(-5);
            else
                s=DateTime.Parse(start);
            if (end == "")
                e = DateTime.Now.AddYears(5);
            else
                e = DateTime.Parse(end);
            return db.Query<TransactionMain>(qry, account_id, s, e);
        }

        public static TransactionMain getTransaction(string transaction_id)
        {
            getConnection();
            var a = db.Query<TransactionMain>("select * from TransactionMain where transaction_id=?", transaction_id);
            if (a.Count == 0) return null; else return a[0];
        }

        public static List<TransactionDetail> getTransactionDetails(string transaction_id)
        {
            getConnection();
            return db.Query<TransactionDetail>("select * from TransactionDetail where transaction_id=?", transaction_id);
        }

        internal static void DeleteTransaction(string transaction_id)
        {
            getConnection();
            db.BeginTransaction();
            db.Execute("delete from TransactionDetail where transaction_id=?", transaction_id);
            db.Execute("delete from TransactionMain where transaction_id=?", transaction_id);
            db.Commit();
        }

        public static void AddTransaction(TransactionMain a)
        {
            getConnection();
            db.Insert(a);
        }

        public static void UpdateTransaction(TransactionMain a)
        {
            getConnection();
            db.Update(a);
        }
        public static void AddTransactionDetails(List<TransactionDetail> a)
        {
            getConnection();
            db.BeginTransaction();
            if(a.Count>0)
                db.Execute("delete from TransactionDetail where transaction_id=?", a[0].transaction_id);
            foreach (TransactionDetail d in a)
            {
                db.Insert(d);
            }
            db.Commit();
        }
        
        #endregion




        internal static decimal getFundTotal(string account_id, string fund_id)
        {
            getConnection();
            var s = from f in db.Table<TransactionDetail>()
                    where f.account_id == account_id && f.fund_id == fund_id
                    select f.amount;
            return s.Sum();
            //var v = db.Query<TransactionDetail>("select * from TransactionDetail where account_id=? and fund_id=?", account_id, fund_id);
            //decimal result = 0;
            //foreach (var a in v)
            //    result += a.amount;
            //return result;
        }

        internal static decimal getFundTotal(string fund_id)
        {
            getConnection();
            var s = from f in db.Table<TransactionDetail>()
                    where f.fund_id == fund_id
                    select f.amount;
            return s.Sum();
        }

        internal static decimal getAccountTotal(string account_id)
        {
            getConnection();
            var s = from f in db.Table<TransactionMain>()
                    where f.account_id == account_id && f.is_active=="1"
                    select f.amount;
            return s.Sum();
            //var v = db.Query<TransactionDetail>("select * from TransactionDetail where account_id=? and fund_id=?", account_id, fund_id);
            //decimal result = 0;
            //foreach (var a in v)
            //    result += a.amount;
            //return result;
        }

        internal static decimal getTransactionDetailAmount(string transaction_id, string fund_id)
        {
            getConnection();
            var s = from d in db.Table<TransactionDetail>()
                    where d.transaction_id == transaction_id
                    && d.fund_id==fund_id
                    select d;

            decimal result = 0;
            foreach (var a in s)
                result += a.amount;
            return result;
        }
    }

    public class NumTable
    {
        public decimal amount;
    }
}