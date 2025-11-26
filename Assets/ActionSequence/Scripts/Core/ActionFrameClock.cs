using System;
using System.Collections;
using System.Collections.Generic;
using BoomFramework;
using UnityEngine;

namespace ActionSequence
{
    public class ActionFrameClock : MonoSingleton<ActionFrameClock>
    {
        private const int ExpectationFrame = 60;                        // 理想帧数
        private const float ExpectationStep = 1f / ExpectationFrame;    // 理想步进
        private const int MaxPreFrameCount = 4;                         // 允许最大追帧次数
        private const float MaxPreFrameTime = 0.25f;                    // 允许最大堆积时间
        public static event Action OnTick;                              // 自造update

        private float accumulateTime;                                   // 累计时间

        // 统计与日志
        [SerializeField] private bool enableStatsLog = true;            // 是否打印统计日志
        [SerializeField] private float logIntervalSeconds = 1f;         // 日志打印周期（秒）

        private float logTimer;                                         // 日志计时器
        private int ticksThisWindow;                                    // 当前窗口调用的逻辑帧次数（OnTick调用次数）
        private int unityUpdatesThisWindow;                             // 当前窗口Unity的Update调用次数
        private int chaseUpdatesThisWindow;                             // 当前窗口发生追帧的Update次数（一次Update内Tick>1）
        private int totalChasedTicksThisWindow;                         // 当前窗口追帧产生的额外Tick总数（Σ(max(0, Tick-1)))
        private int maxChaseBurstThisWindow;                            // 当前窗口单帧内最大连追次数
        private int droppedEventsThisWindow;                            // 当前窗口触发丢帧保护的次数

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // 统计：Unity帧
            unityUpdatesThisWindow++;
            logTimer += deltaTime;

            // 累加用时
            accumulateTime += deltaTime;

            //追帧
            int accumulatePreFrameCount = 0;
            while (accumulateTime >= ExpectationStep && accumulatePreFrameCount < MaxPreFrameCount)
            {
                accumulateTime -= ExpectationStep;
                OnTick?.Invoke();

                accumulatePreFrameCount++;
            }

            // 统计：Tick与追帧情况
            if (accumulatePreFrameCount > 0)
            {
                ticksThisWindow += accumulatePreFrameCount;
                if (accumulatePreFrameCount > 1)
                {
                    chaseUpdatesThisWindow++;
                    totalChasedTicksThisWindow += (accumulatePreFrameCount - 1);
                    if (accumulatePreFrameCount > maxChaseBurstThisWindow)
                    {
                        maxChaseBurstThisWindow = accumulatePreFrameCount;
                    }
                }
            }

            // 丢帧
            if (accumulateTime > MaxPreFrameTime && accumulatePreFrameCount >= MaxPreFrameCount)
            {
                accumulateTime = 0;
                droppedEventsThisWindow++;
            }
            
            // 周期性日志
            if (enableStatsLog && logTimer >= logIntervalSeconds)
            {
                float windowSeconds = logTimer;
                float ticksPerSecond = windowSeconds > 0f ? ticksThisWindow / windowSeconds : 0f;
                float unityFps = windowSeconds > 0f ? unityUpdatesThisWindow / windowSeconds : 0f;
                float chaseUpdateRatio = unityUpdatesThisWindow > 0 ? (float)chaseUpdatesThisWindow / unityUpdatesThisWindow : 0f;

                string status;
                if (droppedEventsThisWindow > 0)
                {
                    status = "丢帧";
                }
                else if (maxChaseBurstThisWindow >= MaxPreFrameCount || chaseUpdateRatio > 0.33f)
                {
                    status = "追帧偏多";
                }
                else if (chaseUpdatesThisWindow == 0)
                {
                    status = "稳定";
                }
                else
                {
                    status = "轻微追帧";
                }

                Debug.Log($"[ActionFrameClock] 逻辑帧≈{ticksPerSecond:F1}/s(目标{ExpectationFrame}), Unity帧≈{unityFps:F1}/s, 追帧Update={chaseUpdatesThisWindow}, 追帧总数={totalChasedTicksThisWindow}, 最大连追={maxChaseBurstThisWindow}, 丢帧={droppedEventsThisWindow}, 评估={status}");

                // 重置窗口
                logTimer = 0f;
                ticksThisWindow = 0;
                unityUpdatesThisWindow = 0;
                chaseUpdatesThisWindow = 0;
                totalChasedTicksThisWindow = 0;
                maxChaseBurstThisWindow = 0;
                droppedEventsThisWindow = 0;
            }
        }

    }
}
