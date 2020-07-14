using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tracer : MonoBehaviour
{
    public GameObject ImpactDust;
    public GameObject BulletImpact;

    public Vector3 direction;
    private Rigidbody body;

    PhotonView photonView;

    float timer = 2.0f;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        body = GetComponent<Rigidbody>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            body.AddForce(direction * 2500.0f);
            //Debug.Log("AddForce: " + direction * 10.0f);

            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                if (PhotonNetwork.IsConnected)
                    PhotonNetwork.Destroy(gameObject);
                else
                    Destroy(gameObject);
            }
        }
    }

    [PunRPC]
    public void Me(Vector3 dir)
    {
        direction = dir;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            if (collision.transform.tag == "Self")
            {
                //Debug.Log("Self Tag");
                return;
            }

            ContactPoint hit = collision.GetContact(0);

            if (PhotonNetwork.IsConnected)
                photonView.RPC("ImpactRPC", RpcTarget.AllBuffered, hit.point, hit.normal);
            else
            {
                ImpactRPC(hit.point, hit.normal);
            }

            // Bullet World Impact
            /* GameObject g = Instantiate(ImpactDust, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
             Destroy(g, 0.5f);


             g = Instantiate(BulletImpact, hit.point + hit.normal * 0.05f, Quaternion.FromToRotation(Vector3.up, hit.normal));
             Destroy(g, 8.5f);*/
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Destroy(gameObject);
            else
                Destroy(gameObject);

            //Debug.Log("Hi: " + collision.transform.name);
        }
    }

    [PunRPC]
    void ImpactRPC(Vector3 pos, Vector3 norm)
    {
        GameObject g = Instantiate(ImpactDust, pos, Quaternion.FromToRotation(Vector3.up, norm));
        Destroy(g, 1.0f);

        g = Instantiate(BulletImpact, pos + norm * 0.05f, Quaternion.FromToRotation(Vector3.up, norm));
        Destroy(g, 5.0f);
    }
}
