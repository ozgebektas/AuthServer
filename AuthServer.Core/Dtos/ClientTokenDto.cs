using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Dtos
{

    //bu classı oluşturma nedenimiz diyelim ki user bilgilerine ihtiyaç duyulmayan bir apimiz var fakat biz güvenlik açısından jwt istiyoruz(hava durumu sitesi gibi)
    //refresh tokena ihtiyaç duyulmayacak

    public class ClientTokenDto
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
    }
}
