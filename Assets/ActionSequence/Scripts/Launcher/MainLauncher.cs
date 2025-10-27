using System.Collections;
using System.Collections.Generic;
using BoomFramework;
using UnityEngine;

namespace ActionSequence
{
    public class MainLauncher : ILauncher
    {
        public void Launch()
        {
            Debug.Log($"[{this.GetType().Name}] 游戏启动");
        }
    }
}
