using System.Threading;
using System.Threading.Tasks;

namespace TomasilvBot {
	class TimeCounter {
		int limit;		// msg number limit
		int time;		// msecs
		int count = 0;

		public TimeCounter(int count, int time) {
			this.limit = count;
			this.time = time;

		}

		public bool Sent() {
			if (isHot) return false;
			count++;
			Task.Run(() => {
				Thread.Sleep(time);
				count--;
			});
			return true;
		}

		public bool isHot {
			get {
				return count > limit;
			}
		}
	}
}
