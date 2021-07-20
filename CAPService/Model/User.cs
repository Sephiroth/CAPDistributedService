using System;
using System.Collections.Generic;

#nullable disable

namespace CAPService.Model
{
    public partial class User
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Pwd { get; set; }
        public byte Sex { get; set; }
        public string Email { get; set; }
    }
}
