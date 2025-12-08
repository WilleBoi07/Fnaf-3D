using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleProximityMonster : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip moveSound;

    [System.Serializable]
    public class ProximityTier
    {
        public List<Transform> waypoints = new List<Transform>();
    }

    [Header("Proximity Tiers")]
    public List<ProximityTier> tiers = new List<ProximityTier>();

    [Header("Movement Settings")]
    public float moveInterval = 8f;

    [Header("Kill Point Settings")]
    public float firstEncounterGrace = 4f;   // Door open when monster arrives
    public float secondEncounterGrace = 2f;  // Door closed on arrival, then opened
    public Vector2 retreatTierRange = new Vector2(3, 6); // Random retreat tier range

    [Header("Billboarding (optional)")]
    public Camera activeCamera;

    private int currentTier = 0;
    private float moveTimer;
    private bool atKillPoint = false;
    private bool attackStarted = false;

    private DoorToggle door; // Reference to the door

    void Start()
    {
        moveTimer = moveInterval;
        MoveToRandomPointInTier(0);

        door = FindObjectOfType<DoorToggle>();
        if (door == null)
        {
            Debug.LogWarning($"{name}: No DoorToggle found in scene!");
        }
    }

    void Update()
    {
        if (!atKillPoint)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f)
            {
                AdvanceToNextTier();
                moveTimer = moveInterval;
            }
        }

        // Billboard facing (sideways only)
        if (activeCamera != null)
        {
            Vector3 lookPos = activeCamera.transform.position;
            lookPos.y = transform.position.y; // keep upright
            transform.LookAt(lookPos);
            transform.Rotate(0, 180f, 0);
        }
    }

    void AdvanceToNextTier()
    {
        currentTier++;
        if (currentTier < tiers.Count)
        {
            MoveToRandomPointInTier(currentTier);
        }
        else
        {
            ReachKillPoint();
        }
    }

    void MoveToRandomPointInTier(int tierIndex)
    {
        if (tierIndex >= tiers.Count || tiers[tierIndex].waypoints.Count == 0)
        {
            Debug.LogWarning($"{name}: Tier {tierIndex} is empty or missing.");
            return;
        }

        var tier = tiers[tierIndex].waypoints;
        Transform chosen = tier[Random.Range(0, tier.Count)];
        transform.position = chosen.position;
        Debug.Log($"{name} moved to {chosen.name} (Tier {tierIndex})");

        // 75% chance to play movement sound
        if (audioSource != null && moveSound != null)
        {
            float chance = Random.value; // 0.0 to 1.0
            if (chance <= 0.75f)
            {
                audioSource.PlayOneShot(moveSound);
                Debug.Log("Monster movement sound played.");
            }
            else
            {
                Debug.Log("Monster moved silently.");
            }
        }
    }

    void ReachKillPoint()
    {
        atKillPoint = true;
        Debug.Log($"{name} has reached the kill point!");

        if (door != null && door.IsOpen())
        {
            // Door open on arrival -> normal 4s timer
            Debug.Log($"{name}: Door is open on arrival -> 4s grace timer started");
            StartCoroutine(ArrivalDoorOpenRoutine());
        }
        else
        {
            // Door closed on arrival -> wait until player opens, or retreat
            Debug.Log($"{name}: Door is closed on arrival -> waiting for player or retreating");
            StartCoroutine(ArrivalDoorClosedRoutine());
        }
    }

    IEnumerator ArrivalDoorOpenRoutine()
    {
        attackStarted = true;
        float timer = firstEncounterGrace;

        while (timer > 0f)
        {
            if (!door.IsOpen())
            {
                Debug.Log($"{name}: Door closed during grace period -> monster retreats");
                RetreatMonster();
                yield break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        Debug.Log($"{name}: Player failed to close door in time -> ATTACK");
        KillPlayer();
    }

    IEnumerator ArrivalDoorClosedRoutine()
    {
        // Monster waits a random amount of time before leaving
        float waitTime = Random.Range(5f, 10f);
        float timer = 0f;

        Debug.Log($"{name}: Waiting {waitTime:F1}s at the door...");

        while (timer < waitTime)
        {
            if (door.IsOpen())
            {
                Debug.Log($"{name}: Player opened the door -> short 2s grace timer");
                StartCoroutine(PeekKillRoutine());
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log($"{name}: Player never opened -> monster retreats");
        RetreatMonster();
    }

    IEnumerator PeekKillRoutine()
    {
        attackStarted = true;
        float timer = secondEncounterGrace;

        while (timer > 0f)
        {
            if (!door.IsOpen())
            {
                Debug.Log($"{name}: Door closed again -> player survived");
                RetreatMonster();
                yield break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        Debug.Log($"{name}: Player opened door too long -> ATTACK");
        KillPlayer();
    }

    void RetreatMonster()
    {
        int retreatTier = Mathf.RoundToInt(Random.Range(retreatTierRange.x, retreatTierRange.y));
        retreatTier = Mathf.Clamp(retreatTier, 0, tiers.Count - 1);

        currentTier = retreatTier;
        MoveToRandomPointInTier(currentTier);

        atKillPoint = false;
        attackStarted = false;
        moveTimer = moveInterval;

        Debug.Log($"{name} retreated to tier {currentTier}");
    }

    void KillPlayer()
    {
        Debug.Log($"{name} KILLED THE PLAYER! Jumpscare triggered!");
        JumpscareManager jm = FindObjectOfType<JumpscareManager>();
        if (jm != null)
            jm.TriggerJumpscare();
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

}
