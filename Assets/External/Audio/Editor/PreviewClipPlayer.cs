using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public static class PreviewClipPlayer
{
    public static void PlayClip(AudioClip clip)
    {
        if (IsClipPlaying())
        {
            StopAllClips();
            return;
        }
        var unityEditorAssembly = typeof(AudioImporter).Assembly;
        var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        var method = audioUtilClass.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
            null
        );
        method.Invoke(null, new object[] { clip, 0, false });
    }

    public static bool IsClipPlaying()
    {
        var unityEditorAssembly = typeof(AudioImporter).Assembly;
        var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        var method = audioUtilClass.GetMethod(
            "IsPreviewClipPlaying",
            BindingFlags.Static | BindingFlags.Public
        );
        var playing = (bool)method.Invoke(null, new object[] { });
        return playing;
    }

    public static void StopAllClips()
    {
        var unityEditorAssembly = typeof(AudioImporter).Assembly;
        var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        var method = audioUtilClass.GetMethod(
            "StopAllPreviewClips",
            BindingFlags.Static | BindingFlags.Public
        );
        method.Invoke(null, null);
    }
}
