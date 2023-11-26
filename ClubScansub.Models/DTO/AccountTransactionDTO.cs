using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Models.DTO
{
    public class AccountTransactionDTO :ApplicationUserAccountEntry
    {
        public MemberDTO CurrentMember { get; set; }
    }
}
