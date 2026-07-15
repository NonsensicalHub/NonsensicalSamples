using System;
using NaughtyAttributes;
using UnityEngine;

namespace TemperatureVisualization
{
    public enum SliceAxis
    {
        XY = 0,
        XZ = 1,
        YZ = 2
    }

    public class TemperatureSliceRenderer : MonoBehaviour
    {
        private static readonly Quaternion s_RotXZ = Quaternion.Euler(90f, 0f, 0f);
        private static readonly Quaternion s_RotYZ = Quaternion.Euler(0f, 90f, 0f);

        [Label("切片材质")]
        [SerializeField] private Material m_SliceMaterial;

        [Label("显示 XY 切片")]
        [SerializeField] private bool m_ShowSliceXY;

        [Label("显示 XZ 切片")]
        [SerializeField] private bool m_ShowSliceXZ;

        [Label("显示 YZ 切片")]
        [SerializeField] private bool m_ShowSliceYZ;

        [Label("XY 切片位置")]
        [SerializeField] private float m_PositionXY = 0.5f;

        [Label("XZ 切片位置")]
        [SerializeField] private float m_PositionXZ = 0.5f;

        [Label("YZ 切片位置")]
        [SerializeField] private float m_PositionYZ = 0.5f;

        private Mesh m_QuadMesh;
        private Material m_MaterialInstance;
        private MaterialPropertyBlock m_PropertyBlock;
        private Texture m_LastTex;
        private Texture m_LastTexPrev;
        private Texture m_LastRamp;
        private float m_LastBlend = -1f;
        private float m_LastTempMin = float.NaN;
        private float m_LastTempMax = float.NaN;
        private float m_LastOpacity = -1f;
        private Vector3 m_LastBoundsMin;
        private Vector3 m_LastBoundsMax;
        private Vector3 m_CachedCenterXY;
        private Vector3 m_CachedCenterXZ;
        private Vector3 m_CachedCenterYZ;
        private Vector3 m_CachedSizeXY;
        private Vector3 m_CachedSizeXZ;
        private Vector3 m_CachedSizeYZ;
        private float m_CachedMinX, m_CachedMaxX, m_CachedMinY, m_CachedMaxY, m_CachedMinZ, m_CachedMaxZ;
        private bool m_BoundsLayoutDirty = true;

        public event Action<SliceAxis, float> SlicePositionChanged;

        public bool ShowSliceXY { get => m_ShowSliceXY; set => m_ShowSliceXY = value; }
        public bool ShowSliceXZ { get => m_ShowSliceXZ; set => m_ShowSliceXZ = value; }
        public bool ShowSliceYZ { get => m_ShowSliceYZ; set => m_ShowSliceYZ = value; }

        public float PositionXY { get => m_PositionXY; set => SetSlicePosition(SliceAxis.XY, value); }
        public float PositionXZ { get => m_PositionXZ; set => SetSlicePosition(SliceAxis.XZ, value); }
        public float PositionYZ { get => m_PositionYZ; set => SetSlicePosition(SliceAxis.YZ, value); }

        public void SetSlicePosition(SliceAxis axis, float normalizedPosition, bool invokeEvent = true)
        {
            normalizedPosition = Mathf.Clamp01(normalizedPosition);
            switch (axis)
            {
                case SliceAxis.XY: m_PositionXY = normalizedPosition; break;
                case SliceAxis.XZ: m_PositionXZ = normalizedPosition; break;
                default: m_PositionYZ = normalizedPosition; break;
            }

            m_BoundsLayoutDirty = true;
            if (invokeEvent) SlicePositionChanged?.Invoke(axis, normalizedPosition);
        }

        public void Render(
            TemperatureFieldInterpolator interpolator,
            TemperatureColorRamp colorRamp,
            TemperatureVolumeBounds bounds,
            float tempMin,
            float tempMax,
            float opacity,
            byte sliceMask = 0xFF)
        {
            if (interpolator == null || colorRamp == null || bounds == null || sliceMask == 0) return;

            EnsureResources();

            Bounds worldBounds = bounds.WorldBounds;
            Vector3 min = worldBounds.min;
            Vector3 max = worldBounds.max;

            if (min != m_LastBoundsMin || max != m_LastBoundsMax)
            {
                m_CachedMinX = min.x; m_CachedMaxX = max.x;
                m_CachedMinY = min.y; m_CachedMaxY = max.y;
                m_CachedMinZ = min.z; m_CachedMaxZ = max.z;
                m_CachedSizeXY = new Vector3(m_CachedMaxX - m_CachedMinX, m_CachedMaxY - m_CachedMinY, 1f);
                m_CachedSizeXZ = new Vector3(m_CachedMaxX - m_CachedMinX, m_CachedMaxZ - m_CachedMinZ, 1f);
                m_CachedSizeYZ = new Vector3(m_CachedMaxZ - m_CachedMinZ, m_CachedMaxY - m_CachedMinY, 1f);
                m_LastBoundsMin = min;
                m_LastBoundsMax = max;
                m_MaterialInstance.SetVector("_BoundsMin", min);
                m_MaterialInstance.SetVector("_BoundsMax", max);
            }

            float cx = (m_CachedMinX + m_CachedMaxX) * 0.5f;
            float cy = (m_CachedMinY + m_CachedMaxY) * 0.5f;
            float cz = (m_CachedMinZ + m_CachedMaxZ) * 0.5f;

            Texture tex = interpolator.CurrentTexture;
            Texture texPrev = interpolator.PreviousTexture;
            Texture ramp = colorRamp.RampTexture;
            float blend = interpolator.BlendFactor;

            if (m_LastTex != tex) { m_MaterialInstance.SetTexture("_TemperatureTex", tex); m_LastTex = tex; }
            if (m_LastTexPrev != texPrev) { m_MaterialInstance.SetTexture("_TemperatureTexPrev", texPrev); m_LastTexPrev = texPrev; }
            if (m_LastRamp != ramp) { m_MaterialInstance.SetTexture("_ColorRamp", ramp); m_LastRamp = ramp; }
            if (!Mathf.Approximately(m_LastBlend, blend)) { m_MaterialInstance.SetFloat("_Blend", blend); m_LastBlend = blend; }
            if (!Mathf.Approximately(m_LastTempMin, tempMin)) { m_MaterialInstance.SetFloat("_TempMin", tempMin); m_LastTempMin = tempMin; }
            if (!Mathf.Approximately(m_LastTempMax, tempMax)) { m_MaterialInstance.SetFloat("_TempMax", tempMax); m_LastTempMax = tempMax; }
            if (!Mathf.Approximately(m_LastOpacity, opacity)) { m_MaterialInstance.SetFloat("_Opacity", opacity); m_LastOpacity = opacity; }

            if ((sliceMask & 1) != 0)
            {
                m_CachedCenterXY = new Vector3(cx, cy, Mathf.Lerp(m_CachedMinZ, m_CachedMaxZ, m_PositionXY));
                DrawSlice(SliceAxis.XY, m_PositionXY, m_CachedCenterXY, m_CachedSizeXY, Quaternion.identity);
            }
            if ((sliceMask & 2) != 0)
            {
                m_CachedCenterXZ = new Vector3(cx, Mathf.Lerp(m_CachedMinY, m_CachedMaxY, m_PositionXZ), cz);
                DrawSlice(SliceAxis.XZ, m_PositionXZ, m_CachedCenterXZ, m_CachedSizeXZ, s_RotXZ);
            }
            if ((sliceMask & 4) != 0)
            {
                m_CachedCenterYZ = new Vector3(Mathf.Lerp(m_CachedMinX, m_CachedMaxX, m_PositionYZ), cy, cz);
                DrawSlice(SliceAxis.YZ, m_PositionYZ, m_CachedCenterYZ, m_CachedSizeYZ, s_RotYZ);
            }
        }

        public void InvalidateCache()
        {
            m_LastTex = null;
            m_LastTexPrev = null;
            m_LastRamp = null;
            m_LastBlend = -1f;
            m_LastTempMin = float.NaN;
            m_BoundsLayoutDirty = true;
        }

        private void DrawSlice(SliceAxis axis, float position, Vector3 center, Vector3 size, Quaternion rotation)
        {
            // DrawMesh 延迟提交：不能改共享 Material，否则后一次会覆盖前一次的轴/位置。
            m_PropertyBlock.SetFloat("_SliceAxis", (float)axis);
            m_PropertyBlock.SetFloat("_SlicePosition", position);
            Graphics.DrawMesh(
                m_QuadMesh,
                Matrix4x4.TRS(center, rotation, size),
                m_MaterialInstance,
                gameObject.layer,
                null,
                0,
                m_PropertyBlock);
        }

        private void EnsureResources()
        {
            if (m_QuadMesh == null) m_QuadMesh = CreateQuadMesh();
            if (m_PropertyBlock == null) m_PropertyBlock = new MaterialPropertyBlock();
            if (m_MaterialInstance == null)
            {
                m_MaterialInstance = m_SliceMaterial != null
                    ? new Material(m_SliceMaterial)
                    : new Material(Shader.Find("TemperatureVisualization/Slice"));
            }
        }

        private void OnDestroy()
        {
            if (m_MaterialInstance != null)
            {
                if (Application.isPlaying) Destroy(m_MaterialInstance);
                else DestroyImmediate(m_MaterialInstance);
            }

            if (m_QuadMesh != null)
            {
                if (Application.isPlaying) Destroy(m_QuadMesh);
                else DestroyImmediate(m_QuadMesh);
            }
        }

        private static Mesh CreateQuadMesh()
        {
            var mesh = new Mesh { name = "TemperatureSliceQuad" };
            mesh.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, 0f), new Vector3(0.5f, -0.5f, 0f),
                new Vector3(0.5f, 0.5f, 0f), new Vector3(-0.5f, 0.5f, 0f)
            };
            mesh.uv = new[]
            {
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f)
            };
            mesh.triangles = new[] { 0, 2, 1, 0, 3, 2 };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
