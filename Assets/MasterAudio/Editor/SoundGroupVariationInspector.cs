using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SoundGroupVariation))]
public class SoundGroupVariationInspector : Editor {
	public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();
		
		EditorGUI.indentLevel = 1;
		var isDirty = false;
		
		SoundGroupVariation _variation = (SoundGroupVariation)target;
		
		if (_variation.logoTexture != null) {
			GUIHelper.DrawTexture(_variation.logoTexture);
		}
	
		_variation.audLocation = (SoundGroupVariation.AudioLocation) EditorGUILayout.EnumPopup("Audio Origin", _variation.audLocation);
		
		switch (_variation.audLocation) {
			case SoundGroupVariation.AudioLocation.Clip:
				_variation.audio.clip = (AudioClip) EditorGUILayout.ObjectField("Audio Clip", _variation.audio.clip, typeof(AudioClip), false);
				break;
			case SoundGroupVariation.AudioLocation.ResourceFile:
				_variation.resourceFileName = EditorGUILayout.TextField("Resource Filename", _variation.resourceFileName);
				break;
		}
		
		_variation.audio.volume = EditorGUILayout.Slider("Volume", _variation.audio.volume, 0f, 1f);
		_variation.audio.pitch = EditorGUILayout.Slider("Pitch", _variation.audio.pitch, 0f, 1f);
		_variation.randomPitch = EditorGUILayout.Slider("Random Pitch", _variation.randomPitch, 0f, 3f);
		_variation.randomVolume = EditorGUILayout.Slider("Random Volume", _variation.randomVolume, 0f, 1f);
		_variation.weight = EditorGUILayout.IntSlider("Weight (Instances)", _variation.weight, 0, 100);
		
		if (GUI.changed || isDirty) {
			EditorUtility.SetDirty(target);
		}
		
		//DrawDefaultInspector();
    }
	
}
