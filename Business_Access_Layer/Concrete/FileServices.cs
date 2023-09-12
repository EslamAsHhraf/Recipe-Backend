using Business_Access_Layer.Abstract;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Concrete
{
    public class FileServices:IFileServices
    {
        public Byte[] GetImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);

            Byte[] b = System.IO.File.ReadAllBytes(imagePath);   // You can use your own method over here.         
            return b;
        }
    }
}
