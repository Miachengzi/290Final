using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFader : MonoBehaviour
{
    public Image element;
    public float delayBeforeShowing;
    public float fadeDuration;
    public float displayDuration;

    void Start()
    {
        Color color = element.color;
        color.a = 0f;
        element.color = color;
        DontDestroyOnLoad(this);
        //StartCoroutine(FadeInAndOut());
    }

    public IEnumerator FadeInAndOut()
    {
        yield return new WaitForSeconds(delayBeforeShowing);
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = element.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            element.color = color;
            yield return null;
        }
        color.a = 1f;
        element.color = color;
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = element.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            element.color = color;
            yield return null;
        }
        color.a = 0f;
        element.color = color;
    }
}
