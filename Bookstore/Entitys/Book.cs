using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Bookstore.Entitys
{
    public class Books
    {
        public List<Book> books { get; set; }
    }

    
    public class Book : Interfaces.IBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; }
    }
}
