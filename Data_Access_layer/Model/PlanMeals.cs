﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Data_Access_layer.Model
{
    public class PlanMeals : BaseEntity
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        [ForeignKey("User.UserId")]
        public int RecipeId { get; set; }
        [ForeignKey("Recipe.RecipeId")]
        public DateTime DateOn { get; set; }
    }
}
