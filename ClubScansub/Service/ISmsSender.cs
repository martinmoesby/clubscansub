using ClubScansub.Models;
using Mailjet.Client;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.Service
{
    public interface ISmsSender
    {
        Task<MailjetResponse> SendSmsAsync(string phonenumber, string message);
        Task<IEnumerable<MailjetResponse>> SendMultipleSmsAsync(IEnumerable<IdentityUser> users, string message);

    }
}
