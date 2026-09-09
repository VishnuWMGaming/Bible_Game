
using BibleGame.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static BibleGame.API.GetBiblesAPI;


namespace BibleGame
{
    public class ServiceURL
    {
        public const string baseURL = "https://indoredev.webmobrildemo.com:10002/api/user/" /*"https://52.22.241.165:10032/api/user/"*/;
        public const string imageURL = /*"https://52.22.241.165:10032/"*/"https://indoredev.webmobrildemo.com:10002/";

        public const string voiceURL = "https://indoredev.webmobrildemo.com:10002/api/tts";

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
        public const string version = "get-latest-app-verson";
        public const string userInfo = "get-leaderboard-user-details";

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

        public static Action VersionCheck;

        public static Action<bool> ResetBoxPosAction;

        public static Action ApplyStyleAction;

        public static Action<MScreenOriatation,bool,bool> ChangeLandscape;

        public static Action<VidLang> SetVidLang;

        public static Action<string> SetAsset;
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

            public static string mCurrentVidChapter;

            public static VidLang mVidLang;
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

        public static class Dialogue
        {
            private static string _text;
            private static int _pointer;

            public static int answerKey;

            private static List<IOptData> optiondatas = new List<IOptData>();

            public static string FormatBoldText(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return text;

                return Regex.Replace(text, @"\*\*(.*?)\*\*", "<b>$1</b>");
            }

            public static void Initialize(TextAsset textAsset)
            {
                _text = textAsset.text;
                _pointer = 0;
                answerKey = -1;
            }

            public static BotChatData ReadNextBot()
            {
                if (string.IsNullOrEmpty(_text))
                    return null;

                int start = _text.IndexOf("{bot}", _pointer);
                if (start == -1)
                    return null;

                start += "{bot}".Length;

                int end = _text.IndexOf("{/bot}", start);
                if (end == -1)
                    return null;

                BotChatData data = new()
                {
                    data = _text.Substring(start, end - start).Trim()
                };

                data.data = FormatBoldText(data.data);

                // Move pointer after {/bot}
                _pointer = end + "{/bot}".Length;

                // Read options until the next bot
                while (true)
                {
                    int optionStart = _text.IndexOf("{option}", _pointer);
                    int nextBot = _text.IndexOf("{bot}", _pointer);

                    if (optionStart == -1 || (nextBot != -1 && optionStart > nextBot))
                    {
                        if (nextBot != -1)
                            _pointer = nextBot;

                        break;
                    }

                    optionStart += "{option}".Length;

                    int optionEnd = _text.IndexOf("{/option}", optionStart);
                    if (optionEnd == -1)
                        break;

                    string option = _text.Substring(optionStart, optionEnd - optionStart);

                    option = FormatBoldText(option);

                    int closeBracket = option.IndexOf(']');

                    data.mOptions.Add(new IOptData
                    {
                        index = int.Parse(option.Substring(1, closeBracket - 1)),
                        value = option.Substring(closeBracket + 1).Trim()
                    });

                    _pointer = optionEnd + "{/option}".Length;
                }

                return data;
            }

            public static BotChatData ReadBotByOption(int optionKey)
            {
                if (optionKey == -1)
                {
                    return ReadNextBot();
                }

                if (string.IsNullOrEmpty(_text))
                    return null;

                string tag = $"[opt]{optionKey}[/opt]";

                while (true)
                {
                    int botStart = _text.IndexOf("{bot}", _pointer);
                    if (botStart == -1)
                        return null;

                    int contentStart = botStart + "{bot}".Length;
                    int botEnd = _text.IndexOf("{/bot}", contentStart);
                    if (botEnd == -1)
                        return null;

                    string botText = _text.Substring(contentStart, botEnd - contentStart).Trim();

                    // -----------------------------
                    // Matching branch
                    // -----------------------------
                    if (botText.StartsWith(tag))
                    {
                        botText = botText.Substring(tag.Length).Trim();

                        BotChatData data = new()
                        {
                            data = botText
                        };

                        _pointer = botEnd + "{/bot}".Length;

                        ReadOptions(data);

                        return data;
                    }
                    else
                    {
                        if (!botText.StartsWith("[opt]"))
                        {
                            BotChatData data = new()
                            {
                                data = botText
                            };

                            _pointer = botEnd + "{/bot}".Length;

                            ReadOptions(data);

                            return data;
                        }

                        _pointer = botEnd + "{/bot}".Length;
                        continue;
                    }

                    // Skip everything until matching branch
                    _pointer = botEnd + "{/bot}".Length;
                }
            }

            private static void ReadOptions(BotChatData data)
            {
                while (true)
                {
                    int optionStart = _text.IndexOf("{option}", _pointer);
                    int nextBot = _text.IndexOf("{bot}", _pointer);

                    if (optionStart == -1 || (nextBot != -1 && optionStart > nextBot))
                    {
                        if (nextBot != -1)
                            _pointer = nextBot;

                        break;
                    }

                    optionStart += "{option}".Length;

                    int optionEnd = _text.IndexOf("{/option}", optionStart);
                    if (optionEnd == -1)
                        break;

                    string option = _text.Substring(optionStart, optionEnd - optionStart);

                    int closeBracket = option.IndexOf(']');

                    data.mOptions.Add(new IOptData
                    {
                        index = int.Parse(option.Substring(1, closeBracket - 1)),
                        value = option.Substring(closeBracket + 1).Trim()
                    });

                    _pointer = optionEnd + "{/option}".Length;
                }
            }

            public static List<IOptData> GetOptions()
            {
                return optiondatas;
            }

            public static void Reset()
            {
                _pointer = 0;
            }

            public static bool HasNextBot()
            {
                return !string.IsNullOrEmpty(_text) &&
                       _text.IndexOf("{bot}", _pointer) != -1;
            }

            public static int Pointer => _pointer;

            public static string NewLineAlignment(string text)
            {
                return string.IsNullOrEmpty(text) ? text : text.Replace("[/n]", "\n");
            }
        }
    }
}
