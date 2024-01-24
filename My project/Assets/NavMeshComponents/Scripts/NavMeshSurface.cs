#pragma warning disable CS0618
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using static UnityEditor.PrefabType;
#pragma warning restore CS0618

namespace NavMeshComponents.Scripts
{
    public enum CollectObjects
    {
        All = 0,
        Volume = 1,
        Children = 2,
    }

    [ExecuteInEditMode]
    [DefaultExecutionOrder(-102)]
    [AddComponentMenu("Navigation/NavMeshSurface", 30)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshSurface : MonoBehaviour
    {
        [FormerlySerializedAs("m_AgentTypeID")] [SerializeField]
        private int mAgentTypeID;
        public int AgentTypeID { get => mAgentTypeID;
            set => mAgentTypeID = value;
        }

        [FormerlySerializedAs("m_CollectObjects")] [SerializeField]
        private CollectObjects mCollectObjects = CollectObjects.All;
        public CollectObjects CollectObjects { get => mCollectObjects;
            set => mCollectObjects = value;
        }

        [FormerlySerializedAs("m_Size")] [SerializeField]
        private Vector3 mSize = new Vector3(10.0f, 10.0f, 10.0f);
        public Vector3 Size { get => mSize;
            set => mSize = value;
        }

        [FormerlySerializedAs("m_Center")] [SerializeField]
        private Vector3 mCenter = new Vector3(0, 2.0f, 0);
        public Vector3 Center { get => mCenter;
            set => mCenter = value;
        }

        [FormerlySerializedAs("m_LayerMask")] [SerializeField]
        private LayerMask mLayerMask = ~0;
        public LayerMask LayerMask { get => mLayerMask;
            set => mLayerMask = value;
        }

        [FormerlySerializedAs("m_UseGeometry")] [SerializeField]
        private NavMeshCollectGeometry mUseGeometry = NavMeshCollectGeometry.RenderMeshes;
        public NavMeshCollectGeometry UseGeometry { get => mUseGeometry;
            set => mUseGeometry = value;
        }

        [FormerlySerializedAs("m_DefaultArea")] [SerializeField]
        private int mDefaultArea;
        public int DefaultArea { get => mDefaultArea;
            set => mDefaultArea = value;
        }

        [SerializeField]
        bool m_IgnoreNavMeshAgent = true;
        public bool IgnoreNavMeshAgent { get => m_IgnoreNavMeshAgent;
            set => m_IgnoreNavMeshAgent = value;
        }

        [FormerlySerializedAs("m_IgnoreNavMeshObstacle")] [SerializeField]
        private bool mIgnoreNavMeshObstacle = true;
        public bool IgnoreNavMeshObstacle { get => mIgnoreNavMeshObstacle;
            set => mIgnoreNavMeshObstacle = value;
        }

        [FormerlySerializedAs("m_OverrideTileSize")] [SerializeField]
        private bool mOverrideTileSize;

        public bool OverrideTileSize { get => mOverrideTileSize;
            set => mOverrideTileSize = value;
        }
        [FormerlySerializedAs("m_TileSize")] [SerializeField]
        private int mTileSize = 256;
        public int TileSize { get => mTileSize;
            set => mTileSize = value;
        }
        [FormerlySerializedAs("m_OverrideVoxelSize")] [SerializeField]
        private bool mOverrideVoxelSize;
        public bool OverrideVoxelSize { get => mOverrideVoxelSize;
            set => mOverrideVoxelSize = value;
        }
        [FormerlySerializedAs("m_VoxelSize")] [SerializeField]
        float mVoxelSize;
        public float VoxelSize { get => mVoxelSize;
            set => mVoxelSize = value;
        }

        // Currently not supported advanced options
        [FormerlySerializedAs("m_BuildHeightMesh")] [SerializeField]
        private bool mBuildHeightMesh;
        public bool BuildHeightMesh { get => mBuildHeightMesh;
            set => mBuildHeightMesh = value;
        }

        // Reference to whole scene navmesh data asset.
        [FormerlySerializedAs("m_NavMeshData")]
        [UnityEngine.Serialization.FormerlySerializedAs("m_BakedNavMeshData")]
        [SerializeField]
        private NavMeshData mNavMeshData;
        public NavMeshData NavMeshData { get => mNavMeshData;
            set => mNavMeshData = value;
        }

        // Do not serialize - runtime only state.
        private NavMeshDataInstance _mNavMeshDataInstance;
        private Vector3 _mLastPosition = Vector3.zero;
        private Quaternion _mLastRotation = Quaternion.identity;

        public static List<NavMeshSurface> ActiveSurfaces { get; } = new List<NavMeshSurface>();

        private void OnEnable()
        {
            Register(this);
            AddData();
        }

        private void OnDisable()
        {
            RemoveData();
            Unregister(this);
        }

        public void AddData()
        {
            if (_mNavMeshDataInstance.valid)
                return;

            if (mNavMeshData != null)
            {
                _mNavMeshDataInstance = NavMesh.AddNavMeshData(mNavMeshData, transform.position, transform.rotation);
                _mNavMeshDataInstance.owner = this;
            }

            var transform1 = transform;
            _mLastPosition = transform1.position;
            _mLastRotation = transform1.rotation;
        }

        public void RemoveData()
        {
            _mNavMeshDataInstance.Remove();
            _mNavMeshDataInstance = new NavMeshDataInstance();
        }

        public NavMeshBuildSettings GetBuildSettings()
        {
            var buildSettings = NavMesh.GetSettingsByID(mAgentTypeID);
            if (buildSettings.agentTypeID == -1)
            {
                Debug.LogWarning("No build settings for agent type ID " + AgentTypeID, this);
                buildSettings.agentTypeID = mAgentTypeID;
            }

            if (OverrideTileSize)
            {
                buildSettings.overrideTileSize = true;
                buildSettings.tileSize = TileSize;
            }
            if (OverrideVoxelSize)
            {
                buildSettings.overrideVoxelSize = true;
                buildSettings.voxelSize = VoxelSize;
            }
            return buildSettings;
        }

        public void BuildNavMesh()
        {
            var sources = CollectSources();

            // Use unscaled bounds - this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe - and since navmesh data has no scaling support - it is the right choice here.
            var sourcesBounds = new Bounds(mCenter, Abs(mSize));
            if (mCollectObjects == CollectObjects.All || mCollectObjects == CollectObjects.Children)
            {
                sourcesBounds = CalculateWorldBounds(sources);
            }

            var data = NavMeshBuilder.BuildNavMeshData(GetBuildSettings(),
                    sources, sourcesBounds, transform.position, transform.rotation);

            if (data != null)
            {
                data.name = gameObject.name;
                RemoveData();
                mNavMeshData = data;
                if (isActiveAndEnabled)
                    AddData();
            }
        }

        public AsyncOperation UpdateNavMesh(NavMeshData data)
        {
            var sources = CollectSources();

            // Use unscaled bounds - this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe - and since navmesh data has no scaling support - it is the right choice here.
            var sourcesBounds = new Bounds(mCenter, Abs(mSize));
            if (mCollectObjects == CollectObjects.All || mCollectObjects == CollectObjects.Children)
                sourcesBounds = CalculateWorldBounds(sources);

            return NavMeshBuilder.UpdateNavMeshDataAsync(data, GetBuildSettings(), sources, sourcesBounds);
        }

        static void Register(NavMeshSurface surface)
        {
            if (ActiveSurfaces.Count == 0)
                NavMesh.onPreUpdate += UpdateActive;

            if (!ActiveSurfaces.Contains(surface))
                ActiveSurfaces.Add(surface);
        }

        static void Unregister(NavMeshSurface surface)
        {
            ActiveSurfaces.Remove(surface);

            if (ActiveSurfaces.Count == 0)
                NavMesh.onPreUpdate -= UpdateActive;
        }

        static void UpdateActive()
        {
            for (var i = 0; i < ActiveSurfaces.Count; ++i)
                ActiveSurfaces[i].UpdateDataIfTransformChanged();
        }

        void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
        {
            // Modifiers
            List<NavMeshModifierVolume> modifiers;
            if (mCollectObjects == CollectObjects.Children)
            {
                modifiers = new List<NavMeshModifierVolume>(GetComponentsInChildren<NavMeshModifierVolume>());
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
            {
                modifiers = NavMeshModifierVolume.activeModifiers;
            }

            foreach (var m in modifiers)
            {
                if ((mLayerMask & (1 << m.gameObject.layer)) == 0)
                    continue;
                if (!m.AffectsAgentType(mAgentTypeID))
                    continue;
                var mcenter = m.transform.TransformPoint(m.center);
                var scale = m.transform.lossyScale;
                var msize = new Vector3(m.size.x * Mathf.Abs(scale.x), m.size.y * Mathf.Abs(scale.y), m.size.z * Mathf.Abs(scale.z));

                var src = new NavMeshBuildSource();
                src.shape = NavMeshBuildSourceShape.ModifierBox;
                src.transform = Matrix4x4.TRS(mcenter, m.transform.rotation, Vector3.one);
                src.size = msize;
                src.area = m.area;
                sources.Add(src);
            }
        }

        List<NavMeshBuildSource> CollectSources()
        {
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();

            List<NavMeshModifier> modifiers;
            if (mCollectObjects == CollectObjects.Children)
            {
                modifiers = new List<NavMeshModifier>(GetComponentsInChildren<NavMeshModifier>());
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
            {
                modifiers = NavMeshModifier.activeModifiers;
            }

            foreach (var m in modifiers)
            {
                if ((mLayerMask & (1 << m.gameObject.layer)) == 0)
                    continue;
                if (!m.AffectsAgentType(mAgentTypeID))
                    continue;
                var markup = new NavMeshBuildMarkup();
                markup.root = m.transform;
                markup.overrideArea = m.overrideArea;
                markup.area = m.area;
                markup.ignoreFromBuild = m.ignoreFromBuild;
                markups.Add(markup);
            }

            if (mCollectObjects == CollectObjects.All)
            {
                NavMeshBuilder.CollectSources(null, mLayerMask, mUseGeometry, mDefaultArea, markups, sources);
            }
            else if (mCollectObjects == CollectObjects.Children)
            {
                NavMeshBuilder.CollectSources(transform, mLayerMask, mUseGeometry, mDefaultArea, markups, sources);
            }
            else if (mCollectObjects == CollectObjects.Volume)
            {
                Matrix4x4 localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                var worldBounds = GetWorldBounds(localToWorld, new Bounds(mCenter, mSize));
                NavMeshBuilder.CollectSources(worldBounds, mLayerMask, mUseGeometry, mDefaultArea, markups, sources);
            }

            if (m_IgnoreNavMeshAgent)
                sources.RemoveAll((x) => (x.component != null && x.component.gameObject.GetComponent<NavMeshAgent>() != null));

            if (mIgnoreNavMeshObstacle)
                sources.RemoveAll((x) => (x.component != null && x.component.gameObject.GetComponent<NavMeshObstacle>() != null));

            AppendModifierVolumes(ref sources);

            return sources;
        }

        static Vector3 Abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
        {
            var absAxisX = Abs(mat.MultiplyVector(Vector3.right));
            var absAxisY = Abs(mat.MultiplyVector(Vector3.up));
            var absAxisZ = Abs(mat.MultiplyVector(Vector3.forward));
            var worldPosition = mat.MultiplyPoint(bounds.center);
            var worldSize = absAxisX * bounds.size.x + absAxisY * bounds.size.y + absAxisZ * bounds.size.z;
            return new Bounds(worldPosition, worldSize);
        }

        Bounds CalculateWorldBounds(List<NavMeshBuildSource> sources)
        {
            // Use the unscaled matrix for the NavMeshSurface
            Matrix4x4 worldToLocal = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            worldToLocal = worldToLocal.inverse;

            var result = new Bounds();
            foreach (var src in sources)
            {
                switch (src.shape)
                {
                    case NavMeshBuildSourceShape.Mesh:
                    {
                        var m = src.sourceObject as Mesh;
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, m.bounds));
                        break;
                    }
                    case NavMeshBuildSourceShape.Terrain:
                    {
                        // Terrain pivot is lower/left corner - shift bounds accordingly
                        var t = src.sourceObject as TerrainData;
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(0.5f * t.size, t.size)));
                        break;
                    }
                    case NavMeshBuildSourceShape.Box:
                    case NavMeshBuildSourceShape.Sphere:
                    case NavMeshBuildSourceShape.Capsule:
                    case NavMeshBuildSourceShape.ModifierBox:
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(Vector3.zero, src.size)));
                        break;
                }
            }
            // Inflate the bounds a bit to avoid clipping co-planar sources
            result.Expand(0.1f);
            return result;
        }

        bool HasTransformChanged()
        {
            if (_mLastPosition != transform.position) return true;
            if (_mLastRotation != transform.rotation) return true;
            return false;
        }

        void UpdateDataIfTransformChanged()
        {
            if (HasTransformChanged())
            {
                RemoveData();
                AddData();
            }
        }

#if UNITY_EDITOR
        bool UnshareNavMeshAsset()
        {
            // Nothing to unshare
            if (mNavMeshData == null)
                return false;

            // Prefab parent owns the asset reference
#pragma warning disable CS0618
            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
#pragma warning restore CS0618
            if (prefabType == Prefab)
                return false;

            // An instance can share asset reference only with its prefab parent
#pragma warning disable CS0618
            var prefab = UnityEditor.PrefabUtility.GetPrefabParent(this) as NavMeshSurface;
#pragma warning restore CS0618
            if (prefab != null && prefab.NavMeshData == NavMeshData)
                return false;

            // Don't allow referencing an asset that's assigned to another surface
            for (var i = 0; i < ActiveSurfaces.Count; ++i)
            {
                var surface = ActiveSurfaces[i];
                if (surface != this && surface.mNavMeshData == mNavMeshData)
                    return true;
            }

            // Asset is not referenced by known surfaces
            return false;
        }

        void OnValidate()
        {
            if (UnshareNavMeshAsset())
            {
                Debug.LogWarning("Duplicating NavMeshSurface does not duplicate the referenced navmesh data", this);
                mNavMeshData = null;
            }

            var settings = NavMesh.GetSettingsByID(mAgentTypeID);
            if (settings.agentTypeID != -1)
            {
                // When unchecking the override control, revert to automatic value.
                const float kMinVoxelSize = 0.01f;
                if (!mOverrideVoxelSize)
                    mVoxelSize = settings.agentRadius / 3.0f;
                if (mVoxelSize < kMinVoxelSize)
                    mVoxelSize = kMinVoxelSize;

                // When unchecking the override control, revert to default value.
                const int kMinTileSize = 16;
                const int kMaxTileSize = 1024;
                const int kDefaultTileSize = 256;

                if (!mOverrideTileSize)
                    mTileSize = kDefaultTileSize;
                // Make sure tilesize is in sane range.
                if (mTileSize < kMinTileSize)
                    mTileSize = kMinTileSize;
                if (mTileSize > kMaxTileSize)
                    mTileSize = kMaxTileSize;
            }
        }
#endif
    }
}
