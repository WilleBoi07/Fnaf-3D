using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    private Light m_Light;
    void Start()
    {
        m_Light = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            m_Light.enabled = !m_Light.enabled;
            yield return new WaitForSeconds(RandomExtensions.LogNormalRandom(0.3f, 1f));

            m_Light.enabled = !m_Light.enabled;
            yield return new WaitForSeconds(RandomExtensions.LogNormalRandom(3.4f, 2));
        }
    }
}
