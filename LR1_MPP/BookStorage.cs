using System;
using System.Collections.Generic;

namespace LR1_MPP
{
    [Serializable]
    public class BookStorage
    {
        public List<Book> Books { get; set; }

        public BookStorage()
        {
            Books = new List<Book>();
        }

        public void AddBook(Book newBook)
        {
            Books.Add(newBook);
        }

        public void DeleteBook(Book book)
        {
            Books.Remove(book);
        }

        public void ClearAllBooks()
        {
            Books.Clear();
        }

        public void SortByBook()
        {
            Books.Sort();
        }

        public void SortByIsbn()
        {
            Books.Sort(Book.CompareByIsbn);
        }

        public void SortByBookName()
        {
            Books.Sort(Book.CompareByBookName);
        }

        public void SortByAuthor()
        {
            Books.Sort(Book.CompareByAuthor);
        }

        public void SortByPublishingHouse()
        {
            Books.Sort(Book.CompareByPublishingHouse);
        }

        public void SortByYear()
        {
            Books.Sort(Book.CompareByYear);
        }

        public void SortByPrice()
        {
            Books.Sort(Book.CompareByPrice);
        }
    }
}
