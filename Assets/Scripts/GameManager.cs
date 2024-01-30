using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public struct Obstacle
    {
        public Vector2Int coordinate;
        public Vector2Int size;
    }
    public Vector2Int mapSize = new Vector2Int(49, 25);
    public Snake snake;
    public AudioClip deathSound;
    public AudioClip eatSound;

    private TextMeshProUGUI _score;
    private GameObject _gameOver;

    [HideInInspector]
    public List<Obstacle> obstaclesList = new();

    // Start is called before the first frame update
    void Start()
    {
        UpdateObstacles();

        _score = GameObject.FindWithTag("Score").GetComponent<TextMeshProUGUI>();

        _gameOver = GameObject.FindWithTag("GameOver");
        if (_gameOver != null)
            _gameOver.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public bool CanSpawnFood(Vector2Int foodCoordinate)
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

        if (snake != null)
        {
            foreach (Transform segment in snake._segments)
            {
                if (foodCoordinate.x == segment.position.x && foodCoordinate.y == segment.position.y)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void IncreaseScore()
    {
        if (_score != null)
            _score.text = (int.Parse(_score.text) + 1).ToString();

        if (eatSound != null)
            AudioSource.PlayClipAtPoint(eatSound, this.transform.position);
    }

    public void Death()
    {
        if (_gameOver != null)
            this._gameOver.SetActive(true);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, this.transform.position);
    }

    public void Restart()
    {
        if (_gameOver != null)
            this._gameOver.SetActive(false);

        if (_score != null)
            this._score.text = "0";
    }
}
