using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        static void Main(string[] args) {
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

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

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs) {
            InlineQueryResult[] results = {
              new InlineQueryResultLocation
              {
                  Id = "1",
                  Latitude = 40.7058316f, // displayed result
                  Longitude = -74.2581888f,
                  Title = "New York",
                  InputMessageContent = new InputLocationMessageContent // message if result is selected
                  {
                      Latitude = 40.7058316f,
                      Longitude = -74.2581888f,
                  }
              },

              new InlineQueryResultLocation
              {
                  Id = "2",
                  Longitude = 52.507629f, // displayed result
                  Latitude = 13.1449577f,
                  Title = "Berlin",
                  InputMessageContent = new InputLocationMessageContent // message if result is selected
                  {
                      Longitude = 52.507629f,
                      Latitude = 13.1449577f
                  }
              }
          };

            await Bot.AnswerInlineQueryAsync(inlineQueryEventArgs.InlineQuery.Id, results, isPersonal: true, cacheTime: 0);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs) {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            if (message.Text.StartsWith("/clickme")) {
                var reply = @"/ClickMeToBecomeTomasilv";
                await Bot.SendTextMessageAsync(message.Chat.Id, reply,
                    replyMarkup: new ReplyKeyboardHide());
            } else if(message.Text.StartsWith("/ClickMeToBecomeTomasilv")){
                var reply = @"You are not the real tomasilv";
                await Bot.SendTextMessageAsync(message.Chat.Id, reply,
                    replyMarkup: new ReplyKeyboardHide());
            }else if (message.Text.StartsWith("/welcome")) {
                var r = rand.Next(0, 4);
                string[] replies = {@"Welcome to become a Resistance, newbie!",
                                       @"I am tomasilv, not tolves",
                                       @"Welcome to join sh_res!",
                                       @"I am the real tomasilv"};
                await Bot.SendTextMessageAsync(message.Chat.Id, replies[r],
                replyMarkup: new ReplyKeyboardHide());
            } else if (message.Text.StartsWith("/join")) {
                var reply = @"/join@werewolfIIbot";
                await Bot.SendTextMessageAsync(message.Chat.Id, reply,
                    replyMarkup: new ReplyKeyboardHide());
            } else if (message.Text.StartsWith("/vote")) {
                var reply = @"/vote"+((User)sender).Username;
                await Bot.SendTextMessageAsync(message.Chat.Id, reply,
                    replyMarkup: new ReplyKeyboardHide());
            }
            else {
                var r = rand.Next(0, 4);
                string[] replies = {@"I am tomasilv",
                                       @"I am tomasilv, not tolves",
                                       @"You are not the real tomasilv",
                                       @"I am the real tomasilv"};
                await Bot.SendTextMessageAsync(message.Chat.Id, replies[r],
                replyMarkup: new ReplyKeyboardHide());
            }
            /*            if (message.Text.StartsWith("/inline")) // send inline keyboard
                                  {
                            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                            var keyboard = new InlineKeyboardMarkup(new[]
                                      {
                                          new[] // first row
                                          {
                                              new InlineKeyboardButton("1.1"),
                                              new InlineKeyboardButton("1.2"),
                                          },
                                          new[] // second row
                                          {
                                              new InlineKeyboardButton("2.1"),
                                              new InlineKeyboardButton("2.2"),
                                          }
                                      });

                            await Task.Delay(500); // simulate longer running task

                            await Bot.SendTextMessageAsync(message.Chat.Id, "Choose",
                                replyMarkup: keyboard);
                        } else if (message.Text.StartsWith("/keyboard")) // send custom keyboard
                                  {
                            var keyboard = new ReplyKeyboardMarkup(new[]
                                      {
                                          new [] // first row
                                          {
                                              new KeyboardButton("1.1"),
                                              new KeyboardButton("1.2"),
                                          },
                                          new [] // last row
                                          {
                                              new KeyboardButton("2.1"),
                                              new KeyboardButton("2.2"),
                                          }
                                      });

                            await Bot.SendTextMessageAsync(message.Chat.Id, "Choose",
                                replyMarkup: keyboard);
                        } else if (message.Text.StartsWith("/photo")) // send a photo
                                  {
                            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                            const string file = @"<FilePath>";

                            var fileName = file.Split('\\').Last();

                            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                                var fts = new FileToSend(fileName, fileStream);

                                await Bot.SendPhotoAsync(message.Chat.Id, fts, "Nice Picture");
                            }
                        } else if (message.Text.StartsWith("/request")) // request location or contact
                                  {
                            var keyboard = new ReplyKeyboardMarkup(new[]
                                      {
                                          new KeyboardButton("Location")
                                          {
                                              RequestLocation = true
                                          },
                                          new KeyboardButton("Contact")
                                          {
                                              RequestContact = true
                                          },
                                      });

                            await Bot.SendTextMessageAsync(message.Chat.Id, "Who or Where are you?", replyMarkup: keyboard);
                        } else {
                            var usage = @"Usage:
                        /inline   - send inline keyboard
                        /keyboard - send custom keyboard
                        /photo    - send a photo
                        /request  - request location or contact
                        ";

                            await Bot.SendTextMessageAsync(message.Chat.Id, usage,
                                replyMarkup: new ReplyKeyboardHide());
                        }
            */
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs) {
            var s = "Received " + callbackQueryEventArgs.CallbackQuery.Data;
            await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, s);
        }
    }
}
