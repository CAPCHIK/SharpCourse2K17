using untitled_folder_2.Interfaces;
using System;
using System.IO;
namespace untitled_folder_2.Models
{
    public class FilePresenter : IPresenter
    {
        public void PresentBook(Book book)
        {
            
            var content = string.Join(" --- ", 
                book.Title,
                book.Author,
                book.Text);
            File.WriteAllText(book.Title + ".txt", content);
            
        }
    }
}