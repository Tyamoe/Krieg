using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    public RectTransform circle;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        circle.transform.Rotate(0f, 0f, 200.0f * Time.deltaTime);
    }
}
