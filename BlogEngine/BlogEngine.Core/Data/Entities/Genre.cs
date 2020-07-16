﻿using System.Collections.Generic;
using BlogEngine.Core.Data.Entities.JoiningEntities;

namespace BlogEngine.Core.Data.Entities
{
    public class Genre
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public byte[] Cover { get; set; }


        // TODO: add rate column

        public List<BlogGenre> BlogGenres { get; set; } = new List<BlogGenre>();
    }
}