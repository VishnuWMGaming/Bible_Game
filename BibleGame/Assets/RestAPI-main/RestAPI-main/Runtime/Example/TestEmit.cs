using System.Collections;
using System.Collections.Generic;
using RestAPI;
using UnityEngine;


public class TestEmit : MonoBehaviour
{
    
    
    
    // Start is called before the first frame update
    void Start()
    {
     //   ApiBase.SetAuthToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImFqYXlrbUBnbWFpbC5jb20iLCJuYW1lIjoiYWpheSIsImFkZHJlc3MiOiJoaXdhcmtoZWQiLCJtb2JpbGUiOiI4OTg5ODk4OTk5IiwiaWF0IjoxNjUzNTU3ODk3LCJleHAiOjE2NTM3NTc4OTd9.Vy6jHF1tmwQeBYtW0d-1Kd9aL5ZKkjMFAolffYoXHzA");
        //CustomAPI.TestEmit(PrintResponse);

        LoginRequestData loginRequestData = new LoginRequestData("ajaykm@gmail.com","1234");
        CustomAPI.LoginUser(loginRequestData,LoginResponseData);


    }

    private void LoginResponseData(bool success, LoginResponse data)
    {
        if (success)
        {
            // get token 
            Debug.LogFormat("Token {0}", data.loginUser);
            ApiBase.SetAuthToken(data.loginUser);
            CustomAPI.TestEmit(PrintResponse);
        }
        else
        {
            // show error popup
        }
    }

    private void PrintResponse(bool sucess, string data)
    {
       Debug.LogFormat("Response {0}", data);
    }
}
