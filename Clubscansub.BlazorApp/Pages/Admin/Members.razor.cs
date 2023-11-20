using ClubScansub.Models.DTO;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.ComponentModel.Design.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Clubscansub.BlazorApp.Pages.Admin
{
    public partial class Members
    {
        [Inject]
        MemberService service { get; set; }

        MemberDTO member = new();


        protected override async Task OnInitializedAsync()
        {
            
            await base.OnInitializedAsync();
            members = (await service.GetAllAsync()).OrderBy(x=>x.UserName).ToList();
        }

        void loadData(LoadDataArgs args)
        {
            count = service.GetCount(args.Filter);
            member.EmailConfirmed= count > 0;
            member.PhoneNumberConfirmed= count > 0; 

            

            Console.WriteLine($"Skip: {args.Skip}, Top: {args.Top}");

        }

        private IList<MemberDTO> members;
        private int count;
    }
}
