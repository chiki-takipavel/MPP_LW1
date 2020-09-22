using System;
using System.Text.RegularExpressions;

namespace LR1_MPP
{
    public class Book : IComparable<Book>, IEquatable<Book>
    {
        const string DEFAULT_ISBN = "0000000000000";
        const string DEFAULT_AUTHOR = "Без автора";
        const string DEFAULT_BOOKNAME = "Без названия";
        const string DEFAULT_PUBLISHINGHOUSE = "Без издательства";

        private string isbn;
        private string author;
        private string bookName;
        private string publishingHouse;
        private int year;
        private Price price;
        private readonly string isbnPattern = @"^\d*$";

        public string Isbn
        {
            get => isbn;
            set => isbn = CheckIsbn(value);
        }
        public string Author
        {
            get => author;
            set => author = !string.IsNullOrWhiteSpace(value) ? value.Trim() : DEFAULT_AUTHOR;
        }
        public string BookName
        {
            get => bookName;
            set => bookName = !string.IsNullOrWhiteSpace(value) ? value.Trim() : DEFAULT_BOOKNAME;
        }
        public string PublishingHouse
        {
            get => publishingHouse;
            set => publishingHouse = !string.IsNullOrWhiteSpace(value) ? value.Trim() : DEFAULT_PUBLISHINGHOUSE;
        }
        public int Year { get; set; }
        public Price Price { get; set; }

        public Book()
        {
        }

        public Book(int year, string bookName, string author, string publishingHouse, string isbn, Price price)
        {
            Year = year;
            BookName = bookName;
            Author = author;
            PublishingHouse = publishingHouse;
            Isbn = isbn;
            Price = price;
        }

        public bool Equals(Book otherBook)
        {
            if (otherBook == null)
                return false;

            return Isbn == otherBook.Isbn &&
                   BookName == otherBook.BookName &&
                   Author == otherBook.Author &&
                   PublishingHouse == otherBook.PublishingHouse &&
                   Year == otherBook.Year &&
                   Price == otherBook.Price;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} - {2}, {3}. {4}. {5}", Author, BookName, PublishingHouse, Year, Isbn, Price);
        }

        private string CheckIsbn(string inputIsbn)
        {
            inputIsbn = inputIsbn.Replace("-", "").Replace(" ", "");
            if (!string.IsNullOrWhiteSpace(inputIsbn) && Regex.IsMatch(inputIsbn, isbnPattern))
            {
                if ((inputIsbn.Length == 10) && (CalcCheckDigitForISBN10(inputIsbn) == inputIsbn[^1]))
                    return inputIsbn;
                else if ((inputIsbn.Length == 13) && (CalcCheckDigitForISBN13(inputIsbn) == inputIsbn[^1]))
                    return inputIsbn;
                else
                    return DEFAULT_ISBN;
            }
            else
                return DEFAULT_ISBN;
        }

        static char CalcCheckDigitForISBN10(string isbn)
        {
            int checkSum = 0;
            for (int i = 0, index = isbn.Length; i < isbn.Length - 1; i++, index--)
            {
                int number = isbn[i] - '0';
                checkSum += number * index;
            }
            char result = (char)((11 - checkSum % 11) % 11 + '0');
            return result;
        }

        static char CalcCheckDigitForISBN13(string isbn)
        {
            int checkSum = 0;
            bool oddIndex = false;
            for (int i = 0; i < isbn.Length - 1; i++)
            {
                int number = isbn[i] - '0';
                checkSum += number * (oddIndex ? 3 : 1);
                oddIndex = !oddIndex;
            }
            char result = (char)((10 - checkSum % 10) % 10 + '0');
            return result;
        }

        public int CompareTo(Book other)
        {
            var result = CompareByYear(this, other);
            if (result == 0)
            {
                result = CompareByBookName(this, other);
                if (result == 0)
                {
                    result = CompareByAuthor(this, other);
                    if (result == 0)
                    {
                        result = CompareByPublishingHouse(this, other);
                        if (result == 0)
                        {
                            result = CompareByIsbn(this, other);
                            if (result == 0)
                                result = CompareByPrice(this, other);
                        }
                    }
                }
            }
            return result;
        }

        public static int CompareByIsbn(Book bookX, Book bookY)
        {
            return string.CompareOrdinal(bookX.Isbn, bookY.Isbn);
        }

        public static int CompareByBookName(Book bookX, Book bookY)
        {
            return string.CompareOrdinal(bookX.BookName, bookY.BookName);
        }

        public static int CompareByAuthor(Book bookX, Book bookY)
        {
            return string.CompareOrdinal(bookX.Author, bookY.Author);
        }

        public static int CompareByPublishingHouse(Book bookX, Book bookY)
        {
            return string.CompareOrdinal(bookX.PublishingHouse, bookY.PublishingHouse);
        }

        public static int CompareByYear(Book bookX, Book bookY)
        {
            if (bookX.Year > bookY.Year)
                return 1;
            else if (bookX.Year < bookY.Year)
                return -1;
            return 0;
        }

        public static int CompareByPrice(Book bookX, Book bookY)
        {
            return bookX.Price.CompareTo(bookY.Price);
        }
    }
}
