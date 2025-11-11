using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionSequence
{
    [CreateAssetMenu(fileName = "ActionClipTable", menuName = "ActionSequence/ActionClipTable")]
    public class ActionClipTable : ScriptableObject
    {
       public List<ActionClipTableRow> ActionClipTbaleRow;
    }

    [Serializable]
    public class ActionClipTableRow
    {
        public int id;
        public string name;
        public ActionClipType type;
        /// <summary>
        /// 发生帧
        /// </summary>
        public int startupFrames;
        /// <summary>
        /// 判定帧
        /// </summary>
        public int activeFrames;
        /// <summary>
        /// 收招帧
        /// </summary>
        public int recoveryFrames;

        #region 位移属性
        /// <summary>
        /// 冲刺方向
        /// </summary>
        public ActionClipDashDirectionType dashDirection;
        /// <summary>
        /// 移动距离
        /// </summary>
        public float moveDistance;
        #endregion
    }
}
