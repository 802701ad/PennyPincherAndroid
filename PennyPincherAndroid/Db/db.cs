using System.Collections.Generic;

using SQLite;
using System.IO;

namespace PennyPincher
{
    class Db
    {

        private static SQLiteConnection db = null;
        private static void getConnection()
        {
            if (db == null)
            {
                db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "PennyPincher.sqlite"));
                db.CreateTable<Account>();
                db.CreateTable<Fund>();
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
        #endregion
    }
}