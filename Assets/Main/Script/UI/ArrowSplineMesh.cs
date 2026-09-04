using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent(typeof(SplineContainer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ArrowSplineMesh : MonoBehaviour
{
    [Header("화살표 높이")]
    [SerializeField] float lineHeight = 1f;
    [Header("화살표 두께")]
    [SerializeField] private float width = 0.3f;
    [Header("화살표 곡선 지점 개수")]
    [SerializeField] private int segmentCount = 80;
    [Header("화살표 머리")]
    [SerializeField] private GameObject arroHeadObj;
    [SerializeField] private float arrowHeadSize = 0.5f;

    private Transform arrowHead;
    private SplineContainer splineContainer;
    private MeshFilter meshFilter;

    public void TurnOff()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.enabled = false;

        if (arrowHead != null)
        {
            arrowHead.gameObject.SetActive(false);
            SpriteRenderer spriteRenderer = arrowHead.GetComponentInChildren<SpriteRenderer>(true);
            if (spriteRenderer != null) spriteRenderer.enabled = false;
        }
    }

    public void TurnOn()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.enabled = true;

        if (arrowHead != null)
        {
            arrowHead.gameObject.SetActive(true);

            SpriteRenderer spriteRenderer = arrowHead.GetComponentInChildren<SpriteRenderer>(true);
            if (spriteRenderer != null) spriteRenderer.enabled = true;
        }
    }

    public void ClearLine()
    {
        // spline 제거
        splineContainer.Spline.Clear();

        // mesh 제거
        if (meshFilter.mesh != null)
        {
            meshFilter.mesh.Clear();
        }

        // 머리 제거
        if (arrowHead != null)
        {
            arrowHead.gameObject.SetActive(false);
        }
    }

    public void DrawingLine(Vector3 startWorld, Vector3 endWorld)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        Spline spline = splineContainer.Spline;
        spline.Clear();

        Vector3 start = transform.InverseTransformPoint(startWorld);
        Vector3 end = transform.InverseTransformPoint(endWorld);

        Vector3 dir = end - start;

        BezierKnot startKnot = new BezierKnot(start);
        BezierKnot endKnot = new BezierKnot(end);

        startKnot.TangentOut = new float3(dir.x * 0.35f, lineHeight, 0f);

        endKnot.TangentIn = new float3(-dir.x * 0.35f, lineHeight, 0f);

        spline.Add(startKnot);
        spline.Add(endKnot);

        BuildMesh();
        CreateOrUpdateArrowHead();
    }

    public void DrawLine(Vector3 startWorld, Vector3 endWorld, float customLineHeight)
    {
        float prevHeight = lineHeight;

        lineHeight = customLineHeight;

        DrawingLine(startWorld, endWorld);

        lineHeight = prevHeight;
    }

    private void BuildMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(segmentCount + 1) * 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[segmentCount * 6];
        Spline spline = splineContainer.Spline;

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;

            spline.Evaluate(t, out float3 pos, out float3 tangent, out float3 up);

            Vector3 position = (Vector3)pos;
            Vector3 dir = ((Vector3)tangent).normalized;

            if (dir == Vector3.zero)
                dir = Vector3.up;

            Vector3 side = Vector3.Cross(Vector3.forward, dir).normalized;

            vertices[i * 2] = position - side * width * 0.5f;
            vertices[i * 2 + 1] = position + side * width * 0.5f;

            // 이미지 하나를 전체 곡선에 한 번만 펴서 입힘
            uvs[i * 2] = new Vector2(0f, t);
            uvs[i * 2 + 1] = new Vector2(1f, t);
        }

        for (int i = 0; i < segmentCount; i++)
        {
            int v = i * 2;
            int tri = i * 6;

            triangles[tri] = v;
            triangles[tri + 1] = v + 1;
            triangles[tri + 2] = v + 2;
            triangles[tri + 3] = v + 1;
            triangles[tri + 4] = v + 3;
            triangles[tri + 5] = v + 2;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private void CreateOrUpdateArrowHead()
    {
        if (arroHeadObj == null) return;

        if (arrowHead == null)
        {
            GameObject obj = Instantiate(arroHeadObj, transform);
            arrowHead = obj.transform;

            SpriteRenderer spriteRenderer = arrowHead.GetComponentInChildren<SpriteRenderer>(true);
            if (spriteRenderer != null) spriteRenderer.enabled = true;
        }

        arrowHead.gameObject.SetActive(true);
       
        Spline spline = splineContainer.Spline;
        spline.Evaluate(1f, out float3 endPos, out float3 tangent, out float3 up);

        Vector3 worldPos = transform.TransformPoint((Vector3)endPos);
        Vector3 dir = transform.TransformDirection(((Vector3)tangent).normalized);
        
        arrowHead.position = worldPos;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        arrowHead.rotation = Quaternion.Euler(0f, 0f, angle);
        arrowHead.localScale = Vector3.one * arrowHeadSize;
    }

    public Vector3 GetCurveCenterWorld(Vector3 startWorld, Vector3 endWorld, float customLineHeight)
    {
        Vector3 start = transform.InverseTransformPoint(startWorld);
        Vector3 end = transform.InverseTransformPoint(endWorld);

        Vector3 dir = end - start;

        Vector3 p0 = start;
        Vector3 p1 = start + new Vector3(dir.x * 0.35f, customLineHeight, 0f);
        Vector3 p2 = end + new Vector3(-dir.x * 0.35f, customLineHeight, 0f);
        Vector3 p3 = end;

        float t = 0.5f;
        float oneMinusT = 1f - t;

        Vector3 localCenter =
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;

        return transform.TransformPoint(localCenter);
    }


    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        meshFilter = GetComponent<MeshFilter>();
    }

}