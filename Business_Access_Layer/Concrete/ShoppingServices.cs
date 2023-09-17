using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;


namespace Business_Access_Layer.Concrete
{
    public class ShoppingServices : IShoppingServices
    {
        private IAuthService _userService;
        private readonly IShopping _shopping;
        private readonly IRepository<Shopping> _shoppingRepository;
        private Response response = new Response();
        public ShoppingServices(IAuthService userService, IShopping shopping, IRepository<Shopping> shoppingRepository)
        {
            _userService = userService;
            _shopping=shopping;
            _shoppingRepository=shoppingRepository;
        }

        public async Task<Response> GetMyShopping()
        {
            UserData data = await _userService.GetMe();
            if (data == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Unauthorized" };
                return response;
            }
            var shoppingList = _shopping.GetShopping(data.Id);
            response.Status = "200";
            response.Data = shoppingList;
            return response;
        }

        public async Task<Response> GetMyPurchased()
        {
            UserData data = await _userService.GetMe();
            if (data == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Unauthorized" };
                return response;
            }
            var shoppingList = _shopping.GetPurchased(data.Id);
            response.Status = "200";
            response.Data = shoppingList;
            return response;
        }
        public async Task<Response> AddPurchased(int id, int quantity)
        {
            Shopping _object = await _shoppingRepository.GetById(id);
            if (_object == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Found" };
                return response;
            }
            if (quantity > _object.QuantityShopping)
            {
                _object.QuantityPurchased = _object.QuantityPurchased + _object.QuantityShopping;
                _object.QuantityShopping = 0;

            }
            else
            {
                _object.QuantityPurchased = _object.QuantityPurchased + quantity;
                _object.QuantityShopping = _object.QuantityShopping - quantity;

            }

            return await Update(_object);
        }
        public async Task<Response> AddShopping(List<Shopping> shopping)
        {
            var UserData = await _userService.GetMe();
            foreach (var entity in shopping)
            {
                if (entity.CreatedBy != UserData.Id)
                {
                    response.Data = new { Title = "Unauthorize user" };
                    response.Status = "401";
                    return response;
                }
                Shopping _object = await _shoppingRepository.GetByName(entity.Title);
                if (_object==null)
                {
                    _object.Title = _object.Title.ToLower();
                     await Create(_object);
                }
                else
                {
                    _object.QuantityShopping = _object.QuantityShopping +1 ;
                     await Update(_object);
                }
                   
            }
            response.Status = "200";
            response.Data = shopping;
            return response;

        }


        public async Task<Response> Update(Shopping shopping)
        {
            var result = await _shoppingRepository.Update(shopping);
            if (result == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Failed" };
                return response;
            }
            response.Status = "200";
            response.Data = result;
            return response;
        }
        public async Task<Response> Delete(Shopping shopping)
        {
            _shoppingRepository.Delete(shopping);
            response.Status = "200";
            response.Data = new { Title = "Deleted" };
            return response;
        }
        public async Task<Response> Create(Shopping shopping)
        {
            var result = await _shoppingRepository.Create(shopping);
            if (result == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Found" };
                return response;
            }
            response.Status = "200";
            response.Data = result;
            return response;
        }
    }
}
