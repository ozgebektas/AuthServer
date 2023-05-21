using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configuration
{
    //burada oluşturduğumuz client classı bizim spa yada mobile app 'e karşılık gelen clientlardır.
    public class Client
    {
        public string Id { get; set; }  
        public string Secret { get; set; }  

        //burada tanımlanacak client hangi api'lere erişim sağlayacak.
        //göndereceğimiz tokenda hangi api'lere erişeceğim bilgisi olacak
        public List<string> Audiences { get; set; }  
    }
}
