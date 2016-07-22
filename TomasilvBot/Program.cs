using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

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
        private static Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        private static int emoji_min = 0x1F600;
        private static int emoji_max = 0x1F650;
        private static string RandEmoji() {
            return char.ConvertFromUtf32(rand.Next(emoji_min, emoji_max));
        }

        private static List<string> stickers = new List<string>(Assembly.GetExecutingAssembly().GetManifestResourceNames());

        private static string[] sentences = {
            @"I am tomasilv",
            @"Welcome to become a Resistance, newbie!",
            @"I am tomasilv, not tolves",
            @"You are not the real tomasilv",
            @"Welcome to join sh_res!",
            @"I am the real tomasilv"};

        private static FileToSend Sticker(int i) {
            var sticker = stickers[(i - 1) % stickers.Count];
            var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sticker);
            if (fileStream == null) {
                Console.WriteLine("NULL");
            }
            return new FileToSend(sticker, fileStream);
        }

        private static void DisplayStickers() {
            foreach (var s in stickers) {
                Console.WriteLine(s);
            }
        }

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

            Console.Title = me.Username;
            Console.WriteLine("Tomasilv Bot starts");
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs) {
            Debugger.Break();
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs) {
            Console.WriteLine("Received choosen inline result: {0}", chosenInlineResultEventArgs.ChosenInlineResult.ResultId);
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs){
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs) {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            if (message.Text.StartsWith("/clickme")) {
                var reply = @"/ClickMeToBecomeTomasilv";
                await Bot.SendTextMessageAsync(message.Chat.Id, reply, replyMarkup: new ReplyKeyboardHide());
            } else if (message.Text.StartsWith("/ClickMeToBecomeTomasilv")) {
                var username = message.From.Username;
                if ("tomasilv".Equals(username, StringComparison.OrdinalIgnoreCase)) {
                    var reply = "You are the real tomasilv";
                    await Bot.SendTextMessageAsync(message.Chat.Id, reply, replyMarkup: new ReplyKeyboardHide());
                } else if (username == null || username.Trim() == "") {
                    var reply = "Ah, you don't have a username!\nIt's a shame you can't become tomasilv";
                    await Bot.SendTextMessageAsync(message.Chat.Id, reply, replyMarkup: new ReplyKeyboardHide());
                } else {
                    string reply;
                    switch (rand.Next(0, 5)) {
                        case 1:
                            reply = "You are not tomasilv, " + username;
                            await Bot.SendTextMessageAsync(message.Chat.Id, reply, replyMarkup: new ReplyKeyboardHide());
                            break;
                        case 2:
                            //Console.WriteLine(17);
                            await Bot.SendStickerAsync(message.Chat.Id, Sticker(17), replyMarkup: new ReplyKeyboardHide());
                            break;
                        case 3:
                            //Console.WriteLine(28);
                            await Bot.SendStickerAsync(message.Chat.Id, Sticker(28), replyMarkup: new ReplyKeyboardHide());
                            break;
                        case 4:
                            //Console.WriteLine(30);
                            await Bot.SendStickerAsync(message.Chat.Id, Sticker(30), replyMarkup: new ReplyKeyboardHide());
                            break;
                        default:
                            reply = "You are " + username + ", not tomasilv";
                            await Bot.SendTextMessageAsync(message.Chat.Id, reply, replyMarkup: new ReplyKeyboardHide());
                            break;
                    }
                }
            } else if (message.Text.StartsWith("/join")) {
                var reply = @"/join@werewolfIIbot";
                await Bot.SendTextMessageAsync(message.Chat.Id, reply, replyMarkup: new ReplyKeyboardHide());
            } else if (message.Text.StartsWith("/vote")) {
                var reply = @"/vote" + message.From.Username;
                await Bot.SendTextMessageAsync(message.Chat.Id, reply + RandEmoji(), replyMarkup: new ReplyKeyboardHide());
            } else if (message.Text.Contains("Lick")) {
                await Bot.SendStickerAsync(message.Chat.Id, Sticker(34), replyMarkup: new ReplyKeyboardHide());
            } else {
                var r = rand.Next(1, stickers.Count + sentences.Length);
                if (r < stickers.Count) {
                    await Bot.SendStickerAsync(message.Chat.Id, Sticker(r), replyMarkup: new ReplyKeyboardHide());
                } else {
                    await Bot.SendTextMessageAsync(message.Chat.Id, sentences[r - stickers.Count] + RandEmoji(), replyMarkup: new ReplyKeyboardHide());
                }
            }

            /*else if (message.Text.StartsWith("/photo")) // send a photo
                                              {
                                        await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                                        const string file = @"<FilePath>";

                                        var fileName = file.Split('\\').Last();

                                        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                                            var fts = new FileToSend(fileName, fileStream);

                                            await Bot.SendPhotoAsync(message.Chat.Id, fts, "Nice Picture");
                                        }
                                    } 
                        */
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs) {
            var s = "Received " + callbackQueryEventArgs.CallbackQuery.Data;
            await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, s);
        }
    }
}
