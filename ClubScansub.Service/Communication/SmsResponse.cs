using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service.Communication
{

    public enum SmsResponseStatusEnum
    {
        OK,
        NumberNotValid,
        RecipientUnknown,
        UnkownError
    }

    public class SmsResponse
    {
        public SmsResponseStatusEnum Status { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public string RecipientName { get; set; } = string.Empty;
        public string RecipientPhoneNumber { get; set; } = string.Empty;

    }
}
