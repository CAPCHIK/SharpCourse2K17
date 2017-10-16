

 namespace untitled_folder_2.Models
 {
     public class Book
     {
        //  public string title;
        // public string Title
        // {
        //     get
        //     {
        //         return title;
        //     }
        //     set
        //     {
        //         title = value;
        //     }
        // }
        
        
        public string Title {get; set;}
        public string Author {get; set;}
        public string Text {get; set;}
        
         public Book()
         {
         }

         public Book(string ti, string a, string te)
         {
             Title = ti; 
             Author = a;
             Text = te;
         }
     }
 }