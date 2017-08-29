using System.Data.Entity;

namespace FacebookClone.Models.Data
{
    public class Db:DbContext
    {

        public DbSet<UserDTO> Users { get; set; }
    }
}