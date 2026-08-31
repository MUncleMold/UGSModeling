using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace UGSModeling.Models
{
    public class Period
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public decimal PeriodNumber { get; set; }
        public string State { get; set; }
        public decimal Duration { get; set; }
        public decimal Cost { get; set; }
    }
}
