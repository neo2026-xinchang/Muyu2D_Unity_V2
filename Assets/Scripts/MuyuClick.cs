using System.Collections;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class MuyuClick : MonoBehaviour
{
    [Header("UI")]
    public UnityEngine.UI.Text countText;
    public UnityEngine.UI.Text totalText;
    public UnityEngine.UI.Text completeText;
    public UnityEngine.UI.Slider progressSlider;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Settings")]
    public int targetCount = 108;
    public float pressedScale = 0.9f;
    public float animationDuration = 0.12f;

    private const string TotalKey = "MUYU_TOTAL_COUNT";

    private int roundCount = 0;
    private int totalCount = 0;

    private Vector3 originalScale;
    private RectTransform rectTransform;

    private Coroutine animationCoroutine;
    private Coroutine completeCoroutine;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        originalScale = transform.localScale;

        totalCount = PlayerPrefs.GetInt(
            TotalKey,
            0
        );

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = targetCount;
            progressSlider.value = roundCount;
        }

        if (completeText != null)
        {
            completeText.gameObject.SetActive(false);
        }

        RefreshUI();
    }


    private void Update()
    {
        Vector2 clickPosition;

        if (TryGetPointerDown(out clickPosition))
        {
            // 判断鼠标 / 手指是不是点在木鱼区域
            bool inside =
                RectTransformUtility.RectangleContainsScreenPoint(
                    rectTransform,
                    clickPosition,
                    null
                );

            if (inside)
            {
                ClickMuyu();
            }
        }
    }


    /// <summary>
    /// 同时兼容旧 Input 和新 Input System
    /// </summary>
    private bool TryGetPointerDown(out Vector2 position)
    {
        position = Vector2.zero;

#if ENABLE_INPUT_SYSTEM

        // 鼠标
        if (
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame
        )
        {
            position = Mouse.current.position.ReadValue();

            return true;
        }


        // 手机触摸
        if (Touchscreen.current != null)
        {
            var touch =
                Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                position =
                    touch.position.ReadValue();

                return true;
            }
        }

#endif


#if ENABLE_LEGACY_INPUT_MANAGER

        // 老 Input Manager 鼠标
        if (Input.GetMouseButtonDown(0))
        {
            position =
                Input.mousePosition;

            return true;
        }


        // 老 Input Manager 触摸
        if (
            Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began
        )
        {
            position =
                Input.GetTouch(0).position;

            return true;
        }

#endif

        return false;
    }


    public void ClickMuyu()
    {
        UnityEngine.Debug.Log("====== 木鱼被敲了 ======");

        roundCount++;

        totalCount++;


        // 保存累计功德
        PlayerPrefs.SetInt(
            TotalKey,
            totalCount
        );

        PlayerPrefs.Save();


        // 音效
        if (
            audioSource != null &&
            audioSource.clip != null
        )
        {
            audioSource.PlayOneShot(
                audioSource.clip
            );
        }


        // 缩放动画
        if (animationCoroutine != null)
        {
            StopCoroutine(
                animationCoroutine
            );
        }

        animationCoroutine =
            StartCoroutine(
                TapAnimation()
            );


        // 满 108 次
        if (roundCount >= targetCount)
        {
            roundCount = 0;

            if (completeCoroutine != null)
            {
                StopCoroutine(
                    completeCoroutine
                );
            }

            completeCoroutine =
                StartCoroutine(
                    ShowComplete()
                );
        }


        RefreshUI();
    }


    private void RefreshUI()
    {
        if (countText != null)
        {
            countText.text =
                "本轮功德：" +
                roundCount +
                " / " +
                targetCount;
        }


        if (totalText != null)
        {
            totalText.text =
                "累计功德：" +
                totalCount;
        }


        if (progressSlider != null)
        {
            progressSlider.value =
                roundCount;
        }
    }


    private IEnumerator TapAnimation()
    {
        Vector3 smallScale =
            originalScale *
            pressedScale;


        float halfTime =
            animationDuration *
            0.5f;


        float time = 0f;


        // 缩小
        while (time < halfTime)
        {
            time +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    time / halfTime
                );


            transform.localScale =
                Vector3.Lerp(
                    originalScale,
                    smallScale,
                    progress
                );


            yield return null;
        }


        time = 0f;


        // 恢复
        while (time < halfTime)
        {
            time +=
                Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    time / halfTime
                );


            transform.localScale =
                Vector3.Lerp(
                    smallScale,
                    originalScale,
                    progress
                );


            yield return null;
        }


        transform.localScale =
            originalScale;
    }


    private IEnumerator ShowComplete()
    {
        if (completeText == null)
        {
            yield break;
        }


        completeText.gameObject.SetActive(
            true
        );


        completeText.text =
            "功德圆满";


        yield return
            new WaitForSecondsRealtime(
                1.5f
            );


        completeText.gameObject.SetActive(
            false
        );
    }


    public void ResetTotalCount()
    {
        roundCount = 0;

        totalCount = 0;


        PlayerPrefs.DeleteKey(
            TotalKey
        );

        PlayerPrefs.Save();


        RefreshUI();


        UnityEngine.Debug.Log(
            "功德记录已清空"
        );
    }
}