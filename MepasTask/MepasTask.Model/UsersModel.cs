using MepasTask.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MepasTask.Model
{
    public class UsersModel : BaseDbDefault
    {
        public string name { get; set; }
        public string surname { get; set; }
        [Required(ErrorMessage = "Kullanıcı Adı Giriniz")]
        public string username { get; set; }

        [Required(ErrorMessage = "Şifre Giriniz")]
        [DataType(DataType.Password)]
        public int password { get; set; }
        public bool status { get; set; }
    }
}
