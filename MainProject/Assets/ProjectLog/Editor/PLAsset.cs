using System;
using System.Globalization;
using UnityEngine;

namespace ProjectLog
{
    public class PLAsset : ScriptableObject
    {
        [SerializeField] private string m_CreationDate;
        public string GetCreationDate => m_CreationDate;
        [SerializeField] private string m_ModifiedDate;
        public string GetModifiedDate => m_ModifiedDate;
        [SerializeField] private string m_Author;
        public string Author => m_Author;

        public void SetCreationDate()
        {
            m_CreationDate = $"{DateTime.UtcNow.ToLocalTime()}";
        }

        public void SetModifiedDate()
        {
            m_ModifiedDate = $"{DateTime.UtcNow.ToLocalTime()}";
        }

        public void SetAuthor(string value)
        {
            m_Author = value;
        }

        // public int AgeInDays
        // {
        //     get
        //     {
        //         DateTime one = DateTime.Parse(m_CreationDate);
        //         TimeSpan age = DateTime.UtcNow - one;
		// 		return age.Days;
        //     }
        // }
    }
}