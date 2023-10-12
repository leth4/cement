using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Tween
{
    public static void Delay(this MonoBehaviour behaviour, float time, Action ended = null)
    {
        behaviour.StartCoroutine(DelayRoutine(time, ended));
    }

    public static void Translate(this MonoBehaviour behaviour, Transform transform, Vector3 from, Vector3 to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        behaviour.StartCoroutine(TranslateRoutine(transform, from, to, time, easeType, loopType, ended));
    }

    public static void Translate(this MonoBehaviour behaviour, out Coroutine coroutine, Transform transform, Vector3 from, Vector3 to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        coroutine = behaviour.StartCoroutine(TranslateRoutine(transform, from, to, time, easeType, loopType, ended));
    }

    public static void Scale(this MonoBehaviour behaviour, Transform transform, Vector3 from, Vector3 to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        behaviour.StartCoroutine(ScaleRoutine(transform, from, to, time, easeType, loopType, ended));
    }

    public static void Scale(this MonoBehaviour behaviour, out Coroutine coroutine, Transform transform, Vector3 from, Vector3 to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        coroutine = behaviour.StartCoroutine(ScaleRoutine(transform, from, to, time, easeType, loopType, ended));
    }

    public static void Rotate(this MonoBehaviour behaviour, Transform transform, Quaternion from, Quaternion to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        behaviour.StartCoroutine(RotateRoutine(transform, from, to, time, easeType, loopType, ended));
    }

    public static void Rotate(this MonoBehaviour behaviour, out Coroutine coroutine, Transform transform, Quaternion from, Quaternion to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        coroutine = behaviour.StartCoroutine(RotateRoutine(transform, from, to, time, easeType, loopType, ended));
    }

    public static void RotateEuler(this MonoBehaviour behaviour, Transform transform, Vector3 from, Vector3 to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        behaviour.StartCoroutine(RotateEulerRoutine(transform, from, to, time, easeType, loopType, ended));
    }

    public static void RotateEuler(this MonoBehaviour behaviour, out Coroutine coroutine, Transform transform, Vector3 from, Vector3 to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        coroutine = behaviour.StartCoroutine(RotateEulerRoutine(transform, from, to, time, easeType, loopType, ended));
    }

    public static void Color(this MonoBehaviour behaviour, SpriteRenderer spriteRenderer, Color from, Color to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        behaviour.StartCoroutine(ColorRoutine(spriteRenderer, from, to, time, easeType, loopType, ended));
    }

    public static void Color(this MonoBehaviour behaviour, out Coroutine coroutine, SpriteRenderer spriteRenderer, Color from, Color to, float time, EaseType easeType = EaseType.Linear, LoopType loopType = LoopType.Once, Action ended = null)
    {
        coroutine = behaviour.StartCoroutine(ColorRoutine(spriteRenderer, from, to, time, easeType, loopType, ended));
    }

    public static void Shake(this MonoBehaviour behaviour, Transform transform, float magnitude, float time, LoopType loopType = LoopType.Once, Action ended = null)
    {
        behaviour.StartCoroutine(ShakeRoutine(transform, magnitude, time, loopType, ended));
    }

    public static void Shake(this MonoBehaviour behaviour, out Coroutine coroutine, Transform transform, float magnitude, float time, LoopType loopType = LoopType.Once, Action ended = null)
    {
        coroutine = behaviour.StartCoroutine(ShakeRoutine(transform, magnitude, time, loopType, ended));
    }

    private static IEnumerator DelayRoutine(float time, Action ended)
    {
        yield return new WaitForSeconds(time);
        ended?.Invoke();
    }

    private static IEnumerator TranslateRoutine(Transform transform, Vector3 from, Vector3 to, float time, EaseType easeType, LoopType loopType, Action ended)
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(from, to, Ease.Sample(t / time, easeType));
            yield return null;
        }

        if (loopType == LoopType.MirrorCycle)
        {
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(to, from, Ease.Sample(t / time, easeType));
                yield return null;
            }
        }

        if (loopType != LoopType.Once) yield return TranslateRoutine(transform, from, to, time, easeType, loopType, ended);

        transform.position = to;
        ended?.Invoke();
    }

    private static IEnumerator ScaleRoutine(Transform transform, Vector3 from, Vector3 to, float time, EaseType easeType, LoopType loopType, Action ended)
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(from, to, Ease.Sample(t / time, easeType));
            yield return null;
        }

        if (loopType == LoopType.MirrorCycle)
        {
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                transform.localScale = Vector3.Lerp(to, from, Ease.Sample(t / time, easeType));
                yield return null;
            }
        }

        if (loopType != LoopType.Once) yield return ScaleRoutine(transform, from, to, time, easeType, loopType, ended);

        transform.localScale = to;
        ended?.Invoke();
    }

    private static IEnumerator RotateRoutine(Transform transform, Quaternion from, Quaternion to, float time, EaseType easeType, LoopType loopType, Action ended)
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(from, to, Ease.Sample(t / time, easeType));
            yield return null;
        }

        if (loopType == LoopType.MirrorCycle)
        {
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                transform.rotation = Quaternion.Lerp(to, from, Ease.Sample(t / time, easeType));
                yield return null;
            }
        }

        if (loopType != LoopType.Once) yield return RotateRoutine(transform, from, to, time, easeType, loopType, ended);

        transform.rotation = to;
        ended?.Invoke();
    }

    private static IEnumerator RotateEulerRoutine(Transform transform, Vector3 from, Vector3 to, float time, EaseType easeType, LoopType loopType, Action ended)
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            transform.eulerAngles = Vector3.Lerp(from, to, Ease.Sample(t / time, easeType));
            yield return null;
        }

        if (loopType == LoopType.MirrorCycle)
        {
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                transform.eulerAngles = Vector3.Lerp(to, from, Ease.Sample(t / time, easeType));
                yield return null;
            }
        }

        if (loopType != LoopType.Once) yield return RotateEulerRoutine(transform, from, to, time, easeType, loopType, ended);

        transform.eulerAngles = to;
        ended?.Invoke();
    }

    private static IEnumerator ColorRoutine(SpriteRenderer spriteRenderer, Color from, Color to, float time, EaseType easeType, LoopType loopType, Action ended)
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            spriteRenderer.color = UnityEngine.Color.Lerp(from, to, Ease.Sample(t / time, easeType));
            yield return null;
        }

        if (loopType == LoopType.MirrorCycle)
        {
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                spriteRenderer.color = UnityEngine.Color.Lerp(to, from, Ease.Sample(t / time, easeType));
                yield return null;
            }
        }

        if (loopType != LoopType.Once) yield return ColorRoutine(spriteRenderer, from, to, time, easeType, loopType, ended);

        spriteRenderer.color = to;
        ended?.Invoke();
    }

    private static IEnumerator ShakeRoutine(Transform transform, float magnitude, float time, LoopType loopType, Action ended)
    {
        var startPosition = transform.position;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = startPosition + new Vector3(Random.Range(-1f, 1f) * magnitude, Random.Range(-1f, 1f) * magnitude, 0);
            yield return null;
        }

        if (loopType != LoopType.Once) yield return ShakeRoutine(transform, magnitude, time, loopType, ended);

        transform.position = startPosition;
        ended?.Invoke();
    }
}

public enum LoopType
{
    Once,
    Cycle,
    MirrorCycle
}