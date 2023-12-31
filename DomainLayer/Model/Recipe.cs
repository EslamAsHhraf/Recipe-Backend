﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Model
{
    public class Recipe : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Steps { get; set; }
        public int Category { get; set; }
        [ForeignKey("Category.CategoryId")]
        public int CreatedBy { get; set; }
        [ForeignKey("User.UserId")]
        public double TotalRating { get; set; }
        public string ImageFile { get; set; }

    }
}
