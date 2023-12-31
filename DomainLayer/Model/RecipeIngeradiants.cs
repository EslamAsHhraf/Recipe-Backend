﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Model
{
    public class RecipeIngredients : BaseEntity
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        [ForeignKey("Recipe.RecipeId")]
        public string Title { get; set; }

    }
}
