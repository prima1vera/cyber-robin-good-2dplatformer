using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Refs")]
    public Health targetHealth;      // если оставить пустым, найдёт в родителе
    public Image fillImage;          // зелёная часть (Image.Type = Filled)
    public Image backgroundImage;    // фон (красный), необязательно
    Canvas canvas;
    CanvasGroup canvasGroup;
    Coroutine animateCoroutine;

    [Header("Behaviour")]
    //public Vector3 worldOffset = new Vector3(0f, 1.5f, 0f);
    public float showSecondsAfterHit = 2f;
    public float smoothTime = 0.12f;

    Camera mainCam;
    Coroutine hideCoroutine;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        mainCam = Camera.main;
    }

    void Start()
    {
        if (targetHealth == null)
            targetHealth = GetComponentInParent<Health>();

        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += OnHealthChanged;
            targetHealth.OnDamaged += OnDamaged;
            targetHealth.OnDied += OnDied;
            // init UI
            OnHealthChanged(targetHealth.currentHealth, targetHealth.maximumHealth);
            // hidden by default until damaged
            canvasGroup.alpha = 0f;
        }
    }

    void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= OnHealthChanged;
            targetHealth.OnDamaged -= OnDamaged;
            targetHealth.OnDied -= OnDied;
        }
    }

    void LateUpdate()
    {
        if (targetHealth == null) return;

        // позиционирование над головой
        //transform.position = targetHealth.transform.position + worldOffset;

        // всегда смотрим к камере (billboard)
        if (mainCam != null)
            transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
    }

    void OnHealthChanged(int current, int max)
    {
        float target = (max > 0) ? (float)current / max : 0f;
        // плавно меняем fillAmount (корутины не накладываются один на другой — запускаем одну)
        if (animateCoroutine != null) StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(AnimateFill(fillImage != null ? fillImage.fillAmount : 0f, target));

    }

    void OnDamaged()
    {
        // показать бар и запустить таймер скрытия
        canvasGroup.alpha = 1f;
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay(showSecondsAfterHit));
    }

    void OnDied()
    {
        // уничтожаем сам бар, либо запустить анимацию
        Destroy(gameObject);
    }

    IEnumerator AnimateFill(float from, float to)
    {
        float t = 0f;
        float dur = Mathf.Max(smoothTime, 0.01f);
        while (t < dur)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(from, to, t / dur);
            if (fillImage != null) fillImage.fillAmount = v;
            yield return null;
        }
        if (fillImage != null) fillImage.fillAmount = to;
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // можно добавить плавное скрытие
        float fadeDur = 0.15f;
        float start = Time.time;
        float startAlpha = canvasGroup.alpha;
        while (Time.time - start < fadeDur)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, (Time.time - start) / fadeDur);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
