using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service.Communication;
using ClubScansub.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service
{
    public class SiteService: ServiceBase, ISiteService
    {
        private readonly ApplicationDbContext context;

        public SiteService(IOptions<ServiceOptions> options, IEmailSender emailSender):base(options, emailSender)
        {
            context = new ApplicationDbContext(dbContextOptions);    
        }

        public Task<Divelocation> AddAsync(Divelocation Item)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Divelocation>> AddAsync(IList<Divelocation> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Divelocation>> AddAsync(params Divelocation[] Items)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Divelocation Item)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Divelocation>> GetAllAsync()
        {
            var data = context.Divelocations.Include(x => x.MeetingLocation).Include(x => x.Image);
            return await data.ToListAsync();
        }

        public Task<Divelocation> GetAsync(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<Divelocation> GetAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<Divelocation> UpdateAsync(Divelocation item)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Divelocation>> UpdateAsync(IList<Divelocation> Items)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Divelocation>> UpdateAsync(params Divelocation[] Items)
        {
            throw new NotImplementedException();
        }
    }
}
