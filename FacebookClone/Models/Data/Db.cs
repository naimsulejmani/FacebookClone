﻿using System.Data.Entity;

namespace FacebookClone.Models.Data
{
    public class Db:DbContext
    {

        public DbSet<UserDTO> Users { get; set; }
        public DbSet<FriendDTO> Friends { get; set; }
        public DbSet<MessageDTO> Messages { get; set; }

        public DbSet<WallDTO> Wall { get; set; }
        public DbSet<OnlineDTO> Online { get; set; }
    }
}