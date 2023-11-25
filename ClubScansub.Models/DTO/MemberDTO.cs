using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClubScansub.Models.DTO
{
    public class MemberDTO : ApplicationUser
    {
        public string[] Roles { get; set; }

    }
}
