using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ComponentMethodCaller : EditorWindow
{
    private List<MethodContext> _methods;
    private Component _component;
    private string _searchText;
    private Vector2 _scrollPosition;

    [MenuItem("CONTEXT/Component/Call Method")]
    public static void CreateWindow(MenuCommand command)
    {
        var component = (Component)command.context;
        var window = GetWindow<ComponentMethodCaller>();
        window.Initialize(component);
    }

    public void Initialize(Component component)
    {
        wantsMouseMove = true;
        titleContent = new(component.GetType().Name);

        _component = component;
        _methods = new();
        _searchText = "";

        var type = _component.GetType();
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(x => x.DeclaringType == type).OrderBy(x => x.Name).ToArray();

        foreach (var method in methods)
        {
            if (method.Name.EndsWith("_Injected")) continue;
            _methods.Add(new()
            {
                Method = method,
                ParameterInfos = method.GetParameters(),
                Parameters = new object[method.GetParameters().Length]
            });
        }

        foreach (var method in _methods)
        {
            for (int i = 0; i < method.Parameters.Length; i++)
            {
                var parameterType = method.ParameterInfos[i].ParameterType;
                method.Parameters[i] = parameterType.IsValueType ? Activator.CreateInstance(parameterType) : null;
            }
        }
    }

    private void OnGUI()
    {
        if (_component == null) Close();
        if (_methods == null) Initialize(_component);

        var evt = Event.current;

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 20;
        _searchText = EditorGUILayout.TextField(EditorGUIUtility.IconContent("Search On Icon"), _searchText);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_scenevis_visible_hover"), GUILayout.Width(30)))
        {
            for (int i = 0; i < _methods.Count; i++) _methods[i].Unfolded = true;
        }
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_SceneViewVisibility"), GUILayout.Width(30)))
        {
            for (int i = 0; i < _methods.Count; i++) _methods[i].Unfolded = false;
        }
        GUILayout.EndHorizontal();

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        EditorGUIUtility.labelWidth = 100;
        for (int i = 0; i < _methods.Count; i++)
        {
            var method = _methods[i];

            if (method.Method.Name.IndexOf(_searchText, 0, StringComparison.OrdinalIgnoreCase) == -1) continue;

            using (var vertical = new EditorGUILayout.VerticalScope("HelpBox"))
            {
                if (vertical.rect.Contains(evt.mousePosition))
                {
                    var labelRect = new Rect(vertical.rect) { height = 24, width = vertical.rect.width - 35 };
                    if (labelRect.Contains(evt.mousePosition) && method.HasParameters && evt.type == EventType.MouseUp && evt.button == 0)
                    {
                        method.Unfolded = !method.Unfolded;
                    }
                    EditorGUI.DrawRect(vertical.rect, new Color(1, 1, 1, 0.08f));
                    Repaint();
                }

                EditorGUILayout.BeginHorizontal();

                if (method.ParameterInfos.Length != 0) EditorGUILayout.Toggle(method.Unfolded, "foldout", GUILayout.Width(15));

                GUILayout.Label($"{method.Method.Name}");

                if (GUILayout.Button(EditorGUIUtility.IconContent("d_PlayButton On@2x"), GUILayout.Width(30), GUILayout.Height(20)))
                {
                    object result = method.Method.Invoke(_component, method.Parameters);
                    LogResult(result);
                }

                EditorGUILayout.EndHorizontal();

                if (method.Unfolded)
                {
                    EditorGUI.indentLevel++;
                    for (int j = 0; j < method.Parameters.Length; j++)
                    {
                        if (method.ParameterInfos[j].IsOut) continue;
                        DrawParameterField(method.ParameterInfos[j].ParameterType, method.ParameterInfos[j].Name, ref method.Parameters[j]);
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawParameterField(Type type, string name, ref object parameter)
    {
        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            EditorGUILayout.LabelField($"Can't display an array", new GUIStyle("label") { normal = new() { textColor = new(1f, .4f, .4f) } });
            return;
        }

        if (type.IsEnum) parameter = EditorGUILayout.EnumPopup(name, (Enum)parameter);
        else if (type == typeof(Int32)) parameter = EditorGUILayout.IntField(name, (Int32)parameter);
        else if (type == typeof(Int64)) parameter = EditorGUILayout.LongField(name, (Int64)parameter);
        else if (type == typeof(Single)) parameter = EditorGUILayout.FloatField(name, (Single)parameter);
        else if (type == typeof(Double)) parameter = EditorGUILayout.DoubleField(name, (Double)parameter);
        else if (type == typeof(String)) parameter = EditorGUILayout.TextField(name, (String)parameter);
        else if (type == typeof(Boolean)) parameter = EditorGUILayout.Toggle(name, (Boolean)parameter);
        else if (type == typeof(Char)) parameter = EditorGUILayout.TextField(name, ((Char)parameter).ToString());
        else if (type == typeof(Byte)) parameter = EditorGUILayout.IntField(name, (Byte)parameter);
        else if (type == typeof(SByte)) parameter = EditorGUILayout.IntField(name, (SByte)parameter);
        else if (type == typeof(UInt16)) parameter = EditorGUILayout.IntField(name, (UInt16)parameter);
        else if (type == typeof(UInt32)) parameter = EditorGUILayout.LongField(name, (UInt32)parameter);
        else if (type == typeof(Decimal)) parameter = EditorGUILayout.FloatField(name, Decimal.ToSingle((Decimal)parameter));
        else if (type == typeof(Vector2)) parameter = EditorGUILayout.Vector2Field(name, (Vector2)parameter);
        else if (type == typeof(Vector3)) parameter = EditorGUILayout.Vector3Field(name, (Vector3)parameter);
        else if (type == typeof(Vector4)) parameter = EditorGUILayout.Vector4Field(name, (Vector4)parameter);
        else if (type == typeof(Color)) parameter = EditorGUILayout.ColorField(name, (Color)parameter);
        else if (type == typeof(Rect)) parameter = EditorGUILayout.RectField(name, (Rect)parameter);
        else if (type == typeof(AnimationCurve)) parameter = EditorGUILayout.CurveField(name, (AnimationCurve)parameter);
        else if (type == typeof(Bounds)) parameter = EditorGUILayout.BoundsField(name, (Bounds)parameter);
        else if (type == typeof(Quaternion)) parameter = Quaternion.Euler(EditorGUILayout.Vector3Field(name, ((Quaternion)parameter).eulerAngles));
        else
            try
            {
                parameter = Convert.ChangeType(EditorGUILayout.ObjectField(name, (UnityEngine.Object)parameter, type, true), type);
            }
            catch
            {
                EditorGUILayout.LabelField($"Can't display the type '{type}'", new GUIStyle("label") { normal = new() { textColor = new(1f, .4f, .4f) } });
            }
    }

    private void LogResult(object result)
    {
        if (result == null) return;

        if (result is not IEnumerable)
        {
            Debug.Log(result.ToString());
            return;
        }

        string resultString = "";
        foreach (var item in result as IEnumerable)
        {
            if (resultString != "") resultString += ", ";
            resultString += item.ToString();
        }
    }

    private class MethodContext
    {
        public MethodInfo Method;
        public ParameterInfo[] ParameterInfos;
        public object[] Parameters;
        public bool Unfolded;
        public bool HasParameters => ParameterInfos.Length != 0;
    }
}