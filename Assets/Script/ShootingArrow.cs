using UnityEngine;
using static UnityEngine.GameObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ShootingArrow : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 target;
    public GameObject arrow;
    public Transform arrowTransform;
    public bool canShoot;
    private float shootTime;
    public float shootDelay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Time.timeScale == 0) return;
        // Get Mouse Position in 2D
        target = mainCam.ScreenToWorldPoint(Input.mousePosition);
        target.z = 0;

        Vector3 difference = target - transform.position;

        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

        if (!canShoot)
        {
            shootTime += Time.deltaTime;
            if (shootTime >= shootDelay)
            {
                canShoot = true;
                shootTime = 0;
            }
        }

        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            canShoot = false;
            Instantiate(arrow, arrowTransform.position, Quaternion.identity);
        }
    }
}
