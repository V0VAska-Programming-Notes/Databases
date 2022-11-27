using System;
using System.Collections.Generic;

namespace EF_Data_First.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
    }
}
