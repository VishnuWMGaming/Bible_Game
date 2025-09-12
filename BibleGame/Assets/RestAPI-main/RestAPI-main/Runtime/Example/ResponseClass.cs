
    
public class LoginRequestData
{
    public string username;
    public string password;

    public LoginRequestData(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

    public class ResponseBase
    {
        public int ResponseCode;
        public string ResponseMessage;
        public bool succeeded;
    }

    public class LoginResponse : ResponseBase
    {
        public string loginUser;
    }
    