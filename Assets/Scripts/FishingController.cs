using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class FishingController : MonoBehaviour
{
    public static FishingController instance;

    [Header("Animator")]
    [SerializeField] private Animator anim;

    [Header("Fish Settings")]
    public Transform fishPoint;
    public GameObject fishPrefab;
    public Vector2 randomWaitRange = new Vector2(1.5f, 4f);

    [Header("UI")]
    public TMP_Text collectedText; // Assign in Inspector

    private bool fishSpawned = false;
    private bool canCast = true;
    private bool justCollected = false;
    private Coroutine fishingRoutine;
    private GameObject spawnedFish;

    private int collectedCount = 0; // number of fishes collected


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Ignore clicks on UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (justCollected)
            {
                justCollected = false; // ignore same-frame click
                return;
            }

            TryCast();
        }

    }

    public void CollectFish()
    {
        if (spawnedFish != null)
            Destroy(spawnedFish);

        collectedCount++;
        UpdateCollectedText();

        canCast = true;
        justCollected = true;
    }

    void TryCast()
    {
        if (!canCast) return;

        StartFishing();
    }

    void StartFishing()
    {
        canCast = false;
        fishSpawned = false;

        anim.SetTrigger("throw");
        anim.SetBool("isSearching", true);

        if (fishingRoutine != null)
            StopCoroutine(fishingRoutine);

        fishingRoutine = StartCoroutine(FishDetectionRoutine());
    }

    IEnumerator FishDetectionRoutine()
    {
        float waitTime = Random.Range(randomWaitRange.x, randomWaitRange.y);
        yield return new WaitForSeconds(waitTime);

        anim.SetTrigger("found");
        anim.SetBool("isSearching", false);

        SpawnFish();
    }

    void SpawnFish()
    {
        if (fishSpawned) return;

        spawnedFish = Instantiate(fishPrefab, fishPoint.position, fishPoint.rotation, fishPoint);

        fishSpawned = true;
    }

    void UpdateCollectedText()
    {
        if (collectedText != null)
            collectedText.text = collectedCount.ToString();
    }

    // --- RESET GAME ---
    public void ResetGame()
    {
        // Destroy any spawned fish
        if (spawnedFish != null)
            Destroy(spawnedFish);

        collectedCount = 0;
        UpdateCollectedText();

        canCast = true;
        fishSpawned = false;
        justCollected = false;

        if (fishingRoutine != null)
            StopCoroutine(fishingRoutine);

        anim.SetBool("isSearching", false);
        anim.SetTrigger("idle"); // optional: return to idle
    }
}
