
using BibleGame.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using static BibleGame.API.GetBiblesAPI;


namespace BibleGame
{
    public class ServiceURL
    {
        public const string baseURL = "http://52.22.241.165:10032/api/user/";
        public const string imageURL = "http://52.22.241.165:10032/";
        public const string signupURL = "register";
        public const string deleteProfile = "delete_profile";
        public const string login = "login";
        public const string verify = "verify_otp";
        public const string profileName = "update_profile";
        public const string updateProfilePic = "update_profile_pic";
        public const string forgetPassword = "forget_password";
        public const string updatePassword = "update_new_password";
        public const string resendOtp = "resend_otp";
        public const string getProfile = "get_profile";
        public const string setProfilePic = "update_profile_pic";
        public const string getChapters = "get-book-chapters";
        public const string getChapterDetail = "get-chapters-details";
        public const string getQuestions = "get-questions";
        public const string submitAnswer = "answer-the-questions";
        public const string getBible = "get-bibles";
        public const string getBibleDetail = "get-bible-details";
        public const string getBookDetail = "get-book-list";
        public const string createStreak = "create-streak";
        public const string getStreak = "get-streaks";
        public const string getStreakDetail = "get-streak-details";
        public const string getTrans = "translate";
        public const string getLeaderBoard = "get-leaderboard";
        public const string getFreeHint = "get_free_hint";
        public const string deductFreeHint = "deduct_hint";


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
        public static Action<StartPage> StartPageAction;
        public static Action<Action> UpdateText;
        public static Action<int> UpdateCoins;

        public static Action<bool> ResetBoxPosAction;

        public static Action ApplyStyleAction;

        public static Action<MScreenOriatation,bool,bool> ChangeLandscape;
    }

   namespace Data
   {
        public static class AppData
        {
            public static LoginData loginData;
            public static OTPData mforgetotpData;
            public static OTPData mSignOtpData;

            public static OTPType otpPage;

            public static StartPage mCurrentPage;

            public static int coins;
            public static int mSavedcoins;

            public static List<BookData> bookDatas;
            public static Language mLanguage;

            public static MScreenOriatation orientation; 
        }

        public  class LoginData
        {
            string email;
            string password;
            string name;
            string churchName;
            Sprite pic;

            public string Email => email;
            public string Password => password; 
            public string Name => name;
            public string Church => churchName;
            public Sprite Pic => pic;

            public  LoginData (string email, string password, string name,string church,Sprite pic = null)
            {
                this.email = email;
                this.password = password;
                this.name = name;
                this.churchName = church;
                this.pic = pic;
            }

            public void UpdateSprite(Sprite pic)
            {
                this.pic = pic;
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

        public class UserData
        {
            public static string bibleId;
            public static string bibleName;
            public static string bookName;
            public static string bookId;
            public static string chapterId;
            public static string gameid;
            public static int coins;

            public static AgeGroup currentAge;
            public static Testament testament;
        }
   }


    namespace Utility
    {
        public static class Utils
        {
            public static string ConvertHtmlToPlainText(string html)
            {

                if (string.IsNullOrEmpty(html))
                    return "";

                // 1. Convert verse numbers to bold format (or just keep them with line break)
                html = Regex.Replace(html,
                    @"<span[^>]*data-number\s*=\s*""(\d+)""[^>]*>.*?</span>",
                    match => $"\n{match.Groups[1].Value} ");

                // 2. Replace added text (like <span class="add">was</span>) with plain content
                html = Regex.Replace(html, @"<span class=""add"">(.*?)</span>", "$1");

                // 3. Remove all remaining HTML tags
                html = Regex.Replace(html, @"<[^>]+>", "");

                // 4. Decode HTML entities properly
                html = WebUtility.HtmlDecode(html);

                // 5. Normalize spacing and line breaks
                html = Regex.Replace(html, @"[ \t\r]+", " ");       // remove extra spaces/tabs
                html = Regex.Replace(html, @"\n\s*", "\n");         // trim spaces after line breaks
                html = Regex.Replace(html, @"\n{2,}", "\n");        // remove multiple blank lines
                html = html.Trim();

                html = html.Replace("¶", "");

                return html;
            }

            public static string GetImageMimeType(string extension)
            {
                switch (extension)
                {
                    case ".jpg":
                    case ".jpeg":
                        return "image/jpeg";
                    case ".png":
                        return "image/png";
                    default:
                        return "UnknownImage; // fallback for unknown types";
                }
            }

            public static Sprite LoadSpriteFromBytes(byte[] imageData)
            {
                Texture2D texture = new Texture2D(2, 2); // Dummy size; will be replaced by actual data
                if (texture.LoadImage(imageData))
                {
                    return Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                }
                return null;
            }
        }
    }
}
