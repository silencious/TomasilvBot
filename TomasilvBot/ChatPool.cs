using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace TomasilvBot {
	class ChatPool {
		private static TimeCounter timeCounter = new TimeCounter(30, 1000);
		private static Dictionary<long, ChatVO> chatVOs = new Dictionary<long, ChatVO>();
		private static Random rand = new Random();

		private static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict) {
			List<TValue> values = Enumerable.ToList(dict.Values);
			int size = dict.Count;
			while (true) {
				yield return values[rand.Next(size)];
			}
		}

		private static void AddChat(long chatId) {
			var chatVO = new ChatVO();
			chatVO.chatId = chatId;
			chatVO.timeSpan = 1000.0f;
			chatVO.timeCounter = new TimeCounter(20, 60000);
			chatVOs.Add(chatId, chatVO);
		}

		private static ChatVO GetChat(long chatId) {
			if (!chatVOs.ContainsKey(chatId)) AddChat(chatId);
			return chatVOs[chatId];
		}

		public static bool IsIdle(long chatId) {
			return (GetChat(chatId).Count == 0);
		}

		public static void AddMsg(Message msg) {
			long chatId = msg.Chat.Id;
			GetChat(chatId).EnqueueMsg(msg);
		}

		public static Message PickMsg() {
			if (timeCounter.isHot) return null;
			foreach (var chatVO in RandomValues(chatVOs)) {
				if (chatVO.timeCounter.isHot || chatVO.Count <= 0) continue;
				return chatVO.DequeueMsg();
			}
			return null;
		}
	}
}
