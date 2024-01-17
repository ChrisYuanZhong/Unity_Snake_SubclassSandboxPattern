using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public float speed = 7f;
    public float sprintMultiplier = 2.5f;
    public float slowMultiplier = 0.4f;
    public Transform segmentPrefab;
    public AudioClip deathSound;
    public AudioClip eatSound;
    public static Snake instance;

    [HideInInspector]
    private float _speedMultiplier = 1.0f;
    public List<Transform> _segments = new();
    private bool _isAlive = true;
    private Vector2 _direction = Vector2.right;
    private Vector2 _cachedInput = Vector2.zero;
    private bool _hasInput = false;
    private float _nextUpdate = 0.0f;
    private Vector3 _originalPosition;

    private TextMeshProUGUI _score;
    private GameObject _gameOver;

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
        _originalPosition = this.transform.position;
        _segments.Add(this.transform);
        Grow();

        _score = GameObject.FindWithTag("Score").GetComponent<TextMeshProUGUI>();

        _gameOver = GameObject.FindWithTag("GameOver");
        if (_gameOver != null)
            _gameOver.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(0);
        }

        if (Input.GetKeyDown(KeyCode.R) && !_isAlive)
        {
            Restart();
        }
        else if (!_isAlive)
        {
            return;
        }

        // Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speedMultiplier = 2.5f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speedMultiplier = 1.0f;
        }

        // Slow
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _speedMultiplier = 0.3f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _speedMultiplier = 1.0f;
        }

        // WASD Movement
        if (!_hasInput)
        {
            if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
            {
                _direction = Vector2.up;
                _hasInput = true;
            }
            else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
            {
                _direction = Vector2.down;
                _hasInput = true;
            }
            else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
            {
                _direction = Vector2.left;
                _hasInput = true;
            }
            else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
            {
                _direction = Vector2.right;
                _hasInput = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
            {
                _cachedInput = Vector2.up;
            }
            else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
            {
                _cachedInput = Vector2.down;
            }
            else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
            {
                _cachedInput = Vector2.left;
            }
            else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
            {
                _cachedInput = Vector2.right;
            }
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

        _nextUpdate = Time.time + (1f / (speed * _speedMultiplier));
        _hasInput = false;

        if (_cachedInput != Vector2.zero)
        {
            _direction = _cachedInput;
            _cachedInput = Vector2.zero;
            _hasInput = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            IncreaseScore();
            Grow();
        }
        else if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Player"))
        {
            Death();
        }
    }

    private void IncreaseScore()
    {
        if (_score != null)
            _score.text = (int.Parse(_score.text) + 1).ToString();

        if (eatSound != null)
            AudioSource.PlayClipAtPoint(eatSound, this.transform.position);
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

        if (_gameOver != null)
            this._gameOver.SetActive(true);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, this.transform.position);
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
        this.transform.position = _originalPosition == null ? Vector3.zero : _originalPosition;
        Grow();
        this._speedMultiplier = 1.0f;
        this._nextUpdate = 0.0f;

        if (_gameOver != null)
            this._gameOver.SetActive(false);

        if (_score != null)
            this._score.text = "0";
    }
}
