using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Concrete
{
    public class PlanMealsService : IPlanMealsService
    {
        private readonly IRepository<PlanMeals> _planRepository;
        private Response response = new Response();

        public PlanMealsService(IRepository<PlanMeals> planRepository)
        {
            _planRepository = planRepository;
        }
        public async Task<Response> GetUsersRecipePlanes(int userid)
        {
            var planes = _planRepository.GetAll();

            var myplanes = planes.Where(plan => plan.AuthorId == userid).ToList();
            response.Status = "200";
            response.Data = myplanes;
            return response;
        }
        public async Task<Response> CreateRecipePlan(PlanMeals plan)
        {
            var planes = _planRepository.GetAll();
            var plane = planes.Where(planex => planex.AuthorId == plan.AuthorId && planex.RecipeId == plan.RecipeId && planex.DateOn==plan.DateOn);
            var isplaneExists = plane.Any();
            if (isplaneExists)
            {
                response.Status = "404";
                response.Data = new { Title = "Created before" };
                return response;
            } 
            var createdplan = await _planRepository.Create(plan);
            if (createdplan == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Created" };
                return response;
            }
            response.Status = "200";
            response.Data = createdplan;
            return response;
        }
        public async Task<Response> UpdateRecipePlan(PlanMeals planid)
        {
            var updatedplan = await _planRepository.Update(planid);
            if (updatedplan == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Failed" };
                return response;
            }
            response.Status = "200";
            response.Data = updatedplan;
            return response;
        }
        public async Task<Response> DeleteRecipePlan(int planid)
        {
            var meal = await _planRepository.GetById(planid);
            if (meal == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Found" };
                return response;
            }
            _planRepository.Delete(meal);
            response.Status = "200";
            response.Data = new { Title = "Deleted" };
            return response;
        }
    }
}
