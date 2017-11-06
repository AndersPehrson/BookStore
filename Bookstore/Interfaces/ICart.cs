using System.Collections.Generic;

namespace Bookstore.Interfaces
{
    public interface ICart
    {
        List<IBook> CartContent { get; set; }
        string Email { get; set; }
    }
}