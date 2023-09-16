using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/PlanedMeals")]
    [ApiController]
    public class PlanedMealsController : Controller
    {
        private readonly IPlanMealsService _planedMealsServices;

        public PlanedMealsController(IPlanMealsService planedMealsServices)
        {
            _planedMealsServices = planedMealsServices;
        }
        [HttpPost("User")]
        public ActionResult<Response> GetPlanesofuser([FromBody]int userid)
        {
            var data = _planedMealsServices.GetUsersRecipePlanes(userid);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
        [HttpPost]
        public async Task<IActionResult> PostPlan([FromBody] PlanMeals plan)
        {
            var data = _planedMealsServices.CreateRecipePlan(plan);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeletePlan(int planid)
        {
            var data = _planedMealsServices.DeleteRecipePlan(planid);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
        [HttpPut]
        public async Task<IActionResult> EditPlan([FromBody] PlanMeals plan)
        {
            var data = _planedMealsServices.UpdateRecipePlan(plan);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
    }
}
