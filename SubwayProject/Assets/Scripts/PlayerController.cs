using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Lanes
{
    Left = 0,
    Center = 1,
    Right = 2
}

public class PlayerController : MonoBehaviour
{
    [Header("POV")]
    [SerializeField]
    bool moveCameraWithRunner;
    [SerializeField]
    Camera camera;

    [Header("Data")]
    [SerializeField]
    int lives;
    [SerializeField]
    float switchDuration;
    [SerializeField]
    float jumpDuration;
    [SerializeField]
    float jumpDistance;
    [SerializeField]
    Lanes currentLane;

    Vector3 oldCamPosition;
    Vector3 targetCamPosition;

    List<Vector3> lanes = new List<Vector3>();
    Lanes previousLane;
    bool isSwitchingLanes = false;
    float elapsedTime = 0f;

    bool isJumping = false;
    bool isFalling = false;
    float jumpingElapsedTime = 0f;
    float baseY;
    
    void Start()
    {
        lanes.Add(new Vector3(-1, transform.position.y, transform.position.z));
        lanes.Add(new Vector3(0, transform.position.y, transform.position.z));
        lanes.Add(new Vector3(1, transform.position.y, transform.position.z));

        transform.position = lanes[(int)currentLane];
        baseY = transform.position.y;
    }

    void Update()
    {
        if (isSwitchingLanes)
        {
            LerpRunner(lanes[(int)previousLane], lanes[(int)currentLane]);
        }
        if (isJumping)
        {
            LerpJump(new Vector3(transform.position.x, baseY, transform.position.z),
                       new Vector3(transform.position.x, baseY + jumpDistance, transform.position.z));
        }
        else if (isFalling)
        {
            LerpJump(new Vector3(transform.position.x, baseY + jumpDistance, transform.position.z),
                       new Vector3(transform.position.x, baseY, transform.position.z));
        }

        SwitchLanes();
        Jump();
    }

    public void Jump(bool hasSwiped = false)
    {
        if((hasSwiped || Input.GetKeyDown(KeyCode.W)) && !isJumping && !isFalling)
        {
            isJumping = true;
            gameObject.GetComponent<Animator>().SetTrigger("Jumping");
        }
    }

    public void SwitchLanes(bool hasSwipedLeft = false, bool hasSwipedRight = false)
    {
        if ((hasSwipedLeft || Input.GetKeyDown(KeyCode.A)) && (int)currentLane > 0 && !isSwitchingLanes)
        {
            isSwitchingLanes = true;
            previousLane = currentLane;
            currentLane = (Lanes)((int)currentLane - 1);

            if(moveCameraWithRunner)
            {
                oldCamPosition = camera.transform.position;
                targetCamPosition = camera.transform.position + new Vector3(-1, 0, 0);
            }
        }
        if ((hasSwipedRight || Input.GetKeyDown(KeyCode.D)) && (int)currentLane < lanes.Count && !isSwitchingLanes)
        {
            isSwitchingLanes = true;
            previousLane = currentLane;
            currentLane = (Lanes)((int)currentLane + 1);

            if (moveCameraWithRunner)
            {
                oldCamPosition = camera.transform.position;
                targetCamPosition = camera.transform.position + new Vector3(1, 0, 0);
            }
        }
    }

    void LerpRunner(Vector3 start, Vector3 destination)
    {
        elapsedTime += Time.deltaTime;
        float lerpIncrement = elapsedTime / switchDuration;

        transform.position = Vector3.Lerp(start, destination, lerpIncrement);

        if (moveCameraWithRunner)
        {
            camera.transform.position = Vector3.Lerp(oldCamPosition, targetCamPosition, lerpIncrement);
        }

        if (elapsedTime >= switchDuration)
        {
            elapsedTime = 0f;
            isSwitchingLanes = false;
        }
    }

    void LerpJump(Vector3 start, Vector3 destination)
    {
        jumpingElapsedTime += Time.deltaTime;
        float lerpIncrement = jumpingElapsedTime / jumpDuration;

        transform.position = Vector3.Lerp(start, destination, lerpIncrement);

        if (jumpingElapsedTime >= jumpDuration)
        {
            jumpingElapsedTime = 0f;

            if(isJumping)
            {
                isJumping = false;
                isFalling = true;
            }
            else if(isFalling)
            {
                isFalling = false;
            }
        }
    }

    void TakeDamage()
    {
        lives--;
        GameManager.Instance.ReduceLife(lives);
        Debug.Log($"You have {lives} left!");

        if(lives <= 0)
        {
            lives = 0;
            Die();
        }
    }

    void Die()
    {
        GameManager.Instance.Restart();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            TakeDamage();
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Coin")
        {
            GameManager.Instance.IncreaseScore();
            collision.gameObject.SetActive(false);
        }
    }
}
