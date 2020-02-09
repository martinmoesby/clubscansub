using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Mailjet.Client;
using Mailjet.Client.Resources.SMS;
using Mailjet.Client.Resources;
using Send = Mailjet.Client.Resources.SMS.Send;
using Microsoft.AspNetCore.Identity;
using ClubScansub.Models;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ClubScansub.Service
{
    public class SmsSender : ISmsSender
    {
        public SmsOptions Options { get; set; }

        private readonly MailjetClient client;

        public SmsSender(IOptions<SmsOptions> smsOptions)
        {
            Options = smsOptions.Value;
            client = new MailjetClient(Options.AuthenticationToken)
            { 
                Version = ApiVersion.V4
            };
        }

        public async Task<MailjetResponse> SendSmsAsync(string phonenumber, string message)
        {

            phonenumber = Regex.Replace(phonenumber, @"\s+", "");

            if (!phonenumber.StartsWith("+45"))
                phonenumber = $"+45{phonenumber}";

            MailjetRequest request = new MailjetRequest()
            {
                Resource = Send.Resource
            }
            .Property(Send.From, Options.From)
            .Property(Send.To, phonenumber)
            .Property(Send.Text, message);

            MailjetResponse response = await client.PostAsync(request);

            return response;
        }

        public async Task<IEnumerable<MailjetResponse>> SendMultipleSmsAsync(IEnumerable<ApplicationUser> users, string message)
        {
            ICollection<MailjetResponse> responses = new List<MailjetResponse>();
            if (users.Count() > 0)
            {
                foreach (var item in users)
                {
                    if (item.PhoneNumberConfirmed && !string.IsNullOrEmpty(item.PhoneNumber))
                    {
                        responses.Add(await SendSmsAsync(item.PhoneNumber, message));
                    }
                    else
                    {
                        var o = new { ErrorMessage = $"User: {item.Name}, Phonenumber: {item.PhoneNumber}, Error : Phone not confirmed", User = item };
                        JObject jObj = JObject.Parse(JsonConvert.SerializeObject(o, new JsonSerializerSettings()
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            Formatting = Formatting.Indented
                        }));

                        responses.Add(new MailjetResponse(false, 999, jObj));
                    }
                }
            }
            return responses;
        }
    }
}
