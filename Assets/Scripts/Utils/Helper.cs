using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    private static Camera _camera;
    public static Camera Camera
    {
        get
        {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }

    public static bool RandomBool()
    {
        return Random.Range(0, 2) == 0;
    }

    public static Ray CameraRay => Camera.ScreenPointToRay(Input.mousePosition);
}
