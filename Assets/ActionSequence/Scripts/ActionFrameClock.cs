using UnityEngine;
using System;
using BoomFramework;

namespace ActionSequence{
    /// <summary>
    /// 动作帧时钟
    /// </summary>
    public sealed class ActionFrameClock : MonoSingleton<ActionFrameClock>
    {
        public const int FramesPerSecond = 60;  //每秒帧数
        private const float Step = 1f / FramesPerSecond;    //步进
        private const int MaxTicksPerFrame = 4; //每帧最大tick数(追帧上限)
        private float accumulator;  //累加器
        public static event Action OnTick;   // 订阅者: 动作系统/角色

        private void Update()
        {
            accumulator += Time.deltaTime;
            int guard = 0;                   // 防止过载追帧过多
            while (accumulator >= Step && guard++ < MaxTicksPerFrame)
            {
                accumulator -= Step;
                OnTick?.Invoke();
            }
            if (guard >= MaxTicksPerFrame) {
                Debug.Log($"[{this.GetType().Name}] 累加器: {accumulator}");
                accumulator = 0f; // 丢帧以避免“螺旋死锁”
            }
        }
    }
}