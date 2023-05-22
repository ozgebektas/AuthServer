using AuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{
    //Idnetity üyelik tablolar
    //tabloları aynı dbContext içinde tutmak istiyoruz.üyelikle ilgili işlemleri ayrı bir Dbcontextde,Product ve RefreshToken ayrı DbContextde iki ayrı veri tabanı açılacak.Performans açısından iyi bir durum değil
    //eğer üyelik sistemi dışında çokfazla tablo yoksa aynı dbContexte toplayabiliriz
    //Bu yüzden AppDbContext DbContextden inheritance almayıp (üyelikle ilgili bir context olacağı için) identity DbContext'den miras alınacak
    //kullanıcıdan gelen bilgiler IdentityDbContext içerisinden gelecek.User alanı oluşturmamıza gerek yok.(claim,token,roller,)

    public class AppDbContext:IdentityDbContext<UserApp,IdentityRole,string>
    {
        //burada belirttiğimiz constractor IdentityDbContext 'in constractor 'u 
        //options AppDbContext üzerinden belirtilecek
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken> UserRefreshToken { get; set; }



        //product ve RefreshToken objelerimi configür etmek istersek(yani db de bu tablolar oluşurken bu tablolara ait sütunların yapıları ne olucak)
        //required,null gibi yapılar;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(builder);
        }

    }
}
