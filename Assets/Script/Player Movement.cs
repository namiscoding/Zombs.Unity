using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Camera m_Camera;
    private Vector3 mouseP;

    void Start()
    {
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // X? l� di chuy?n
        rb.linearVelocity = moveInput * speed;

        // L?y v? tr� chu?t trong t?a ?? th? gi?i
        mouseP = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        mouseP.z = transform.position.z; // ??m b?o z kh?p v?i z c?a nh�n v?t (cho 2D)

        // T�nh h??ng t? nh�n v?t t?i chu?t
        Vector3 rotation = mouseP - transform.position;

        // T�nh g�c t?i chu?t theo ??
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // C?ng th�m 180 ?? ?? ??i m?t h??ng ng??c l?i
        rotZ += 160f;

        // �p d?ng quay cho nh�n v?t
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}