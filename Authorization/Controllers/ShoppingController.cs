using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using DomainLayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RecipeAPI.Controllers
{
    [Route("api/shopping")]
    [ApiController]
    public class ShoppingController : Controller
    {

        private readonly IShoppingServices _shoppingService;
        private readonly IAuthService _userService;


        public ShoppingController(IShoppingServices shoppingService, IAuthService userService)
        {
            _shoppingService = shoppingService;
            _userService = userService;
        }
        [HttpGet("getShopping")]
        public ActionResult<Response> GetShopping()
        {
            var data = _shoppingService.GetMyShopping();

            return StatusCode(Int16.Parse(data.Result.Status), data.Result);

        }
        [HttpGet("getPurchased")]

        public ActionResult<Response> Purchased()
        {
            var data = _shoppingService.GetMyPurchased();

            return StatusCode(Int16.Parse(data.Result.Status), data.Result);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteShopping(int id)
        {
            var recipe = await _shoppingService.GetById(id);
            if (recipe.Status == "404")
            {
                return StatusCode(Int16.Parse(recipe.Status), recipe);

            }
            
            var DeleteResponse = _shoppingService.Delete((Shopping)recipe.Data);

            return StatusCode(Int16.Parse(DeleteResponse.Result.Status), DeleteResponse.Result);
        }


        [HttpPost("addShopping")]
        public async Task<ActionResult<Response>> AddShopping([FromBody] List<Shopping> shopping)
        {
            var DeletResponse = _shoppingService.AddShopping(shopping);

            return StatusCode(Int16.Parse(DeletResponse.Result.Status), DeletResponse.Result);
            
        }

        [HttpPost("addPurchased")]
        public async Task<ActionResult<Response>> AddPurchased([Required] [FromQuery] int id, [Required] [FromQuery] int quantity)
        {
            if (!ModelState.IsValid)
            {
                // Handle validation errors
                return BadRequest(ModelState);
            }
            var DeletResponse = _shoppingService.AddPurchased(id, quantity);

            return StatusCode(Int16.Parse(DeletResponse.Result.Status), DeletResponse.Result);

        }
    }
}
