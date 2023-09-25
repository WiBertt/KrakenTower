using UnityEngine;
using UnityEditor;

namespace ProjectLog
{
	[CustomEditor(typeof(PLDevlogAsset))]
	public class InspectorPLDevlogAsset : Editor
	{
		private PLDevlogAsset asset;
		public override void OnInspectorGUI()
		{
			asset = (PLDevlogAsset)target;
			GUIStyle style = GUI.skin.textField;
			style.wordWrap = true;
			style.richText = true;

			EditorGUILayout.LabelField(PLConstants.ASSET_CREATED);
			EditorGUILayout.LabelField($"{asset.GetCreationDate}", style);
			EditorGUILayout.LabelField(PLConstants.ASSET_MODIFIED);
			EditorGUILayout.LabelField($"{asset.GetModifiedDate}", style);
			EditorGUILayout.LabelField(PLConstants.ASSET_AUTHOR);
			EditorGUILayout.LabelField(asset.Author, style);
			EditorGUILayout.LabelField(PLConstants.ASSET_HEADER);
			EditorGUILayout.LabelField(asset.Title, style);
			EditorGUILayout.LabelField(PLConstants.ASSET_BODY);
			EditorGUILayout.LabelField(asset.Body, style);
		}
	}
}