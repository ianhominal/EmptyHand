using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserModel
    {
        public int UserId { get; set; }     
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string Email { get; set; }
    }
}
