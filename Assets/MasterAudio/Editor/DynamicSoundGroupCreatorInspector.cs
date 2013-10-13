using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(DynamicSoundGroupCreator))]
public class DynamicSoundGroupCreatorInspector : Editor {
	public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();
		
		EditorGUI.indentLevel = 1;
		var isDirty = false;
		
		DynamicSoundGroupCreator _creator = (DynamicSoundGroupCreator)target;
		
		if (_creator.logoTexture != null) {
			GUIHelper.DrawTexture(_creator.logoTexture);
		}
	
        EditorGUI.indentLevel = 0;  // Space will handle this for the header
		
		
		_creator.createOnAwake = EditorGUILayout.Toggle("Auto-create Groups", _creator.createOnAwake);
		if (_creator.createOnAwake) {
			GUIHelper.ShowColorWarning("*Groups will be created as soon as this object is in the Scene.");
		} else {
			GUIHelper.ShowColorWarning("*You will need to call this object's CreateGroups method.");
		}

		_creator.disableGOAfter = EditorGUILayout.Toggle("Disable after creation", _creator.disableGOAfter);
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        _creator.soundGroupsAreExpanded = GUIHelper.Foldout(_creator.soundGroupsAreExpanded, "Dynamic Sound Groups");

		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
		GUIContent content;
        var collapseIcon = '\u2261'.ToString();
        content = new GUIContent(collapseIcon, "Click to collapse all");
        var masterCollapse = GUILayout.Button(content, EditorStyles.toolbarButton);

        var expandIcon = '\u25A1'.ToString();
        content = new GUIContent(expandIcon, "Click to expand all");
        var masterExpand = GUILayout.Button(content, EditorStyles.toolbarButton);
		if (masterExpand) {
			ExpandCollapseAllSoundGroups(_creator, true);
		} 
		if (masterCollapse) {
			ExpandCollapseAllSoundGroups(_creator, false);
		}
        EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndHorizontal();
		
		if (_creator.soundGroupsAreExpanded) {
			int? addIndex = null;
			int? removeIndex = null;
			
			
			for (var i = 0; i < _creator.soundGroupsToCreate.Count; i++) {
				var aGroup = _creator.soundGroupsToCreate[i];
				
		        EditorGUI.indentLevel = 1;  // Space will handle this for the header
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
		        
				var soundGroupName = string.Empty;
				
				switch (aGroup.audLocation) {
					case SoundGroupVariation.AudioLocation.Clip:
						if (aGroup.clip != null) {
							soundGroupName = UtilStrings.CapitalizeFirstLetter(aGroup.clip.name);
						}
						break;
					case SoundGroupVariation.AudioLocation.ResourceFile:
					 	soundGroupName = UtilStrings.CapitalizeFirstLetter(aGroup.resourceFileName);
						break;
				}
				
				if (string.IsNullOrEmpty(soundGroupName)) {
					soundGroupName = "New Sound Group " + (i + 1);
				}
				
				aGroup.isExpanded = GUIHelper.Foldout(aGroup.isExpanded, soundGroupName);
			
				EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

				var groupButtonPressed = GUIHelper.AddFoldOutListItemButtons(i, _creator.soundGroupsToCreate.Count, "Sound Group", false, false);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndHorizontal();
				
				if (aGroup.isExpanded) {
			        EditorGUI.indentLevel = 0;  // Space will handle this for the header
					aGroup.audLocation = (SoundGroupVariation.AudioLocation) EditorGUILayout.EnumPopup("Audio Origin", aGroup.audLocation);
					
					switch (aGroup.audLocation) {
						case SoundGroupVariation.AudioLocation.Clip:
							aGroup.clip = (AudioClip) EditorGUILayout.ObjectField("Audio Clip", aGroup.clip, typeof(AudioClip), false);
							break;
						case SoundGroupVariation.AudioLocation.ResourceFile:
							aGroup.resourceFileName = EditorGUILayout.TextField("Resource Filename", aGroup.resourceFileName);
							break;
					}
					
					aGroup.volume = EditorGUILayout.Slider("Volume", aGroup.volume, 0f, 1f);
					aGroup.pitch = EditorGUILayout.Slider("Pitch", aGroup.pitch, 0f, 1f);
					aGroup.randomPitch = EditorGUILayout.Slider("Random Pitch", aGroup.randomPitch, 0f, 3f);
					aGroup.randomVolume = EditorGUILayout.Slider("Random Volume", aGroup.randomVolume, 0f, 1f);
					aGroup.weight = EditorGUILayout.IntSlider("Weight (Instances)", aGroup.weight, 0, 100);
				}
				
				switch (groupButtonPressed) {
					case GUIHelper.DTFunctionButtons.Add:
						addIndex = i;
						break;
					case GUIHelper.DTFunctionButtons.Remove:
						removeIndex = i;
						break;
				}
			}
			
			if (addIndex.HasValue) {
				var newGroup = new DynamicSoundGroupInfo();
				_creator.soundGroupsToCreate.Insert(addIndex.Value + 1, newGroup);
			} else if (removeIndex.HasValue) {
				if (_creator.soundGroupsToCreate.Count <= 1) {
					GUIHelper.ShowAlert("You cannot delete the last new Sound Group. You can delete the prefab if you don't need any.");
				} else {
					_creator.soundGroupsToCreate.RemoveAt(removeIndex.Value); 
				}
			}			
		}
		
		if (GUI.changed || isDirty) {
			EditorUtility.SetDirty(target);
		}
		
		//DrawDefaultInspector();
    }

	public void ExpandCollapseAllSoundGroups(DynamicSoundGroupCreator _creator, bool shouldExpand) {
		for (var i = 0; i < _creator.soundGroupsToCreate.Count; i++) {
			_creator.soundGroupsToCreate[i].isExpanded = shouldExpand;
		}
	}
}
