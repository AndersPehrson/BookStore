using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bookstore.Interfaces;

namespace Bookstore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IBookstoreService
    {
        private List<IBook> lstCart = new List<IBook>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public Task<IEnumerable<IBook>> GetBooksAsync(string searchString)
        {
            var datahandler = new Handlers.Datahandler();
            return datahandler.GetBooksAsync(searchString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitGrids();
        }

        private void InitGrids()
        {
            // Define and run the task.
            var taskA = Task.Run(() => GetBooksAsync(""));

            taskA.Wait();

            var res = taskA.Result;

            lstCart.Clear();

            dgrLibrary.DataContext = res;
            dgrCart.DataContext = lstCart;
            taskA.Dispose();
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in dgrLibrary.SelectedItems)
            {
                lstCart.Add((Entitys.Book)item);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            var text = txtTitle.Text;
            var taskA = Task.Run(() => GetBooksAsync(text));

            taskA.Wait();

            var res = taskA.Result;

            dgrLibrary.DataContext = res;
            taskA.Dispose();
        }

        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EmailPopup();
            var dialogRes = dialog.ShowDialog();
            if (dialogRes != null && dialogRes == true)
            {
                
                var cart = new Entitys.Cart();
                cart.CartContent = lstCart;
                cart.Email = dialog.txtEmail.Text;
                var taskA = Task.Run(() => PlaceOrderAsync(cart));

                taskA.Wait();

                var res = taskA.Result;

                var text = "Order placed\n" +
                    "An order of the following books were successfully placed:\n";

                foreach (IBook book in res)
                {
                    text += $"'{book.Title}' by {book.Author}\n";
                }

                text += "\nSee notification e-mail to the given adress for further details.";
                MessageBox.Show(text);
            }
            InitGrids();
        }

        public Task<IEnumerable<IBook>> PlaceOrderAsync(ICart cart)
        {
            var datahandler = new Handlers.Datahandler();
            return datahandler.PlaceOrderAsync(cart);
        }
    }
}
