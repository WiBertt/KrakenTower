using UnityEngine;

namespace ProjectLog
{
	public class PLDevlogAsset : PLAsset
	{
		[SerializeField] private string m_DevlogTitle = "";
		public string Title => m_DevlogTitle;
		[SerializeField] private string m_DevlogBody = "";
		public string Body => m_DevlogBody;

		public void SetTitle(string value)
		{
			value = ProjectLogWindow.SanitizeString(value);
			m_DevlogTitle = value;
		}

		public void SetBody(string value)
		{
			m_DevlogBody = value;
		}
	}
}