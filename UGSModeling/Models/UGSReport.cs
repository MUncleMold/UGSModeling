using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace UGSModeling.Models
{
    public class UGSReport
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string Date { get; set; }
        public string Path { get; set; }
    }
}
