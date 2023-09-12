﻿using Business_Access_Layer.Common;
using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IReviewService
    {
        public Task<Response> GetRecipeReviews(int id);
        public Task<Response> CreateReview(Review rating);
    }
}