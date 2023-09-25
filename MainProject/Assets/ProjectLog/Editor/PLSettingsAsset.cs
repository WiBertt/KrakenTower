using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ProjectLog
{
	public class PLSettingsAsset : PLAsset
	{

		[SerializeField] private bool m_AskBeforeDeleting = true;
		public bool AskBeforeDeleting => m_AskBeforeDeleting;
		[SerializeField] private int m_CullThreshold;
		public int CullThreshold => m_CullThreshold;
		[SerializeField] private List<AuthorProfile> m_Authors = new List<AuthorProfile>();
		public List<AuthorProfile> Authors => m_Authors;
		[SerializeField] private bool m_ShowAuthorNamesInTodo = true;
		public bool ShowAuthorNamesInTodo => m_ShowAuthorNamesInTodo;
		[SerializeField] private bool m_TruncateDevlogs = true;
		public bool TruncateDevlogs => m_TruncateDevlogs;
		[SerializeField] private int m_TruncateLimit = 500;
		public int TruncateLimit => m_TruncateLimit;

		public void SetAskBeforeDeleting(bool value)
		{
			m_AskBeforeDeleting = value;
		}

		public void SetCullThreshold(int value)
		{
			m_CullThreshold = value;
		}

		public void SetShowAuthorNamesInTodo(bool value)
		{
			m_ShowAuthorNamesInTodo = value;
		}

		public void AddAuthor(string name = "")
		{
			var newAuthor = new AuthorProfile();
			if (name == "") name = $"{PLConstants.AUTHOR_DEFAULT_NAME} {m_Authors.Count}";
			newAuthor.SetAuthorName(name);
			m_Authors.Add(newAuthor);
		}

		public void RemoveAuthor(int idx)
		{
			m_Authors.RemoveAt(idx);
		}

		public List<string> GetAuthorNames()
		{
			var names = new List<string>();
			m_Authors.ForEach(x => names.Add(x.AuthorName));
			return names;
		}

		public void SetTruncateDevlogs(bool value)
		{
			m_TruncateDevlogs = value;
		}
	}

	[System.Serializable]
	public class AuthorProfile
	{
		[SerializeField] private string m_AuthorName = PLConstants.AUTHOR_DEFAULT_NAME;
		public string AuthorName => m_AuthorName;
		public void SetAuthorName(string value)
		{
			m_AuthorName = value;
		}
	}
}