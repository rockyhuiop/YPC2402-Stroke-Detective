using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkinnedCollider : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] MeshCollider collider;
    private Transform self;
    //if keep update the collider
    public bool KeepRendering=false;
    //if the mesh collider is set
    public bool ColliderSet=false;
    //set the mesh received from SkinnedMeshRenderer to mesh collider
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
        //add mesh collider at euntime
        self.AddComponent<MeshCollider>();
        //get the mesh collider
        collider=GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!meshRenderer) {
            //find the skinned mesh render to get the mesh
            try {
                meshRenderer=self.Find("UMARenderer").GetComponent<SkinnedMeshRenderer>();
            } catch{

            }
        } else {
            //set the mesh of mesh collider only once 
            if (!ColliderSet||KeepRendering) {
                UpdateCollider();
                ColliderSet=true;
            }
        }
    }
}
