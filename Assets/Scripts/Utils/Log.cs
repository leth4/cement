using UnityEngine;

public class Log
{
    public static string Delimiter = ", ";

    public static void Message(params object[] messages)
    {
        string text = "";
        for (int i = 0; i < messages.Length; i++)
        {
            text += messages[i].ToString();
            if (i < messages.Length - 1) text += Delimiter;
        }
        Debug.Log(text);
    }

    public static void Warning(params object[] messages)
    {
        string text = "";
        for (int i = 0; i < messages.Length; i++)
        {
            text += messages[i].ToString();
            if (i < messages.Length - 1) text += Delimiter;
        }
        Debug.LogWarning(text);
    }

    public static void Error(params object[] messages)
    {
        string text = "";
        for (int i = 0; i < messages.Length; i++)
        {
            text += messages[i].ToString();
            if (i < messages.Length - 1) text += Delimiter;
        }
        Debug.LogError(text);
    }
}
