using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp_1.Models;

namespace WebApp_1.Infrastructure
{
    public class AppDbContext : DbContext
    {
        
        
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {



        }
    }
}
