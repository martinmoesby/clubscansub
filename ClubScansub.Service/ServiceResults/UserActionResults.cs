using ClubScansub.Models;

namespace ClubScansub.Service.ServiceResults
{
    public enum UserActionResultEnum
    {
        Register,
        ForgotPassword
    }
    public abstract class UserActionResult
    {
        private readonly ApplicationUser? user;
        protected bool requireConfirmedAccount;

        public UserActionResult()
        {
            user = null;
            requireConfirmedAccount = false;
            Errors = new List<string>();
        }

        public UserActionResult(ApplicationUser user, bool requireConfirmedAccount)
        {
            this.user = user;
            this.requireConfirmedAccount = requireConfirmedAccount;
            Errors = new List<string>();
        }

        public ApplicationUser? User => user;
        public IList<string> Errors { get; }
        public bool IsSucceesfull => Errors.Count() == 0;
        public bool RequireConfirmedEmail => requireConfirmedAccount;
        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }

    public class RegisterUserResult : UserActionResult
    {

        public RegisterUserResult(ApplicationUser user, bool requireConfirmedAccount) : base(user, requireConfirmedAccount)
        {

        }

        public RegisterUserResult() : base()
        {

        }

    }

    public class ForgotPasswordResult : UserActionResult
    {

        public string Message { get; set; } = "";
        public ForgotPasswordResult(string email, bool confirmedAccount)
        {
            requireConfirmedAccount = confirmedAccount;
            AddError($"'{email}' has not been confirmed. Please check youor mail box for a confirmation email and follow the instructions");

        }
        public ForgotPasswordResult(string Message)
        {
            this.Message = Message;
        }
    }

    public class ResetPasswordResult : UserActionResult
    {

    }
}

