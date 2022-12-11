using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Components
    private Rigidbody rb;
    private Animator playerAnim;
    private AudioSource playerAudio;
    public ParticleSystem particleExplosion;
    public ParticleSystem particleDirtSplatter;
    public AudioClip jumpSound;
    public AudioClip crashSound;

    //Serialized 
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityMod;

    //bools
    public bool gameOver = false;
    private bool canDash = true;

    public int jumpNumber = 0;
    public float speed = 10;
    public float score = 0;
    public float scoreMultiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        //Init components
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        
        //Physics
        Physics.gravity *= gravityMod;
    }

    // Update is called once per frame
    void Update()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && !gameOver && jumpNumber < 2)
        {
            Jump();
        }
        //GameOver
        if (gameOver)
        {
            particleDirtSplatter.Stop();
        }
        //Score
        else
        {
            score += Time.deltaTime * scoreMultiplier;
            Debug.Log("Score: " + score.ToString());
        }
        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            canDash = false;
            StartCoroutine("Dash");
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpNumber = 0;
            particleDirtSplatter.Play();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameOver = true;
            playerAnim.SetBool("Death_b", true);
            particleExplosion.Play();
            playerAudio.PlayOneShot(crashSound, 1.0f);
            Debug.Log("Game Over");
        }
    }

    void Jump()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        playerAnim.SetTrigger("Jump_trig");
        particleDirtSplatter.Stop();
        playerAudio.PlayOneShot(jumpSound, 1.0f);
        jumpNumber++;
    }
    IEnumerator Dash()
    {
        canDash = false;
        scoreMultiplier = 3;
        speed = 30;
        yield return new WaitForSeconds(1f);
        speed = 10;
        yield return new WaitForSeconds(5f);
        canDash = true;
        scoreMultiplier = 1;

    }
}
