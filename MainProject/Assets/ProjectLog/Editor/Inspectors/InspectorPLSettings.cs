using UnityEngine;
using UnityEditor;

namespace ProjectLog
{
	[CustomEditor(typeof(PLSettingsAsset))]
	public class InspectorPLSettings : Editor
	{
		private PLSettingsAsset asset;
		int cullThreshold;
		int m_RenameIdx = -1;
		public override void OnInspectorGUI()
		{
			asset = (PLSettingsAsset)target;

			// ProjectLog main settings
			EditorGUILayout.LabelField(PLConstants.SETTINGS_HEADER_PROJECTLOG, ProjectLogWindow.StyleHeader());

			// Ask before deletion toggle
			EditorGUILayout.BeginVertical(ProjectLogWindow.StyleBox());
			var askBeforeDeleting = asset.AskBeforeDeleting;
			asset.SetAskBeforeDeleting(EditorGUILayout.Toggle(PLConstants.LABEL_ASK_BEFORE_DELETING, askBeforeDeleting));

			// Authors management
			EditorGUILayout.BeginHorizontal();
			if (ProjectLogWindow.Button(PLConstants.SETTINGS_ADD_AUTHOR, ProjectLogWindow.StyleButton(), PLConstants.HeaderButtonWidth, 20))
			{
				asset.AddAuthor();
				ProjectLogWindow.SaveAsset(asset);
			}
			EditorGUILayout.EndHorizontal();
			for (var i = 0; i < asset.Authors.Count; i++)
			{
				var authorProfile = asset.Authors[i];
				EditorGUILayout.BeginHorizontal();

				var authorName = $"{authorProfile.AuthorName}";
				var color = GUI.color;
				if (m_RenameIdx == i)
				{
					// GUI.color = color * 2;
					authorName = EditorGUILayout.TextField(authorName, ProjectLogWindow.StyleEditting(), GUILayout.Height(PLConstants.SmallButtonSize));
					authorProfile.SetAuthorName(authorName);
					GUI.color = color;
					if (ProjectLogWindow.Button(PLConstants.LABEL_DONE, ProjectLogWindow.StyleButton(), PLConstants.HeaderButtonWidth, PLConstants.SmallButtonSize))
					{
						m_RenameIdx = -1;
						ProjectLogWindow.SaveAsset(asset);
					}
				}
				else
				{
					EditorGUILayout.LabelField(authorName, ProjectLogWindow.StyleNormal());
					if (ProjectLogWindow.Button(PLConstants.PROJECTLOG_ENTRY_SMALL, ProjectLogWindow.StyleButton(), PLConstants.SmallButtonSize, PLConstants.SmallButtonSize))
					{
						m_RenameIdx = i;
					}
					if (ProjectLogWindow.Button(PLConstants.PROJECTLOG_REMOVE_ENTRY_SMALL, ProjectLogWindow.StyleButton(), PLConstants.SmallButtonSize, PLConstants.SmallButtonSize))
					{
						asset.RemoveAuthor(i);
						ProjectLogWindow.SaveAsset(asset);
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField(PLConstants.SETTINGS_HEADER_TASKLIST, ProjectLogWindow.StyleHeader());
			EditorGUILayout.BeginVertical(ProjectLogWindow.StyleBox());

			// Hide task item after x days
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(PLConstants.SETTINGS_CULL_AFTER, GUILayout.Width(250));
			cullThreshold = EditorGUILayout.IntField(asset.CullThreshold, GUILayout.Width(30));
			cullThreshold = cullThreshold < 0 ? 0 : cullThreshold;
			asset.SetCullThreshold(cullThreshold);
			EditorGUILayout.LabelField(PLConstants.TASK_ITEM_DAYS, GUILayout.Width(30));

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.LabelField(PLConstants.SETTINGS_ITEMS_ACTION_INFO);

			// Show author name toggle
			EditorGUILayout.BeginHorizontal();
			var showAuthorInTodo = asset.ShowAuthorNamesInTodo;
			showAuthorInTodo = EditorGUILayout.Toggle(PLConstants.SETTINGS_SHOW_AUTHOR_NAME, showAuthorInTodo);
			asset.SetShowAuthorNamesInTodo(showAuthorInTodo);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

			// DEVLOG settings
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(PLConstants.SETTINGS_HEADER_DEVLOG, ProjectLogWindow.StyleHeader());

			// Truncate devlog toggle 
			EditorGUILayout.BeginVertical(ProjectLogWindow.StyleBox());
			EditorGUILayout.BeginHorizontal();
			var truncateDevlog = asset.TruncateDevlogs;
			truncateDevlog = EditorGUILayout.Toggle(PLConstants.SETTINGS_TRUNCATE_DEVLOG, truncateDevlog);
			asset.SetTruncateDevlogs(truncateDevlog);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

			// Info box
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(PLConstants.SETTINGS_HEADER_ABOUT_PROJECTLOG, ProjectLogWindow.StyleHeader());
			EditorGUILayout.BeginVertical(ProjectLogWindow.StyleBox());
			EditorGUILayout.LabelField(PLConstants.SETTINGS_ABOUT_PROJECTLOG_1, ProjectLogWindow.StyleNormal());
			EditorGUILayout.LabelField(string.Format(PLConstants.SETTINGS_ABOUT_PROJECTLOG_2, PLConstants.VersionNumber), ProjectLogWindow.StyleNormal());
			EditorGUILayout.EndVertical();
		}
	}
}