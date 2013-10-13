using UnityEngine;
using UnityEditor;
using System.Collections;

public static class GUIHelper {
	private const string ALERT_TITLE = "Master Audio Alert";
	private const string ALERT_OK_TEXT = "Ok";
	private const string FOLD_OUT_TOOLTIP = "Click to expand or collapse";
    private const int CONTROLS_DEFAULT_LABEL_WIDTH = 140;

	public enum DTFunctionButtons { 
		None, 
		Add, 
		Remove, 
		Mute,
		Solo,
		Go,
		ShiftUp,
		ShiftDown,
		Play
	}

	public static void DrawTexture(Texture tex) {
		Rect rect = GUILayoutUtility.GetRect(0f, 0f);
		rect.width = tex.width;
		rect.height = tex.height;
		GUILayout.Space(rect.height);
		GUI.DrawTexture(rect, tex);
	}

	public static MasterAudio GetSingleMasterAudio() {
		var masterAudio = (MasterAudio) GameObject.FindObjectOfType(typeof(MasterAudio));
		return masterAudio;
	}
	
	public static PlaylistController GetSinglePlaylistController() {
		var controller = (PlaylistController) GameObject.FindObjectOfType(typeof(PlaylistController));
		return controller;
	}
	
	public static DTFunctionButtons AddMixerBusButtons(GroupBus gb, MasterAudio sounds) {
		var deleteIcon = sounds.deleteTexture;
		
		var muteContent = new GUIContent(sounds.muteOffTexture, "Click to mute bus");
		
		if (gb.isMuted) {
			muteContent.image = sounds.muteOnTexture;
		}
		
		var soloContent = new GUIContent(sounds.soloOffTexture, "Click to solo bus");

		if (gb.isSoloed) {
			soloContent.image = sounds.soloOnTexture;
		}
		
		bool soloPressed = GUILayout.Button(soloContent, EditorStyles.toolbarButton);
		bool mutePressed = GUILayout.Button(muteContent, EditorStyles.toolbarButton);
		
        bool removePressed = false; 

		if (!Application.isPlaying) {
			removePressed = GUILayout.Button(new GUIContent(deleteIcon, "Click to delete bus"), EditorStyles.toolbarButton);
		}
		
        // Return the pressed button if any
        if (removePressed == true) {
			return DTFunctionButtons.Remove;
		}          
		if (soloPressed == true) {
			return DTFunctionButtons.Solo;
		}
		if (mutePressed == true) {
			return DTFunctionButtons.Mute;
		}

		return DTFunctionButtons.None;
	}
	
    public static DTFunctionButtons AddMixerButtons(MasterAudioGroup aGroup, string itemName, MasterAudio sounds)
    {
		var deleteIcon = sounds.deleteTexture;
		var settingsIcon = sounds.settingsTexture;
		
		var muteContent = new GUIContent(sounds.muteOffTexture, "Click to mute " + itemName);
		
		if (aGroup.isMuted) {
			muteContent.image = sounds.muteOnTexture;
		}
		
		var soloContent = new GUIContent(sounds.soloOffTexture, "Click to solo " + itemName);

		if (aGroup.isSoloed) {
			soloContent.image = sounds.soloOnTexture;
		}
		
		bool soloPressed = GUILayout.Button(soloContent, EditorStyles.toolbarButton);
		bool mutePressed = GUILayout.Button(muteContent, EditorStyles.toolbarButton);
		
		// Remove Button - Process presses later
        bool goPressed = GUILayout.Button(new GUIContent(settingsIcon, "Click to edit " + itemName), EditorStyles.toolbarButton);
        bool removePressed = false; 
		bool playPressed = false;
		
		if (Application.isPlaying) {
			playPressed = GUILayout.Button(new GUIContent(sounds.playTexture, "Click to play sound"), EditorStyles.toolbarButton);
		} else {
			removePressed = GUILayout.Button(new GUIContent(deleteIcon, "Click to delete " + itemName), EditorStyles.toolbarButton);
		}

        // Return the pressed button if any
        if (playPressed == true) {
			return DTFunctionButtons.Play;
		}
		if (removePressed == true) {
			return DTFunctionButtons.Remove;
		}         
		if (soloPressed == true) {
			return DTFunctionButtons.Solo;
		}
		if (mutePressed == true) {
			return DTFunctionButtons.Mute;
		}
		if (goPressed == true) {
			return DTFunctionButtons.Go;
		}
		
        return DTFunctionButtons.None;
    }
	
 	public static DTFunctionButtons AddSingleFoldOutListItemButtons()
    {
        EditorGUI.indentLevel = 1;
		
		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

        // A little space between button groups
        GUILayout.Space(14);

        // Add button - Process presses later
        bool addPressed = GUILayout.Button(new GUIContent("+", "Click to add new item after the last one"),
                                           EditorStyles.toolbarButton);

        GUILayout.Space(4);
		
		// Remove Button - Process presses later
        bool removePressed = GUILayout.Button(new GUIContent("-", "Click to remove last item"),
                                              EditorStyles.toolbarButton);

        EditorGUILayout.EndHorizontal();

        // Return the pressed button if any
        if (removePressed == true) {
			return DTFunctionButtons.Remove;
		}
        
		if (addPressed == true) {
			return DTFunctionButtons.Add;
		}

        return DTFunctionButtons.None;
    }
	
    public static DTFunctionButtons AddFoldOutListItemButtons(int position, int totalPositions, string itemName, bool showAfterText, bool showMoveButtons = false)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

        // A little space between button groups
        GUILayout.Space(24);
		
		bool upPressed = false;
		bool downPressed = false;
		
		if (showMoveButtons) {
	        if (position > 0) {
				// the up arrow.
				var upArrow = '\u25B2'.ToString();
        		upPressed = GUILayout.Button(new GUIContent(upArrow, "Click to shift " + itemName + " up"),
                                          EditorStyles.toolbarButton);
			}

			if (position < totalPositions - 1) {
	        	// The down arrow will move things towards the end of the List
				var dnArrow = '\u25BC'.ToString();
	        	downPressed = GUILayout.Button(new GUIContent(dnArrow, "Click to shift " + itemName + " down"), 
					EditorStyles.toolbarButton);
			}
		}

			
        // Add button - Process presses later
        var buttonText = "Click to add new " + itemName;
		if (showAfterText) {
			buttonText += " after this one";
		}
		bool addPressed = GUILayout.Button(new GUIContent("+", buttonText),
                                           EditorStyles.toolbarButton);

		// Remove Button - Process presses later
        bool removePressed = GUILayout.Button(new GUIContent("-", "Click to remove " + itemName),
	                                              EditorStyles.toolbarButton);

        EditorGUILayout.EndHorizontal();

        // Return the pressed button if any
        if (removePressed == true) {
			return DTFunctionButtons.Remove;
		}         
		if (addPressed == true) {
			return DTFunctionButtons.Add;
		}
		if (upPressed) {
			return DTFunctionButtons.ShiftUp;
		}
		if (downPressed) {
			return DTFunctionButtons.ShiftDown;
		}

        return DTFunctionButtons.None;
    }
	
	public static bool Foldout(bool expanded, string label)
    {
        EditorGUIUtility.LookLikeInspector();
        var content = new GUIContent(label, FOLD_OUT_TOOLTIP);
        expanded = EditorGUILayout.Foldout(expanded, content);
        LookLikeControls();

        return expanded;
    }
	
	public static void ShowColorWarning(string warningText) {
		ShowColorWarning(warningText, Color.green);
	}
	
	public static void ShowColorWarning(string warningText, Color color) {
		GUI.color = color;
		EditorGUILayout.LabelField(warningText, EditorStyles.miniLabel);
		GUI.color = Color.white;
	}
	
    public static void LookLikeControls()
    {
        EditorGUIUtility.LookLikeControls(CONTROLS_DEFAULT_LABEL_WIDTH);
    }

	public static void ShowAlert(string text) {
		EditorUtility.DisplayDialog(GUIHelper.ALERT_TITLE, text,
				GUIHelper.ALERT_OK_TEXT);
	}
}
