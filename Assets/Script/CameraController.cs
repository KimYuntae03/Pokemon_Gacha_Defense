using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("카메라가 보여줄 수 있는 전체 영역")]
    [SerializeField]
    private Transform bottomLeftLimit;

    [SerializeField]
    private Transform topRightLimit;


    [Header("드래그 이동")]
    [SerializeField]
    private float dragSpeed = 0.02f;


    [Header("확대 / 축소")]
    [SerializeField]
    private float zoomSpeed = 0.5f;

    // 가장 가까이 확대할 수 있는 값
    [SerializeField]
    private float minZoom = 4f;

    // 시작 화면에서 사용할 줌
    [SerializeField]
    private float startZoom = 7f;


    private Camera cam;

    private Vector2 lastPointerPosition;
    private bool isDragging;

    // 실제로 허용되는 최대 축소값
    private float maxAllowedZoom;


    private void Awake()
    {
        cam = GetComponent<Camera>();
    }


    private void Start()
    {
        if (bottomLeftLimit == null ||
            topRightLimit == null)
        {
            Debug.LogError(
                "CameraController: 카메라 경계 오브젝트가 등록되지 않았습니다.",
                this
            );

            enabled = false;
            return;
        }

        CalculateMaxAllowedZoom();

        /*
         * Inspector에서 설정한 시작 줌이
         * 맵보다 더 크게 설정되어 있어도 자동 제한한다.
         */
        startZoom =
            Mathf.Clamp(
                startZoom,
                minZoom,
                maxAllowedZoom
            );

        cam.orthographicSize = startZoom;

        /*
         * 게임 시작 시 카메라를
         * 제한 영역 정중앙에 놓는다.
         */

        ClampCameraPosition();
    }


    private void Update()
    {
        // HandleDrag(); 
        HandleZoom();

        ClampCameraPosition();
    }


    private void HandleDrag()
    {
        if (Mouse.current == null)
        {
            return;
        }


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            lastPointerPosition =
                Mouse.current.position.ReadValue();

            isDragging = true;
        }


        if (Mouse.current.leftButton.isPressed &&
            isDragging)
        {
            Vector2 currentPointerPosition =
                Mouse.current.position.ReadValue();

            Vector2 delta =
                currentPointerPosition
                - lastPointerPosition;


            /*
             * 현재 Zoom 크기에 따라 이동 감도를 조정한다.
             * 확대된 상태에서는 조금 움직이고,
             * 축소된 상태에서는 조금 더 많이 움직인다.
             */
            float zoomRatio =
                cam.orthographicSize / startZoom;


            Vector3 move =
                new Vector3(
                    -delta.x,
                    -delta.y,
                    0f
                );


            transform.position +=
                move
                * dragSpeed
                * zoomRatio;


            lastPointerPosition =
                currentPointerPosition;
        }


        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }


    private void HandleZoom()
    {
        if (Mouse.current == null)
        {
            return;
        }


        float scroll =
            Mouse.current.scroll.ReadValue().y;


        if (Mathf.Abs(scroll) < 0.01f)
        {
            return;
        }


        cam.orthographicSize -=
            scroll
            * zoomSpeed
            * 0.01f;


        cam.orthographicSize =
            Mathf.Clamp(
                cam.orthographicSize,
                minZoom,
                maxAllowedZoom
            );


        ClampCameraPosition();
    }


    private void CalculateMaxAllowedZoom()
    {
        float mapWidth =
            topRightLimit.position.x
            - bottomLeftLimit.position.x;

        float mapHeight =
            topRightLimit.position.y
            - bottomLeftLimit.position.y;


        /*
         * 세로 기준 최대 Orthographic Size
         *
         * orthographicSize는
         * 화면 세로 길이의 절반이다.
         */
        float maxZoomByHeight =
            mapHeight / 2f;


        /*
         * 가로 기준 최대 Orthographic Size.
         *
         * cameraWidth =
         * orthographicSize * aspect
         */
        float maxZoomByWidth =
            mapWidth
            / (2f * cam.aspect);


        /*
         * 가로 / 세로 중 더 작은 값을 사용해야
         * 어느 방향으로도 맵 밖이 보이지 않는다.
         */
        maxAllowedZoom =
            Mathf.Min(
                maxZoomByHeight,
                maxZoomByWidth
            );


        if (maxAllowedZoom < minZoom)
        {
            Debug.LogWarning(
                "카메라 제한 영역이 Min Zoom보다 작습니다. " +
                "BottomLeftLimit / TopRightLimit 위치를 확인하세요.",
                this
            );
        }
    }


    private void ClampCameraPosition()
    {
        float cameraHalfHeight =
            cam.orthographicSize;

        float cameraHalfWidth =
            cameraHalfHeight * cam.aspect;


        float minX =
            bottomLeftLimit.position.x
            + cameraHalfWidth;

        float maxX =
            topRightLimit.position.x
            - cameraHalfWidth;

        float minY =
            bottomLeftLimit.position.y
            + cameraHalfHeight;

        float maxY =
            topRightLimit.position.y
            - cameraHalfHeight;


        Vector3 position =
            transform.position;


        if (minX <= maxX)
        {
            position.x =
                Mathf.Clamp(
                    position.x,
                    minX,
                    maxX
                );
        }
        else
        {
            position.x =
                GetMapCenter().x;
        }


        if (minY <= maxY)
        {
            position.y =
                Mathf.Clamp(
                    position.y,
                    minY,
                    maxY
                );
        }
        else
        {
            position.y =
                GetMapCenter().y;
        }


        transform.position =
            position;
    }


    private void CenterCamera()
    {
        Vector3 center =
            GetMapCenter();

        transform.position =
            new Vector3(
                center.x,
                center.y,
                transform.position.z
            );
    }


    private Vector3 GetMapCenter()
    {
        return new Vector3(
            (
                bottomLeftLimit.position.x
                + topRightLimit.position.x
            ) / 2f,

            (
                bottomLeftLimit.position.y
                + topRightLimit.position.y
            ) / 2f,

            0f
        );
    }
}