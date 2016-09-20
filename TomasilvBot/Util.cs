using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Telegram.Bot.Types;

namespace TomasilvBot {
	class Util {
		private static string logFile = "tomasilv.log";
		private static StreamWriter logger = System.IO.File.AppendText(logFile);

		public static void log(string s) {
			logger.WriteLine("{0}\t{1}", DateTime.Now.ToString(), s);
			logger.Flush();
		}

		private static Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

		public static int randi(int floor = 0, int ceil = 10) {
			return floor < ceil ? rand.Next(floor, ceil) : rand.Next(ceil, floor);
		}

		public static float randf(float floor = 0.0f, float ceil = 1.0f) {
			return (float)rand.NextDouble() * (ceil - floor) + floor;
		}

		public static string rollDice(int value, int count = 1, int add = 0, int take = int.MaxValue) {
			int ceil = value + 1;
			List<int> l = new List<int>();
			string reply = "";
			for (int i = 0; i < count; i++) {
				l.Add(Util.randi(1, ceil));
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

		private static int emoji_min = 0x1F600;
		private static int emoji_max = 0x1F650;
		public static string RandEmoji() {
			return char.ConvertFromUtf32(Util.randi(emoji_min, emoji_max));
		}

		public static List<string> stickers = new List<string>(Assembly.GetExecutingAssembly().GetManifestResourceNames());
		public static FileToSend Sticker(int i) {
			var sticker = stickers[(i - 1) % stickers.Count];
			var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sticker);
			if (fileStream == null) {
				//Console.WriteLine("NULL");
			}
			return new FileToSend(sticker, fileStream);
		}

		public static void DisplayStickers() {
			foreach (var s in stickers) {
				//Console.WriteLine(s);
			}
		}
	}
}
