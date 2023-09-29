using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IFileServices
    {
        public Byte[] GetImage(string imageName);
        public void DeleteImage(string imageName);
        public Task<string> SaveImage(IFormFile imageFile,string fileName);
    }
}
