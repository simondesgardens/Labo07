using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.InputSystem;

public class JammoPlayer : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 0.1f;

    [Header("Inputs")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    private CharacterController characterController;
    private float verticalVelocity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        var camera = Camera.main!;
        var cameraTransform = camera.transform;
        var up = cameraTransform.up;
        var forward = cameraTransform.forward;
        var right = cameraTransform.right;

        // Retirer la rotation x et z (garder le movement horizontal).
        forward.y = 0f;
        right.y = 0f;

        // Lire les entrées du joueur.
        var moveInput = moveAction.action.ReadValue<Vector2>();
        var jumpInput = jumpAction.action.triggered;

        var horizontalMovement = Vector3.zero;

        // Si le joueur veut pas bouger, ne pas faire bouger le joueur.
        if (moveInput != Vector2.zero)
        {
            var moveDirection = forward * moveInput.y + right * moveInput.x;
            horizontalMovement = moveDirection * (speed * Time.deltaTime);

            var lookRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        // Partie sur le saut
        var gravity = Physics.gravity;
        var isGrounded = characterController.isGrounded;

        // La vélocité est de zéro si on touche lo sol.
        if (isGrounded)
        {
            verticalVelocity = 0f;
        }

        // Calculer la vélocité lorsqu'on saute.
        //
        // Velocity^2 = 2 x Aceeleration (Gravité) x Déplacement (Hauteur voulue)
        if (isGrounded && jumpInput)
        {
            verticalVelocity = Mathf.Sqrt(2 * -gravity.y * 3f);
        }

        // Appliquer la gravité
        verticalVelocity += gravity.y * Time.deltaTime;

        // Calculer le mouvement vertical
        var verticalMovement = up * (verticalVelocity * Time.deltaTime);

        // Appliquer le mouvement
        characterController.Move(horizontalMovement + verticalMovement);
    }
}