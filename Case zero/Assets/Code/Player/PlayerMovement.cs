using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInputController input;

    [Header("Movement")]
    public float moveSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputController>();
    }

    private void Update()
    {
        if (input.DodgePressed)
            Debug.Log("DODGE!");

        if (input.ParryPressed)
            Debug.Log("PARRY!");

        if (input.MeleePressed)
            Debug.Log("MELEE!");

        if (input.RangedPressed)
            Debug.Log("RANGED!");

        if (input.MenuPressed)
            Debug.Log("MENU!");
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input.MoveInput * moveSpeed;
    }
}