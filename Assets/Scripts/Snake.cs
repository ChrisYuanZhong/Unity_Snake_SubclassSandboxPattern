using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public float speed = 5f;
    public float speedMultiplier = 1.0f;
    public Transform segmentPrefab;
    public static Snake instance;

    [HideInInspector]
    public List<Transform> _segments = new();
    private bool _isAlive = true;
    private Vector2 _direction = Vector2.right;
    private float _nextUpdate = 0.0f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _segments.Add(this.transform);
        Grow();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
        {
            _direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
        {
            _direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
        {
            _direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
        {
            _direction = Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.R) && !_isAlive)
        {
            Restart();
        }
    }

    private void FixedUpdate()
    {
        if (Time.time < _nextUpdate || !_isAlive)
        {
            return;
        }

        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        this.transform.position += new Vector3(_direction.x, _direction.y, 0);

        _nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Grow();
        }
        else if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Player"))
        {
            Death();
        }
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);
    }

    private void Death()
    {
        this._isAlive = false;
    }

    private void Restart()
    {
        this._isAlive = true;
        this._direction = Vector2.right;

        for (int i = 1; i < this._segments.Count; i++)
        {
            Destroy(this._segments[i].gameObject);
        }

        this._segments.Clear();

        this._segments.Add(this.transform);
        this.transform.position = Vector3.zero;
        Grow();
        this._nextUpdate = 0.0f;
    }
}
