using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Food : MonoBehaviour
{
    public GameManager gameManager;

    private Vector2Int mapSize = new Vector2Int(49, 25);

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        }

        mapSize = gameManager.mapSize;

        SpawnFood();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SpawnFood();
        }
    }

    public void SpawnFood()
    {
        Vector2Int foodCoordinate;

        Assert.IsNotNull(gameManager);

        do
        {
            // Generate a random coordinate
            foodCoordinate = new Vector2Int(Random.Range(-mapSize.x / 2 + 1, mapSize.x / 2), Random.Range(-mapSize.y / 2 + 1, mapSize.y / 2));

            // Check if the coordinate is occupied by an obstacle
        } while (!gameManager.CanSpawnFood(foodCoordinate));

        // Change the food's position to the new coordinate
        this.transform.position = new Vector3(foodCoordinate.x, foodCoordinate.y, 0);
    }
}