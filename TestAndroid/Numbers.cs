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

namespace TestAndroid
{
    public class Numbers
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public float Number { get; set; }

        public override string ToString()
        {
            return string.Format("[ ID={0}, Number={1}", Id, Number);
        }
    }
}