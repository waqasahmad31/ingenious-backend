﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ingenious.Models.Entities
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
