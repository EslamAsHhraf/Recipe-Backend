using Business_Access_Layer.Common;
using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IPlanMealsService
    {
        public Task<Response> GetUsersRecipePlanes(int userid);
        public Task<Response> CreateRecipePlan(PlanMeals plan);
        public Task<Response> UpdateRecipePlan(PlanMeals planid);
        public Task<Response> DeleteRecipePlan(int planid);

    }
}
