using System.Collections.Generic;
using UnityEngine;

// 메모리 내 Dialog 데이터의 싱글톤 저장소
// - 게임 시작 시 CSV를 읽어 DialogEven Bundle들을 구성
// - DialogManager에 Bundle을 반환
public class DialogDatabase : MonoBehaviour
{
    public static DialogDatabase Instance { get; private set; }
    // EventID >> bundle
    private Dictionary<int, DialogEvent> bundles;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        Load();
    }

    // CSV를 읽어 모든 Event를 메모리에 올림
    public void Load()
    {
        bundles = DialogParser.BuildEventBundles("Dialog", "DialogSelect");
        Debug.Log($"DialogDatabase: Load() {bundles.Count} ok");
    }

    // 특정 EventBundle 반환: 없는 경우 null 반환
    public DialogEvent GetEvent(int eventId)
    {
        if (bundles != null && bundles.TryGetValue(eventId, out var b)) return b;
        return null;
    }
}