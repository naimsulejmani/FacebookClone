﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacebookClone.Models.Data
{
    [Table("tblWall")]
    public class WallDTO
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime DateEdited { get; set; }
        [ForeignKey("Id")]
        public virtual UserDTO Users { get; set; }

    }
}