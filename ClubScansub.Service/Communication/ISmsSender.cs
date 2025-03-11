using ClubScansub.Models;
using Mailjet.Client;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Service.Communication
{
    public interface ISmsSender
    {
        Task<SmsResponse> SendSmsAsync(string phonenumber, string message);
        Task<IEnumerable<SmsResponse>> SendMultipleSmsAsync(IEnumerable<IdentityUser> users, string message);

    }
}
