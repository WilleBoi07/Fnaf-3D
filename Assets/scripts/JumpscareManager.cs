using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class JumpscareManager : MonoBehaviour
{
    [Header("Jumpscare Images")]
    public Image backgroundImage;   // Optional: static, dark screen, etc.
    public Image jumpscareImage;    // Main scare image

    [Header("Audio")]
    public AudioSource screamSound;

    [Header("Timing Settings")]
    public float backgroundFadeInTime = 0.3f;
    public float jumpscareFadeInTime = 0.15f;
    public float holdTime = 0.8f;
    public float fadeOutTime = 0.5f;

    public void TriggerJumpscare()
    {
        StartCoroutine(JumpscareRoutine());
    }

    private IEnumerator JumpscareRoutine()
    {
        if (backgroundImage != null)
        {
            Color bg = backgroundImage.color;
            float t = 0f;
            while (t < backgroundFadeInTime)
            {
                bg.a = Mathf.Lerp(0, 1, t / backgroundFadeInTime);
                backgroundImage.color = bg;
                t += Time.deltaTime;
                yield return null;
            }
            bg.a = 1;
            backgroundImage.color = bg;
        }

        yield return new WaitForSeconds(0.1f);

        if (screamSound) screamSound.Play();

        if (jumpscareImage != null)
        {
            Color fg = jumpscareImage.color;
            float t = 0f;
            while (t < jumpscareFadeInTime)
            {
                fg.a = Mathf.Lerp(0, 1, t / jumpscareFadeInTime);
                jumpscareImage.color = fg;
                t += Time.deltaTime;
                yield return null;
            }
            fg.a = 1;
            jumpscareImage.color = fg;
        }

        yield return new WaitForSeconds(holdTime);

        float fade = 0f;
        while (fade < fadeOutTime)
        {
            if (backgroundImage != null)
            {
                Color bg = backgroundImage.color;
                bg.a = Mathf.Lerp(1, 0, fade / fadeOutTime);
                backgroundImage.color = bg;
            }

            if (jumpscareImage != null)
            {
                Color fg = jumpscareImage.color;
                fg.a = Mathf.Lerp(1, 0, fade / fadeOutTime);
                jumpscareImage.color = fg;
            }

            fade += Time.deltaTime;
            yield return null;
        }

        if (backgroundImage) backgroundImage.color = new Color(0, 0, 0, 0);
        if (jumpscareImage) jumpscareImage.color = new Color(0, 0, 0, 0);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#endif
    }
}
