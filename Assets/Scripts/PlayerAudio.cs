using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(PlayerDeath))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Footsteps")]
    [SerializeField]
    private AudioClip footstepSFX;

    [SerializeField]
    private float footstepVolume = 1f;

    [SerializeField]
    private float footstepDelay = 0.5f;

    [Header("Jumping")]
    [SerializeField]
    private AudioClip jumpStartSFX;

    [SerializeField]
    private float jumpStartVolume = 1f;

    [SerializeField]
    private AudioClip jumpLandSFX;

    [SerializeField]
    private float jumpLandVolume = 1f;

    [SerializeField]
    private float groundedGraceTime = 0.1f;

    [SerializeField]
    private float minAirborneTime = 0.3f;

    [Header("Wall Jump")]
    [SerializeField]
    private AudioClip wallJumpStartSFX;

    [SerializeField]
    private float wallJumpStartVolume = 1f;

    [Header("Dash")]
    [SerializeField]
    private AudioClip dashSFX;

    [SerializeField]
    private float dashVolume = 1f;

    [Header("Death")]
    [SerializeField]
    private AudioClip deathSFX;

    [SerializeField]
    private float deathVolume = 1f;

    private CharacterMovement characterMovement;
    private PlayerDeath playerDeath;

    private bool wasAirborne;
    private float groundedGraceTimer;
    private float airborneTimer;
    private bool hasDied;

    private void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        playerDeath = GetComponent<PlayerDeath>();

        StartCoroutine(PlayFootsteps());
    }

    private void Update()
    {
        bool isGrounded = characterMovement.isGrounded;

        if (isGrounded)
        {
            groundedGraceTimer = groundedGraceTime;
            airborneTimer = 0f;
        }
        else
        {
            groundedGraceTimer -= Time.deltaTime;
            airborneTimer += Time.deltaTime;
        }

        if (airborneTimer >= minAirborneTime)
            wasAirborne = true;

        if (!playerDeath.isDead)
        {
            // Jumping
            if (characterMovement.didJump && !DialogueManager.Instance.IsDialogueActive())
            {
                AudioManager.Instance.PlayAudio(jumpStartSFX, jumpStartVolume);
                wasAirborne = true;
            }

            // Wall jump
            if (characterMovement.didWallJump && !DialogueManager.Instance.IsDialogueActive())
            {
                AudioManager.Instance.PlayAudio(wallJumpStartSFX, wallJumpStartVolume);
                wasAirborne = true;
            }

            // Landing
            if (wasAirborne && isGrounded)
            {
                AudioManager.Instance.PlayAudio(jumpLandSFX, jumpLandVolume);
                wasAirborne = false;
            }

            // Dash
            if (characterMovement.didDash)
                AudioManager.Instance.PlayAudio(dashSFX, dashVolume);
        }
        else if (!hasDied) // death
        {
            AudioManager.Instance.PlayAudio(deathSFX, deathVolume);
            hasDied = true;
        }
    }

    private IEnumerator PlayFootsteps()
    {
        while (true)
        {
            if (
                !playerDeath.isDead
                && groundedGraceTimer > 0f
                && Mathf.Abs(characterMovement.moveInputX) > 0.1f
            )
            {
                AudioManager.Instance.PlayAudio(footstepSFX, footstepVolume);
                yield return new WaitForSeconds(footstepDelay);
            }
            else
                yield return null;
        }
    }
}
