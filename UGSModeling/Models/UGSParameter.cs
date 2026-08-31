using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace UGSModeling.Models
{
    public class UGSParameter
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Unit { get; set; }
        public string Value { get; set; }
    }
}
