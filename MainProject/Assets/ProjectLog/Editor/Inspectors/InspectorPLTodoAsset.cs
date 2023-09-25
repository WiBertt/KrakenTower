using UnityEngine;
using UnityEditor;

namespace ProjectLog
{
	[CustomEditor(typeof(PLTasklistAsset))]
	public class InspectorPLTaskAsset : Editor
	{
		private PLTasklistAsset asset;
		public override void OnInspectorGUI()
		{
			asset = (PLTasklistAsset)target;
			GUIStyle style = GUI.skin.textField;
			style.wordWrap = true;
			style.richText = true;

			EditorGUILayout.LabelField(PLConstants.ASSET_CREATED);
			EditorGUILayout.LabelField($"{asset.GetCreationDate}", style);
			EditorGUILayout.LabelField(PLConstants.ASSET_MODIFIED);
			EditorGUILayout.LabelField($"{asset.GetModifiedDate}", style);
			EditorGUILayout.LabelField(PLConstants.ASSET_AUTHOR);
			EditorGUILayout.LabelField(asset.Author, style);
			EditorGUILayout.LabelField(PLConstants.ASSET_TASK);
			EditorGUILayout.LabelField(asset.TaskDescription, style);
			EditorGUILayout.Toggle(PLConstants.ASSET_TASK_COMPLETE, asset.IsDone);
		}
	}
}