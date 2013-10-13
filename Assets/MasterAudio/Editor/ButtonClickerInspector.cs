using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ButtonClicker))]
[CanEditMultipleObjects]
public class ButtonClickerInspector : Editor
{
	private List<string> groupNames = null;
	private bool maInScene;
	
	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeControls();
		EditorGUI.indentLevel = 1;
		
		var ma = GUIHelper.GetSingleMasterAudio();
		if (ma != null) {
			GUIHelper.DrawTexture(ma.logoTexture);
		}
		
		ButtonClicker sounds = (ButtonClicker)target;
		
		maInScene = ma != null;		
		if (maInScene) {
			groupNames = ma.GroupNames;
		}

		if (maInScene) {
			var existingIndex = groupNames.IndexOf(sounds.mouseDownSound);

			int? groupIndex = null;
			
			if (existingIndex >= 0) {
				groupIndex = EditorGUILayout.Popup("Mouse Down Sound", existingIndex, groupNames.ToArray());
			} else if (existingIndex == -1 && sounds.mouseDownSound == MasterAudio.NO_GROUP_NAME) {
				groupIndex = EditorGUILayout.Popup("Mouse Down Sound", existingIndex, groupNames.ToArray());
			} else { // non-match
				GUIHelper.ShowColorWarning("Sound Type found no match. Choose one from 'All Sound Types'.");
				sounds.mouseDownSound = EditorGUILayout.TextField("Mouse Down Sound", sounds.mouseDownSound);
				var newIndex = EditorGUILayout.Popup("All Sound Types", -1, groupNames.ToArray());
				if (newIndex >= 0) {
					groupIndex = newIndex;
				}
			}
			
			if (groupIndex.HasValue) {
				if (groupIndex.Value == -1) {
					sounds.mouseDownSound = MasterAudio.NO_GROUP_NAME;
				} else {
					sounds.mouseDownSound = groupNames[groupIndex.Value];
				}
			}
		} else {
			sounds.mouseDownSound = EditorGUILayout.TextField("Mouse Down Sound", sounds.mouseDownSound);
		}
		
		if (maInScene) {
			var existingIndex = groupNames.IndexOf(sounds.mouseUpSound);

			int? groupIndex = null;
			
			if (existingIndex >= 0) {
				groupIndex = EditorGUILayout.Popup("Mouse Up Sound", existingIndex, groupNames.ToArray());
			} else if (existingIndex == -1 && sounds.mouseUpSound == MasterAudio.NO_GROUP_NAME) {
				groupIndex = EditorGUILayout.Popup("Mouse Up Sound", existingIndex, groupNames.ToArray());
			} else { // non-match
				GUIHelper.ShowColorWarning("Sound Type found no match. Choose one from 'All Sound Types'.");
				sounds.mouseUpSound = EditorGUILayout.TextField("Mouse Up Sound", sounds.mouseUpSound);
				var newIndex = EditorGUILayout.Popup("All Sound Types", -1, groupNames.ToArray());
				if (newIndex >= 0) {
					groupIndex = newIndex;
				}
			}
			
			if (groupIndex.HasValue) {
				if (groupIndex.Value == -1) {
					sounds.mouseUpSound = MasterAudio.NO_GROUP_NAME;
				} else {
					sounds.mouseUpSound = groupNames[groupIndex.Value];
				}
			}
		} else {
			sounds.mouseUpSound = EditorGUILayout.TextField("Mouse Up Sound", sounds.mouseUpSound);
		}
		
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}

		//DrawDefaultInspector();
	}
}
