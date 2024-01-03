using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioReceiver
{
    public static void StartMusic()
    {
        if (!AudioManager.Instance.IsPlaying(SoundEnum.music)) AudioManager.Instance.Play(SoundEnum.music, 0, true);
    }

    public static void SceneTransition()
    {
        AudioManager.Instance.Play(SoundEnum.next);
    }

    public static void LevelSolved()
    {
        AudioManager.Instance.Play(SoundEnum.next);
    }

    public static void NewCardUnlocked()
    {
        AudioManager.Instance.Play(SoundEnum.win);
    }

    public static void ButtonPressed()
    {
        AudioManager.Instance.Play(SoundEnum.ui);
    }

    public static void CardAdded()
    {
        var sounds = new List<SoundEnum> { SoundEnum.start_1, SoundEnum.start_2, SoundEnum.start_3 };
        AudioManager.Instance.Play(sounds[Random.Range(0, sounds.Count)], 0);
    }

    public static void CardHover()
    {
        AudioManager.Instance.Play(SoundEnum.hover);
    }

    public static void AbilityApplied()
    {
        AudioManager.Instance.Play(SoundEnum.apply);
    }

    public static void AbilityUndone()
    {
        AudioManager.Instance.Play(SoundEnum.undone);
    }
}
