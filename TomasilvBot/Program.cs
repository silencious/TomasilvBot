using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

namespace TomasilvBot {
    class Program {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("257573874:AAH05EerVbxiTj6wczqiitnHhM2Yp-aqECA");
        private static AutoResetEvent autoEvent = new AutoResetEvent(false);
        private static string logFile = "tomasilv.log";
        private static StreamWriter logger = System.IO.File.AppendText(logFile);

        private static void log(string s) {
            logger.WriteLine("{0}\t{1}", DateTime.Now.ToString(), s);
            logger.Flush();
        }

        private static Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        private static int randi(int floor = 0, int ceil = 10) {
            return floor < ceil ? rand.Next(floor, ceil) : rand.Next(ceil, floor);
        }

        private static float randf(float floor = 0.0f, float ceil = 1.0f) {
            return (float)rand.NextDouble() * (ceil - floor) + floor;
        }

        private static string rollDice(int value, int count = 1, int add = 0, int take = int.MaxValue) {
            int ceil = value + 1;
            List<int> l = new List<int>();
            string reply = "";
            for (int i = 0; i < count; i++) {
                l.Add(randi(1, ceil));
            }
            if (count > take) {
                l = l.OrderByDescending(i => i).Take(take).ToList();
            }
            int acc = l[0];
            reply += acc.ToString();
            for (int i = 1; i < l.Count; i++) {
                acc += l[i];
                reply = reply + '+' + l[i].ToString();
            }
            if (add > 0) {
                acc += add;
                reply = reply + '+' + add.ToString();
            }
            if (count > 1 || add > 0) {
                reply = reply + '=' + acc.ToString();
            }
            return reply;
        }

        private static string[] sentences = {
            @"I am tomasilv",
            @"Welcome to become a Resistance, newbie!",
            @"I am tomasilv, not tolves",
            @"You are not the real tomasilv",
            @"Welcome to join sh_res!",
            @"I am the real tomasilv"};

        private static string werewolf_bot = "werewolfIIbot";

        private static int emoji_min = 0x1F600;
        private static int emoji_max = 0x1F650;
        private static string RandEmoji() {
            return char.ConvertFromUtf32(randi(emoji_min, emoji_max));
        }

        private static List<string> stickers = new List<string>(Assembly.GetExecutingAssembly().GetManifestResourceNames());
        private static FileToSend Sticker(int i) {
            var sticker = stickers[(i - 1) % stickers.Count];
            var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sticker);
            if (fileStream == null) {
                //Console.WriteLine("NULL");
            }
            return new FileToSend(sticker, fileStream);
        }

        private static void DisplayStickers() {
            foreach (var s in stickers) {
                //Console.WriteLine(s);
            }
        }
        private static char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\n', '\r' };

        static void Main(string[] args) {
            stickers.Sort();
            stickers.RemoveAt(0);
            //DisplayStickers();
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            var me = Bot.GetMeAsync().Result;

            //Console.Title = me.Username;
            log("Tomasilv Bot starts");
            Bot.StartReceiving();
            while (true) {
                autoEvent.WaitOne();
            }
            //Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs) {
            //Debugger.Break();
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs) {
            //Console.WriteLine("Received choosen inline result: {0}", chosenInlineResultEventArgs.ChosenInlineResult.ResultId);
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs) {
            //Console.WriteLine("Received inline queary: {0}", inlineQueryEventArgs.InlineQuery.Query.ToString());
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs) {
            var message = messageEventArgs.Message;
            var text = message.Text;
            if (message == null || message.Type != MessageType.TextMessage) return;

            if (text.StartsWith("/clickme")) {
                var reply = @"/ClickMeToBecomeTomasilv";
                await Bot.SendTextMessageAsync(message.Chat.Id, reply + RandEmoji());
                return;
            } else if (text.StartsWith("/ClickMeToBecomeTomasilv")) {
                var username = message.From.Username;
                if ("tomasilv".Equals(username, StringComparison.OrdinalIgnoreCase)) {
                    var reply = "You are the real tomasilv @" + username;
                    await Bot.SendTextMessageAsync(message.Chat.Id, reply + RandEmoji());
                } else if (username == null || username.Trim() == "") {
                    var reply = "Ah, you don't have a username!\nIt's a shame you can't become tomasilv";
                    await Bot.SendTextMessageAsync(message.Chat.Id, reply + RandEmoji());
                } else {
                    string reply;
                    switch (randi(0, 5)) {
                        case 1:
                            reply = "You are not tomasilv, @" + username;
                            await Bot.SendTextMessageAsync(message.Chat.Id, reply);
                            break;
                        case 2:
                            //Console.WriteLine(17);
                            await Bot.SendStickerAsync(message.Chat.Id, Sticker(17));
                            break;
                        case 3:
                            //Console.WriteLine(28);
                            await Bot.SendStickerAsync(message.Chat.Id, Sticker(28));
                            break;
                        case 4:
                            //Console.WriteLine(30);
                            await Bot.SendStickerAsync(message.Chat.Id, Sticker(30));
                            break;
                        default:
                            reply = "You are @" + username + ", not tomasilv";
                            await Bot.SendTextMessageAsync(message.Chat.Id, reply);
                            break;
                    }
                }
                return;
            } else if (text.StartsWith("/join")) {
                var reply = @"/join@" + werewolf_bot;
                await Bot.SendTextMessageAsync(message.Chat.Id, reply);
                return;
            } else if (text.StartsWith("/lick")) {
                await Bot.SendStickerAsync(message.Chat.Id, Sticker(34));
                return;
            } else if (text.StartsWith("/randf")) {    // rand float
                string[] words = text.Split(delimiterChars);
                float min, max;
                if (words.Length >= 3 && float.TryParse(words[1], out min) && float.TryParse(words[2], out max)) {
                    float result = randf(min, max);
                    int accuracy;
                    if (!int.TryParse(words[0].Substring("/randf".Length), out accuracy)) {
                        accuracy = 2;
                    }
                    await Bot.SendTextMessageAsync(message.Chat.Id, result.ToString("F" + accuracy));
                } else {
                    // display help message or ui 
                    var reply = "use: /randf(n) min max";
                    await Bot.SendTextMessageAsync(message.Chat.Id, reply);
                }
                return;
            } else if (text.StartsWith("/rand")) {  // rand int
                string[] words = text.Split(delimiterChars);
                int min, max;
                if (words.Length >= 3 && int.TryParse(words[1], out min) && int.TryParse(words[2], out max)) {
                    int result = randi(min, max);
                    await Bot.SendTextMessageAsync(message.Chat.Id, result.ToString());
                } else {
                    // display help message or ui 
                    var reply = "use: /rand min max";
                    await Bot.SendTextMessageAsync(message.Chat.Id, reply);
                }
                return;
            } else if (text.StartsWith("/vote")) {
                var reply = @"/vote" + message.From.Username;
                await Bot.SendTextMessageAsync(message.Chat.Id, reply + RandEmoji());
                return;
            } else if (Regex.IsMatch(text, @"^/\d*d\d+(k\d+)?(\+\d+)?")) {
                char[] delimiter = { '/', 'd', 'k', '+' };
                string[] args = text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                string reply = "Invalid dice format, use examples: 2d4, d20, 3d2+1, 3d20k1";
                int value, count;
                switch (args.Length) {
                    case 1:
                        if (int.TryParse(args[0], out value)) {
                            reply = rollDice(value);
                        }
                        break;
                    case 2:
                        if (int.TryParse(args[0], out count) && int.TryParse(args[1], out value)) {
                            reply = rollDice(value, count);
                        }
                        break;
                    case 3:
                        int arg2;
                        if (int.TryParse(args[0], out count) && int.TryParse(args[1], out value) && int.TryParse(args[2], out arg2)) {
                            if (text.Contains('k')) {
                                reply = rollDice(value, count, 0, arg2);
                            } else if (text.Contains('+')) {
                                reply = rollDice(value, count, arg2, count);
                            }
                        }
                        break;
                    case 4:
                        int take, add;                        
                        if (int.TryParse(args[0], out count) && int.TryParse(args[1], out value) && int.TryParse(args[2], out take) && int.TryParse(args[3], out add)) {
                            reply = rollDice(value, count, add, take);
                        }
                        break;
                }
                await Bot.SendTextMessageAsync(message.Chat.Id, reply);
                return;
            }
            // no matching pattern, default reply
            var r = randi(1, stickers.Count + sentences.Length);
            if (r < stickers.Count) {
                await Bot.SendStickerAsync(message.Chat.Id, Sticker(r));
            } else {
                await Bot.SendTextMessageAsync(message.Chat.Id, sentences[r - stickers.Count] + RandEmoji());
            }
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs) {
            var s = "Received " + callbackQueryEventArgs.CallbackQuery.Data;
            await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, s);
        }
    }
}
