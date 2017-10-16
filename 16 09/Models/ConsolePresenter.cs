using untitled_folder_2.Interfaces;
using System;
namespace untitled_folder_2.Models
{
    public class ConsolePresenter : IPresenter
    {
        public void PresentBook(Book book)
        {
            Console.WriteLine($"Title is : {book.Title}");
            Console.WriteLine($"Author is : {book.Author}");
            Console.WriteLine($"Text is : {book.Text}");
        }
    }
}