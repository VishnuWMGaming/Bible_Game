
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
        public const string login = "login";
        public const string verify = "verify_otp";
        public const string profileName = "update_profile";
        public const string forgetPassword = "forget_password";
        public const string updatePassword = "update_new_password";
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
            public static OTPData otpData;
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

        public enum OTPType { sign ,forget};

        public class OTPData
        {
            string otp;
            public string Otp => otp;

            OTPType type;
            public OTPType OTPType => type;

            public OTPData (string otp,OTPType oTPType)
            {
                this.otp = otp;
                this.type = oTPType;
            }
        }
   }

}
