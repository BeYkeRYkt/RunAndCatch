using UnityEngine;
using System.Collections;

public class PointAtMouse : MonoBehaviour
{
    public Plane playerPlane;
    public Transform Player;
    public Ray ray;

    // Update is called once per frame
    void Update()
    {
        playerPlane = new Plane(Vector3.up, transform.position);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist;

        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

            transform.rotation = targetRotation;
        }

    }
}
