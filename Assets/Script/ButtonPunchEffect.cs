using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SelectableButton : MonoBehaviour, IPointerDownHandler
{
    // 게임 내 모든 SelectableButton들을 관리하기 위한 정적(Static) 변수
    private static SelectableButton currentSelectedButton;

    private Vector3 originalScale;
    public float scaleMultiplier = 1.2f; // 커질 비율 (기존 크기의 1.2배)
    public float duration = 0.2f;       // 애니메이션 재생 시간

    void Start()
    {
        // 처음 시작할 때 본인의 기본 크기 저장
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 1. 만약 이미 이 버튼이 선택된 상태라면 무시 (원하는 경우 토글 해제 로직으로 변경 가능)
        if (currentSelectedButton == this) return;

        // 2. 다른 버튼이 이미 커져있다면, 그 버튼을 원래 크기로 되돌림
        if (currentSelectedButton != null)
        {
            currentSelectedButton.ResetButtonSize();
        }

        // 3. 현재 버튼을 선택된 버튼으로 지정
        currentSelectedButton = this;

        // 4. 내 버튼 크기 키우기 (DOTween)
        transform.DOKill();
        transform.DOScale(originalScale * scaleMultiplier, duration).SetEase(Ease.OutBack);
    }

    // 버튼 크기를 원래대로 되돌리는 메서드
    public void ResetButtonSize()
    {
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);
    }
}