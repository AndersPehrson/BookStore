using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookstore.Interfaces;

namespace Bookstore.Entitys
{
    class Cart : Interfaces.ICart
    {
        public List<IBook> CartContent { get; set; }
        public string Email { get; set ; }
    }
}
