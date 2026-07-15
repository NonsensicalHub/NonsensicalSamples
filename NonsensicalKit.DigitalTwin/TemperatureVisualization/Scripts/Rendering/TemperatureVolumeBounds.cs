using NaughtyAttributes;
using UnityEngine;

namespace TemperatureVisualization
{
    /// <summary>
    /// 可配置大小的立方体体积边界，作为插值与渲染的空间参照。
    /// 在 Scene 视图中选中后可像 BoxCollider 一样拖拽调整大小与位置。
    /// </summary>
    [ExecuteAlways]
    public class TemperatureVolumeBounds : MonoBehaviour
    {
        [Label("本地中心偏移")]
        [SerializeField] private Vector3 m_Center;

        [Label("体积尺寸")]
        [SerializeField] private Vector3 m_Size = new Vector3(10f, 6f, 8f);

        [Label("显示 Gizmo")]
        [SerializeField] private bool m_ShowGizmo = true;

        [Label("Scene 视图可编辑")]
        [SerializeField] private bool m_EditBoundsInScene = true;

        [Label("Gizmo 颜色")]
        [SerializeField] private Color m_GizmoColor = new Color(0.2f, 0.8f, 1f, 0.35f);

        private Bounds m_CachedBounds;
        private Vector3 m_CachedPosition;
        private Quaternion m_CachedRotation;
        private Vector3 m_CachedLossyScale;
        private Vector3 m_CachedCenter;
        private Vector3 m_CachedSize;
        private bool m_BoundsDirty = true;

        public Vector3 Center
        {
            get => m_Center;
            set
            {
                m_Center = value;
                m_BoundsDirty = true;
            }
        }

        public Vector3 Size
        {
            get => m_Size;
            set
            {
                m_Size = Vector3.Max(value, Vector3.one * 0.1f);
                m_BoundsDirty = true;
            }
        }

        public bool ShowGizmo
        {
            get => m_ShowGizmo;
            set => m_ShowGizmo = value;
        }

        public bool EditBoundsInScene
        {
            get => m_EditBoundsInScene;
            set => m_EditBoundsInScene = value;
        }

        public Bounds WorldBounds
        {
            get
            {
                if (m_BoundsDirty
                    || transform.position != m_CachedPosition
                    || transform.rotation != m_CachedRotation
                    || transform.lossyScale != m_CachedLossyScale
                    || m_Center != m_CachedCenter
                    || m_Size != m_CachedSize)
                {
                    Vector3 worldCenter = transform.TransformPoint(m_Center);
                    Vector3 worldSize = Vector3.Scale(m_Size, Abs(transform.lossyScale));
                    m_CachedBounds = new Bounds(worldCenter, worldSize);
                    m_CachedPosition = transform.position;
                    m_CachedRotation = transform.rotation;
                    m_CachedLossyScale = transform.lossyScale;
                    m_CachedCenter = m_Center;
                    m_CachedSize = m_Size;
                    m_BoundsDirty = false;
                }

                return m_CachedBounds;
            }
        }

        public void SetWorldBounds(Bounds worldBounds)
        {
            Vector3 localCenter = transform.InverseTransformPoint(worldBounds.center);
            Vector3 lossyScale = Abs(transform.lossyScale);
            Vector3 localSize = new Vector3(
                worldBounds.size.x / Mathf.Max(lossyScale.x, 1e-6f),
                worldBounds.size.y / Mathf.Max(lossyScale.y, 1e-6f),
                worldBounds.size.z / Mathf.Max(lossyScale.z, 1e-6f));

            m_Center = localCenter;
            m_Size = Vector3.Max(localSize, Vector3.one * 0.1f);
            m_BoundsDirty = true;
        }

        public Vector3 WorldToNormalized(Vector3 worldPosition)
        {
            Bounds bounds = WorldBounds;
            Vector3 local = worldPosition - bounds.min;
            Vector3 size = bounds.size;
            return new Vector3(local.x / size.x, local.y / size.y, local.z / size.z);
        }

        public Vector3 NormalizedToWorld(Vector3 normalized)
        {
            Bounds bounds = WorldBounds;
            Vector3 size = bounds.size;
            return bounds.min + new Vector3(normalized.x * size.x, normalized.y * size.y, normalized.z * size.z);
        }

        public bool Contains(Vector3 worldPosition) => WorldBounds.Contains(worldPosition);

        private void OnValidate()
        {
            m_Size = Vector3.Max(m_Size, Vector3.one * 0.1f);
            m_BoundsDirty = true;
        }

        private void OnDrawGizmos()
        {
            DrawBoundsGizmo(false);
        }

        private void OnDrawGizmosSelected()
        {
            DrawBoundsGizmo(true);
        }

        private void DrawBoundsGizmo(bool selected)
        {
            if (!m_ShowGizmo) return;

            Bounds bounds = WorldBounds;
            Color fill = m_GizmoColor;
            if (selected)
                fill.a = Mathf.Clamp01(m_GizmoColor.a + 0.15f);

            Gizmos.color = new Color(fill.r, fill.g, fill.b, fill.a * 0.2f);
            Gizmos.DrawCube(bounds.center, bounds.size);
            Gizmos.color = fill;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        private static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }
}
