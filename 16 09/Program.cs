using System;
using untitled_folder_2.Interfaces;
using untitled_folder_2.Models;

namespace untitled_folder_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Book b = new Book()
            {
                Title = "Evgeny Onegin",
                Author = "Pushkin",
                Text = "Good book"
            };

            Book b2 = new Book("Mertvie dushi", "Gogol", "die, darling");

            BooksStorage storage = new FileBooksStorage(new ConsolePresenter());
            storage.AddBook(b);
            storage.AddBook(b2);
            System.Console.WriteLine(storage.BooksCount);
            
            storage.PresentBooks();
            

            System.Console.WriteLine($"title is : {b.Title}");

            b.Title = "new title";
            System.Console.WriteLine($"title 2 is : {b.Title}");
            
            Console.WriteLine("Hello World!");
        }
    }
}
