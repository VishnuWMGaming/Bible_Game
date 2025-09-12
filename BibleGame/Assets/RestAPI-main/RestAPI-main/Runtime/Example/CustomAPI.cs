using System.Collections.Generic;
using Newtonsoft.Json;
using RestAPI;
using UnityEngine;


public class CustomAPI : ApiBase
{
    
    public delegate void TestsCallback(bool sucess, string data);
 public delegate void LoginCallBack(bool success, LoginResponse data);
 
    
    
    public static void TestEmit(TestsCallback callback =null)
    {
        WebRequest(ServiceUrl.BaseURL+ServiceUrl.GetUserDetails,string.Empty, (url, success, data) => ResponseTest(success,data,callback) );
    }

    public static void LoginUser(LoginRequestData requestData ,LoginCallBack loginCallBack)
    {
        string json = JsonConvert.SerializeObject(requestData);
        WebRequest(ServiceUrl.BaseURL + ServiceUrl.Login, json, (url, success, data) => HandleLogin(success,data, loginCallBack));
    }

    private static void HandleLogin(bool aSuccess, object aData, LoginCallBack loginCallBack)
    {
       Debug.LogFormat("LoginResponse {0}",aData);

       if (aSuccess )
       {
           LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(aData.ToString());
           loginCallBack?.Invoke(loginResponse.succeeded,loginResponse);
           

       }
       
    }


    public static void TestEmit2(TestsCallback callback =null)
    {
        WebRequest("http://44.195.125.80:8090/user/get_profile",string.Empty, (url, success, data) => ResponseTest(success,data,callback) );
    }

    private static void ResponseTest( bool asuccess, object adata , TestsCallback callback)
    {
        Debug.LogFormat("Data {0}", adata);
        callback?.Invoke( asuccess,adata.ToString());
    }
}