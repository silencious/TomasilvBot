using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomasilvBot {
	class WelcomeDic {
		private static Dictionary<long, string> dic = new Dictionary<long, string>();
		public static string GetWelcomeMsg(long groupId) {
			string s = "";
			dic.TryGetValue(groupId, out s);
			return s;
		}
		public static void SetWelcomeMsg(long groupId, string msg){
			dic[groupId] = msg; 
		}
		public static bool Contains(long groupId) {
			return dic.ContainsKey(groupId);
		}
	}
}
