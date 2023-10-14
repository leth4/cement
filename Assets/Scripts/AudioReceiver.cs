using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioReceiver
{
    public static void StartMusic()
    {
        if (!AudioManager.Instance.IsPlaying(SoundEnum.CementCement)) AudioManager.Instance.Play(SoundEnum.CementCement);
    }
    
    public static void SceneTransition()
    {

    }

    public static void LevelSolved()
    {

    }

    public static void NewCardUnlocked()
    {

    }

    public static void ButtonPressed()
    {

    }

    public static void CardAdded()
    {

    }

    public static void AbilityApplied()
    {

    }

    public static void AbilityUndone()
    {

    }
}
