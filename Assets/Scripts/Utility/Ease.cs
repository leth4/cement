using UnityEngine;

public static class Ease
{
    public static float Linear(float t) => t;

    public static float QuadIn(float t) => t * t;
    public static float QuadOut(float t) => 1 - QuadIn(1 - t);
    public static float QuadInOut(float t)
    {
        if (t < 0.5) return QuadIn(t * 2) / 2;
        return 1 - QuadIn((1 - t) * 2) / 2;
    }

    public static float CubicIn(float t) => t * t * t;
    public static float CubicOut(float t) => 1 - CubicIn(1 - t);
    public static float CubicInOut(float t)
    {
        if (t < 0.5) return CubicIn(t * 2) / 2;
        return 1 - CubicIn((1 - t) * 2) / 2;
    }

    public static float QuartIn(float t) => t * t * t * t;
    public static float QuartOut(float t) => 1 - QuartIn(1 - t);
    public static float QuartInOut(float t)
    {
        if (t < 0.5) return QuartIn(t * 2) / 2;
        return 1 - QuartIn((1 - t) * 2) / 2;
    }

    public static float QuintIn(float t) => t * t * t * t * t;
    public static float QuintOut(float t) => 1 - QuintIn(1 - t);
    public static float QuintInOut(float t)
    {
        if (t < 0.5) return QuintIn(t * 2) / 2;
        return 1 - QuintIn((1 - t) * 2) / 2;
    }

    public static float SineIn(float t) => (float)-Mathf.Cos(t * Mathf.PI / 2);
    public static float SineOut(float t) => (float)Mathf.Sin(t * Mathf.PI / 2);
    public static float SineInOut(float t) => (float)(Mathf.Cos(t * Mathf.PI) - 1) / -2;

    public static float ExpoIn(float t) => (float)Mathf.Pow(2, 10 * (t - 1));
    public static float ExpoOut(float t) => 1 - ExpoIn(1 - t);
    public static float ExpoInOut(float t)
    {
        if (t < 0.5) return ExpoIn(t * 2) / 2;
        return 1 - ExpoIn((1 - t) * 2) / 2;
    }

    public static float CircIn(float t) => -((float)Mathf.Sqrt(1 - t * t) - 1);
    public static float CircOut(float t) => 1 - CircIn(1 - t);
    public static float CircInOut(float t)
    {
        if (t < 0.5) return CircIn(t * 2) / 2;
        return 1 - CircIn((1 - t) * 2) / 2;
    }

    public static float ElasticIn(float t) => 1 - ElasticOut(1 - t);
    public static float ElasticOut(float t)
    {
        float p = 0.3f;
        return (float)Mathf.Pow(2, -10 * t) * (float)Mathf.Sin((t - p / 4) * (2 * Mathf.PI) / p) + 1;
    }
    public static float ElasticInOut(float t)
    {
        if (t < 0.5) return ElasticIn(t * 2) / 2;
        return 1 - ElasticIn((1 - t) * 2) / 2;
    }

    public static float BackIn(float t)
    {
        float s = 1.70158f;
        return t * t * ((s + 1) * t - s);
    }
    public static float BackOut(float t) => 1 - BackIn(1 - t);
    public static float BackInOut(float t)
    {
        if (t < 0.5) return BackIn(t * 2) / 2;
        return 1 - BackIn((1 - t) * 2) / 2;
    }

    public static float BounceIn(float t) => 1 - BounceOut(1 - t);
    public static float BounceOut(float t)
    {
        float div = 2.75f;
        float mult = 7.5625f;

        if (t < 1 / div)
        {
            return mult * t * t;
        }
        else if (t < 2 / div)
        {
            t -= 1.5f / div;
            return mult * t * t + 0.75f;
        }
        else if (t < 2.5 / div)
        {
            t -= 2.25f / div;
            return mult * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / div;
            return mult * t * t + 0.984375f;
        }
    }
    public static float BounceInOut(float t)
    {
        if (t < 0.5) return BounceIn(t * 2) / 2;
        return 1 - BounceIn((1 - t) * 2) / 2;
    }

    public static float Sample(float t, EaseType type)
    {
        return type switch
        {
            EaseType.Linear => Linear(t),
            EaseType.SineIn => SineIn(t),
            EaseType.SineOut => SineOut(t),
            EaseType.SineInOut => SineInOut(t),
            EaseType.QuadIn => QuadIn(t),
            EaseType.QuadOut => QuadOut(t),
            EaseType.QuadInOut => QuadInOut(t),
            EaseType.CubicIn => CubicIn(t),
            EaseType.CubicOut => CubicOut(t),
            EaseType.CubicInOut => CubicInOut(t),
            EaseType.QuartIn => QuartIn(t),
            EaseType.QuartOut => QuartOut(t),
            EaseType.QuartInOut => QuartInOut(t),
            EaseType.QuintIn => QuintIn(t),
            EaseType.QuintOut => QuintOut(t),
            EaseType.QuintInOut => QuintInOut(t),
            EaseType.ExpoIn => ExpoIn(t),
            EaseType.ExpoOut => ExpoOut(t),
            EaseType.ExpoInOut => ExpoInOut(t),
            EaseType.CircIn => CircIn(t),
            EaseType.CircOut => CircOut(t),
            EaseType.CircInOut => CircInOut(t),
            EaseType.BackIn => BackIn(t),
            EaseType.BackOut => BackOut(t),
            EaseType.BackInOut => BackInOut(t),
            EaseType.ElasticIn => ElasticIn(t),
            EaseType.ElasticOut => ElasticOut(t),
            EaseType.ElasticInOut => ElasticInOut(t),
            EaseType.BounceIn => BounceIn(t),
            EaseType.BounceOut => BounceOut(t),
            EaseType.BounceInOut => BounceInOut(t),
            _ => 0
        };
    }
}

public enum EaseType
{
    Linear,
    SineIn, SineOut, SineInOut,
    QuadIn, QuadOut, QuadInOut,
    CubicIn, CubicOut, CubicInOut,
    QuartIn, QuartOut, QuartInOut,
    QuintIn, QuintOut, QuintInOut,
    ExpoIn, ExpoOut, ExpoInOut,
    CircIn, CircOut, CircInOut,
    BackIn, BackOut, BackInOut,
    ElasticIn, ElasticOut, ElasticInOut,
    BounceIn, BounceOut, BounceInOut,
}