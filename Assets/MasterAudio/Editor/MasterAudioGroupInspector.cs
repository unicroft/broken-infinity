using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MasterAudioGroup))]
public class MasterAudioGroupInspector : Editor {
	private bool isValid = true;
	
	public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();
		
		EditorGUI.indentLevel = 1;
		var isDirty = false;
		
		MasterAudio ma = null;
		ma = GUIHelper.GetSingleMasterAudio();
		if (ma == null) {
			isValid = false;
		} 
		
		MasterAudioGroup _group = (MasterAudioGroup)target;
		_group = RescanChildren(_group);
		
		if (!isValid) {
			return;
		}

		if (_group.logoTexture != null) {
			GUIHelper.DrawTexture(_group.logoTexture);
		}
		
		_group.groupMasterVolume = EditorGUILayout.Slider("Group Master Volume", _group.groupMasterVolume, 0f, 1f);
		
		_group.neverInterruptClips = EditorGUILayout.Toggle("Never Interrupt Clips", _group.neverInterruptClips);
		
		_group.limitPolyphony = EditorGUILayout.Toggle("Limit Polyphony", _group.limitPolyphony);
		if (_group.limitPolyphony) {
			int maxVoices = 0;
			for (var i = 0; i < _group.groupVariations.Count; i++) {
				var variation = _group.groupVariations[i];
				maxVoices += variation.weight;
			}
			
			_group.voiceLimitCount = EditorGUILayout.IntSlider("Polyphony Voice Limit", _group.voiceLimitCount, 1, maxVoices);
		}
		
		_group.limitMode = (MasterAudioGroup.LimitMode) EditorGUILayout.EnumPopup("Replay Limit Mode", _group.limitMode);
		switch (_group.limitMode) {
			case MasterAudioGroup.LimitMode.FrameBased:
				_group.limitPerXFrames = EditorGUILayout.IntSlider("Min Frames Between", _group.limitPerXFrames, 1, 120);
				break;
			case MasterAudioGroup.LimitMode.TimeBased:
				_group.minimumTimeBetween = EditorGUILayout.Slider("Min Seconds Between", _group.minimumTimeBetween, 0.1f, 10f);
				break;
		}
		
		if (!Application.isPlaying) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(13);
			GUILayout.Label("Actions", EditorStyles.wordWrappedLabel, GUILayout.Width(50f));
			GUILayout.Space(87);
			GUI.contentColor = Color.green;
			if (GUILayout.Button(new GUIContent("Equalize Weights", "Reset Weights to zero"), EditorStyles.toolbarButton, GUILayout.Width(120))) {
				isDirty = true;
				EqualizeWeights(_group);
			}	
			GUI.contentColor = Color.white;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}
		
		EditorGUILayout.Separator();
		
		int? deadChildIndex = null;

		EditorGUILayout.Separator();
		
		// new variation settings
        EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		_group.showNewVariationSettings = EditorGUILayout.Toggle("Create New Variations", _group.showNewVariationSettings);
		EditorGUILayout.EndHorizontal();		
		
		if (_group.showNewVariationSettings) {
			EditorGUILayout.BeginVertical();
			var anEvent = Event.current;

			GUI.color = Color.yellow;
			
			var dragArea = GUILayoutUtility.GetRect(0f,35f,GUILayout.ExpandWidth(true));
			GUI.Box (dragArea, "Drag Audio clips here to create variations!");

			GUI.color = Color.white;
			
			switch (anEvent.type) {
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if(!dragArea.Contains(anEvent.mousePosition)) {
						break;
					}
					
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					
					if(anEvent.type == EventType.DragPerform) {
						DragAndDrop.AcceptDrag();
						
						foreach (var dragged in DragAndDrop.objectReferences) {
							var aClip = dragged as AudioClip;
							if(aClip == null) {
								continue;
							}
							
							CreateVariation(_group, ma, aClip);
						}
					}
					Event.current.Use();
					break;
			}
			EditorGUILayout.EndVertical();
		}
		// end new variation settings

		if (_group.groupVariations.Count == 0) {
			GUIHelper.ShowColorWarning("You currently have no variations.");
		} else {
			for (var i = 0; i < _group.groupVariations.Count; i++) {
				var variation = _group.groupVariations[i];
	        	EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
				EditorGUILayout.LabelField(variation.name, EditorStyles.boldLabel);
	
				EditorGUILayout.EndHorizontal();
				
				if (variation.audio == null) {
					GUIHelper.ShowColorWarning(string.Format("The variation: '{0}' has no Audio Source.", variation.name));
					break;
				}
				
				variation.audLocation = (SoundGroupVariation.AudioLocation) EditorGUILayout.EnumPopup("Audio Origin", variation.audLocation);
				
				switch (variation.audLocation) {
					case SoundGroupVariation.AudioLocation.Clip:
						variation.audio.clip = (AudioClip) EditorGUILayout.ObjectField("Audio Clip", variation.audio.clip, typeof(AudioClip), false);
						break;
					case SoundGroupVariation.AudioLocation.ResourceFile:
						variation.resourceFileName = EditorGUILayout.TextField("Resource Filename", variation.resourceFileName);
						break;
				}
				
				variation.audio.volume = EditorGUILayout.Slider("Volume", variation.audio.volume, 0f, 1f);
				variation.audio.pitch = EditorGUILayout.Slider("Pitch", variation.audio.pitch, 0f, 1f);
				
				EditorUtility.SetDirty(variation.audio);

				variation.randomPitch = EditorGUILayout.Slider("Random Pitch", variation.randomPitch, 0f, 3f);
				variation.randomVolume = EditorGUILayout.Slider("Random Volume", variation.randomVolume, 0f, 1f);
				variation.weight = EditorGUILayout.IntSlider("Weight (Instances)", variation.weight, 0, 100);
			
		        EditorGUILayout.BeginHorizontal();
		
		        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
		        // A little space between button groups
		        GUILayout.Space(6);
				
				if (!Application.isPlaying) {
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(6);
		
					variation.possibleName = EditorGUILayout.TextField(variation.possibleName, GUILayout.MinWidth(80));
					GUI.contentColor = Color.green;
					if (GUILayout.Button(new GUIContent("Rename", "Click to rename this variation"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
						variation.name = variation.possibleName;
						variation.name = variation.possibleName;
					}
					GUI.contentColor = Color.white;
					
					GUILayout.Space(6);
					
					if (GUILayout.Button(new GUIContent(_group.settingsTexture, "Click to goto variation"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
						Selection.activeObject = variation; 
					}
	
					GUILayout.Space(6);
					
					if (GUILayout.Button(new GUIContent(_group.deleteTexture, "Click to delete this variation"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
						deadChildIndex = i;
						isDirty = true;
					}
					
					EditorGUILayout.EndHorizontal();
				}
				
				GUILayout.FlexibleSpace();
		
		        EditorGUILayout.EndHorizontal();
		        EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Separator();
			}
		}
		
		if (deadChildIndex.HasValue) {
			var deadVar = _group.groupVariations[deadChildIndex.Value];
			
			if (deadVar != null) {
				// delete variation from Hierarchy
				GameObject.DestroyImmediate(deadVar.gameObject);
			}
			
			// delete group.
			_group.groupVariations.RemoveAt(deadChildIndex.Value);
		}
		
		if (GUI.changed || isDirty) {
			EditorUtility.SetDirty(target);
		}
		
		//DrawDefaultInspector();
    }
	
	private MasterAudioGroup RescanChildren(MasterAudioGroup group) {
		var newChildren = new List<SoundGroupVariation>();
		
		var childNames = new List<string>();
		
		SoundGroupVariation variation = null;
		
		for (var i = 0; i < group.transform.childCount; i++) {
			var child = group.transform.GetChild(i);
			
			if (!Application.isPlaying) {
				if (childNames.Contains(child.name)) {
					GUIHelper.ShowColorWarning("You have more than one variation named: " + child.name + ".");
					GUIHelper.ShowColorWarning("Please ensure each variation of this Group has a unique name.");
					isValid = false;
					return null;
				}
			}
			
			childNames.Add(child.name);
			
			variation = child.GetComponent<SoundGroupVariation>();
			
			newChildren.Add(variation);
		}
		
		group.groupVariations = newChildren;
		return group;
	}
	
	public void EqualizeWeights(MasterAudioGroup _group) {
		foreach (var variation in _group.groupVariations) {
			variation.weight = 1;
		}
	}
	
	public void CreateVariation(MasterAudioGroup group, MasterAudio ma, AudioClip clip) {
		var clipName = clip.name;
		
		if (group.transform.FindChild(clipName) != null) {
			GUIHelper.ShowAlert("You already have a variation for this Group named '" + clipName + "'. \n\nPlease rename these variations when finished to be unique, or you may not be able to play them by name if you have a need to.");
		}
		
		var newVar = (GameObject) GameObject.Instantiate(ma.soundGroupVariationTemplate.gameObject, group.transform.position, Quaternion.identity);
		newVar.audio.clip = clip;
		newVar.transform.name = clipName;
		newVar.transform.parent = group.transform;
		var variation = newVar.GetComponent<SoundGroupVariation>();
		variation.possibleName = clipName;
	}
}
