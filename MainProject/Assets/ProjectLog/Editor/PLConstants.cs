namespace ProjectLog
{
    public class PLConstants
    {
        public const string DataAssetsRoot = "Assets/ProjectLog/Data/";
        public const string DataAssetsTasks = "Tasks/";
        public const string DataAssetsDevlog = "DevLog/";
        public const string VersionNumber = "v1.2.2";
        public const string MenuPathOpenMain = "Tools/ProjectLog/📓 Open ProjectLog Panel";
        public const string MenuPathOpenSettings = "Tools/ProjectLog/⚙ Open ProjectLog Settings";
        public const string DefaultSettingsPath = DataAssetsRoot + "PLSettings.asset";
        public const int WindowMinWidth = 420;
        public const int WindowMinHeight = 300;
        public const int HeaderButtonWidth = 140;
        public const int HeaderButtonHeight = 20;
        public const int SmallButtonSize = 25;
        public const int TasksLeftItems = 8;

        public const string ASSET_CREATED = "Created:";
        public const string ASSET_MODIFIED = "Modified:";
        public const string ASSET_DAYS_OLD = "Days old:";
        public const string ASSET_AUTHOR = "Author:";
        public const string ASSET_HEADER = "Body:";
        public const string ASSET_BODY = "Created:";
        public const string ASSET_TASK = "Task:";
        public const string ASSET_TASK_COMPLETE = "Task complete:";
        public const string ASSET_NAME_TASK = "TaskListItem_{0}";
        public const string ASSET_NAME_DEVLOG = "DevlogItem_{0}";
        public const string ASSET_NAME_SETTINGS = "PLSettings";
        public const string AUTHOR_DEFAULT_NAME = "Default";

        public const string PROJECTLOG_WINDOW_TITLE = "ProjectLog";
        public const string PROJECTLOG_CHOICE_TASKLIST = "☑ Tasks";
        public const string PROJECTLOG_CHOICE_DEVLOG = "✍ Dev Log";
        public const string PROJECTLOG_ENTRY_SMALL = "✎";
        public const string PROJECTLOG_REMOVE_ENTRY_SMALL = "✘";
        public const string DEVLOG_EDIT_WINDOW_TITLE = "Editing Devlog '{0}'";
        public const string DEVLOG_CREATE_YOUR_FIRST_DEVLOG = "<b><size=\"18\">✎ Create your\nfirst devlog!</size></b>";
        public const string DEVLOG_ENTRIES = "<b><size=\"18\">✍ Dev log entries ({0})</size></b>";
        public const string DEVLOG_ADD_DEVLOG = "✚ Add devlog";
        public const string DEVLOG_ASSET_TITLE = "<b><size=\"18\">{0}</size></b>";
        public const string DEVLOG_RICH_TEXT_INFO_0 = "You can use the following HTML tags to add enhance your logs:";
        public const string DEVLOG_RICH_TEXT_INFO_1 = "\t* < b > <b>bold</b> < / b >";
        public const string DEVLOG_RICH_TEXT_INFO_2 = "\t* < i > <i>italics</i> < / i >";
        public const string DEVLOG_RICH_TEXT_INFO_3 = "\t* < size = 8> <size=8>size 8</size> < / size >";
        public const string DEVLOG_RICH_TEXT_INFO_4 = "\t* < size = 16> <size=12>size 16</size> < / size >";
        public const string DEVLOG_RICH_TEXT_INFO_5 = "\t* < color = colorname> <color=yellow>color</color> < / color >";
        public const string DEVLOG_OPEN_DEVLOG = " View devlog ";
        public const string SETTINGS_TRUNCATE_DEVLOG = "Truncate devlogs";
        public const string SETTINGS_CULL_AFTER = "Hide completed items when it's older then ";
        public const string SETTINGS_ITEMS_ACTION_INFO = "(When 0, everything is shown)";
        public const string SETTINGS_ADD_FIRST_AUTHOR = "<b><size=\"18\">✎ No authors found.\nAdd one now.</size></b>";
        public const string TASK_CREATE_YOUR_FIRST_TASK = "<b><size=\"18\">✎ Create your\nfirst task!</size></b>";
        public const string TASK_HEADER_UNFINISHED_TASKS = "<b><size=\"18\">☐ In progress tasks ({0})</size></b>";
        public const string TASK_HEADER_FINISHED_TASKS = "<b><size=\"18\">☑ Completed tasks ({0})</size></b>";
        public const string TASK_ADD_TASK = "✚ Add new task";
        public const string TASK_CLEAR_TASKS = "✘ Clear all tasks";
        public const string TASK_ITEM_DAYS = "days";
        public const string TASK_SORT_PRIORITY = "Priority";
        public const string TASK_SORT_DATE = "Date";
        public const string TASK_SORT_AUTHOR = "Author";
        public const string TASK_NO_FINISHED_TASKS = "No finished tasks yet.";
        // public const string TASK_TASK_INFORMATION = "{0} - {1} Days old - Last modified: {2}";
        public const string TASK_TASK_INFORMATION = "{0} - Last modified: {1}";
        public const string LABEL_ASK_BEFORE_DELETING = "Ask before deleting asset";
        public const string LABEL_ALL_TASKS_FINISHED = "Well done! You finished all tasks!";
        public const string LABEL_TITLE = "Title";
        public const string LABEL_BODY = "Body";
        public const string LABEL_ENABLE_RICH_TEXT = "Enable rich text";
        public const string LABEL_AUTHOR = "Author";
        public const string LABEL_PUBLISH = "Publish";
        public const string LABEL_DONE = "Done";
        public const string LABEL_DELETE = "Delete";
        public const string LABEL_CANCEL = "Cancel";
        public const string LABEL_CROSS = "✘";
        public const string LABEL_TRUNCATE = "... [...]";
        public const string LABEL_REVERSE = "Reverse";
        public const string LABEL_CLOSE_ON_PUBLISH = "Close on publish";
        public const string SETTINGS_ADD_AUTHOR = "✚ Add author";
        public const string SETTINGS_SHOW_AUTHOR_NAME = "Show author name: ";

        public const string SETTINGS_HEADER_PROJECTLOG = "<b><size=\"18\">ProjectLog</size></b>";
        public const string SETTINGS_HEADER_TASKLIST = "<b><size=\"18\">Task list</size></b>";
        public const string SETTINGS_HEADER_DEVLOG = "<b><size=\"18\">Dev log</size></b>";
        public const string SETTINGS_HEADER_ABOUT_PROJECTLOG = "<b><size=\"18\">About ProjectLog</size></b>";

        public const string SETTINGS_ABOUT_PROJECTLOG_1 = "Thank you for using ProjectLog. I hope it helps you create better projects.\nIf you have any comments, suggestions, scribbles, love letters, drawings, songs, poems, demo's of your game, or any thought about ProjectLog, don't hesitate to share them with me!";
        public const string SETTINGS_ABOUT_PROJECTLOG_2 = "ProjectLog {0} - Feedback: <a href=\"mailto:projectlog@daanboon.nl\">projectlog@daanboon.nl</a>";
    }
}