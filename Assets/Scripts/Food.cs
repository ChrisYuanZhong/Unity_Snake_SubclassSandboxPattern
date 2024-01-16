using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    struct Obstacle
    {
        public Vector2Int coordinate;
        public Vector2Int size;
    }

    public Vector2Int mapSize = new Vector2Int(49, 25);
    private List<Obstacle> obstaclesList = new();

    // Start is called before the first frame update
    void Start()
    {
        UpdateObstacles();
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

        do
        {
            // Generate a random coordinate
            foodCoordinate = new Vector2Int(Random.Range(-mapSize.x / 2 + 1, mapSize.x / 2), Random.Range(-mapSize.y / 2 + 1, mapSize.y / 2));

        // Check if the coordinate is occupied by an obstacle
        } while (!CanSpawnFood(foodCoordinate));

        // Change the food's position to the new coordinate
        this.transform.position = new Vector3(foodCoordinate.x, foodCoordinate.y, 0);
    }

    private bool CanSpawnFood(Vector2Int foodCoordinate)
    {
        // Check if the coordinate is occupied by an obstacle
        foreach (Obstacle obstacle in obstaclesList)
        {
            if (foodCoordinate.x >= obstacle.coordinate.x - obstacle.size.x / 2 && foodCoordinate.x <= obstacle.coordinate.x + obstacle.size.x / 2 &&
                foodCoordinate.y >= obstacle.coordinate.y - obstacle.size.y / 2 && foodCoordinate.y <= obstacle.coordinate.y + obstacle.size.y / 2)
            {
                return false;
            }
        }

        if (Snake.instance != null)
        {
            foreach (Transform segment in Snake.instance._segments)
            {
                if (foodCoordinate.x == segment.position.x && foodCoordinate.y == segment.position.y)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void UpdateObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        // Record all the grid coordinates that are occupied by obstacles
        foreach (GameObject obstacle in obstacles)
        {
            Vector2Int obstacleCoordinate = new Vector2Int((int)obstacle.transform.position.x, (int)obstacle.transform.position.y);
            Vector2Int obstacleSize = new Vector2Int((int)obstacle.transform.localScale.x, (int)obstacle.transform.localScale.y);

            obstaclesList.Add(new Obstacle { coordinate = obstacleCoordinate, size = obstacleSize });
        }
    }
}
