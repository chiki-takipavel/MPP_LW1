using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace LR1_MPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string defaultXmlFileName = "Books";
        private const string defaultXmlExtension = ".xml";
        private const string fileXmlFilter = "XML-файл (.xml)|*.xml";

        enum States
        {
            AddBook,
            EditBook
        }
        States currentState;
        private readonly Microsoft.Win32.OpenFileDialog openXmlFileDialog;
        private readonly Microsoft.Win32.SaveFileDialog saveXmlFileDialog;
        private BookStorage bookStorage;

        public MainWindow()
        {
            InitializeComponent();
            currentState = States.AddBook;
            bookStorage = new BookStorage();
            lvBooks.ItemsSource = bookStorage.Books;
            cbCulture.ItemsSource = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            cbCulture.SelectedItem = CultureInfo.GetCultureInfo("be");

            openXmlFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = defaultXmlExtension,
                Filter = fileXmlFilter
            };
            saveXmlFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = defaultXmlFileName,
                DefaultExt = defaultXmlExtension,
                Filter = fileXmlFilter
            };
        }

        private void IntegerValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ButtonOpenClick(object sender, RoutedEventArgs e)
        {
            if (openXmlFileDialog.ShowDialog() == true)
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(BookStorage));
                using FileStream file = new FileStream(openXmlFileDialog.FileName, FileMode.Open);
                try
                {
                    bookStorage = xmlFormatter.Deserialize(file) as BookStorage;
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Не удалось десериализовать объект", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                lvBooks.ItemsSource = bookStorage.Books;
                lvBooks.Items.Refresh();
            }
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            if (saveXmlFileDialog.ShowDialog() == true)
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(BookStorage));
                using FileStream file = new FileStream(saveXmlFileDialog.FileName, FileMode.Create);
                xmlFormatter.Serialize(file, bookStorage);
            }
        }

        private void ButtonAddOrEditBookClick(object sender, RoutedEventArgs e)
        {
            if (currentState == States.AddBook)
            {
                Book newBook = new Book
                {
                    BookName = tbBookName.Text,
                    Author = tbAuthor.Text,
                    PublishingHouse = tbPublishingHouse.Text,
                    Isbn = tbIsbn.Text,
                    Price = new Price()
                };
                try
                {
                    newBook.Year = int.Parse(tbYear.Text);
                }
                catch
                {
                    newBook.Year = 0;
                }
                try
                {
                    tbPrice.Text = tbPrice.Text.Replace(".", ",");
                    newBook.Price.Value = decimal.Parse(tbPrice.Text);
                }
                catch
                {
                    newBook.Price.Value = decimal.Zero;
                }
                newBook.Price.Culture = (CultureInfo)cbCulture.SelectedItem;

                bookStorage.AddBook(newBook);
            }
            else if (currentState == States.EditBook)
            {
                Book book = (Book)lvBooks.SelectedItem;
                book.BookName = tbBookName.Text;
                book.Author = tbAuthor.Text;
                book.PublishingHouse = tbPublishingHouse.Text;
                book.Isbn = tbIsbn.Text;
                try
                {
                    book.Year = int.Parse(tbYear.Text);
                }
                catch
                {
                    book.Year = 0;
                }
                try
                {
                    tbPrice.Text = tbPrice.Text.Replace(".", ",");
                    book.Price.Value = decimal.Parse(tbPrice.Text);
                }
                catch
                {
                    book.Price.Value = decimal.Zero;
                }
                book.Price.Culture = (CultureInfo)cbCulture.SelectedItem;
                currentState = States.AddBook;
                btnAddOrEditBook.Content = "Добавить книгу";
                lvBooks.IsEnabled = true;
            }

            lvBooks.Items.Refresh();
        }

        private void ListViewBooksMouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (result.VisualHit.GetType() != typeof(ListViewItem))
            {
                lvBooks.SelectedItem = null;
                lvBooks.ContextMenu.Visibility = bookStorage.Books.Count != 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void MenuItemEditClick(object sender, RoutedEventArgs e)
        {
            currentState = States.EditBook;
            btnAddOrEditBook.Content = "Редактировать книгу";
            lvBooks.IsEnabled = false;
            Book book = (Book)lvBooks.SelectedItem;
            tbYear.Text = book.Year.ToString();
            tbBookName.Text = book.BookName;
            tbAuthor.Text = book.Author;
            tbPublishingHouse.Text = book.PublishingHouse;
            tbIsbn.Text = book.Isbn;
            tbPrice.Text = book.Price.Value.ToString();
            cbCulture.SelectedItem = book.Price.Culture;
        }

        private void MenuItemDeleteClick(object sender, RoutedEventArgs e)
        {
            Book book = (Book)lvBooks.SelectedItem;
            if (book != null)
            {
                bookStorage.DeleteBook(book);
                lvBooks.Items.Refresh();
            }
        }

        private void MenuItemClearAllClick(object sender, RoutedEventArgs e)
        {
            bookStorage.ClearAllBooks();
            lvBooks.Items.Refresh();
        }

        private void ColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            string sortBy = column.Tag.ToString();
            switch (sortBy)
            {
                case "Year":
                    bookStorage.SortByYear();
                    break;
                case "BookName":
                    bookStorage.SortByBookName();
                    break;
                case "Author":
                    bookStorage.SortByAuthor();
                    break;
                case "PublishingHouse":
                    bookStorage.SortByPublishingHouse();
                    break;
                case "ISBN":
                    bookStorage.SortByIsbn();
                    break;
                case "Price":
                    bookStorage.SortByPrice();
                    break;
                default:
                    break;
            }
            lvBooks.Items.Refresh();
        }
    }
}
