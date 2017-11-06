using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookstore.Interfaces;
using Bookstore.Entitys;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Net.Mail;

namespace Bookstore.Handlers
{
    class Datahandler: IBookstoreService
    {
        static Books _books = null;

        static IEnumerable<IBook> GetBooks(string searchStr)
        {
            

            if(_books == null)
                _books = InitiateBooks();

            var selectedBooks = from b in _books.books
                                where b.Title.Contains(searchStr) || b.Author.Contains(searchStr)
                                select b;

            return (IEnumerable<IBook>)selectedBooks;
        }

        private static Books InitiateBooks()
        {
            var books = new Books();
            // Create a request for the URL. 		
            WebRequest request = WebRequest.Create("https://raw.githubusercontent.com/contribe/contribe/dev/arbetsprov-net/books.json");
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();



            books = JsonConvert.DeserializeObject<Books>(responseFromServer);

            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
            return books;
        }

        public Task<IEnumerable<IBook>> GetBooksAsync(string searchString)
        {
            return Task.Run(() => GetBooks(searchString));
        }

       
        public Task<IEnumerable<IBook>> PlaceOrderAsync(ICart cart)
        {
            return Task.Run(() => PlaceOrder(cart));
        }

        private IEnumerable<IBook> PlaceOrder(ICart cart)
        {
            var available = from c in cart.CartContent
                            where c.InStock > 0
                            select c;

            foreach (Book book in available)
            {
                var tochange = (from b in _books.books
                                where b.Author == book.Author && b.Title == book.Title
                                select b).FirstOrDefault();

                tochange.InStock--;
            }
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("zebra.swip.net");

                mail.From = new MailAddress("pehrson.anders@swipnet.se");
                mail.To.Add(cart.Email);
                mail.Subject = "Placed Order at Bookstore";
                mail.Body = "Thank you for placing an order in Book Store\n\n" +
                    "You ordered the following books:\n";

                foreach (Book book in cart.CartContent)
                {
                    mail.Body += $"'{book.Title}' by {book.Author}\n";
                }

                var nonAvailable = from c in cart.CartContent
                                   where c.InStock <= 0
                                   select c;


                if (nonAvailable.Any())
                {
                    mail.Body += "\nwhere the following books is out of stock and will be sent later or refunded if out of print:\n";

                    foreach (Book book in nonAvailable)
                    {
                        mail.Body += $"'{book.Title}' by {book.Author}\n";
                    }
                }

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("mz88493", "5NH-or-5");
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);

                
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return available;
        }
    }
}
