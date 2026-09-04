using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleCameraManager : MonoBehaviour
{
    [SerializeField] private Camera battleCamera;
    [SerializeField] private Transform focusCenter;

    [Header("전투 연출")]
    [SerializeField] private float startDistance = 2.5f;
    [SerializeField] private float clashDistance = 1.0f;
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private float cameraSize = 35f;

    [Header("수동 카메라")]
    [SerializeField] private float zoomSpeed = 3f;
    [SerializeField] private float minCameraSize = 20f;
    [SerializeField] private float maxCameraSize = 60f;
    [SerializeField] private float dragSpeed = 0.01f;

    [Header("카메라 이동 제한")]
    [SerializeField] private bool useMoveLimit = true;
    [SerializeField] private Vector2 minCameraPosition = new Vector2(-5f, -3f);
    [SerializeField] private Vector2 maxCameraPosition = new Vector2(5f, 3f);

    [Header("더블 클릭 간격 프레임")]
    [SerializeField] private float doubleClickTime = 0.3f;

    private float lastClickTime = -1f;
    private float defaultCameraSize;
    private Vector3 defaultCameraPos;

    private bool canManualControl = true;
    private bool isDragging = false;

    public IEnumerator FocusTwoCharacters(Transform a, Transform b, BattleSide aSide, BattleSide bSide)
    {
        Vector3 center = focusCenter != null ? focusCenter.position : Vector3.zero;
        Vector3 aOriginal = a.position;
        Vector3 bOriginal = b.position;
        Vector3 leftStart = center + Vector3.left * startDistance * 0.5f;
        Vector3 rightStart = center + Vector3.right * startDistance * 0.5f;

        Vector3 aStart = new Vector3();
        Vector3 bStart = new Vector3();

        if (aSide == BattleSide.Player)
        {
            aStart = leftStart;
            bStart = rightStart;
        }

        if (aSide == BattleSide.Enemy)
        {
            aStart = rightStart;
            bStart = leftStart;
        }

        Vector3 leftClose = center + Vector3.left * clashDistance * 0.5f;
        Vector3 rightClose = center + Vector3.right * clashDistance * 0.5f;
        Vector3 aEnd = new Vector3();
        Vector3 bEnd = new Vector3();

        if (aSide == BattleSide.Player)
        {
            aEnd = leftClose;
            bEnd = rightClose;
        }

        if (aSide == BattleSide.Enemy)
        {
            aEnd = rightClose;
            bEnd = leftClose;
        }

        HideOthers(a, b);

        battleCamera.transform.position = new Vector3(center.x, center.y, battleCamera.transform.position.z);
        battleCamera.fieldOfView = cameraSize;

        yield return MovePair(a, aOriginal, aStart, b, bOriginal, bStart);
        yield return MovePair(a, aStart, aEnd, b, bStart, bEnd);

        a.position = aEnd;
        b.position = bEnd;
    }

    private IEnumerator MovePair(Transform a, Vector3 aFrom, Vector3 aTo, Transform b, Vector3 bFrom, Vector3 bTo)
    {
        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            a.position = Vector3.Lerp(aFrom, aTo, t);
            b.position = Vector3.Lerp(bFrom, bTo, t);

            yield return null;
        }

        a.position = aTo;
        b.position = bTo;
    }

    public void SetManualControl(bool value)
    {
        canManualControl = value;

        if (!value) isDragging = false;
    }

    public void ResetCamera()
    {
        battleCamera.transform.position = defaultCameraPos;
        battleCamera.fieldOfView = defaultCameraSize;
    }

    private void HandleZoom()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Approximately(scroll, 0f)) return;

        float direction = Mathf.Sign(scroll);

        battleCamera.fieldOfView -= direction * zoomSpeed;

        battleCamera.fieldOfView = Mathf.Clamp(
            battleCamera.fieldOfView,
            minCameraSize,
            maxCameraSize
        );
    }

    private void HandleDrag()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsPointerOverUI())
            {
                isDragging = false;
                return;
            }
            if (Time.unscaledTime - lastClickTime <= doubleClickTime)
            {
                ResetCamera();

                isDragging = false;
                lastClickTime = -1f;
                return;
            }

            lastClickTime = Time.unscaledTime;
            isDragging = true;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (!isDragging) return;

        if (!Mouse.current.leftButton.isPressed) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float worldUnitsPerPixel = (battleCamera.fieldOfView * 2f) / Screen.height;
        Vector3 move = new Vector3(-mouseDelta.x * dragSpeed, -mouseDelta.y * dragSpeed, 0f);
        Vector3 newPosition = battleCamera.transform.position + move;

        if (useMoveLimit)
        {
            newPosition.x = Mathf.Clamp(
                newPosition.x,
                minCameraPosition.x,
                maxCameraPosition.x
            );

            newPosition.y = Mathf.Clamp(
                newPosition.y,
                minCameraPosition.y,
                maxCameraPosition.y
            );
        }

        battleCamera.transform.position = newPosition;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    public IEnumerator EndFocus(Transform a, Vector3 aOriginal, Transform b, Vector3 bOriginal)
    {
        yield return MovePair(a, a.position, aOriginal, b, b.position, bOriginal);

        ShowAll();

        battleCamera.transform.position = defaultCameraPos;
        battleCamera.fieldOfView = defaultCameraSize;
    }

    public Vector3 GetPosition(Transform target)
    {
        return target.position;
    }

    private void HideOthers(Transform a, Transform b)
    {
        BattleAnimationPlayer[] players = FindObjectsByType<BattleAnimationPlayer>(FindObjectsSortMode.None);

        foreach (BattleAnimationPlayer player in players)
        {
            Transform root = player.transform.root;

            if (player.transform == a || player.transform == b) continue;

            SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }

    private void ShowAll()
    {
        SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = true;
        }

    }

    private void Update()
    {
        if (!canManualControl) return;

        HandleZoom();
        HandleDrag();
    }

    private void Awake()
    {
        if (battleCamera == null) battleCamera = Camera.main;

        defaultCameraSize = battleCamera.fieldOfView;
        defaultCameraPos = battleCamera.transform.position;
    }
}