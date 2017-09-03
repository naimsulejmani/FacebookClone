﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacebookClone.Models.Data
{
    [Table("tblMessages")]
    public class MessageDTO
    {

        [Key]
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public bool Read { get; set; }
        [ForeignKey("From")]
        public virtual UserDTO FromUser { get; set; }

        [ForeignKey("To")]
        public virtual UserDTO ToUser { get; set; }

    }
}