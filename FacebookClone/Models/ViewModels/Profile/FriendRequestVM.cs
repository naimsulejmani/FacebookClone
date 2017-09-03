﻿using FacebookClone.Models.Data;

namespace FacebookClone.Models.ViewModels.Profile
{
    public class FriendRequestVM
    {
        public FriendRequestVM()
        {
            
        }

        public FriendRequestVM(FriendDTO row)
        {
            User1 = row.User1;
            User2 = row.User2;
            Active = row.Active;
        }
        public int User1 { get; set; }
        public int User2 { get; set; }
        public  bool Active { get; set; }

    }
}