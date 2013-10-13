using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EventSounds))]
[CanEditMultipleObjects]
public class AudioEventInspector : Editor
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
		
		EventSounds sounds = (EventSounds)target;
		
		maInScene = ma != null;		
		if (maInScene) {
			groupNames = ma.GroupNames;
		}
		
		GUILayout.Label("Group Controls", EditorStyles.boldLabel);
		sounds.soundSpawnMode = (MasterAudio.SoundSpawnLocationMode) EditorGUILayout.EnumPopup("Sound Spawn Mode", sounds.soundSpawnMode);
		sounds.disableSounds = EditorGUILayout.Toggle("Disable Sounds", sounds.disableSounds);

		EditorGUILayout.Separator();
		GUILayout.Label("Trigger Sounds", EditorStyles.boldLabel);

		var disabledText = "";
		if (sounds.disableSounds) {
			disabledText = " (DISABLED) ";
		}

		List<bool> changedList = new List<bool>();

		// trigger sounds

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useEnableSound = EditorGUILayout.Toggle("Enable Sound" + disabledText, sounds.useEnableSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useEnableSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.enableSound, false));
		}

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useDisableSound = EditorGUILayout.Toggle("Disable Sound" + disabledText, sounds.useDisableSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useDisableSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.disableSound, false));
		}


		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useVisibleSound = EditorGUILayout.Toggle("Visible Sound" + disabledText, sounds.useVisibleSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useVisibleSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.visibleSound, false));
		}

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useInvisibleSound = EditorGUILayout.Toggle("Invisible Sound" + disabledText, sounds.useInvisibleSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useInvisibleSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.invisibleSound, false));
		}

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useCollisionSound = EditorGUILayout.Toggle("Collision Sound" + disabledText, sounds.useCollisionSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useCollisionSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.collisionSound, true));
		}

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useTriggerSound = EditorGUILayout.Toggle("Trigger Sound" + disabledText, sounds.useTriggerSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useTriggerSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.triggerSound, true));
		}

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useMouseEnterSound = EditorGUILayout.Toggle("Mouse Enter Sound" + disabledText, sounds.useMouseEnterSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useMouseEnterSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.mouseEnterSound, false));
		}

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useMouseClickSound = EditorGUILayout.Toggle("Mouse Click Sound" + disabledText, sounds.useMouseClickSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useMouseClickSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.mouseClickSound, false));
		}


		EditorGUI.indentLevel = 0;

		EditorGUILayout.Separator();
		GUILayout.Label("Trigger Sounds for PoolManager Plugin", EditorStyles.boldLabel);

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useSpawnedSound = EditorGUILayout.Toggle("Spawned Sound" + disabledText, sounds.useSpawnedSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useSpawnedSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.spawnedSound, false));
		}

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		sounds.useDespawnedSound = EditorGUILayout.Toggle("Despawned Sound" + disabledText, sounds.useDespawnedSound);
		EditorGUILayout.EndHorizontal();
		if (sounds.useDespawnedSound && !sounds.disableSounds) {
			changedList.Add(RenderAudioEvent(sounds.despawnedSound, false));
		}

		if (GUI.changed || changedList.Contains(true)) {
			EditorUtility.SetDirty(target);
		}

		//DrawDefaultInspector();
	}

	private bool RenderAudioEvent(AudioEvent aEvent, bool showLayerTagFilter)
	{
		bool isDirty = false;

		EditorGUI.indentLevel = 2;
		
		if (maInScene) {
			var existingIndex = groupNames.IndexOf(aEvent.soundType);

			int? groupIndex = null;
			
			if (existingIndex >= 0) {
				groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
			} else if (existingIndex == -1 && aEvent.soundType == MasterAudio.NO_GROUP_NAME) {
				groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
			} else { // non-match
				GUIHelper.ShowColorWarning("Sound Group found no match. Choose one from 'All Sound Groups'.");
				aEvent.soundType = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
				var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, groupNames.ToArray());
				if (newIndex >= 0) {
					groupIndex = newIndex;
				}
			}
			
			if (groupIndex.HasValue) {
				if (groupIndex.Value == -1) {
					aEvent.soundType = MasterAudio.NO_GROUP_NAME;
				} else {
					aEvent.soundType = groupNames[groupIndex.Value];
				}
			}
		} else {
			aEvent.soundType = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
		}
		
		aEvent.volume = EditorGUILayout.Slider("Volume", aEvent.volume, 0f, 1f);
		
		#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		#else
		aEvent.delaySound = EditorGUILayout.Slider("Delay Sound (sec)", aEvent.delaySound, 0f, 10f);
		#endif
		
		aEvent.emitParticles = EditorGUILayout.Toggle("Emit Particle", aEvent.emitParticles);
		if (aEvent.emitParticles) {
			aEvent.particleCountToEmit = EditorGUILayout.IntSlider("Particle Count", aEvent.particleCountToEmit, 1, 100);
		}

		if (showLayerTagFilter) {
			aEvent.useLayerFilter = EditorGUILayout.BeginToggleGroup("Layer filters", aEvent.useLayerFilter);
			if (aEvent.useLayerFilter) {
				for (var i = 0; i < aEvent.matchingLayers.Count; i++) {
					aEvent.matchingLayers[i] = EditorGUILayout.LayerField("Layer Match " + (i + 1), aEvent.matchingLayers[i]);
				}
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(24);
				if (GUILayout.Button(new GUIContent("Add", "Click to add a layer match at the end"), GUILayout.Width(60))) {
					aEvent.matchingLayers.Add(0);
					isDirty = true;
				}
				if (aEvent.matchingLayers.Count > 1) {
					if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last layer match"), GUILayout.Width(60))) {
						aEvent.matchingLayers.RemoveAt(aEvent.matchingLayers.Count - 1);
						isDirty = true;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndToggleGroup();

			aEvent.useTagFilter = EditorGUILayout.BeginToggleGroup("Tag filter", aEvent.useTagFilter);
			if (aEvent.useTagFilter) {
				for (var i = 0; i < aEvent.matchingTags.Count; i++) {
					aEvent.matchingTags[i] = EditorGUILayout.TagField("Tag Match " + (i + 1), aEvent.matchingTags[i]);
				}
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(24);
				if (GUILayout.Button(new GUIContent("Add", "Click to add a tag match at the end"), GUILayout.Width(60))) {
					aEvent.matchingTags.Add("Untagged");
					isDirty = true;
				}
				if (aEvent.matchingTags.Count > 1) {
					if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last tag match"), GUILayout.Width(60))) {
						aEvent.matchingTags.RemoveAt(aEvent.matchingLayers.Count - 1);
						isDirty = true;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndToggleGroup();
		}

		return isDirty;
	}
}
