using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOB_WebApp.Models
{
    public class HomeCodes
    {
        //Id for homecodes
        public int Id { get; set; }
        //The homecode
        public string Code { get; set; }
        //The adress for the homecode
        public string Address { get; set; }
    }
}
