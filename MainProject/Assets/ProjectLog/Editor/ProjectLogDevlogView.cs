using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ProjectLog
{
	public class ProjectLogDevlogView
	{
		public class DevlogEditWindow : EditorWindow
		{
			PLDevlogAsset m_TargetAsset;
			PLDevlogAsset m_DummyAsset;
			bool m_EnableRichtText = true;
			bool m_CloseOnPublish = true;
			Vector2 m_Scroll;
			public void Open(PLDevlogAsset _asset)
			{
				m_TargetAsset = _asset;
				m_DummyAsset = ScriptableObject.CreateInstance<PLDevlogAsset>();
				m_DummyAsset.SetTitle(m_TargetAsset.Title);
				m_DummyAsset.SetAuthor(m_TargetAsset.Author);
				m_DummyAsset.SetBody(m_TargetAsset.Body);
				var window = (DevlogEditWindow)EditorWindow.GetWindow(typeof(DevlogEditWindow));
				window.titleContent = new GUIContent(string.Format(PLConstants.DEVLOG_EDIT_WINDOW_TITLE, m_TargetAsset.Title));
				var size = new Vector2(PLConstants.WindowMinWidth, PLConstants.WindowMinHeight);
				window.minSize = size;
				window.Show();
			}

			void OnDestroy()
			{
				if (m_TargetAsset.GetCreationDate == null && m_TargetAsset.Body == "")
				{
					ProjectLogWindow.Instance.DeleteAsset(m_TargetAsset);
				}
			}

			void OnGUI()
			{
				EditorGUILayout.LabelField(PLConstants.LABEL_TITLE);
				var title = m_DummyAsset.Title;
				title = EditorGUILayout.TextField(title);

				EditorGUILayout.LabelField(PLConstants.LABEL_AUTHOR);
				var authors = ProjectLogWindow.Instance.PLSettings.GetAuthorNames();
				var author = m_DummyAsset.Author;

				var authorIdx = authors.IndexOf(author);
				if (authorIdx == -1)
				{
					authorIdx = 0;
					EditorGUILayout.LabelField(author);
				}
				else
				{
					authorIdx = EditorGUILayout.Popup(authorIdx, authors.ToArray(), ProjectLogWindow.StyleDropdown(), GUILayout.Width(PLConstants.HeaderButtonWidth), GUILayout.Height(PLConstants.HeaderButtonHeight));
					author = authors[authorIdx];
				}

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(PLConstants.LABEL_BODY, GUILayout.Width(80));
				m_EnableRichtText = GUILayout.Toggle(m_EnableRichtText, PLConstants.LABEL_ENABLE_RICH_TEXT);
				EditorGUILayout.EndHorizontal();

				var style = GUI.skin.textArea;
				style.wordWrap = true;
				style.richText = m_EnableRichtText;
				var body = m_DummyAsset.Body;
				m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll);
				body = EditorGUILayout.TextArea(body, style, GUILayout.ExpandHeight(true));
				EditorGUILayout.EndScrollView();

				EditorGUILayout.LabelField(PLConstants.DEVLOG_RICH_TEXT_INFO_0, ProjectLogWindow.StyleNormal());
				EditorGUILayout.LabelField(PLConstants.DEVLOG_RICH_TEXT_INFO_1, ProjectLogWindow.StyleNormal());
				EditorGUILayout.LabelField(PLConstants.DEVLOG_RICH_TEXT_INFO_2, ProjectLogWindow.StyleNormal());
				EditorGUILayout.LabelField(PLConstants.DEVLOG_RICH_TEXT_INFO_3, ProjectLogWindow.StyleNormal());
				EditorGUILayout.LabelField(PLConstants.DEVLOG_RICH_TEXT_INFO_4, ProjectLogWindow.StyleNormal());
				EditorGUILayout.LabelField(PLConstants.DEVLOG_RICH_TEXT_INFO_5, ProjectLogWindow.StyleNormal());

				m_DummyAsset.SetTitle(title);
				m_DummyAsset.SetAuthor(author);
				m_DummyAsset.SetBody(body);

				m_CloseOnPublish = EditorGUILayout.Toggle(PLConstants.LABEL_CLOSE_ON_PUBLISH, m_CloseOnPublish);
				EditorGUILayout.BeginHorizontal();

				EditorGUI.BeginDisabledGroup(title == "");
				if (ProjectLogWindow.Button(PLConstants.LABEL_PUBLISH, ProjectLogWindow.StyleButton(), PLConstants.HeaderButtonWidth, PLConstants.HeaderButtonHeight))
				{
					m_TargetAsset.SetTitle(m_DummyAsset.Title);
					m_TargetAsset.SetAuthor(m_DummyAsset.Author);
					m_TargetAsset.SetBody(m_DummyAsset.Body);
					if (m_TargetAsset.GetCreationDate == null)
					{
						m_TargetAsset.SetCreationDate();
					}
					m_TargetAsset.SetModifiedDate();
					ProjectLogWindow.SaveAsset(m_TargetAsset);
					ProjectLogWindow.Instance.LoadDevlogs();
					if (m_CloseOnPublish)
					{
						Close();
					}
				}
				EditorGUI.EndDisabledGroup();
				if (ProjectLogWindow.Button(PLConstants.LABEL_CANCEL, ProjectLogWindow.StyleButton(), PLConstants.HeaderButtonWidth, PLConstants.HeaderButtonHeight))
				{
					Close();
					ProjectLogWindow.Instance.LoadDevlogs();
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		public class DevlogViewWindow : EditorWindow
		{
			PLDevlogAsset m_TargetAsset;
			PLDevlogAsset m_DummyAsset;
			Vector2 m_Scroll;
			public void Open(PLDevlogAsset _asset)
			{
				m_TargetAsset = _asset;
				m_DummyAsset = ScriptableObject.CreateInstance<PLDevlogAsset>();
				m_DummyAsset.SetTitle(m_TargetAsset.Title);
				m_DummyAsset.SetAuthor(m_TargetAsset.Author);
				m_DummyAsset.SetBody(m_TargetAsset.Body);
				var window = (DevlogViewWindow)EditorWindow.GetWindow(typeof(DevlogViewWindow));
				window.titleContent = new GUIContent(m_TargetAsset.Title);
				var size = new Vector2(PLConstants.WindowMinWidth, PLConstants.WindowMinHeight);
				window.minSize = size;
				window.Show();
			}

			void OnGUI()
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField(string.Format(PLConstants.DEVLOG_ASSET_TITLE, m_TargetAsset.Title), ProjectLogWindow.StyleHeader());
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField($"{m_TargetAsset.Author} - {m_TargetAsset.GetCreationDate}");
				EditorGUILayout.EndHorizontal();
				ProjectLogWindow.DrawHorizontalLine();
				m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll);
				EditorGUILayout.LabelField(m_TargetAsset.Body, ProjectLogWindow.StyleDevlogBody());
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndVertical();
			}
		}

		private List<PLDevlogAsset> m_DevlogListAssets = new List<PLDevlogAsset>();
		private Vector2 m_TodoScrollTodo;

		public void View(bool reload)
		{
			m_DevlogListAssets = ProjectLogWindow.CheckAssetsValidity(m_DevlogListAssets);

			if (reload)
			{
				LoadDevlogAssets();
			}

			if (m_DevlogListAssets.Count == 0)
			{
				RenderMakeFirstItem();
			}
			else
			{
				RenderDevlogItems();
			}
		}

		public void LoadDevlogAssets()
		{
			AssetDatabase.Refresh();
			if (Directory.Exists(PLConstants.DataAssetsRoot) == false)
			{
				return;
			}
			var path = $"{PLConstants.DataAssetsRoot}{PLConstants.DataAssetsDevlog}";

			m_DevlogListAssets.Clear();
			var assets = Directory.GetFiles(path).ToList();

			if (assets.Count > 0)
			{
				foreach (var item in assets)
				{
					if (item.Contains(".meta"))
					{
						continue;
					}
					var asset = AssetDatabase.LoadAssetAtPath<PLDevlogAsset>(item);
					m_DevlogListAssets.Add(asset);
				}
			}
			SortDevLogsByDate();
		}

		private void RenderMakeFirstItem()
		{
			var w = 200;
			var h = 75;
			var rect = new Rect((Screen.width / 2) - (w / 2), (Screen.height / 2) - h, w, h);
			GUILayout.BeginArea(rect);

			var hasAuthors = ProjectLogWindow.Instance.PLSettings.Authors.Count > 0;
			if (hasAuthors)
			{
				if (ProjectLogWindow.Button(PLConstants.DEVLOG_CREATE_YOUR_FIRST_DEVLOG, ProjectLogWindow.StyleButton(), w, h))
				{
					MakeNewDevlogItem();
				}
			}
			else
			{
				if (ProjectLogWindow.Button(PLConstants.SETTINGS_ADD_FIRST_AUTHOR, ProjectLogWindow.StyleButton(), w, h))
				{
					ProjectLogWindow.OpenSettings();
				}
			}
			GUILayout.EndArea();
		}

		private void RenderDevlogItems()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(string.Format(PLConstants.DEVLOG_ENTRIES, m_DevlogListAssets.Count), ProjectLogWindow.StyleHeader());
			if (ProjectLogWindow.Button(PLConstants.DEVLOG_ADD_DEVLOG, ProjectLogWindow.StyleButton(), PLConstants.HeaderButtonWidth, PLConstants.HeaderButtonHeight))
			{
				MakeNewDevlogItem();
			}
			EditorGUILayout.EndHorizontal();
			m_TodoScrollTodo = EditorGUILayout.BeginScrollView(m_TodoScrollTodo, ProjectLogWindow.StyleScrollview());
			for (var i = 0; i < m_DevlogListAssets.Count; i++)
			{
				if (i > 0)
				{
					EditorGUILayout.Space();
				}
				var item = m_DevlogListAssets[i];
				RenderAssetInfo(item, i);
			}
			EditorGUILayout.EndScrollView();
		}

		private void RenderAssetInfo(PLDevlogAsset asset, int idx)
		{
			EditorGUILayout.BeginVertical(ProjectLogWindow.StyleBox());
			EditorGUILayout.LabelField(string.Format(PLConstants.DEVLOG_ASSET_TITLE, asset.Title), ProjectLogWindow.StyleHeader());
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"{asset.Author} - {asset.GetCreationDate}");

			if (ProjectLogWindow.Button(PLConstants.PROJECTLOG_ENTRY_SMALL, ProjectLogWindow.StyleButton(), PLConstants.SmallButtonSize, PLConstants.SmallButtonSize))
			{
				MakeNewDevlogItem(asset);
			}

			if (ProjectLogWindow.Button(PLConstants.PROJECTLOG_REMOVE_ENTRY_SMALL, ProjectLogWindow.StyleButton(), PLConstants.SmallButtonSize, PLConstants.SmallButtonSize))
			{
				ProjectLogWindow.Instance.DeleteAsset(asset, LoadDevlogAssets);
			}
			EditorGUILayout.EndHorizontal();
			ProjectLogWindow.DrawHorizontalLine();
			var body = asset.Body;
			var limit = ProjectLogWindow.Instance.PLSettings.TruncateLimit;
			if (ProjectLogWindow.Instance.PLSettings.TruncateDevlogs && body.Length > limit)
			{
				body = body.Remove(limit);
				body += PLConstants.LABEL_TRUNCATE;
				EditorGUILayout.LabelField(body, ProjectLogWindow.StyleDevlogBody());
			}
			else
			{
				EditorGUILayout.LabelField(body, ProjectLogWindow.StyleDevlogBody());
			}
			RenderExpandButton(asset);
			EditorGUILayout.EndVertical();
		}

		private void RenderExpandButton(PLDevlogAsset asset)
		{
			if (ProjectLogWindow.Button(PLConstants.DEVLOG_OPEN_DEVLOG, false))
			{
				var editWindow = ScriptableObject.CreateInstance<DevlogViewWindow>();
				editWindow.Open(asset);
			}
		}

		private void SortDevLogsByDate()
		{
			m_DevlogListAssets = m_DevlogListAssets.OrderBy(x => x.GetCreationDate).Reverse().ToList();
		}

		private void MakeNewDevlogItem(PLDevlogAsset asset = null)
		{
			if (asset == null)
			{
				asset = ProjectLogWindow.MakeDevlogAsset(asset);
			}

			var editWindow = ScriptableObject.CreateInstance<DevlogEditWindow>();
			editWindow.Open(asset);
		}
	}
}