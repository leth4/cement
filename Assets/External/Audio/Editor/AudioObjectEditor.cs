using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioObject))]
public class AudioObjectEditor : Editor
{
    private AudioObject _audio;
    private AudioManager _audioManager;
    private bool _channelsShown = false;
    private bool _soundsShown = false;
    private bool _mixerShown = false;
    private Dictionary<Sound, bool> _foldoutStates;
    private bool _hasMissingClips;
    private Color _backgroundColor = new Color(0.8f, 0.8f, 0.8f);
    private string[] _soundEnumsFilePaths = { "Assets/Scripts/Audio/SoundEnums.cs", "Assets/External/Audio/SoundEnums.cs" };

    private void OnEnable()
    {
        _audio = (AudioObject)target;
        _audioManager = FindObjectOfType<AudioManager>();
        FillFoldoutStateDictionary();
        Undo.undoRedoPerformed += FillFoldoutStateDictionary;
        if (!_audio.EnumsAssigned) AssignEnums();
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= FillFoldoutStateDictionary;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(_audio, "Changed an Audio Object");
        EditorUtility.SetDirty(_audio);
        GUI.backgroundColor = _backgroundColor;

        _channelsShown = EditorGUILayout.BeginFoldoutHeaderGroup(_channelsShown, "Channels");
        if (_channelsShown) ShowChannels();
        EditorGUILayout.EndFoldoutHeaderGroup();

        _soundsShown = EditorGUILayout.BeginFoldoutHeaderGroup(_soundsShown, "Sounds");
        if (_soundsShown) ShowSounds();
        EditorGUILayout.EndFoldoutHeaderGroup();

        _mixerShown = EditorGUILayout.BeginFoldoutHeaderGroup(_mixerShown, "Mixer");
        if (_mixerShown) ShowMixer();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5);
        if (_hasMissingClips)
            EditorGUILayout.HelpBox("Some AudioClips are missing!", MessageType.Warning);

        if (GUILayout.Button("Generate Enums"))
            GenerateEnums();
    }

    private void ShowChannels()
    {
        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < _audio.Channels.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _audio.Channels[i].Name = EditorGUILayout.TextField(_audio.Channels[i].Name);
            _audio.Channels[i].Name = CleanupName(_audio.Channels[i].Name);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close"), GUILayout.Width(30)))
            {
                _audio.Channels.RemoveAt(i);
                Repaint();
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus")))
        {
            _audio.Channels.Add(new Channel("New"));
        }

        if (EditorGUI.EndChangeCheck()) ChangeDuplicateChannelNames();
    }

    private void ShowSounds()
    {
        EditorGUI.BeginChangeCheck();
        _hasMissingClips = false;

        foreach (var channel in _audio.Channels)
        {
            GUI.backgroundColor = Color.clear;
            using (var vertical = new EditorGUILayout.VerticalScope(new GUIStyle() { border = new RectOffset() }))
            {
                CreateDropArea(vertical.rect, channel);
                GUI.backgroundColor = _backgroundColor;
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(channel.Name, EditorStyles.boldLabel);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus"), GUILayout.Width(30)))
                {
                    AddSound(channel);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < channel.Sounds.Count; i++)
            {
                using (var vertical = new EditorGUILayout.VerticalScope("Box"))
                {
                    CreateDropArea(vertical.rect, sound: channel.Sounds[i]);
                    if (!ShowSoundEditor(channel.Sounds[i]))
                    {
                        RemoveSound(channel.Sounds[i], channel);
                        Repaint();
                        break;
                    }
                }
            }
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck()) ChangeDuplicateSoundNames();
        }
    }

    private bool ShowSoundEditor(Sound sound)
    {
        GUILayout.BeginHorizontal();
        string soundName = sound.Name;
        if (sound.Clips.Count == 0)
        {
            _hasMissingClips = true;
            soundName += " (No Audio Clips)";
            GUI.color = new Color(1f, 0.3f, 0.3f, 1);
        }
        else
            foreach (var clip in sound.Clips)
            {
                if (clip != null && clip.AudioClip != null) continue;
                _hasMissingClips = true;
                soundName += " (Has Unassigned Clips)";
                GUI.color = new Color(1f, 0.3f, 0.3f, 1);
                break;
            }
        _foldoutStates[sound] = EditorGUILayout.Foldout(_foldoutStates[sound], soundName, true);
        GUI.color = Color.white;
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close"), GUILayout.Width(30)))
            return false;
        GUILayout.EndHorizontal();

        if (!_foldoutStates[sound]) return true;
        EditorGUI.indentLevel++;
        GUILayout.Space(2);

        sound.Name = CleanupName(EditorGUILayout.TextField("Name", sound.Name));

        EditorGUIUtility.labelWidth = 80;
        GUILayout.BeginHorizontal();
        sound.HasPitchVariation = EditorGUILayout.ToggleLeft("Pitch Variation", sound.HasPitchVariation);
        EditorGUI.BeginDisabledGroup(!sound.HasPitchVariation);
        sound.PitchVariation = EditorGUILayout.Slider(sound.PitchVariation, 0, 0.5f);
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = 0;

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        EditorGUI.indentLevel -= 2;
        GUILayout.BeginVertical("HelpBox");

        for (int i = 0; i < sound.Clips.Count; i++)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"  {i + 1}", GUILayout.Width(25));
            sound.Clips[i].AudioClip = (AudioClip)EditorGUILayout.ObjectField(sound.Clips[i].AudioClip, typeof(AudioClip), false);
            EditorGUI.BeginDisabledGroup(sound.Clips[i] == null);
            var symbol = EditorGUIUtility.IconContent(PreviewClipPlayer.IsClipPlaying() ? "d_PauseButton On" : "d_PlayButton On");
            if (GUILayout.Button(symbol, GUILayout.Width(30)))
            {
                PreviewClipPlayer.PlayClip(sound.Clips[i].AudioClip);
            }
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close"), GUILayout.Width(30)))
            {
                sound.Clips.RemoveAt(i);
                break;
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus")))
        {
            sound.Clips.Add(new(null, 1));
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        EditorGUI.indentLevel++;
        GUILayout.Space(2);
        return true;
    }

    private void FillFoldoutStateDictionary()
    {
        _foldoutStates = new Dictionary<Sound, bool>();
        foreach (var channel in _audio.Channels)
            foreach (var sound in channel.Sounds)
                _foldoutStates.Add(sound, false);
    }

    private void ChangeDuplicateChannelNames()
    {
        bool changed = false;
        foreach (var channel in _audio.Channels)
        {
            foreach (var other in _audio.Channels)
            {
                if (channel == other) continue;
                if (channel.Name == other.Name)
                {
                    other.Name += "_New";
                    changed = true;
                }
            }
        }
        if (changed) ChangeDuplicateChannelNames();
    }

    private void ChangeDuplicateSoundNames()
    {
        bool changed = false;
        foreach (var channel in _audio.Channels)
            foreach (var sound in channel.Sounds)
            {
                foreach (var otherChannel in _audio.Channels)
                    foreach (var otherSound in otherChannel.Sounds)
                    {
                        if (otherSound == sound) continue;
                        if (otherSound.Name == sound.Name)
                        {
                            otherSound.Name += "_New";
                            changed = true;
                        }
                    }
            }
        if (changed) ChangeDuplicateSoundNames();
    }

    private void ShowMixer()
    {
        GUILayout.BeginVertical("Box");
        foreach (var channel in _audio.Channels)
        {
            GUILayout.Label(channel.Name, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            foreach (var sound in channel.Sounds)
            {
                EditorGUILayout.LabelField(sound.Name, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var clip in sound.Clips)
                {
                    if (clip == null || clip.AudioClip == null) continue;
                    GUILayout.BeginHorizontal();
                    clip.Volume = EditorGUILayout.Slider(clip.AudioClip.name, clip.Volume, 0, 1);
                    if (_audioManager != null)
                    {
                        var symbol = EditorGUIUtility.IconContent(_audioManager.IsPlayingInEditor(clip) ? "d_PauseButton On" : "d_PlayButton On");
                        if (GUILayout.Button(symbol, GUILayout.Width(30)))
                            _audioManager?.PlaySoundInEditor(clip, sound);
                        _audioManager.SetClipVolume(clip);
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(5);
        }
        GUILayout.EndVertical();

        if (_audioManager == null)
        {
            _audioManager = (AudioManager)EditorGUILayout.ObjectField("Audio Manager", _audioManager, typeof(AudioManager), true);
        }
    }

    private void AddSound(Channel channel, Sound sound = null)
    {
        if (sound == null)
            sound = new Sound("New");
        channel.Sounds.Add(sound);
        _foldoutStates.Add(sound, false);
    }

    private void RemoveSound(Sound sound, Channel channel)
    {
        channel.Sounds.Remove(sound);
        if (_foldoutStates.ContainsKey(sound))
            _foldoutStates.Remove(sound);
    }

    private void CreateDropArea(Rect rect, Channel channel = null, Sound sound = null)
    {
        Event evt = Event.current;
        if (evt.type is not EventType.DragPerform and not EventType.DragUpdated) return;
        if (!rect.Contains(evt.mousePosition)) return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (evt.type is not EventType.DragPerform) return;

        DragAndDrop.AcceptDrag();

        foreach (var draggedObject in DragAndDrop.objectReferences)
        {
            if (draggedObject is not AudioClip) continue;
            if (sound != null)
            {
                sound.Clips.Add(new((AudioClip)draggedObject, 1));
            }
            else if (channel != null)
            {
                var newSound = new Sound("");
                newSound.Clips.Add(new((AudioClip)draggedObject, 1));
                newSound.Name = CleanupName(newSound.Clips[0].AudioClip.name);
                AddSound(channel, newSound);
            }
        }
    }

    private void GenerateEnums()
    {
        System.Console.WriteLine(System.DateTime.Now.ToString());

        var soundEnumsFilePath = "";

        foreach (var path in _soundEnumsFilePaths)
        {
            if (!File.Exists(path)) continue;

            soundEnumsFilePath = path;
            break;
        }

        if (soundEnumsFilePath == "")
        {
            Debug.LogWarning("Can't find SoundEnums.cs. Please place the Audio/ folder either in the External/ or Scripts/ folder, or manually change the path in the AudioObjectEditor.cs script.");
            return;
        }

        using (StreamWriter streamWriter = new StreamWriter(soundEnumsFilePath))
        {
            streamWriter.WriteLine("public enum ChannelEnum");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tNone = -1,");
            for (int i = 0; i < _audio.Channels.Count; i++)
            {
                streamWriter.WriteLine($"\t{_audio.Channels[i].Name} = {i},");
            }
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();

            streamWriter.WriteLine("public enum SoundEnum");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tNone = -1,");
            int soundIndex = 0;
            foreach (var channel in _audio.Channels)
            {
                foreach (var sound in channel.Sounds)
                {
                    streamWriter.WriteLine($"\t{sound.Name} = {soundIndex},");
                    soundIndex++;
                }
            }
            streamWriter.WriteLine("}");
        }
        _audio.EnumsAssigned = false;
        AssetDatabase.Refresh();
    }

    private void AssignEnums()
    {
        _audio.EnumsAssigned = true;

        for (int i = 0; i < _audio.Channels.Count; i++)
        {
            _audio.Channels[i].Enum = (ChannelEnum)i;
        }

        int soundIndex = 0;
        foreach (var channel in _audio.Channels)
        {
            foreach (var sound in channel.Sounds)
            {
                sound.Enum = (SoundEnum)soundIndex;
                soundIndex++;
            }
        }

        Debug.Log($"Enums generated and assigned!");
    }

    private string CleanupName(string name)
    {
        return Regex.Replace(name, @"[^a-zA-Z0-9_]", "");
    }
}