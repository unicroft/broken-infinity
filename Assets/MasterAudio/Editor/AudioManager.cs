using UnityEngine;
using System.Collections;
using UnityEditor;

public class AudioManager : EditorWindow {
	Vector2 scrollPos = Vector2.zero;
	
	[MenuItem("Window/Audio Manager")]
	static void Init()
    {
        EditorWindow.GetWindow(typeof(AudioManager));
    }
	
	void OnGUI() {
		scrollPos = GUI.BeginScrollView (
				new Rect (0, 0, position.width, position.height), 
				scrollPos, 
				new Rect (0, 0, 520, 220)
		);	
		
		var playlistCont = GUIHelper.GetSinglePlaylistController();
		var plControllerInScene = playlistCont != null;

		Texture header = (Texture) Resources.LoadAssetAtPath("Assets/MasterAudio/Sources/Textures/inspector_header_master_audio.png", typeof(Texture));
		if (header != null) {
			GUIHelper.DrawTexture(header);
		}
		Texture settings = (Texture) Resources.LoadAssetAtPath("Assets/MasterAudio/Sources/Textures/gearIcon.png", typeof(Texture));
		
		var ma = GUIHelper.GetSingleMasterAudio();
		
		GUIHelper.ShowColorWarning("The Master Audio prefab holds sound FX group and mixer controls. Add this first (only one per scene).");
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		
		EditorGUILayout.LabelField("Master Audio prefab", GUILayout.Width(300));
		if (ma == null) {
			GUI.contentColor = Color.green;
			if (GUILayout.Button(new GUIContent("Create", "Create Master Audio prefab"), EditorStyles.toolbarButton, GUILayout.Width(100))) {
				CreateMasterAudio();
			}
			GUI.contentColor = Color.white;
		} else { 
			if (settings != null) { 
				if (GUILayout.Button(new GUIContent(settings, "Master Audio Settings"), EditorStyles.toolbarButton)) {
					Selection.activeObject = ma.transform;
				}
			}
			EditorGUILayout.LabelField("Exists in scene", EditorStyles.boldLabel);
		}
		
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		
		// Playlist Controller
		GUIHelper.ShowColorWarning("The Playlist Controller prefab controls sets of songs (or other audio) and ducking. Only one per scene.");
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		EditorGUILayout.LabelField("Playlist Controller prefab", GUILayout.Width(300));

		if (!plControllerInScene) {
			GUI.contentColor = Color.green;
			if (GUILayout.Button(new GUIContent("Create", "Place a Playlist Controller prefab in the current scene."), EditorStyles.toolbarButton, GUILayout.Width(100))) { 
				CreatePlaylistController();
			}
			GUI.contentColor = Color.white;
		} else {
			if (settings != null) { 
				if (GUILayout.Button(new GUIContent(settings, "Playlist Sontroller Settings"), EditorStyles.toolbarButton)) {
					Selection.activeObject = playlistCont.transform;
				}
			}
			EditorGUILayout.LabelField("Exists in scene", EditorStyles.boldLabel);
		}

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		if (!plControllerInScene) {
			GUIHelper.ShowColorWarning("*There is no Playlist Controller in the scene. Music will not play.", Color.yellow);
		}
		
		EditorGUILayout.Separator();
		// Dynamic Sound Group Creators
		GUIHelper.ShowColorWarning("The Dynamic Sound Group Creator prefab can create Sound Groups on the fly. No limit per scene.");
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		EditorGUILayout.LabelField("Dynamic Sound Group Creator prefab", GUILayout.Width(300));

		GUI.contentColor = Color.green;
		if (GUILayout.Button(new GUIContent("Create", "Place a Dynamic Sound Group prefab in the current scene."), EditorStyles.toolbarButton, GUILayout.Width(100))) { 
			CreateDynamicSoundGroupCreator();
		}
		GUI.contentColor = Color.white;

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		
		GUI.EndScrollView();
	}
	
	private void CreateMasterAudio() {
		var ma = Resources.LoadAssetAtPath("Assets/MasterAudio/Prefabs/MasterAudio.prefab", typeof(GameObject));
		if (ma == null) {
			Debug.LogError("Could not find MasterAudio prefab. Please drag it into the scene yourself. It is located under MasterAudio/Prefabs.");
			return;
		}
		var go = GameObject.Instantiate(ma);
		go.name = "MasterAudio";
	}
	
	private void CreatePlaylistController() {
		var pc = Resources.LoadAssetAtPath("Assets/MasterAudio/Prefabs/PlaylistController.prefab", typeof(GameObject));
		if (pc == null) {
			Debug.LogError("Could not find PlaylistController prefab. Please drag it into the scene yourself. It is located under MasterAudio/Prefabs.");
			return;
		}
		var go = GameObject.Instantiate(pc);
		go.name = "PlaylistController";
	}
	
	private void CreateDynamicSoundGroupCreator() {
		var pc = Resources.LoadAssetAtPath("Assets/MasterAudio/Prefabs/DynamicSoundGroupCreator.prefab", typeof(GameObject));
		if (pc == null) {
			Debug.LogError("Could not find DynamicSoundGroupCreator prefab. Please drag it into the scene yourself. It is located under MasterAudio/Prefabs.");
			return;
		}
		var go = GameObject.Instantiate(pc);
		go.name = "DynamicSoundGroupCreator";
	}
}
