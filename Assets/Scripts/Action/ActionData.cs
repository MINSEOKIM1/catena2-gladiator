using System;

namespace Action
{
    [Serializable]
    public class ActionData
    {
        public string name;

        public int damageMin;
        public int damageMax;

        public float duration;
        public float correctTimingStart;
        public float correctTimingEnd;
        public float delayAfterAction;
    }
}
