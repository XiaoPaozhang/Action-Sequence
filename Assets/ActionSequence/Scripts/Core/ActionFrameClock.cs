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

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // 累加用时
            accumulateTime += deltaTime;

            //追帧
            int accumulatePreFrameCount = 0;
            while (accumulateTime >= ExpectationStep && accumulatePreFrameCount < MaxPreFrameCount)
            {
                accumulateTime -= ExpectationStep;
                OnTick?.Invoke();
                Debug.Log("OnTick");

                accumulatePreFrameCount++;
            }

            // 丢帧
            if (accumulateTime > MaxPreFrameTime && accumulatePreFrameCount >= MaxPreFrameCount)
            {
                accumulateTime = 0;
            }

        }

    }
}
