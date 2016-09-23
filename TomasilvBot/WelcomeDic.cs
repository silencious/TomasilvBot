using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace TomasilvBot {
	class WelcomeDic {
		private static SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|TomasilvDB.db");
		private static string table = "Groups";
		private static string getSQL = "select $attr1 from $table where $attr2 = $value";
		private static string setSQL = "replace into $table ($attr1, $attr2) values ($value1, $value2)";
		private static string welcomeMsgAttr = "welcomeMsg";
		public static string GetWelcomeMsg(long groupId) {
			string s = "";
			if (!CheckAndOpenConn(conn)) {
				return s;
			}
			using (SQLiteCommand cmd = new SQLiteCommand(getSQL.Replace("$attr1", welcomeMsgAttr).
				Replace("$table", table).
				Replace("$attr2", "id").
				Replace("$value", groupId.ToString()), conn)) {
				var reader = cmd.ExecuteReader();
				if (reader.Read()) {
					s = reader[welcomeMsgAttr].ToString();
				}
			}
			return s;
		}
		public static bool SetWelcomeMsg(long groupId, string msg) {
			if (!CheckAndOpenConn(conn)) {
				return false;
			}
			using (SQLiteCommand cmd = new SQLiteCommand(setSQL.Replace("$table", table).
				Replace("$attr1", "id").
				Replace("$attr2", welcomeMsgAttr).
				Replace("$value1", groupId.ToString()).
				Replace("$value2", "'" + msg + "'"), conn)) {
				cmd.ExecuteNonQuery();
			}
			return true;
		}
		public static bool Contains(long groupId) {
			return GetWelcomeMsg(groupId).Trim() == "";
		}
		private static bool CheckAndOpenConn(SQLiteConnection c) {
			try {
				c.Open();
				return true;
			} catch (Exception e) {
				Util.log(e.ToString());
				return false;
			}
		}
	}
}
