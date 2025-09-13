using System;
using UnityEngine;

namespace Action
{
    [Serializable]
    public class ActionData
    {
        [Header("Info")]
        public string name;

        [Header("Damage")]
        public int damageMin;
        public int damageMax;

        [Header("Timing")]
        public float durationBase;
        public float correctTimingStartRatio;
        public float correctTimingEndRatio;
        public float delayAfterAction;
    }
}
