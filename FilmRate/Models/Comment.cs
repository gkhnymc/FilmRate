using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FilmReview.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public string CommentText { get; set; }
        public System.DateTime ReleaseDate { get; set; }
        public int UserID { get; set; }
        public int FilmID { get; set; }

        public virtual User User { get; set; }
        public virtual Film Film { get; set; }
    }
}