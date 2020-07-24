using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    public RectTransform circle;
    public GameObject t;

    void Awake()
    {
        t = circle.GetChild(0).gameObject;
        t.transform.SetParent(circle.parent, true);
    }

    void OnDisable()
    {
        t.SetActive(false);
    }

    void OnEnable()
    {
        t.SetActive(true);
    }

    void Update()
    {
        circle.transform.Rotate(0f, 0f, 200.0f * Time.deltaTime);
    }
}
