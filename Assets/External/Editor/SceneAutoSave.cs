using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

internal class SceneAutoSave : MonoBehaviour
{
    private static CancellationTokenSource _tokenSource;
    private static Task _task;

    [InitializeOnLoadMethod]
    private static void OnInitialize()
    {
        CancelTask();

        _tokenSource = new CancellationTokenSource();
        _task = SaveInterval(_tokenSource.Token);
    }

    private static void CancelTask()
    {
        if (_task == null) return;
        _tokenSource.Cancel();
        _task.Wait();
    }

    private static async Task SaveInterval(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(SceneAutoSaveSettings.instance.Timeout * 1000 * 60, token);

            if (!SceneAutoSaveSettings.instance.Enabled) continue;
            if (Application.isPlaying || BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling) continue;
            if (!UnityEditorInternal.InternalEditorUtility.isApplicationActive) continue;

            EditorSceneManager.SaveOpenScenes();
        }
    }
}

public class SceneAutoSaveSettingsWindow : EditorWindow
{
    [MenuItem("Tools/Settings/Auto Save Settings", false, 10000)]
    public static void ShowWindow()
    {
        GetWindow<SceneAutoSaveSettingsWindow>("Auto Save Settings");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.BeginChangeCheck();
        SceneAutoSaveSettings.instance.Enabled = EditorGUILayout.Toggle("Enabled", SceneAutoSaveSettings.instance.Enabled);
        SceneAutoSaveSettings.instance.Timeout = EditorGUILayout.IntField("Timeout", SceneAutoSaveSettings.instance.Timeout);
        if (EditorGUI.EndChangeCheck()) SceneAutoSaveSettings.instance.SaveChanges();
        EditorGUILayout.EndVertical();
    }
}

[FilePath("/ProjectSettings/AutoSaveSettings.asset", FilePathAttribute.Location.ProjectFolder)]
public class SceneAutoSaveSettings : ScriptableSingleton<SceneAutoSaveSettings>
{
    public bool Enabled = true;
    public int Timeout = 1;

    public void SaveChanges() => Save(true);
}

