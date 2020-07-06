using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    public PlayerController player;
    public Transform Parent;
    public Transform Root;

    private Vector3 currPosition;
    private Vector3 rootStartPosition;
    private Vector3 parentStartPosition;

    void Start()
    {
        currPosition = transform.position;
        rootStartPosition = Root.position;
        parentStartPosition = Parent.position;
    }
    
    void Update()
    {
        if(rootStartPosition.y != Root.position.y)
        {
            float y = rootStartPosition.y - Root.position.y;
            float py = parentStartPosition.y - Parent.position.y;

            currPosition = transform.position;
            currPosition.y -= y - py;

            transform.position = currPosition;

            rootStartPosition = Root.position;
            parentStartPosition = Parent.position;
        }
    }
}
