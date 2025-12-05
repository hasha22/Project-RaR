using UnityEngine;

// Dialog System 테스트하려고 만든 스크립트
public class TestManager : MonoBehaviour
{
    // 게임 시작 후 자동으로 대화 이벤트 실행
    void Start()
    {
        // DialogManager 싱글톤이 이미 씬에 있도록
        DialogManager.Instance.PlayEvent(1);
    }
}