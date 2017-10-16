using System.Collections.Generic;
using untitled_folder_2.Interfaces;
using System.Linq;

namespace untitled_folder_2.Models
{
    public abstract class BooksStorage
    {
        private IPresenter presenter;

        public int BooksCount => GetAllBooks().Count();

        public BooksStorage(IPresenter presenter)
        {
            this.presenter = presenter;
        }

        public abstract void AddBook(Book book);

        public abstract IEnumerable<Book> GetAllBooks();

        public void PresentBooks()
        {
            foreach (var book in GetAllBooks())
            {
                presenter.PresentBook(book);
            }
        }
    }
}