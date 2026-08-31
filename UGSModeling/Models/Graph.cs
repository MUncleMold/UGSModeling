using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace UGSModeling.Models
{
    public class Graph
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string XParam { get; set; }
        public string YParam { get; set; }
        public string GraphDesc { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
    }
}
