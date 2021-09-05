using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HomeBudgetApp.Domain
{
    public class User
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "You must enter all fields!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "You must enter all fields!")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "You must enter all fields!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "You must enter all fields!")]
        public string Password { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
