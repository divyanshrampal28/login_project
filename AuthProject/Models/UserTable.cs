using System;
using System.Collections.Generic;

namespace AuthProject.Models
{
    public partial class UserTable
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
