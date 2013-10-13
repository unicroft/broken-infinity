using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlaylistController))]
public class PlaylistControllerInspector : Editor {
	public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();
		EditorGUI.indentLevel = 1;
		
		PlaylistController controller = (PlaylistController)target;
		
		var ma = GUIHelper.GetSingleMasterAudio();
		if (ma != null) {
			GUIHelper.DrawTexture(ma.logoTexture);
		}
		
		var oldVolume = controller.masterPlaylistVolume;
		controller.masterPlaylistVolume = EditorGUILayout.Slider("Playlist Master Volume", controller.masterPlaylistVolume, 0f, 1f);
		
		if (oldVolume != controller.masterPlaylistVolume && Application.isPlaying) {
			PlaylistController.MasterVolume = controller.masterPlaylistVolume;
		}
		controller.crossFadeTime = EditorGUILayout.Slider("Cross-fade Time (sec)", controller.crossFadeTime, 0f, 10f);
		
        EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		controller.showDuckingControls = EditorGUILayout.Toggle("Show Ducking Controls", controller.showDuckingControls);
		EditorGUILayout.EndHorizontal();
		
		if (controller.showDuckingControls) {
			controller.duckedVolumeMultiplier = EditorGUILayout.Slider("Ducked Vol Multiplier", controller.duckedVolumeMultiplier, 0f, 1f);
			controller.duckedTimePercentage = EditorGUILayout.IntSlider("Rise Vol After % of Clip", controller.duckedTimePercentage, 0, 100);
		}
		
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
		
		//DrawDefaultInspector();
    }
}
