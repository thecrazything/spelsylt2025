using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutBehaviour : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 1.0f;
    private bool _isFading = false;
    private float _fadeTimer = 0.0f;

    private Image _blackoutImage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _blackoutImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FadeIn()
    {
        if (_isFading) return;
        _isFading = true;
        _fadeTimer = 0.0f;
        StartCoroutine(Fade(0.0f, 1.0f));
    }

    public void FadeOut()
    {
        if (_isFading) return;
        _isFading = true;
        _fadeTimer = 0.0f;
        StartCoroutine(Fade(1.0f, 0.0f));
    }
    
    public IEnumerator Fade(float startAlpha, float endAlpha)
    {
        while (_fadeTimer < _fadeDuration)
        {
            _fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_fadeTimer / _fadeDuration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            Color color = _blackoutImage.color;
            color.a = alpha;
            _blackoutImage.color = color;
            yield return null;
        }
        Color finalColor = _blackoutImage.color;
        finalColor.a = endAlpha;
        _blackoutImage.color = finalColor;
        _isFading = false;
    }
}
