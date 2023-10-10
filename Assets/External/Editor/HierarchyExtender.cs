using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class HierarchyExtender
{
    private static GameObject[] _selectedPreviously;
    private static Texture2D _boxStyleBackgroundTexture = new(1, 1);

    static HierarchyExtender()
    {
        EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUI;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

        _boxStyleBackgroundTexture.SetPixels(new Color[] { new Color(0.15f, 0.15f, 0.15f) });
        _boxStyleBackgroundTexture.Apply();
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect rect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject == null) return;

        if (Selection.activeGameObject != gameObject && gameObject.name.StartsWith("-"))
        {
            var style = new GUIStyle("box");
            style.normal.background = _boxStyleBackgroundTexture;
            style.padding = new RectOffset(0, 0, 0, 0);
            style.fontStyle = FontStyle.Bold;

            GUI.Box(rect, gameObject.name.ToUpper().Replace("-", ""), style);
            return;
        }

        rect.x += rect.width - 15;

        if (HierarchySettings.instance.ShowToggles)
        {
            rect.width = 14;
            gameObject.SetActive(EditorGUI.Toggle(rect, gameObject.activeInHierarchy));
            rect.width = 20;
        }

        if (HierarchySettings.instance.ShowComponents)
        {
            var components = gameObject.GetComponents<Component>();
            rect.x -= components.Length * 15 - 10;
            foreach (var component in components)
            {
                if (component is Transform) continue;
                var icon = EditorGUIUtility.ObjectContent(component, component.GetType());
                GUI.Label(rect, icon.image);
                rect.x += 15;
            }
        }
    }
}

public class HierarchySettingsWindow : EditorWindow
{
    [MenuItem("Tools/Settings/Hierarchy Settings", false, 10000)]
    public static void ShowWindow()
    {
        GetWindow<HierarchySettingsWindow>("Hierarchy Settings");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.BeginChangeCheck();
        HierarchySettings.instance.ShowComponents = EditorGUILayout.Toggle("Show Components", HierarchySettings.instance.ShowComponents);
        HierarchySettings.instance.ShowToggles = EditorGUILayout.Toggle("Show Toggles", HierarchySettings.instance.ShowToggles);
        if (EditorGUI.EndChangeCheck()) HierarchySettings.instance.SaveChanges();
        EditorGUILayout.EndVertical();
    }
}

[FilePath("/ProjectSettings/HierarchySettings.asset", FilePathAttribute.Location.ProjectFolder)]
public class HierarchySettings : ScriptableSingleton<HierarchySettings>
{
    public bool ShowComponents = false;
    public bool ShowToggles = true;

    public void SaveChanges() => Save(true);
}
