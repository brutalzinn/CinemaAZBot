﻿using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{

    public class Movie
    {
        public int id { get; set; }
        public string title { get; set; }
        public string overview { get; set; }
        public string release_date { get; set; }
        public double popularity { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public string poster_path { get; set; }
        public bool adult { get; set; } 

        public string backdrop_path { get; set; }
        public List<int> genre_ids { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public bool video { get; set; }

        public Details details { get; set; }
    }
}
