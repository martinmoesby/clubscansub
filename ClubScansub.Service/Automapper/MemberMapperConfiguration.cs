using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClubScansub.Models;
using ClubScansub.Models.DTO;

namespace ClubScansub.Service.Automapper
{
    internal class MemberMapperConfiguration 
    {
        public MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, MemberDTO>()
                .ReverseMap();
            });

            return config;
        }
    }
}
