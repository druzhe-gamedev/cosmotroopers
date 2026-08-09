using UnityEngine;

namespace CodeBase.Application.Grid.Interaction
{
    public class Hologram : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;

        public void SetMesh(Mesh mesh) => _meshFilter.sharedMesh = mesh;
    }
}