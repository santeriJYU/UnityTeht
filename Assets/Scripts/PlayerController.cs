using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;

    Renderer rend;

    Material playerNormal;
    Material playerCharge;
    Material playerChargeFull;
    Material playerSuper;

    private Rigidbody rb;
    private int count;
    private int speed;

    private int charge;
    private int maxCharge;

    Vector3 jump;
    Vector3 superJump;

    bool isJumping;
    bool isSuperJumping;
    bool isCharging;
    bool isCharged;

    private void Start()
    {
        rend = GetComponent<Renderer>();

        playerNormal = Resources.Load("Materials/Player", typeof(Material)) as Material;
        playerCharge = Resources.Load("Materials/PlayerCharge", typeof(Material)) as Material;
        playerChargeFull = Resources.Load("Materials/PlayerChargeFull", typeof(Material)) as Material;
        playerSuper = Resources.Load("Materials/PlayerSuper", typeof(Material)) as Material;

        rb = GetComponent<Rigidbody>();
        count = 0;
        speed = 10;

        charge = 0;
        maxCharge = 50;

        jump = new Vector3(0, 400, 0);
        superJump = new Vector3(0, 1000, 0);

        isJumping = false;
        isSuperJumping = false;
        isCharging = false;
        isCharged = false;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        rb.AddForce(movement * speed);

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && !isSuperJumping && !isCharging && !isCharged)
        {
            rb.AddForce(jump);
            isJumping = true;
        }

        if (rb.transform.position.y < 0)
        {
            restartGame();
        }

        if (!isJumping && !isSuperJumping)
        {
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                if (isCharged)
                {
                    rb.AddForce(superJump);
                    changeMaterial(playerSuper);

                    isSuperJumping = true;
                    isCharged = false;
                }
                else if (isCharging)
                {
                    changeMaterial(playerNormal);
                    isCharging = false;
                }

                charge = 0;
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCharging = true;
                changeMaterial(playerCharge);
            }
            else if (Input.GetKey(KeyCode.LeftControl) && isCharging)
            {
                if (charge < maxCharge)
                {
                    charge++;
                }
                else
                {
                    isCharging = false;
                    isCharged = true;
                    changeMaterial(playerChargeFull);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isJumping)
            {
                isJumping = false;
            }
            else if (isSuperJumping)
            {
                isSuperJumping = false;
                changeMaterial(playerNormal);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;

            if (count >= 10)
            {
                restartGame();
            }
        }
    }

    void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void changeMaterial(Material material)
    {
        Material[] mats = rend.materials;
        mats[0] = material;
        rend.materials = mats;
    }

}
