using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Fish : MonoBehaviour
{
    [SerializeField] private float velocityCoeff = 1.0f;
    private bool gameStart = false;
    private enum Direction {None, Up, Down, Right, Left };
    private Direction currentDirection = Direction.None;
    private Dictionary<Direction, Vector2> directionMap;
    private Dictionary<Vector3, Direction> orientationMap;
    private bool isAlive = false;
    private Vector3 startingPosition = new Vector3(-8, 0, 0);
    private void Awake()
    {
        isAlive = true;
        directionMap = new Dictionary<Direction, Vector2>()
        {
            {Direction.None, Vector2.zero },
            {Direction.Left, Vector2.left },
            {Direction.Right, Vector2.right },
            {Direction.Up, Vector2.up },
            {Direction.Down, Vector2.down }
        };

        orientationMap = new Dictionary<Vector3, Direction>()
        {
            {Vector3.zero, Direction.Right },
            {new Vector3(0, 0, 90), Direction.Down },
            {new Vector3(0, 0, 180), Direction.Left },
            {new Vector3(0, 0, 270), Direction.Up }
        };
    }

    void Update()
    {
        if (gameStart && currentDirection == Direction.None)
        {
            GetComponent<Rigidbody2D>().velocity = velocityCoeff * Vector2.right;
            currentDirection = Direction.Right;
        }

        else if (!gameStart)
        {
            Debug.Log(isAlive);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameStart = true;
            }

            else if (!isAlive && Input.GetKeyDown(KeyCode.Return))
            {
                Reset();
            }
        }

        else
        {
            HandleDirectionChange();
        }

    }

    private void HandleDirectionChange()
    {
        if (!gameStart)
        {
            currentDirection = Direction.None;
        }

        else
        {
            GetComponent<Rigidbody2D>().velocity = velocityCoeff * directionMap[currentDirection];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pizza"))
        {
            Vector3 pizzaOrientation = collision.transform.localEulerAngles;
            currentDirection = orientationMap[pizzaOrientation];
            HandleRotation(pizzaOrientation);
            collision.gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Enemy"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            isAlive = false;
            GetComponent<SpriteRenderer>().enabled = false;
            gameStart = false;
        }
    }

    private void HandleRotation(Vector3 pizzaOrientation)
    {
        Debug.Log(currentDirection);
        if (currentDirection == Direction.Down || currentDirection == Direction.Up)
        {
            transform.Rotate(-1.0f * pizzaOrientation, Space.Self);
        }
        GetComponent<SpriteRenderer>().flipY = (currentDirection == Direction.Left);
    }

    private void Reset()
    {
        isAlive = true;
        transform.position = startingPosition;
        transform.Rotate(Vector3.zero);
        GetComponent<SpriteRenderer>().enabled = true;
        currentDirection = Direction.None;
    }
}
