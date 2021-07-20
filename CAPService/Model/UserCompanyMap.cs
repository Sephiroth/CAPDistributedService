using System;
using System.Collections.Generic;

#nullable disable

namespace CAPService.Model
{
    public partial class UserCompanyMap
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public ulong CompanyId { get; set; }
    }
}
