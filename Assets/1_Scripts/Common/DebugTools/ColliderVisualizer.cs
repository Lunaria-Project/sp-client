using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class ColliderVisualizer : MonoBehaviour
{
    [SerializeField] private Color _gizmosColor = new(1f, 0f, 0f, 0.65f);

    private bool IsVisible =>
#if UNITY_EDITOR
        PlayerPrefsManager.GetBool(PrefKey.ColliderVisualize);
#else
    false;
#endif
    private Collider2D _collider;
    private Mesh _mesh;

    private void OnEnable()
    {
        if (_collider == null)
        {
            _collider = GetComponent<Collider2D>();
            if (_collider == null)
            {
                LogManager.LogWarning("[ColliderVisualizer] Collider2D가 필요합니다.");
                return;
            }
        }

        RebuildCollider();
    }

    private void OnDisable()
    {
        ClearMesh();
    }

    private void OnDrawGizmos()
    {
        if (!IsVisible) return;
        if (_collider == null) return;
        if (_collider is EdgeCollider2D) return;

        if (_mesh == null || _mesh.vertexCount == 0) return;

        var prevColor = Gizmos.color;
        var prevMatrix = Gizmos.matrix;

        Gizmos.color = _gizmosColor;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawMesh(_mesh);

        Gizmos.matrix = prevMatrix;
        Gizmos.color = prevColor;
    }

    private void ClearMesh()
    {
        if (_mesh == null) return;
        if (Application.isPlaying)
        {
            Destroy(_mesh);
        }
        else
        {
            DestroyImmediate(_mesh);
        }

        _mesh = null;
    }

    [Button]
    private void RebuildCollider()
    {
        if (!IsVisible) return;
        if (_collider == null) return;
        if (_collider is EdgeCollider2D) return;

        ClearMesh();

        var newMesh = _collider.CreateMesh(true, true);
        if (newMesh != null && newMesh.vertexCount > 0)
        {
            if (newMesh.normals == null || newMesh.normals.Length != newMesh.vertexCount)
            {
                var norms = new Vector3[newMesh.vertexCount];
                for (var i = 0; i < norms.Length; i++)
                {
                    norms[i] = Vector3.back;
                }

                newMesh.normals = norms;
            }
            newMesh.RecalculateBounds();
            _mesh = newMesh;
        }

#if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
#endif
    }
}