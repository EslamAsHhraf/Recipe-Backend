using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_layer.Model
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Steps { get; set; }
        public int Category { get; set; }
        public int CreatedBy { get; set; }
        public double TotalRating { get; set; }
        public byte[] ImageFile { get; set; }

    }
}
