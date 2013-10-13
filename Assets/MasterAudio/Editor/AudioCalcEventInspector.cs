using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EventCalcSounds))]
[CanEditMultipleObjects]
public class AudioCalcEventInspector : Editor {
	private List<string> groupNames = null;
	private bool maInScene;

	public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();
		EditorGUI.indentLevel = 1;

		var ma = GUIHelper.GetSingleMasterAudio();
		if (ma != null) {
			GUIHelper.DrawTexture(ma.logoTexture);
		}
		
		EventCalcSounds sounds = (EventCalcSounds)target;

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
		
		var aud = sounds.GetComponent<AudioSource>();
		if (aud == null || aud.clip == null) {
			GUI.color = Color.green;
			EditorGUILayout.LabelField("Audio Source Ended Sound - needs AudioSource component.", EditorStyles.whiteMiniLabel);
			GUI.color = Color.white;
			sounds.useAudioSourceEndedSound = false;
		} else {
			sounds.useAudioSourceEndedSound = EditorGUILayout.BeginToggleGroup("Audio Source Ended Sound" + disabledText, sounds.useAudioSourceEndedSound);
			if (sounds.useAudioSourceEndedSound && !sounds.disableSounds) {
				EditorGUI.indentLevel = 2;

				if (maInScene) {
					var existingIndex = groupNames.IndexOf(sounds.audioSourceEndedSound.soundType);
		
					int? groupIndex = null;
					
					if (existingIndex >= 0) {
						groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
					} else if (existingIndex == -1 && sounds.audioSourceEndedSound.soundType == MasterAudio.NO_GROUP_NAME) {
						groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
					} else { // non-match
						GUIHelper.ShowColorWarning("Sound Group found no match. Choose one from 'All Sound Groups'.");
						sounds.audioSourceEndedSound.soundType = EditorGUILayout.TextField("Sound Group", sounds.audioSourceEndedSound.soundType);
						var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, groupNames.ToArray());
						if (newIndex >= 0) {
							groupIndex = newIndex;
						}
					}
					
					if (groupIndex.HasValue) {
						if (groupIndex.Value == -1) {
							sounds.audioSourceEndedSound.soundType = MasterAudio.NO_GROUP_NAME;
						} else {
							sounds.audioSourceEndedSound.soundType = groupNames[groupIndex.Value];
						}
					}
				} else {
					sounds.audioSourceEndedSound.soundType = EditorGUILayout.TextField("Sound Group", sounds.audioSourceEndedSound.soundType);
				}

		    	sounds.audioSourceEndedSound.volume = EditorGUILayout.Slider("Volume", sounds.audioSourceEndedSound.volume, 0f, 1f);
				#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
				#else
				sounds.audioSourceEndedSound.delaySound = EditorGUILayout.Slider("Delay Sound (sec)", sounds.audioSourceEndedSound.delaySound, 0f, 10f);
				#endif
				sounds.audioSourceEndedSound.emitParticles = EditorGUILayout.Toggle("Emit Particle", sounds.audioSourceEndedSound.emitParticles);
				sounds.audioSourceEndedSound.particleCountToEmit = EditorGUILayout.IntSlider("Particle Count", sounds.audioSourceEndedSound.particleCountToEmit, 1, 100);
			}
			EditorGUILayout.EndToggleGroup();
		}
		
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
		
		//DrawDefaultInspector();
    }
}
