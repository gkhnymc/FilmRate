using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FilmReview.Models
{
    public class Film
    {
        public int ID { get; set; }
        public string FilmName { get; set; }
        public string Genre { get; set; }
        public System.DateTime ReleaseDate { get; set; }
        public string Director { get; set; }
        public double Rate { get; set; }
        public string ImagePath { get; set; }
        public string FilmDescription { get; set; }
        public string TrailerLink { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

    }
}