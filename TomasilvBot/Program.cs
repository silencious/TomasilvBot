using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TomasilvBot {
	class Program {
		private static readonly TelegramBotClient Bot = new TelegramBotClient("257573874:AAH05EerVbxiTj6wczqiitnHhM2Yp-aqECA");
		private static AutoResetEvent autoEvent = new AutoResetEvent(false);

		private static string[] sentences = {
            @"I am tomasilv",
            @"Welcome to become a Resistance, newbie!",
            @"I am tomasilv, not tolves",
            @"You are not the real tomasilv",
            @"Welcome to join sh_res!",
            @"I am the real tomasilv"};

		private static string werewolf_bot = "werewolfIIbot";

		private static char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\n', '\r' };

		static void Main(string[] args) {
			Util.stickers.Sort();
			Util.stickers.RemoveAt(0);
			//DisplayStickers();
			Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
			Bot.OnMessage += BotOnMessageReceived;
			Bot.OnMessageEdited += BotOnMessageReceived;
			Bot.OnInlineQuery += BotOnInlineQueryReceived;
			Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
			Bot.OnReceiveError += BotOnReceiveError;

			var me = Bot.GetMeAsync().Result;

			//Console.Title = me.Username;
			Util.log("Tomasilv Bot starts");
			Bot.StartReceiving();
			while (true) {
				try {
					autoEvent.WaitOne();
				} catch (Exception e) {
					Util.log(e.ToString());
				}
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
			if (message == null) return;

			if (!ChatPool.IsIdle(message.Chat.Id)) {
				ChatPool.AddMsg(message);
				message = ChatPool.PickMsg();
				if (message == null) {
					Thread.Sleep(34);	// about 30 msgs per sec
				}
			}

			switch (message.Type) {
				case MessageType.ServiceMessage:
					DoServiceMessage(message);
					break;
				case MessageType.TextMessage:
					DoTextMessage(message);
					break;
				default:
					break;
			}
		}

		private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs) {
			var s = "Received " + callbackQueryEventArgs.CallbackQuery.Data;
			await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, s);
		}
		private static async void DoServiceMessage(Message message) {
			if (message.NewChatMember != null) {
				var user = message.NewChatMember;
				string reply = "/welcome @" + user.Username;
				await Bot.SendTextMessageAsync(message.Chat.Id, reply + Util.RandEmoji());
			}
		}

		private static async void DoTextMessage(Message message) {
			var text = message.Text;
			if (text.StartsWith("/clickme")) {
				var reply = @"/ClickMeToBecomeTomasilv";
				await Bot.SendTextMessageAsync(message.Chat.Id, reply + Util.RandEmoji());
				return;
			} else if (text.StartsWith("/ClickMeToBecomeTomasilv")) {
				ClickMe(message);
				return;
			} else if (text.StartsWith("/join")) {
				var reply = @"/join@" + werewolf_bot;
				await Bot.SendTextMessageAsync(message.Chat.Id, reply);
				return;
			} else if (text.StartsWith("/lick")) {
				await Bot.SendStickerAsync(message.Chat.Id, Util.Sticker(34));
				return;
			} else if (text.StartsWith("/randf")) {    // rand float
				string[] words = text.Split(delimiterChars);
				float min, max;
				if (words.Length >= 3 && float.TryParse(words[1], out min) && float.TryParse(words[2], out max)) {
					float result = Util.randf(min, max);
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
					int result = Util.randi(min, max);
					await Bot.SendTextMessageAsync(message.Chat.Id, result.ToString());
				} else {
					// display help message or ui 
					var reply = "use: /rand min max";
					await Bot.SendTextMessageAsync(message.Chat.Id, reply);
				}
				return;
			} else if (text.StartsWith("/vote")) {
				var reply = @"/vote" + message.From.Username;
				await Bot.SendTextMessageAsync(message.Chat.Id, reply + Util.RandEmoji());
				return;
			} else if (Regex.IsMatch(text, @"^/\d*d\d+(k\d+)?(\+\d+)?")) {
				RollDice(message);
				return;
			} else {
				// no matching pattern, default reply
				var r = Util.randi(1, Util.stickers.Count + sentences.Length);
				if (r < Util.stickers.Count) {
					await Bot.SendStickerAsync(message.Chat.Id, Util.Sticker(r), false, message.MessageId);
				} else {
					await Bot.SendTextMessageAsync(message.Chat.Id, sentences[r - Util.stickers.Count] + Util.RandEmoji(), false, false, message.MessageId);
				}
			}
		}

		private static async void ClickMe(Message message) {
			var username = message.From.Username;
			if ("tomasilv".Equals(username, StringComparison.OrdinalIgnoreCase)) {
				var reply = "You are the real tomasilv @" + username;
				await Bot.SendTextMessageAsync(message.Chat.Id, reply + Util.RandEmoji(), false, false, message.MessageId);
			} else if (username == null || username.Trim() == "") {
				var reply = "Ah, you don't have a username!\nIt's a shame you can't become tomasilv";
				await Bot.SendTextMessageAsync(message.Chat.Id, reply + Util.RandEmoji(), false, false, message.MessageId);
			} else {
				string reply;
				switch (Util.randi(0, 5)) {
					case 1:
						reply = "You are not tomasilv, @" + username;
						await Bot.SendTextMessageAsync(message.Chat.Id, reply, false, false, message.MessageId);
						break;
					case 2:
						//Console.WriteLine(17);
						await Bot.SendStickerAsync(message.Chat.Id, Util.Sticker(17), false, message.MessageId);
						break;
					case 3:
						//Console.WriteLine(28);
						await Bot.SendStickerAsync(message.Chat.Id, Util.Sticker(28), false, message.MessageId);
						break;
					case 4:
						//Console.WriteLine(30);
						await Bot.SendStickerAsync(message.Chat.Id, Util.Sticker(30), false, message.MessageId);
						break;
					default:
						reply = "You are @" + username + ", not tomasilv";
						await Bot.SendTextMessageAsync(message.Chat.Id, reply, false, false, message.MessageId);
						break;
				}
			}
		}

		private static async void RollDice(Message message) {
			string text = message.Text;
			char[] delimiter = { '/', 'd', 'k', '+' };
			string[] args = text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
			string reply = "Invalid dice format, use examples: 2d4, d20, 3d2+1, 3d20k1";
			int value, count;
			switch (args.Length) {
				case 1:
					if (int.TryParse(args[0], out value)) {
						reply = Util.rollDice(value);
					}
					break;
				case 2:
					if (int.TryParse(args[0], out count) && int.TryParse(args[1], out value)) {
						reply = Util.rollDice(value, count);
					}
					break;
				case 3:
					int arg2;
					if (int.TryParse(args[0], out count) && int.TryParse(args[1], out value) && int.TryParse(args[2], out arg2)) {
						if (text.Contains('k')) {
							reply = Util.rollDice(value, count, 0, arg2);
						} else if (text.Contains('+')) {
							reply = Util.rollDice(value, count, arg2, count);
						}
					}
					break;
				case 4:
					int take, add;
					if (int.TryParse(args[0], out count) && int.TryParse(args[1], out value) && int.TryParse(args[2], out take) && int.TryParse(args[3], out add)) {
						reply = Util.rollDice(value, count, add, take);
					}
					break;
			}
			await Bot.SendTextMessageAsync(message.Chat.Id, reply);
		}
	}
}
