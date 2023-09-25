using UnityEngine;

namespace ProjectLog
{
    public enum TaskPriority
    {
        HighPriority = 0,
        MediumPriority,
        LowPriority,
    }

    public class PLTasklistAsset : PLAsset
    {
        [SerializeField] private string m_TaskDescription;
        public string TaskDescription => m_TaskDescription;
        [SerializeField] private bool m_IsDone = false;
        public bool IsDone => m_IsDone;
        [SerializeField] private TaskPriority m_Priority = TaskPriority.MediumPriority;
        public TaskPriority Priority => m_Priority;
        public string PrioritySymbol
        {
            get
            {
                switch (m_Priority)
                {
                    case TaskPriority.LowPriority: return "↓";
                    case TaskPriority.MediumPriority: return "•";
                    case TaskPriority.HighPriority: return "↑";
                };
                return "•";
            }
        }

        public void SetDone(bool setModified = false)
        {
            m_IsDone = true;
            if (setModified)
            {
                SetModifiedDate();
            }
        }

        public void SetNotDone(bool setModified = false)
        {
            m_IsDone = false;
            if (setModified)
            {
                SetModifiedDate();
            }
        }

        public void SetPriority(TaskPriority value)
        {
            m_Priority = value;
            SetModifiedDate();
        }

        public void SetTaskDescription(string value)
        {
            value = value.Replace("\n", " ");
            value = value.Replace("\r", " ");
            value = value.Replace("\t", " ");
            value = value.Replace("  ", " ");
            m_TaskDescription = value;
            SetModifiedDate();
        }
    }
}