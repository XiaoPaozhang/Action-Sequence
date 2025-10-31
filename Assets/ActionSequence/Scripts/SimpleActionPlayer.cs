using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace ActionSequence
{
	public class SimpleActionPlayer : MonoBehaviour
	{
		[Serializable]
		public enum ClipType { Move, Attack, Wait }

		[Serializable]
		public class Clip
		{
			[Header("ID")]
			public string id = "clip";
			[Header("类型")]
			public ClipType type = ClipType.Move;
			[Header("前摇帧数")]
			public int startupFrames = 6;
			[Header("生效帧数")]
			public int activeFrames = 0;
			[Header("后摇帧数")]
			public int recoveryFrames = 2;
			[Header("瞬移触发帧 (-1 关闭)")]
			public int impulseFrame = 0;
			[Header("位移冲量X")]
			public float moveImpulseX = 0f;
			[Header("位移冲量Y")]
			public float moveImpulseY = 0f;
			[Header("伤害")]
			public int damage = 0;
			[Header("命中窗口偏移")]
			public Vector2 hitOffset = Vector2.zero;
			[Header("命中窗口半径")]
			public float hitRadius = 0.5f;
		}

		[Header("最小序列（按下触发键播放）")]
		public List<Clip> sequence = new List<Clip>();

		[Header("触发按键")]
		public KeyCode triggerKey = KeyCode.Mouse0;
		public KeyCode triggerAltKey = KeyCode.Space;

		private bool isPlaying;
		private int clipIndex;
		private int frameInClip;    // 当前片段帧数
		private Clip current;
        [SerializeField]
		private bool hitActive;
        [SerializeField]
		private bool impulseApplied;
		[SerializeField]
		private bool hasMovementLock;

		private void Reset()
		{
			// 组件添加到物体时填充演示用序列
			sequence = new List<Clip>
			{
				// 瞬移在第0帧触发，前摇8，生效0，后摇2 -> 总10帧
				new Clip { id = "clip.step", type = ClipType.Move, startupFrames = 8, activeFrames = 0, recoveryFrames = 2, impulseFrame = 0, moveImpulseX = 2.5f },
				// 斩击：前摇6，生效4，后摇8；命中窗口=生效窗口；瞬移不使用（impulseFrame=-1）
				new Clip { id = "clip.slash", type = ClipType.Attack, startupFrames = 6, activeFrames = 4, recoveryFrames = 8, impulseFrame = -1, damage = 20, hitOffset = new Vector2(0.8f, 0.2f), hitRadius = 0.6f },
				// 等待：前摇6，生效0，后摇0
				new Clip { id = "clip.wait", type = ClipType.Wait, startupFrames = 6, activeFrames = 0, recoveryFrames = 0, impulseFrame = -1 },
			};
		}

		private void Awake()
		{
			if (sequence == null || sequence.Count == 0)
			{
				// 运行期若未在 Inspector 配置，也填充演示序列
				Reset();
			}
		}

		private void OnEnable() => ActionFrameClock.OnTick += Tick60;
		private void OnDisable()
		{
			ActionFrameClock.OnTick -= Tick60;
			if (hasMovementLock)
			{
				ActionLock.Release();
				hasMovementLock = false;
			}
		}

		private void Update()
		{
			if (!isPlaying && (Input.GetKeyDown(triggerKey) || Input.GetKeyDown(triggerAltKey)))
			{
				StartSequence();
			}
		}

		public void StartSequence()
		{
			if (sequence == null || sequence.Count == 0) return;
			isPlaying = true;
			clipIndex = 0;
			if (!hasMovementLock)
			{
				ActionLock.Acquire();
				hasMovementLock = true;
			}
			PlayCurrentClip();
		}

		private void PlayCurrentClip()
		{
			current = sequence[clipIndex];
			frameInClip = 0;
			impulseApplied = false;
			hitActive = false;
		}

		private void Tick60()
		{
			if (!isPlaying || current == null) return;

			// 在指定帧施加位移冲量（简单直接修改位置，便于观察）
			if (!impulseApplied)
			{
				if (current.impulseFrame >= 0 && frameInClip == current.impulseFrame && (Mathf.Abs(current.moveImpulseX) > 0f || Mathf.Abs(current.moveImpulseY) > 0f))
				{
					transform.position += new Vector3(current.moveImpulseX, current.moveImpulseY, 0f);
					impulseApplied = true;
				}
			}

			// 命中窗口：使用“生效阶段”作为命中窗口（仅 Attack）
			bool inActiveWindow = frameInClip >= current.startupFrames && frameInClip < (current.startupFrames + current.activeFrames);
			hitActive = current.type == ClipType.Attack && inActiveWindow;

			frameInClip++;

			// 本片段总帧 = 前摇 + 生效 + 后摇
			int totalFrames = Mathf.Max(0, current.startupFrames) + Mathf.Max(0, current.activeFrames) + Mathf.Max(0, current.recoveryFrames);
			if (frameInClip >= totalFrames)
			{
				clipIndex++;
				if (clipIndex >= sequence.Count)
				{
					isPlaying = false;
					current = null;
					hitActive = false;
					if (hasMovementLock)
					{
						ActionLock.Release();
						hasMovementLock = false;
					}
					return;
				}
				PlayCurrentClip();
			}
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			if (!hitActive || current == null) return;

			Gizmos.color = Color.yellow;
			Vector3 center = transform.position + new Vector3(current.hitOffset.x, current.hitOffset.y, 0f);
			Gizmos.DrawWireSphere(center, current.hitRadius > 0f ? current.hitRadius : 0.4f);
		}
	}
}
