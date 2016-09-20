using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace TomasilvBot {
	class ChatVO {
		public long chatId = 0;
		public float timeSpan = 1000.0f;	//miliseconds
		public TimeCounter timeCounter;
		DateTimeOffset lastMsgTimestamp = DateTimeOffset.UtcNow;
		Queue<Message> msgQueue = new Queue<Message>();

		public int Count {
			get {
				return msgQueue.Count;
			}
		}

		public void EnqueueMsg(Message msg) {
			msgQueue.Enqueue(msg);
		}

		public Message DequeueMsg() {
			if (msgQueue.Count <= 0) return null;
			DateTimeOffset currTimestamp = DateTimeOffset.UtcNow;
			if (currTimestamp.Subtract(lastMsgTimestamp).TotalMilliseconds < timeSpan) return null;
			lastMsgTimestamp = currTimestamp;
			return msgQueue.Dequeue();
		}
	}
}
