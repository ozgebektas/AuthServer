using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Dtos
{//biz model de UserApp için identityUser kullandığımız için burada clienta olabildiğince az ve ham biligi gitmeli(Dtoları bunun için ürettik)
    public class UserAppDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
    }
}
