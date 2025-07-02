using App.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    public class PremiumUpgradeRequest : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public DateTime ExpiredDate { get; set; }
        public bool IsAccepted { get; set; } = false;
    }
}
