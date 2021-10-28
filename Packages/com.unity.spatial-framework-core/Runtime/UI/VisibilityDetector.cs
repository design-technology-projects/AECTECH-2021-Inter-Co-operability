using System.Collections.Generic;
using UnityEngine;

namespace Unity.SpatialFramework.Interaction
{
    class VisibilityDetector : MonoBehaviour
    {
        [SerializeField]
        float m_FOVReduction = 0.75f;

        readonly Vector3[] m_Corners = new Vector3[4];
        readonly Plane[] m_Planes = new Plane[6];
        readonly HashSet<IWillRender> m_Visibles = new HashSet<IWillRender>();
        readonly List<IWillRender> m_WillRenders = new List<IWillRender>();

        void Awake()
        {
            Canvas.willRenderCanvases += OnWillRenderCanvases;
        }

        void OnDestroy()
        {
            Canvas.willRenderCanvases -= OnWillRenderCanvases;
        }

        void OnWillRenderCanvases()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return;

            var projection = Matrix4x4.Perspective(mainCamera.fieldOfView * m_FOVReduction, mainCamera.aspect,
                mainCamera.nearClipPlane, mainCamera.farClipPlane);

            var worldToProjection = projection * mainCamera.worldToCameraMatrix;
            GeometryUtility.CalculateFrustumPlanes(worldToProjection, m_Planes);

            m_WillRenders.Clear();
            GetComponentsInChildren(m_WillRenders);
            foreach (var willRender in m_WillRenders)
            {
                var rectTransform = willRender.rectTransform;
                rectTransform.GetLocalCorners(m_Corners);
                if (GeometryUtility.TestPlanesAABB(m_Planes, GeometryUtility.CalculateBounds(m_Corners, rectTransform.localToWorldMatrix)))
                {
                    if (m_Visibles.Add(willRender))
                    {
                        willRender.OnBecameVisible();
                        willRender.removeSelf = Remove;
                    }
                }
                else
                {
                    Remove(willRender);
                }
            }
        }

        void Remove(IWillRender willRender)
        {
            if (m_Visibles.Remove(willRender))
                willRender.OnBecameInvisible();
        }
    }
}
