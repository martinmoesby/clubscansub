using Azure;
using Mailjet.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Send = Mailjet.Client.Resources.SMS.Send;

namespace ClubScansub.Service.Communication
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

        public async Task<SmsResponse> SendSmsAsync(string phonenumber, string message)
        {

            phonenumber = Regex.Replace(phonenumber, @"\s+", "");

            if (!phonenumber.StartsWith("+45"))
                phonenumber = $"+45{phonenumber}";

            //MailjetRequest request = new MailjetRequest()
            //{
            //    Resource = Send.Resource
            //}
            //.Property(Send.From, Options.From)
            //.Property(Send.To, phonenumber)
            //.Property(Send.Text, message);

            //MailjetResponse response = await client.PostAsync(request);


            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Token",
                "kfXpsvxJS0iTUUmmKCLY_dciOKvp9n_cUX7OS1bNq-IOkYb1-xfSenYCo_iniyKV"
            );

            var msisdn = long.Parse(phonenumber);

            var messages = new
            {
                sender = "Scansub",
                message,
                recipients = new[] { new { msisdn } },
            };

            using var resp = await client.PostAsync(
                "https://gatewayapi.com/rest/mtsms",
                JsonContent.Create(messages)
            );


            var result = new SmsResponse()
            {
                Status = SmsResponseStatusEnum.OK
            };

            // On 2xx, print the SMS IDs received back from the API
            // otherwise print the response content to see the error:
            if (resp.IsSuccessStatusCode && resp.Content != null)
            {
                //Console.WriteLine("success!");
                var content = await resp.Content.ReadFromJsonAsync<Dictionary<string, dynamic>>();
                foreach (var smsId in content["ids"].EnumerateArray())
                {
                    result.Message = $"allocated SMS id: {smsId}";
                }
            }
            else if (resp.Content != null)
            {
                var content = await resp.Content.ReadAsStringAsync();
                result.Message = $"failed :{content}";
            }
            return result;
        }

        public async Task<IEnumerable<SmsResponse>> SendMultipleSmsAsync(IEnumerable<IdentityUser> users, string message)
        {
            ICollection<SmsResponse> responses = new List<SmsResponse>();
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
                        var o = new SmsResponse() { 
                            Message = $"User: {item.UserName}, Phonenumber: {item.PhoneNumber}, Error : Phone not confirmed", 
                            RecipientName = item.UserName,
                            RecipientPhoneNumber = item.PhoneNumber
                        };

                        responses.Add(o);
                    }
                }
            }
            return responses;
        }
    }
}
