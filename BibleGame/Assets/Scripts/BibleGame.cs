
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BibleGame
{
    public class ServiceURL
    {
        public const string baseURL = "http://52.22.241.165:10032/api/user/";
        public const string signupURL = "register";
    }

    namespace UI
    {
        public static class BibleUI
        {
            public static UIController iController;

            public static void Init(UIController controller)
            {
                iController = controller;
            }
        }
    }

    public static class Actions
    {
        public static Action<CanvasType> ChangePanelActions;
    }

   namespace Data
   {
        public static class AppData
        {
            public static LoginData loginData;
            public static OtpData otpData;
        }

        public  class LoginData
        {
            string email;
            string password;
            string name;

            public string Email => email;
            public string Password => password; 
            public string Name => name; 

            public  LoginData (string email, string password, string name)
            {
                this.email = email;
                this.password = password;
                this.name = name;
            }
        }

        public class OtpData
        {
            string otp;
            public string Otp => otp;

            public OtpData (string otp)
            {
                this.otp = otp;    
            }
        }
   }

    namespace API
    {

       
    }
}
