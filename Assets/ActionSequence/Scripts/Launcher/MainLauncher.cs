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
            // 配置为目标 60fps（渲染）并同步物理步长
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Time.fixedDeltaTime = 1f / 60f;

            Debug.Log($"[{this.GetType().Name}] 游戏启动，目标帧率=60");
        }
    }
}
