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
        AudioManager.Instance.Play(SoundEnum.next);
    }

    public static void LevelSolved()
    {
        AudioManager.Instance.Play(SoundEnum.win);
    }

    public static void NewCardUnlocked()
    {
        AudioManager.Instance.Play(SoundEnum.new_card_2);
    }

    public static void ButtonPressed()
    {
        AudioManager.Instance.Play(SoundEnum.ui);
    }

    public static void CardAdded()
    {

    }

    public static void CardHover()
    {

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
