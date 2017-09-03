﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FacebookClone.Models.Data;

namespace FacebookClone.Models.ViewModels.Profile
{
    public class WallVM
    {
        public WallVM()
        {

        }

        public WallVM(WallDTO row)
        {
            Id = row.Id;
            Message = row.Message;
            DateEdited = row.DateEdited;
        }

        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime DateEdited { get; set; }
    }
}