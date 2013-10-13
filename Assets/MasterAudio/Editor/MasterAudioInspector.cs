using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MasterAudio))] 
public class MasterAudioInspector : Editor {
    private const string NEW_BUS_NAME = "[NEW BUS]";
	private const string RENAME_ME_BUS_NAME = "[BUS NAME]";
	private bool isValid = true;
	
	public List<MasterAudioGroup> groups = new List<MasterAudioGroup>();
	
	private void ScanGroups(MasterAudio sounds) {
		this.groups.Clear();
		
		var names = new List<string>();
		
		for (var i = 0; i < sounds.transform.childCount; i++) {
			var aChild = sounds.transform.GetChild(i);
			if (names.Contains(aChild.name)) {
				GUIHelper.ShowColorWarning("You have more than one group named '" + aChild.name + "'.");
				GUIHelper.ShowColorWarning("Please rename one of them before continuing.");
				isValid = false;
				return;
			}
			
			names.Add(aChild.name);
			this.groups.Add(aChild.GetComponent<MasterAudioGroup>());
		}

		if (sounds.groupByBus) {
			this.groups.Sort(delegate(MasterAudioGroup g1, MasterAudioGroup g2)     {
    	    	if (g1.busIndex == g2.busIndex) {
					return g1.name.CompareTo(g2.name);
				} else {
					return g1.busIndex.CompareTo(g2.busIndex);
				}
	    	});
		} else {
			this.groups.Sort(delegate(MasterAudioGroup g1, MasterAudioGroup g2)     {
    	    	return g1.name.CompareTo(g2.name);
	    	});
		}
	}
	
	private List<string> GroupNameList {
		get {
			var groupNames = new List<string>();
			groupNames.Add(MasterAudio.NO_GROUP_NAME);
			
			for (var i = 0; i < groups.Count; i++) {
				groupNames.Add(groups[i].name);
			}
			
			return groupNames;
		}
	}
		
	public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();
		EditorGUI.indentLevel = 0;
		
		MasterAudio sounds = (MasterAudio)target;

		if (sounds.logoTexture != null) {
			GUIHelper.DrawTexture(sounds.logoTexture);
		}
		
		this.ScanGroups(sounds);	
		
		if (!isValid) {
			return;
		}
		
		var groupNameList = GroupNameList;
		
		var maxChars = 9;
		var busList = new List<string>();
		busList.Add(MasterAudioGroup.NO_BUS);
		busList.Add(NEW_BUS_NAME);

		GroupBus bus = null;
		for (var i = 0; i < sounds.groupBuses.Count; i++) {
			bus = sounds.groupBuses[i];
			busList.Add(bus.busName);
			
			if (bus.busName.Length > maxChars) {
				maxChars = bus.busName.Length;
			}
		}
		var busListWidth = 9 * maxChars;
		
		bool isDirty = false;

		var playlistCont = GUIHelper.GetSinglePlaylistController();
		var plControllerInScene = playlistCont != null;
		
		var volumeBefore = sounds.masterAudioVolume;
		sounds.masterAudioVolume = EditorGUILayout.Slider("Master Audio Volume", sounds.masterAudioVolume, 0f, 1f);
		
		if (volumeBefore != sounds.masterAudioVolume) {
			// fix it for realtime adjustments!
			MasterAudio.MasterVolumeLevel = sounds.masterAudioVolume;
		}
		
		sounds.allowRetriggerAfterPercentage = EditorGUILayout.IntSlider("Retrigger Percentage", sounds.allowRetriggerAfterPercentage, 0, 100);
		sounds.audioSourceMode = (MasterAudio.AudioSourceMode) EditorGUILayout.EnumPopup("Audio Source Mode", sounds.audioSourceMode);
		sounds.missingLogMode = (MasterAudio.MissingSoundLogSeverity) EditorGUILayout.EnumPopup("Missing Sound Log Mode", sounds.missingLogMode);
		sounds.persistBetweenScenes = EditorGUILayout.Toggle("Persist Across Scenes", sounds.persistBetweenScenes);
		if (sounds.persistBetweenScenes && plControllerInScene) {
			GUIHelper.ShowColorWarning("*Playlist Controller will also persist between scenes!");
		}
		sounds.LogSounds = EditorGUILayout.Toggle("Log Sounds", sounds.LogSounds);

		EditorGUI.indentLevel = 0;
       	EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		
		sounds.showMusicDucking = EditorGUILayout.Toggle("Show Music Ducking", sounds.showMusicDucking);		
		EditorGUILayout.EndHorizontal();
		
		if (sounds.showMusicDucking) {
			sounds.EnableMusicDucking = EditorGUILayout.Toggle("Enable Ducking", sounds.EnableMusicDucking);

			EditorGUI.indentLevel = 2;
			var buttonPressed = GUIHelper.AddSingleFoldOutListItemButtons();
			
			switch (buttonPressed) {
				case GUIHelper.DTFunctionButtons.Add:
					sounds.musicDuckingSounds.Add(MasterAudio.NO_GROUP_NAME);	
					isDirty = true;
					break;
				case GUIHelper.DTFunctionButtons.Remove:
					if (sounds.musicDuckingSounds.Count > 0) {
						sounds.musicDuckingSounds.RemoveAt(sounds.musicDuckingSounds.Count - 1);
					}	
					isDirty = true;
					break;
			}
			
			if (sounds.musicDuckingSounds.Count == 0) {
			 	GUIHelper.ShowColorWarning("No ducking sounds set up yet.");
			} else {
				for (var i = 0; i < sounds.musicDuckingSounds.Count; i++) {
					var index = groupNameList.IndexOf(sounds.musicDuckingSounds[i]);
					if (index == -1) {
						index = 0;
					}
					var newIndex = EditorGUILayout.Popup(index, groupNameList.ToArray());
					if (newIndex >= 0) {
						sounds.musicDuckingSounds[i] = groupNameList[newIndex];
					}
				}
			}
		}

		// Sound Groups Start		
        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel = 0;  // Space will handle this for the header

        EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        sounds.areGroupsExpanded = EditorGUILayout.Toggle("Show Group Mixer", sounds.areGroupsExpanded);

		EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();
		
		GameObject groupToDelete = null;
		
		if (sounds.areGroupsExpanded) {
			EditorGUI.indentLevel = 0;

			// create groups start
			EditorGUILayout.BeginVertical();
			var anEvent = Event.current;

			GUI.color = Color.yellow;
			
			var dragArea = GUILayoutUtility.GetRect(0f,35f,GUILayout.ExpandWidth(true));
			GUI.Box (dragArea, "Drag Audio clips here to create groups!");

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
							
							CreateSoundGroup(sounds, aClip);
						}
					}
					Event.current.Use();
					break;
			}
			EditorGUILayout.EndVertical();
			// create groups end
			
			EditorGUILayout.LabelField("Group Control", EditorStyles.miniBoldLabel);
			
			if (sounds.groupBuses.Count > 0) {
				sounds.groupByBus = GUILayout.Toggle(sounds.groupByBus, "Group by Bus");
			}
			
			GUIHelper.DTFunctionButtons groupButtonPressed = GUIHelper.DTFunctionButtons.None;
			
			MasterAudioGroup aGroup = null;
			if (this.groups.Count == 0) {
				GUIHelper.ShowColorWarning("You currently have zero sound groups.");
			} else {
				int? busToCreate = null;
				
				if (this.groups.Count >= MasterAudio.MAX_SOUND_GROUPS - 1) {
					GUIHelper.ShowColorWarning("*Free version allows a maximum of " + MasterAudio.MAX_SOUND_GROUPS + " Sound Groups.");
				}
				
				for (var l = 0; l < this.groups.Count; l++) {
					EditorGUI.indentLevel = 0;
					aGroup = this.groups[l];
		
					string sType = string.Empty;
					if (Application.isPlaying) {
						sType = aGroup.name;
					}
					
		            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
		        	var groupName = aGroup.name;

					EditorGUILayout.LabelField(groupName, EditorStyles.label, GUILayout.MinWidth(50));    
					//GUILayout.Space(90);
					
					EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
					
					// find bus.
					var selectedBusIndex = aGroup.busIndex == -1 ? 0 : aGroup.busIndex;
					
					GUI.contentColor = Color.white;
					GUI.color = Color.cyan;
					
					var busIndex = EditorGUILayout.Popup("", selectedBusIndex, busList.ToArray(), GUILayout.Width(busListWidth));
					if (busIndex == -1) {
						busIndex = 0; // remove later
					}
					
					aGroup.busIndex = busIndex;
					GUI.color = Color.white;
					
					if (selectedBusIndex != busIndex) {
						if (aGroup.busIndex == 1) {
							busToCreate = l;
						} else if (Application.isPlaying && busIndex >= MasterAudio.HARD_CODED_BUS_OPTIONS) {
							var statGroup = MasterAudio.GrabGroup(sType);
							statGroup.busIndex = busIndex;
						}
					}
					
					GUI.contentColor = Color.green;
					GUILayout.TextField("V " + aGroup.groupMasterVolume.ToString("N2"), 6, EditorStyles.miniLabel);

					aGroup.groupMasterVolume = GUILayout.HorizontalSlider(aGroup.groupMasterVolume, 0f, 1f, GUILayout.Width(60));

					GUI.contentColor = Color.white;
					groupButtonPressed = GUIHelper.AddMixerButtons(aGroup, "Group", sounds);

			        EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndHorizontal();
					
					switch (groupButtonPressed) {
						case GUIHelper.DTFunctionButtons.Play:
							MasterAudio.PlaySound(aGroup.name);
							break;
						case GUIHelper.DTFunctionButtons.Mute:
							isDirty = true;
						
							if (Application.isPlaying) {
								if (aGroup.isMuted) {
									MasterAudio.UnmuteGroup(sType);
								} else {
									MasterAudio.MuteGroup(sType);
								}
							} else {
								aGroup.isMuted = !aGroup.isMuted;
								if (aGroup.isMuted) {
									aGroup.isSoloed = false;
								}
							}
							break;
						case GUIHelper.DTFunctionButtons.Solo:
							isDirty = true;
							if (Application.isPlaying) {
								if (aGroup.isSoloed) {							
									MasterAudio.UnsoloGroup(sType);
								} else {
									MasterAudio.SoloGroup(sType);
								}
							} else {
								aGroup.isSoloed = !aGroup.isSoloed;
								if (aGroup.isSoloed) {
									aGroup.isMuted = false;
								} 
							}
							break;
						case GUIHelper.DTFunctionButtons.Go:
							Selection.activeObject = aGroup.transform;				
							break;
						case GUIHelper.DTFunctionButtons.Remove:
							isDirty = true;
							groupToDelete = aGroup.transform.gameObject;
							break;
					}
					
					EditorUtility.SetDirty(aGroup);
				}
				
				if (busToCreate.HasValue) {
					CreateBus(busToCreate.Value, sounds);
				}
				
				if (groupToDelete != null) {
					GameObject.DestroyImmediate(groupToDelete);
				}
				
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUI.contentColor = Color.green;
				if (GUILayout.Button(new GUIContent("Mute/Solo Reset", "Turn off all group mute and solo switches"), EditorStyles.toolbarButton, GUILayout.Width(120))) { 
					isDirty = true;
					for (var l = 0; l < this.groups.Count; l++) {
						aGroup = this.groups[l];
						aGroup.isSoloed = false;
						aGroup.isMuted = false;
					}
					
				}
				
				GUILayout.Space(6);
				
				if (GUILayout.Button(new GUIContent("Max Group Volumes", "Reset all group volumes to full"), EditorStyles.toolbarButton, GUILayout.Width(120))) { 
					isDirty = true;
					for (var l = 0; l < this.groups.Count; l++) {
						aGroup = this.groups[l];
						aGroup.groupMasterVolume = 1f;
					}
				}

				GUI.contentColor = Color.white;
				
				EditorGUILayout.EndHorizontal();
			}
			// Sound Groups End

			// Buses
			if (sounds.groupBuses.Count > 0) {
				EditorGUILayout.Separator();
				EditorGUILayout.LabelField("Bus Control", EditorStyles.miniBoldLabel);
				
				GroupBus aBus = null;
				GUIHelper.DTFunctionButtons busButtonPressed = GUIHelper.DTFunctionButtons.None;
				int? busToDelete = null;
				int? busToSolo = null;
				int? busToMute = null;
				
				for (var i = 0; i < sounds.groupBuses.Count; i++) {
					aBus = sounds.groupBuses[i];
					
		            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
					
					aBus.busName = EditorGUILayout.TextField("", aBus.busName);
					
					EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
					
					GUI.contentColor = Color.green;
					
					GUILayout.TextField("V " + aBus.volume.ToString("N2"), 6, EditorStyles.miniLabel);
					aBus.volume = GUILayout.HorizontalSlider(aBus.volume, 0f, 1f, GUILayout.Width(60));

					GUI.contentColor = Color.white;
					
					busButtonPressed = GUIHelper.AddMixerBusButtons(aBus, sounds);
					
					switch (busButtonPressed) {
						case GUIHelper.DTFunctionButtons.Remove:
							busToDelete = i;
							break;
						case GUIHelper.DTFunctionButtons.Solo:
							busToSolo = i;
							break;
						case GUIHelper.DTFunctionButtons.Mute:
							busToMute = i;
							break;
					}
					
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndHorizontal();
				}
				
				if (busToDelete.HasValue) {
					DeleteBus(busToDelete.Value, sounds);
				}
				if (busToMute.HasValue) {
					MuteBus(busToMute.Value, sounds);
				}
				if (busToSolo.HasValue) {
					SoloBus(busToSolo.Value, sounds);
				}
				
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUI.contentColor = Color.green;
				
				if (GUILayout.Button(new GUIContent("Mute/Solo Reset", "Turn off all bus mute and solo switches"), EditorStyles.toolbarButton, GUILayout.Width(120))) { 
					isDirty = true;
					for (var l = 0; l < sounds.groupBuses.Count; l++) {
						aBus = sounds.groupBuses[l];
						aBus.isSoloed = false;
						aBus.isMuted = false;
					}
				}
				
				GUILayout.Space(6);
				
				if (GUILayout.Button(new GUIContent("Max Bus Volumes", "Reset all bus volumes to full"), EditorStyles.toolbarButton, GUILayout.Width(120))) { 
					isDirty = true;
					for (var l = 0; l < sounds.groupBuses.Count; l++) {
						aBus = sounds.groupBuses[l];
						aBus.volume = 1f;
					}
				}
				
				GUI.contentColor = Color.white;
				
				EditorGUILayout.EndHorizontal();
			}
		}
		// Sound Buses End

		
		// Music playlist Start		
        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel = 0;  // Space will handle this for the header
		
		if (sounds.audioSourceMode == MasterAudio.AudioSourceMode.PlaylistController) {
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
	        sounds.playListExpanded = EditorGUILayout.Toggle("Show Playlist Settings", sounds.playListExpanded);
	
			EditorGUILayout.EndHorizontal();
	        EditorGUILayout.EndHorizontal();
	
			if (sounds.playListExpanded) {
				GUIHelper.ShowColorWarning("*If you choose Start playlist on Awake, the top playlist will be used.");
				sounds.playlistControls._startPlaylistOnAwake = EditorGUILayout.Toggle("Start playlist on Awake", sounds.playlistControls._startPlaylistOnAwake );
				sounds.playlistControls._shuffle = EditorGUILayout.Toggle("Shuffle mode", sounds.playlistControls._shuffle);
				sounds.playlistControls._autoAdvance = EditorGUILayout.Toggle("Auto advance clips", sounds.playlistControls._autoAdvance);
				sounds.playlistControls._repeatPlaylist = EditorGUILayout.Toggle("Repeat Playlist", sounds.playlistControls._repeatPlaylist);
				sounds.playlistControls._loopClips = EditorGUILayout.Toggle("Loop clips", sounds.playlistControls._loopClips);
				
				if (sounds.playlistControls._autoAdvance && sounds.playlistControls._loopClips) {
					GUIHelper.ShowColorWarning("*You cannot use looping and auto advance at the same time.");
				}
				
				EditorGUILayout.Separator();
				
				if (!plControllerInScene) {
					GUIHelper.ShowColorWarning("There is no Playlist Controller in the scene. Music will not play.");
					GUI.contentColor = Color.green;
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(10);
					if (GUILayout.Button(new GUIContent("Create Playlist Controller"), EditorStyles.toolbarButton, GUILayout.Width(150))) {
						var go = GameObject.Instantiate(sounds.playlistControllerPrefab);
						go.name = "PlaylistController";
					}
					EditorGUILayout.EndHorizontal();
					GUI.contentColor = Color.white;
					EditorGUILayout.Separator();
				}
				
		        EditorGUI.indentLevel = 0;  // Space will handle this for the header

				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
		        sounds.playlistEditorExpanded = GUIHelper.Foldout(sounds.playlistEditorExpanded, "Playlists");
	
		        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
				
				if (sounds.musicPlaylists.Count > 0) {
					GUIContent content;
			        var collapseIcon = '\u2261'.ToString();
			        content = new GUIContent(collapseIcon, "Click to collapse all");
			        var masterCollapse = GUILayout.Button(content, EditorStyles.toolbarButton);
			
			        var expandIcon = '\u25A1'.ToString();
			        content = new GUIContent(expandIcon, "Click to expand all");
			        var masterExpand = GUILayout.Button(content, EditorStyles.toolbarButton);
					if (masterExpand) {
						ExpandCollapseAllPlaylists(sounds, true);
					} 
					if (masterCollapse) {
						ExpandCollapseAllPlaylists(sounds, false);
					}
			        EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndHorizontal();
					
					if (sounds.playlistEditorExpanded) {
						int? playlistToRemove = null;
						int? playlistToInsertAt = null;
						int? playlistToMoveUp = null;
						int? playlistToMoveDown = null;
						
						for (var i = 0; i < sounds.musicPlaylists.Count; i++) {
							EditorGUILayout.Separator();
							var aList = sounds.musicPlaylists[i];
							
							EditorGUI.indentLevel = 1;
							EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
							aList.isExpanded = GUIHelper.Foldout(aList.isExpanded, "Playlist: " + aList.playlistName);
							
							var playlistButtonPressed = GUIHelper.AddFoldOutListItemButtons(i, sounds.musicPlaylists.Count, "playlist", false, true);
							
							EditorGUILayout.EndHorizontal();
		
							if (!aList.isExpanded) {
								continue;
							}
							
							EditorGUI.indentLevel = 5;
							aList.playlistName = EditorGUILayout.TextField("Name", aList.playlistName);
							
							EditorGUILayout.BeginVertical();
							var anEvent = Event.current;
				
							GUI.color = Color.yellow;
							
							var dragArea = GUILayoutUtility.GetRect(0f,35f,GUILayout.ExpandWidth(true));
							GUI.Box (dragArea, "Drag Audio clips here to add to playlist!");
				
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
											
											AddSongToPlaylist(aList, aClip);
										}
									}
									Event.current.Use();
									break;
							}
							EditorGUILayout.EndVertical();
							
							EditorGUI.indentLevel = 2;
							
							int? addIndex = null;
							int? removeIndex = null;
							int? moveUpIndex = null;
							int? moveDownIndex = null;
							
							for (var j = 0; j < aList.MusicSettings.Count; j++) {
						        var aSong = aList.MusicSettings[j];
								var clipName = aSong.clip == null ? "Empty" : aSong.clip.name;
								EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
								EditorGUI.indentLevel = 2;
								aSong.isExpanded = GUIHelper.Foldout(aSong.isExpanded, clipName);
								var songButtonPressed = GUIHelper.AddFoldOutListItemButtons(j, aList.MusicSettings.Count, "clip", false, true);
								EditorGUILayout.EndHorizontal();
								
								if (aSong.isExpanded) {
									EditorGUI.indentLevel = 6;
									aSong.clip = (AudioClip) EditorGUILayout.ObjectField("Clip", aSong.clip, typeof(AudioClip), true);
									aSong.volume = EditorGUILayout.Slider("Volume", aSong.volume, 0f, 1f);
									aSong.pitch = EditorGUILayout.Slider("Pitch", aSong.pitch, 0f, 10f);
								}
								
								switch (songButtonPressed) {
									case GUIHelper.DTFunctionButtons.Add:
										addIndex = j;
										break;
									case GUIHelper.DTFunctionButtons.Remove:
										removeIndex = j;
										break;
									case GUIHelper.DTFunctionButtons.ShiftUp:
										moveUpIndex = j;
										break;
									case GUIHelper.DTFunctionButtons.ShiftDown:
										moveDownIndex = j;
										break;
								}
							}
							
							if (addIndex.HasValue) {
								var mus = new MusicSetting();
								aList.MusicSettings.Insert(addIndex.Value + 1, mus);
							} else if (removeIndex.HasValue) {
								if (aList.MusicSettings.Count <= 1) {
									GUIHelper.ShowAlert("You cannot delete the last clip. You do not have to use the clips though.");
								} else {
									aList.MusicSettings.RemoveAt(removeIndex.Value); 
								}
							} else if (moveUpIndex.HasValue) {
								var item = aList.MusicSettings[moveUpIndex.Value];
								aList.MusicSettings.Insert(moveUpIndex.Value - 1, item);
								aList.MusicSettings.RemoveAt(moveUpIndex.Value + 1);
							} else if (moveDownIndex.HasValue) {
								var index = moveDownIndex.Value + 1;
				
								var item = aList.MusicSettings[index];
								aList.MusicSettings.Insert(index - 1, item);
								aList.MusicSettings.RemoveAt(index + 1);
							}
							
							switch (playlistButtonPressed) {
								case GUIHelper.DTFunctionButtons.Remove:
									playlistToRemove = i;
									break;
								case GUIHelper.DTFunctionButtons.Add:
									playlistToInsertAt = i;
									break;
								case GUIHelper.DTFunctionButtons.ShiftUp:
									playlistToMoveUp = i;
									break;
								case GUIHelper.DTFunctionButtons.ShiftDown:
									playlistToMoveDown = i;
									break;
							}
						}
						
						if (playlistToRemove.HasValue) {
							if (sounds.musicPlaylists.Count <= 1) {
								GUIHelper.ShowAlert("You cannot delete the last Playlist. You do not have to use it though.");
							} else {
								sounds.musicPlaylists.RemoveAt(playlistToRemove.Value);
							}
						}
						if (playlistToInsertAt.HasValue) {
							var pl = new MasterAudio.Playlist();
							sounds.musicPlaylists.Insert(playlistToInsertAt.Value + 1, pl);
						}
						if (playlistToMoveUp.HasValue) {
							var item = sounds.musicPlaylists[playlistToMoveUp.Value];
							sounds.musicPlaylists.Insert(playlistToMoveUp.Value - 1, item);
							sounds.musicPlaylists.RemoveAt(playlistToMoveUp.Value + 1);
						}
						if (playlistToMoveDown.HasValue) {
							var index = playlistToMoveDown.Value + 1;
			
							var item = sounds.musicPlaylists[index];
							sounds.musicPlaylists.Insert(index - 1, item);
							sounds.musicPlaylists.RemoveAt(index + 1);
						}
					}
			    } else {
			     	GUILayout.FlexibleSpace();
			        EditorGUILayout.EndHorizontal();
			        EditorGUILayout.EndHorizontal();
			    }
			}
			// Music playlist End
		} else {
	        EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
	        GUIHelper.ShowColorWarning("Playlist Controller disabled by setting: 'Audio Source Mode'");
	
			EditorGUILayout.EndHorizontal();
		}
		
		
		if (GUI.changed || isDirty) {
			EditorUtility.SetDirty(target);
		}
		
		//DrawDefaultInspector();
    }
	
	private void AddSongToPlaylist(MasterAudio.Playlist pList, AudioClip aClip) {
		var lastClip = pList.MusicSettings[pList.MusicSettings.Count - 1];
		
		MusicSetting mus;
		
		if (lastClip.clip == null) {
			mus = lastClip;
			mus.clip = aClip;
		} else {
			mus = new MusicSetting() {
				clip = aClip,
				volume = 1f,
				pitch = 1f,
				isExpanded = true
			};

			pList.MusicSettings.Add(mus);
		}
	}
	
	private void CreateSoundGroup(MasterAudio sounds, AudioClip aClip) {
		var groupName = aClip.name;
		
		if (sounds.soundGroupTemplate == null || sounds.soundGroupVariationTemplate == null) {
			GUIHelper.ShowAlert("Your MasterAudio prefab has been altered and cannot function properly. Please Revert it before continuing.");
			return;
		}
	
		if (sounds.transform.FindChild(groupName) != null) {
			GUIHelper.ShowAlert("You already have a Sound Group named '" + groupName + "'. Please rename one of them when finished.");
		}
		
		GameObject newGroup = (GameObject) GameObject.Instantiate(sounds.soundGroupTemplate.gameObject, sounds.transform.position, Quaternion.identity);		

		var groupTrans = newGroup.transform;
		groupTrans.name = UtilStrings.CapitalizeFirstLetter(groupName);
		groupTrans.parent = sounds.transform;
		
		var sName = groupName;
		SoundGroupVariation variation = null;
		
		GameObject newVariation = (GameObject) GameObject.Instantiate(sounds.soundGroupVariationTemplate.gameObject, groupTrans.position, Quaternion.identity);		
		
		variation = newVariation.GetComponent<SoundGroupVariation>();
		variation.possibleName = groupName;
		
		newVariation.audio.clip = aClip;
		newVariation.transform.name = sName;
		newVariation.transform.parent = groupTrans;
	}
	
	private void CreateBus(int groupIndex, MasterAudio ma) {
		var newBus = new GroupBus() {
			busName = RENAME_ME_BUS_NAME
		};
		ma.groupBuses.Add(newBus);
		
		this.groups[groupIndex].busIndex = MasterAudio.HARD_CODED_BUS_OPTIONS + ma.groupBuses.Count - 1;
	}

	private void SoloBus(int busIndex, MasterAudio ma) {
		var bus = ma.groupBuses[busIndex];
		bus.isSoloed = !bus.isSoloed;
		if (bus.isSoloed) {
			bus.isMuted = false;
		}
		
		MasterAudioGroup aGroup = null;
		string sType = string.Empty;
		
		for (var i = 0; i < this.groups.Count; i++) {
			aGroup = this.groups[i];
			
			if (aGroup.busIndex != MasterAudio.HARD_CODED_BUS_OPTIONS + busIndex) {
				continue;
			}
		
			sType = aGroup.name;
			
			if (Application.isPlaying) {
				if (aGroup.isSoloed) {
					MasterAudio.UnsoloGroup(sType);
				} else {
					MasterAudio.SoloGroup(sType);
				}
			} 

			aGroup.isSoloed = bus.isSoloed;
			if (bus.isSoloed) {
				aGroup.isMuted = false;
			}
		}
	}

	private void MuteBus(int busIndex, MasterAudio ma) {
		var bus = ma.groupBuses[busIndex];
		bus.isMuted = !bus.isMuted;
		if (bus.isSoloed) {
			bus.isSoloed = false;
		}

		MasterAudioGroup aGroup = null;
		
		for (var i = 0; i < this.groups.Count; i++) {
			aGroup = this.groups[i];
			
			if (aGroup.busIndex != MasterAudio.HARD_CODED_BUS_OPTIONS + busIndex) {
				continue;
			}

			aGroup.isMuted = !aGroup.isMuted;
			if (bus.isMuted) {
				aGroup.isSoloed = false;
			}
		}
	}
	
	private void DeleteBus(int busIndex, MasterAudio ma) {
		ma.groupBuses.RemoveAt(busIndex);
		
		MasterAudioGroup aGroup = null;
		
		for (var i = 0; i < this.groups.Count; i++) {
			aGroup = this.groups[i];
			if (aGroup.busIndex == -1) {
				continue;
			}
			if (aGroup.busIndex != busIndex + MasterAudio.HARD_CODED_BUS_OPTIONS) {					
				continue;
			}
			
			aGroup.busIndex = -1;
		}

		// adjust all other groups in case the indexes moved
		for (var i = 0; i < this.groups.Count; i++) {
			aGroup = this.groups[i];
			if (aGroup.busIndex == -1) {
				continue;
			}
			if (aGroup.busIndex < MasterAudio.HARD_CODED_BUS_OPTIONS) {
				continue;
			}
			aGroup.busIndex--;
		}
	}
	
	private void ExpandCollapseAllPlaylists(MasterAudio sounds, bool expand) {
		for (var i = 0; i < sounds.musicPlaylists.Count; i++) {
			var aList = sounds.musicPlaylists[i];
			aList.isExpanded = expand;
			
			for (var j = 0; j < aList.MusicSettings.Count; j++) {
				var aSong = aList.MusicSettings[j];
				aSong.isExpanded = expand;
			}
		}
	}
}
