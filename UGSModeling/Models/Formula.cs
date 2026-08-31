using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace UGSModeling.Models
{
    public class Formula
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FormDesc { get; set; }
        public string RecordForm { get; set; }
        public string Bind { get; set; }
        public string Params { get; set; }
    }
}
