using Hotel.Configuration;
using System.Collections.Generic;

namespace Hotel.Models
{
    public class AuthResponse : AuthResult
    {
        public static AuthResponse Create(string token = null)
        {
            return new AuthResponse { Token = token };
        }

        public AuthResponse Fill(string token)
        {
            Token = token;
            return this;
        }

        public override AuthResponse Fail(string message = "")
        {
            return base.Fail(message) as AuthResponse;
        }

        public override AuthResponse FailAndError(string message = "")
        {
            return base.FailAndError(message) as AuthResponse;
        }

        public override AuthResponse Successed(string message = "")
        {
            return base.Successed(message) as AuthResponse;
        }

        public override AuthResponse AddErrors(string error)
        {
            return base.AddErrors(error) as AuthResponse;
        }

        public override AuthResponse AddErrors(List<string> errors)
        {
            return base.AddErrors(errors) as AuthResponse;
        }
    }
}
