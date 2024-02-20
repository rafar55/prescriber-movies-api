namespace MoviesDb.Domain.Exceptions;

public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException() : base("Email or password is incorrect")
    {
    }
}
