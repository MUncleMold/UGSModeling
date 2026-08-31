using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace UGSModeling.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Post { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
