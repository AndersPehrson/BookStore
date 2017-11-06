using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Interfaces
{
    public interface IBook

    {

        string Title { get; }

        string Author { get; }

        decimal Price { get; }

        int InStock { get; }

    }
}
