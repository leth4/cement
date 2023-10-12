
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Shape))]
public class ShapeEditor : Editor
{
    private Shape _shape;

    private int _currentBrush;

    private Color trueColor = new Color(.1f, .1f, .1f);
    private Color falseColor = new Color(.9f, .9f, .9f);

    private void OnEnable()
    {
        _shape = (Shape)target;

        _shape.Rows = _shape.Rows <= 0 ? 1 : _shape.Rows;
        _shape.Columns = _shape.Columns <= 0 ? 1 : _shape.Columns;
        if (_shape.GridList == null || _shape.GridList.Count == 0) _shape.GridList = new List<bool>() { false };
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        var newRows = Mathf.Max(1, EditorGUILayout.IntField("Rows", _shape.Rows));
        var newColumns = Mathf.Max(1, EditorGUILayout.IntField("Columns", _shape.Columns));
        var offset = EditorGUILayout.Vector2IntField("Offset", _shape.Offset);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_shape, "Changed the size of the level");

            var newGrid = new List<bool>();

            for (int i = 0; i < newRows; i++)
            {
                for (int j = 0; j < newColumns; j++)
                {
                    var index = i * _shape.Columns + j;
                    if (index < _shape.GridList.Count && j < _shape.Columns)
                    {
                        newGrid.Add(_shape.GridList[index]);
                    }
                    else
                    {
                        newGrid.Add(false);
                    }
                }
            }

            _shape.GridList = newGrid;
            _shape.Offset = offset;
            _shape.Rows = newRows;
            _shape.Columns = newColumns;

            EditorUtility.SetDirty(_shape);
        }

        using (var verticalScope = new EditorGUILayout.VerticalScope("HelpBox"))
        {
            var size = 30;

            for (int i = 0; i < _shape.Rows; i++)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(size * _shape.Columns));
                for (int j = 0; j < _shape.Columns; j++)
                {
                    GUILayout.BeginVertical();
                    DrawBox(i * _shape.Columns + j, size);
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
        }

        if (GUILayout.Button("Erase"))
        {
            Undo.RecordObject(_shape, "Erased the level");
            for (int i = 0; i < _shape.Rows * _shape.Columns; i++)
            {
                _shape.GridList[i] = false;
            }
            EditorUtility.SetDirty(_shape);
        }
    }

    private void DrawBox(int index, int size)
    {
        var evt = Event.current;

        var colorBoxStyle = new GUIStyle("box")
        {
            padding = new RectOffset(2, 2, 2, 2),
            margin = new RectOffset(0, 0, 0, 0),
            fixedHeight = size,
            fixedWidth = size
        };

        var color = _shape.GridList[index] ? trueColor : falseColor;

        GUILayout.Box(TextureFromColor(color, size), colorBoxStyle);
        if (evt.isMouse && GUILayoutUtility.GetLastRect().Contains(evt.mousePosition))
        {
            Undo.RecordObject(_shape, "Changed the level");
            _shape.GridList[index] = evt.button == 0 ? true : false;
            EditorUtility.SetDirty(_shape);
        }
    }

    private static Texture2D TextureFromColor(Color color, int size)
    {
        Color[] pix = new Color[size * size];
        for (int i = 0; i < pix.Length; i++) pix[i] = color;
        Texture2D texture = new Texture2D(size, size);
        texture.SetPixels(pix);
        texture.Apply();
        return texture;
    }
}