using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace ProjectLog
{
    public class ProjectLogWindow : EditorWindow
    {
        private static ProjectLogWindow instance;
        public static ProjectLogWindow Instance
        {
            get { return instance; }
            set { instance = value; }
        }
        private int m_ViewTab = 0;
        private int m_PreviousViewTab = 0;
        private ProjectLogTasklistView m_PLTasksView;
        private ProjectLogDevlogView m_PLDevlogView;
        private PLSettingsAsset m_PLSettings;
        public PLSettingsAsset PLSettings => m_PLSettings;

        [MenuItem(PLConstants.MenuPathOpenMain)]
        static void OpenProjectLogPanel()
        {
            if (instance == null)
            {
                var window = (ProjectLogWindow)EditorWindow.GetWindow(typeof(ProjectLogWindow));
                window.titleContent = new GUIContent(PLConstants.PROJECTLOG_WINDOW_TITLE);
                var size = new Vector2(PLConstants.WindowMinWidth, PLConstants.WindowMinHeight);
                window.minSize = size;
                window.Show();
                instance = window;
            }
        }

        [MenuItem(PLConstants.MenuPathOpenSettings)]
        public static void OpenSettings()
        {
            var path = PLConstants.DefaultSettingsPath;
            var asset = (PLSettingsAsset)AssetDatabase.LoadAssetAtPath(path, typeof(PLSettingsAsset));
            if (asset == null)
            {
                MakeSettingsAsset();
            }

            asset = (PLSettingsAsset)AssetDatabase.LoadAssetAtPath(path, typeof(PLSettingsAsset));
            if (asset)
            {
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
            }
        }

        [InitializeOnLoad]
		internal static class SetDockedMinSize
		{
			// Fix for fixed size docked windows courtesy of baba_s on Unity Forums
			// https://forum.unity.com/threads/editorwindow-minsize-not-working-if-docked.1259909/

			static SetDockedMinSize()
			{
				// https://github.com/Unity-Technologies/UnityCsReference/blob/bf25390e5c79172c3d3e9a6b755680679e1dbd50/Editor/Mono/HostView.cs#L94
				var type = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
				var fieldInfo = type.GetField("k_DockedMinSize", BindingFlags.Static | BindingFlags.NonPublic);

				fieldInfo!.SetValue(null, new Vector2(PLConstants.WindowMinWidth, PLConstants.WindowMinHeight));
			}
		}

        private void InitialiseProjectLogPanel()
        {
            var root = $"{PLConstants.DataAssetsRoot}";
            var path = root;
            MakeAssetFolder(path);

            path = $"{root}{PLConstants.DataAssetsTasks}";
            MakeAssetFolder(path);

            path = $"{root}{PLConstants.DataAssetsDevlog}";
            MakeAssetFolder(path);

            if (m_PLSettings == null)
            {
                LoadSettings();
            }

            if (m_PLTasksView == null)
            {
                m_PLTasksView = new ProjectLogTasklistView();
            }
            LoadTasks();

            if (m_PLDevlogView == null)
            {
                m_PLDevlogView = new ProjectLogDevlogView();
            }
            LoadDevlogs();
        }

        private void MakeAssetFolder(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        private void OnFocus()
		{
            if (instance == null)
            {
                instance = this;
            }
            InitialiseProjectLogPanel();
        }

        private void OnDisable()
        {
            AssetDatabase.SaveAssets();
        }

        public static GUIStyle StyleBox()
        {
            var style = new GUIStyle(GUI.skin.textArea);
            style.margin.top = 5;
            style.margin.bottom = 5;
            style.margin.right = 5;
            style.margin.left = 5;
            style.padding.top = 5;
            style.padding.bottom = 5;
            style.padding.right = 5;
            style.padding.left = 5;
            return style;
        }

        public static GUIStyle StyleButton()
        {
            var style = new GUIStyle(GUI.skin.button);
            style.richText = true;
            style.margin.top = 2;
            style.margin.bottom = 2;
            style.margin.right = 2;
            style.margin.left = 2;
            return style;
        }

        public static GUIStyle StyleNormal()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;
            style.richText = true;
            style.margin.top = 2;
            style.margin.bottom = 2;
            style.margin.right = 2;
            style.margin.left = 2;
            return style;
        }

        public static GUIStyle StyleDropdown()
        {
            var style = new GUIStyle(GUI.skin.button);
            style.wordWrap = true;
            style.richText = true;
            style.margin.top = 2;
            style.margin.bottom = 2;
            style.margin.right = 2;
            style.margin.left = 2;
            style.alignment = TextAnchor.MiddleCenter;
            return style;
        }

        public static GUIStyle StyleEditting()
        {
            var style = new GUIStyle(GUI.skin.textField);
            style.wordWrap = true;
            style.margin.top = 2;
            style.margin.bottom = 2;
            style.margin.right = 2;
            style.margin.left = 2;
            return style;
        }

        public static GUIStyle StyleHeader()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.richText = true;
            style.margin.top = 10;
            style.margin.bottom = 20;
            style.padding.bottom = -5;
            style.margin.right = 2;
            style.margin.left = 2;
            return style;
        }

        public static GUIStyle StyleScrollview()
        {
            var style = new GUIStyle(GUI.skin.scrollView);
            style.margin.top = 5;
            style.margin.bottom = 5;
            style.margin.right = 5;
            style.margin.left = 5;
            return style;
        }

        public static GUIStyle StyleDevlogBody()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.richText = true;
            style.wordWrap = true;
            style.margin.top = 5;
            style.margin.bottom = 5;
            style.margin.right = 5;
            style.margin.left = 5;
            style.fontSize = 14;
            return style;
        }

        public static GUIStyle StyleSmallPrint()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.richText = true;
            style.margin.top = 0;
            style.padding.bottom = 0;
            style.margin.right = 0;
            style.margin.left = 0;
            style.fontSize = 10;
            return style;
        }


        void OnGUI()
        {
            var w = Screen.width;
            var h = Screen.height;

            m_PreviousViewTab = m_ViewTab;
            m_ViewTab = GUILayout.Toolbar(m_ViewTab, new string[] { PLConstants.PROJECTLOG_CHOICE_TASKLIST, PLConstants.PROJECTLOG_CHOICE_DEVLOG }, GUILayout.Height(35));
            var reload = m_ViewTab != m_PreviousViewTab;

            switch (m_ViewTab)
            {
                case 0:
                    m_PLTasksView.View(reload);
                    break;
                case 1:
                    m_PLDevlogView.View(reload);
                    break;
            }
        }

        public static void DrawHorizontalLine()
        {
            EditorGUILayout.Space();
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.black;
            Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.width + 10, rect.y));
            Handles.color = new Color(0.3f, 0.3f, 0.3f);
            Handles.DrawLine(new Vector2(rect.x, rect.y + 1), new Vector2(rect.width + 10, rect.y + 1));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        public static PLTasklistAsset MakeTaskAsset()
        {
            var path = $"{PLConstants.DataAssetsRoot}{PLConstants.DataAssetsTasks}";
            var itemIdx = CreateAssetTime();
            var assetName = string.Format(PLConstants.ASSET_NAME_TASK, itemIdx);
            var asset = ScriptableObject.CreateInstance<PLTasklistAsset>();
            asset.SetCreationDate();
            asset.SetModifiedDate();
            asset.SetAuthor(ProjectLogWindow.Instance.PLSettings.Authors[0].AuthorName);
            AssetDatabase.CreateAsset(asset, $"{path}{assetName}.asset");
            SaveAsset(asset);
            return asset;
        }

        public static PLDevlogAsset MakeDevlogAsset(PLDevlogAsset asset = null)
        {
            var path = $"{PLConstants.DataAssetsRoot}{PLConstants.DataAssetsDevlog}";
            var itemIdx = CreateAssetTime();
            var assetName = string.Format(PLConstants.ASSET_NAME_DEVLOG, itemIdx);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<PLDevlogAsset>();
            }
            asset.SetAuthor(ProjectLogWindow.Instance.PLSettings.Authors[0].AuthorName);
            AssetDatabase.CreateAsset(asset, $"{path}{assetName}.asset");
            SaveAsset(asset);
            return asset;
        }

        public static PLSettingsAsset MakeSettingsAsset()
        {
            var path = $"{PLConstants.DataAssetsRoot}";
            var itemIdx = CreateAssetTime();
            var assetName = PLConstants.ASSET_NAME_SETTINGS;
            var asset = ScriptableObject.CreateInstance<PLSettingsAsset>();
            AssetDatabase.CreateAsset(asset, $"{path}{assetName}.asset");
            SaveAsset(asset);
            return asset;
        }

        private void LoadSettings()
        {
            AssetDatabase.Refresh();
            var assetName = $"{PLConstants.DataAssetsRoot}{PLConstants.ASSET_NAME_SETTINGS}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<PLSettingsAsset>(assetName);
            if (asset == null)
            {
                asset = MakeSettingsAsset();
            }
            m_PLSettings = asset;

        }

        public static List<T> CheckAssetsValidity<T>(List<T> refList)
        {
            if (refList.Count > 0)
            {
                for (var i = refList.Count - 1; i >= 0; i--)
                {
                    var item = refList[i];
                    if (item == null)
                    {
                        refList.RemoveAt(i);
                    }
                }
            }

            return refList;
        }

        public static void SaveAsset(PLAsset asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        public void DeleteAsset(PLAsset asset, UnityAction callback = null)
        {
            var canDelete = !PLSettings.AskBeforeDeleting;
            if (canDelete == false)
            {
                canDelete = EditorUtility.DisplayDialog(PLConstants.LABEL_DELETE, PLConstants.LABEL_ASK_BEFORE_DELETING, PLConstants.LABEL_DELETE, PLConstants.LABEL_CANCEL);
            }

            if (canDelete)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.DeleteAsset(path);
            }

            callback?.Invoke();
        }

        private static int CreateAssetTime()
        {
            return (int)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        }


        public void LoadTasks()
        {
			m_PLTasksView?.LoadTaskAssets();
        }

        public void LoadDevlogs()
        {
            m_PLDevlogView?.LoadDevlogAssets();
        }

        public static bool Button(string label, bool expandWidth = true)
        {
            return GUILayout.Button(label, GUILayout.ExpandWidth(expandWidth));
        }
        public static bool Button(string label, GUIStyle style, bool expandWidth = true)
        {
            return GUILayout.Button(label, style, GUILayout.ExpandWidth(expandWidth));
        }
        public static bool Button(string label, GUIStyle style, int width, int height)
        {
            return GUILayout.Button(label, style, GUILayout.Width(width), GUILayout.Height(height));
        }

        public static string SanitizeString(string s)
        {
            s = s.Replace("\n", " ");
            s = s.Replace("\r", " ");
            s = s.Replace("\t", " ");
            s = s.Replace("  ", " ");
            return s;
        }
    }
}