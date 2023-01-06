
using Microsoft.EntityFrameworkCore;
using QuizAppApi.Models;
using System.Collections.Generic;

namespace QuizAppApi.Infrastructure.DBContext
{
    public class DbInfo : DbContext
    {
        public DbInfo(DbContextOptions<DbInfo> options)
            : base(options)
        {

        }
        public DbSet<Questions> QuestionsTable { get; set; } 
        public DbSet<User> UsersTable { get; set; }
    }
}
