using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{//Unit Of Work Pattern bize veritabanı ile yapılacak işlemleri tek bir kanal üzerinden yapmamızı sağlar.

    public class UnitOfWork : IUnitOfWork
    {
        //savechanges işelmlerini yapmak için DbContext nesnesine ihtiyaç duyulur.
        private readonly DbContext _context;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public void Commit()
        {
           _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
