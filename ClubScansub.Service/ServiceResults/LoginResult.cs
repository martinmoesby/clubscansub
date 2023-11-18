using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service.ServiceResults
{
    public class LoginResult
    {
        private readonly bool _success;
        private List<string> _errors = new List<string>();
        public LoginResult(bool isSucces)
        {
            _success = isSucces;
        }

        public bool IsSuccessFul => _success;
        public bool UseMFA { get; set; }
        public bool Locked { get; set; }

        public List<string> Errors => _errors;
        public void AddError(string error)
        {
            _errors.Add(error);
        }
    }
}
