using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class SeaVisualManager : MonoBehaviour
{
    public static SeaVisualManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image seaBackgroundFront; // 앞쪽 이미지
    [SerializeField] private Image seaBackgroundBack; // 뒤쪽 이미지: 대기하고 있음
    [SerializeField] private RectTransform trashContainer;
    [SerializeField] private GameObject trashPrefab;

    private List<GameObject> activeTrashes = new List<GameObject>();
    private ReefData currentData;
    private int currentStepIndex = -1;

    private void Awake() => Instance = this;

    private void Start()
    {
        ResourceManager.instance.OnPurityChanged += UpdateSeaVisuals;
        ReefManager.Instance.OnReefSwitched += (type) => RefreshReefVisuals();
    }

    public void RefreshReefVisuals()
    {
        currentData = ReefManager.Instance.activeReefData;
        currentStepIndex = -1; // 인덱스 초기화
        
        ClearAllTrashes(); // 다른 Reef로 이동 시 기존 쓰레기 즉시 파괴

        int currentPurity = 100; // 해당 Reef의 Purity 값 가져오기
        if (ResourceManager.instance.purityByReef.ContainsKey(currentData.reefType))
        {
            currentPurity = ResourceManager.instance.purityByReef[currentData.reefType];
        }

        UpdateSeaVisuals(currentPurity);
    }

    // Purity 값에 따라 바다 이미지 단계와 쓰레기 유무를 결정
    private void UpdateSeaVisuals(int purity)
    {
        if (currentData == null || currentData.seaBackgroundImages.Count == 0) return;

        // 1. 바다 배경 단계 계산 (예: 3단계인 경우 100-67 / 66-34 / 33-0)
        int stepCount = currentData.seaBackgroundImages.Count;
        int newStepIndex = Mathf.Clamp(stepCount - 1 - (purity * stepCount / 101), 0, stepCount - 1);

        // 단계가 바뀔 때만 서서히 이미지 교체
        if (newStepIndex != currentStepIndex)
        {
            ChangeSeaImage(currentData.seaBackgroundImages[newStepIndex]);
            currentStepIndex = newStepIndex;
        }

        // 2. 쓰레기 관리 로직: 임계값 체크
        ManageTrash(purity);
    }

    // 두 개의 이미지를 교차시켜 서서히 바뀌도록 하는 함수
    private void ChangeSeaImage(Sprite nextSprite)
    {
        // 뒤쪽 이미지 세팅
        seaBackgroundBack.sprite = nextSprite;
        seaBackgroundBack.color = Color.white;
        
        // 앞쪽 이미지를 서서히 투명하게 만들어 뒤쪽 이미지가 보이게 함
        seaBackgroundFront.DOFade(0f, 2f).OnComplete(() => {
            // 페이드 후 앞쪽 이미지 교체 및 다시 불투명하게 세팅
            seaBackgroundFront.sprite = nextSprite;
            seaBackgroundFront.color = new Color(1, 1, 1, 1);
        });
    }

    private void ManageTrash(int purity)
    {
        if (purity <= currentData.pollutionThreshold)
        {
            // 임계값 도달 시 쓰레기가 없는 경우 생성
            if (activeTrashes.Count == 0) SpawnTrash();
        }
        else
        {
            // 수치 벗어날 시 제거
            if (activeTrashes.Count > 0) ClearAllTrashesWithFade();
        }
    }

    private void SpawnTrash()
    {
        if (currentData.trashSprites.Count == 0) return;

        // 무작위 쓰레기 하나 선택해서 생성
        GameObject trash = Instantiate(trashPrefab, trashContainer);
        activeTrashes.Add(trash);

        Image img = trash.GetComponent<Image>();
        img.sprite = currentData.trashSprites[Random.Range(0, currentData.trashSprites.Count)];
        
        // 서서히 나타남
        img.color = new Color(1, 1, 1, 0);
        img.DOFade(1f, 2f);

        // 둥실둥실: yoyo
        trash.transform.localPosition = new Vector3(Random.Range(-600, 600), Random.Range(-350, 350), 0);
        trash.transform.DOLocalMoveY(trash.transform.localPosition.y + 25f, 2.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void ClearAllTrashesWithFade()
    {
        foreach (var trash in activeTrashes)
        {
            trash.GetComponent<Image>().DOFade(0f, 1.5f).OnComplete(() => Destroy(trash));
        }

        activeTrashes.Clear();
    }

    private void ClearAllTrashes()
    {
        foreach (var trash in activeTrashes) Destroy(trash);
        activeTrashes.Clear();
    }
}