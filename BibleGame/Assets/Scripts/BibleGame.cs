using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.Events;

namespace BibleGame
{
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
        }

        public  class LoginData
        {
           string email;
           string password;

            public  LoginData (string email, string password)
            {
                this.email = email;
                this.password = password;
            }
        }
   }

    namespace API
    {

        public class  ServiceURL
        {
            public const string baseURL = "http://52.22.241.165:10032/api/user/";
            public const string loginURL = "";
            public const string signupURL = "";
        }

    }
}
