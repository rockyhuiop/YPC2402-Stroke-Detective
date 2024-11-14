using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedCollider : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] MeshCollider collider;
    private Transform self;
    private bool ColliderSet=false;
    public void UpdateCollider() {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
    }
    // Start is called before the first frame update
    void Start()
    {
        self=GetComponent<Transform>();
        collider=GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!meshRenderer) {
            meshRenderer=self.Find("UMARenderer").GetComponent<SkinnedMeshRenderer>();
            if (!ColliderSet) {
                UpdateCollider();
                ColliderSet=true;
            }
        }
    }
}
