using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ProjectLog
{
    public enum ListSortMethod
    {
        Priority,
        DateAscending,
        Author,
        Completed,
    }

    public class ProjectLogTasklistView
    {
        private List<PLTasklistAsset> m_TaskListAssets = new List<PLTasklistAsset>();
        private int m_EditIdx = -1;
        private bool m_IsNewTask;
        private Vector2 m_TasksScrollInProgress;
        private Vector2 m_TasksScrollCompleted;
        private int m_TaskInProgressSorting;
        private bool m_TaskInProgressSortingReverse;
        private int m_TaskDoneSorting;
        private bool m_TaskDoneSortingReverse;

        public void View(bool reload)
        {
            m_TaskListAssets = ProjectLogWindow.CheckAssetsValidity(m_TaskListAssets);
            SortList(ListSortMethod.Completed);

            if (reload)
            {
                LoadTaskAssets();
            }

            if (m_TaskListAssets.Count == 0)
            {
                RenderMakeFirstTask();
            }
            else
            {
                RenderInProgressTasks();
                EditorGUILayout.Space();
                RenderCompleteTasks();
            }
        }

        public void LoadTaskAssets()
        {
            AssetDatabase.Refresh();
            if (Directory.Exists(PLConstants.DataAssetsRoot) == false)
            {
                return;
            }
            var path = $"{PLConstants.DataAssetsRoot}{PLConstants.DataAssetsTasks}";

            m_TaskListAssets.Clear();
            var assets = Directory.GetFiles(path).ToList();

            if (assets.Count > 0)
            {
                foreach (var item in assets)
                {
                    if (item.Contains(".meta"))
                    {
                        continue;
                    }
                    var asset = AssetDatabase.LoadAssetAtPath<PLTasklistAsset>(item);
                    m_TaskListAssets.Add(asset);
                }
            }
        }

        private void RenderMakeFirstTask()
        {
            var w = 200;
            var h = 75;
            var rect = new Rect((Screen.width / 2) - (w / 2), (Screen.height / 2) - h, w, h);
            GUILayout.BeginArea(rect);
            var hasAuthors = ProjectLogWindow.Instance.PLSettings.Authors.Count > 0;
            if (hasAuthors)
            {
                if (ProjectLogWindow.Button(PLConstants.TASK_CREATE_YOUR_FIRST_TASK, ProjectLogWindow.StyleButton(), w, h))
                {
                    MakeTaskAsset();
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

        private void RenderInProgressTasks()
        {
            var taskCount = m_TaskListAssets.FindAll(x => x.IsDone == false).ToList().Count;
            EditorGUILayout.LabelField(string.Format(PLConstants.TASK_HEADER_UNFINISHED_TASKS, taskCount), ProjectLogWindow.StyleHeader());
            EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
            m_TaskInProgressSorting = GUILayout.Toolbar(m_TaskInProgressSorting, new string[] { PLConstants.TASK_SORT_PRIORITY, PLConstants.TASK_SORT_DATE, PLConstants.TASK_SORT_AUTHOR }, GUILayout.Width(PLConstants.WindowMinWidth - 150));
            m_TaskInProgressSortingReverse = EditorGUILayout.ToggleLeft(PLConstants.LABEL_REVERSE, m_TaskInProgressSortingReverse, GUILayout.Width(70));
            if (ProjectLogWindow.Button(PLConstants.TASK_ADD_TASK, ProjectLogWindow.StyleButton(), PLConstants.HeaderButtonWidth, PLConstants.HeaderButtonHeight))
            {
                MakeTaskAsset();
            }
            EditorGUILayout.EndHorizontal();

            var foundNone = true;
            var scrollHeight = ProjectLogWindow.Instance.position.height / 3;
            m_TasksScrollInProgress = EditorGUILayout.BeginScrollView(m_TasksScrollInProgress, ProjectLogWindow.StyleScrollview(), GUILayout.Height(scrollHeight));

            var tasksList = SortList((ListSortMethod)m_TaskInProgressSorting);
            if (m_TaskInProgressSortingReverse)
            {
                tasksList.Reverse();
            }
            if (m_EditIdx != -1 && m_IsNewTask)
            {
                var task = m_TaskListAssets[m_EditIdx];
                RenderEditInfo(task);
                tasksList.Remove(task);
            }
            for (var i = 0; i < tasksList.Count; i++)
            {
                var item = tasksList[i];
                if (item.IsDone == false)
                {
                    foundNone = false;

                    if (i == m_EditIdx && !m_IsNewTask)
                    {
                        RenderEditInfo(item);
                    }
                    else
                    {
                        RenderAssetInfo(item, i);
                    }
                }
            }
            if (foundNone)
            {
                EditorGUILayout.LabelField(PLConstants.LABEL_ALL_TASKS_FINISHED);
            }
            EditorGUILayout.EndScrollView();
        }

        private void RenderCompleteTasks()
        {
            var doneCount = m_TaskListAssets.FindAll(x => x.IsDone).ToList().Count;
            EditorGUILayout.LabelField(string.Format(PLConstants.TASK_HEADER_FINISHED_TASKS, doneCount), ProjectLogWindow.StyleHeader());
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
            m_TaskDoneSorting = GUILayout.Toolbar(m_TaskDoneSorting, new string[] { PLConstants.TASK_SORT_PRIORITY, PLConstants.TASK_SORT_DATE, PLConstants.TASK_SORT_AUTHOR }, GUILayout.Width(PLConstants.WindowMinWidth - 150));
            m_TaskDoneSortingReverse = EditorGUILayout.ToggleLeft(PLConstants.LABEL_REVERSE, m_TaskDoneSortingReverse, GUILayout.Width(70));
            if (ProjectLogWindow.Button(PLConstants.TASK_CLEAR_TASKS, ProjectLogWindow.StyleButton(), PLConstants.HeaderButtonWidth, PLConstants.HeaderButtonHeight))
            {
                DeleteAllCompletedTasks();
            }
            EditorGUILayout.EndHorizontal();

            var foundNone = true;
            m_TasksScrollCompleted = EditorGUILayout.BeginScrollView(m_TasksScrollCompleted, ProjectLogWindow.StyleScrollview());

            var tasksList = SortList((ListSortMethod)m_TaskDoneSorting);
            if (m_TaskDoneSortingReverse)
            {
                tasksList.Reverse();
            }
            var threshold = ProjectLogWindow.Instance.PLSettings.CullThreshold;
            for (var i = 0; i < tasksList.Count; i++)
            {
                var item = tasksList[i];
                if (item.IsDone)
                {
                    if (threshold > 0 /*&& item.AgeInDays > threshold*/)
                    {
                        continue;
                    }

                    foundNone = false;
                    RenderAssetInfo(item, 0);
                }
            }
            if (foundNone)
            {
                EditorGUILayout.LabelField(PLConstants.TASK_NO_FINISHED_TASKS);
            }
            EditorGUILayout.EndScrollView();
        }

        private void RenderAssetInfo(PLTasklistAsset asset, int idx)
        {
            if (asset == null)
            {
                return;
            }
            EditorGUILayout.BeginHorizontal(ProjectLogWindow.StyleBox());
            EditorGUILayout.BeginVertical(GUILayout.Width(PLConstants.TasksLeftItems));

            EditorGUILayout.LabelField($"{asset.PrioritySymbol}", ProjectLogWindow.StyleSmallPrint(), GUILayout.Width(16));

            var isDoneControl = asset.IsDone;
            var isDone = asset.IsDone;
            isDone = EditorGUILayout.Toggle(isDone, GUILayout.Width(PLConstants.SmallButtonSize));
            EditorGUILayout.EndVertical();

            if (isDone)
            {
                asset.SetDone(isDone != isDoneControl);
                EditorUtility.SetDirty(asset);
            }
            else
            {
                asset.SetNotDone(isDone != isDoneControl);
                EditorUtility.SetDirty(asset);
            }

            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(isDone);
            EditorGUILayout.LabelField($"{asset.TaskDescription}", ProjectLogWindow.StyleNormal());
            EditorGUILayout.LabelField(string.Format(PLConstants.TASK_TASK_INFORMATION, asset.Author, asset.GetModifiedDate), ProjectLogWindow.StyleSmallPrint());
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            if (!isDone)
            {
                if (ProjectLogWindow.Button(PLConstants.PROJECTLOG_ENTRY_SMALL, ProjectLogWindow.StyleButton(), PLConstants.SmallButtonSize, PLConstants.SmallButtonSize))
                {
                    m_EditIdx = idx;
                }
            }
            if (ProjectLogWindow.Button(PLConstants.LABEL_CROSS, ProjectLogWindow.StyleButton(), PLConstants.SmallButtonSize, PLConstants.SmallButtonSize))
            {
                ProjectLogWindow.Instance.DeleteAsset(asset, LoadTaskAssets);
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderEditInfo(PLTasklistAsset asset)
        {
            EditorGUILayout.BeginHorizontal(ProjectLogWindow.StyleBox());
            EditorGUILayout.BeginVertical();

            var taskDescription = EditorGUILayout.TextArea($"{asset.TaskDescription}", ProjectLogWindow.StyleEditting(), GUILayout.Height(75));
            asset.SetTaskDescription(taskDescription);

            var authors = ProjectLogWindow.Instance.PLSettings.GetAuthorNames();
            var author = asset.Author;

            EditorGUILayout.BeginHorizontal();
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

            asset.SetAuthor(author);
            var taskPrio = (TaskPriority)EditorGUILayout.EnumPopup(asset.Priority, ProjectLogWindow.StyleDropdown(), GUILayout.Width(PLConstants.HeaderButtonWidth), GUILayout.Height(PLConstants.HeaderButtonHeight));
            asset.SetPriority(taskPrio);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            var done = ProjectLogWindow.Button(PLConstants.LABEL_DONE, ProjectLogWindow.StyleButton(), PLConstants.HeaderButtonWidth, PLConstants.HeaderButtonHeight);
            var cancel = ProjectLogWindow.Button(PLConstants.LABEL_CANCEL, ProjectLogWindow.StyleButton(), PLConstants.HeaderButtonWidth, PLConstants.HeaderButtonHeight);
            EditorGUILayout.EndHorizontal();

            var e = Event.current;
            if (done ||
                e.type == EventType.KeyUp &&
                (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape || e.keyCode == KeyCode.KeypadEnter))
            {
                asset.SetModifiedDate();
                ProjectLogWindow.SaveAsset(asset);
                m_EditIdx = -1;
                m_IsNewTask = false;
                EditorGUI.FocusTextInControl("");
            }

            if (cancel)
            {
                m_EditIdx = -1;
                EditorGUI.FocusTextInControl("");
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        List<PLTasklistAsset> SortList(ListSortMethod sorting)
        {
            var result = m_TaskListAssets.ToList();

            switch (sorting)
            {
                case ListSortMethod.Author: result = result./*OrderBy(x => x.AgeInDays).*/OrderBy(x => x.Author).ToList(); break;
                case ListSortMethod.Completed: result = result./*OrderBy(x => x.AgeInDays).*/OrderBy(x => x.Author).OrderBy(x => x.IsDone).ToList(); break;
                case ListSortMethod.DateAscending: result = result./*OrderBy(x => x.AgeInDays).*/ToList(); break;
                case ListSortMethod.Priority: result = result.OrderBy(x => x.Priority).ToList(); break;
            };

            return result;
        }

        private void DeleteAllCompletedTasks()
        {
            var canDelete = !ProjectLogWindow.Instance.PLSettings.AskBeforeDeleting;
            Debug.Log(canDelete);
            if (canDelete == false)
            {
                canDelete = EditorUtility.DisplayDialog(PLConstants.LABEL_DELETE, PLConstants.LABEL_ASK_BEFORE_DELETING, PLConstants.LABEL_DELETE, PLConstants.LABEL_CANCEL);
            }

            if (canDelete)
            {
                for (var i = m_TaskListAssets.Count - 1; i >= 0; i--)
                {
                    var item = m_TaskListAssets[i];
                    if (item.IsDone)
                    {
                        var path = AssetDatabase.GetAssetPath(item);
                        AssetDatabase.DeleteAsset(path);
                    }
                }
                LoadTaskAssets();
            }
        }

        private void MakeTaskAsset()
        {
            ProjectLogWindow.MakeTaskAsset();
            LoadTaskAssets();
            m_EditIdx = m_TaskListAssets.Count - 1;
            m_IsNewTask = true;
        }
    }
}