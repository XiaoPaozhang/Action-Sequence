using UnityEngine;

namespace ActionSequence
{
	/// <summary>
	/// 全局动作锁：在前摇/生效/后摇期间禁止玩家移动输入
	/// 使用计数方式，支持嵌套 Acquire/Release
	/// </summary>
	public static class ActionLock
	{
		private static int s_lockCounter = 0;

		public static bool IsLocked => s_lockCounter > 0;

		public static void Acquire()
		{
			s_lockCounter++;
		}

		public static void Release()
		{
			if (s_lockCounter > 0) s_lockCounter--;
		}

		public static void Reset()
		{
			s_lockCounter = 0;
		}
	}
}


