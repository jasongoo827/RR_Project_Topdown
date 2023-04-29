using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : MonoBehaviour
{
    private Vector3 shootDir;

    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        Destroy(gameObject, 3f);
    }

}
