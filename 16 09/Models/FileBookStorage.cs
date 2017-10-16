using System.Collections.Generic;
using System.IO;
using untitled_folder_2.Interfaces;
using System.Linq;

namespace untitled_folder_2.Models
{
    public class FileBooksStorage : BooksStorage
    {
        public FileBooksStorage(IPresenter presenter) : base(presenter)
        {}

        public override void AddBook(Book book)
        {
            var content = string.Join('\n', 
                book.Title,
                book.Author,
                book.Text);
            File.WriteAllText(book.Title + ".txt", content);
        }

        public override IEnumerable<Book> GetAllBooks()
        {
            return Directory
            .GetFiles(Directory.GetCurrentDirectory())
            .Where(F => Path.GetExtension(F) == ".txt")
            .Select(F => File.ReadAllText(F))
            .Select(C => C.Split('\n'))
            .Select(P => new Book
            {
                Title = P[0],
                Author = P[1],
                Text = P[2]
            });
        }
    }
}
