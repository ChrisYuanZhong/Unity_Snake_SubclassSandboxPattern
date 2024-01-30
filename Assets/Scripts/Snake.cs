using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Snake : MonoBehaviour
{
    List<Ability> abilities = new List<Ability>();
    private int _currentAbility = 0;

    public GameManager gameManager;
    public float speed = 7f;
    public float sprintMultiplier = 2.5f;
    public float slowMultiplier = 0.4f;
    public Transform segmentPrefab;

    [HideInInspector]
    public float _speedMultiplier = 1.0f;
    public List<Transform> _segments = new();
    private bool _isAlive = true;
    [HideInInspector]
    public Vector2 _direction = Vector2.right;
    private Vector2 _cachedInput = Vector2.zero;
    private bool _hasInput = false;
    private float _nextUpdate = 0.0f;
    private Vector3 _originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        }

        abilities.Add(new SpeedUp(this));
        abilities.Add(new Jump(this));
        abilities.Add(new SlowDown(this));

        _originalPosition = this.transform.position;
        _segments.Add(this.transform);
        Grow();

        SetBodyColor(abilities[_currentAbility].BodyColor);
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

        // Abilities
        // Use scroll wheel to change abilities
        if (Input.mouseScrollDelta.y > 0)
        {
            abilities[_currentAbility].Deactivate();

            _currentAbility++;
            if (_currentAbility >= abilities.Count)
                _currentAbility = 0;

            SetBodyColor(abilities[_currentAbility].BodyColor);

            print(_currentAbility);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            abilities[_currentAbility].Deactivate();

            _currentAbility--;
            if (_currentAbility < 0)
                _currentAbility = abilities.Count - 1;

            SetBodyColor(abilities[_currentAbility].BodyColor);

            print(_currentAbility);
        }

        // Activate ability
        if (Input.GetKeyDown(KeyCode.Space))
        {
            abilities[_currentAbility].Activate();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            abilities[_currentAbility].Deactivate();
        }

        //// Sprint
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    _speedMultiplier = 2.5f;
        //}
        //else if (Input.GetKeyUp(KeyCode.LeftShift))
        //{
        //    _speedMultiplier = 1.0f;
        //}

        //// Slow
        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //{
        //    _speedMultiplier = 0.3f;
        //}
        //else if (Input.GetKeyUp(KeyCode.LeftControl))
        //{
        //    _speedMultiplier = 1.0f;
        //}

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
            if (gameManager != null)
                gameManager.IncreaseScore();

            Grow();
        }
        else if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Player"))
        {
            Death();
        }
    }

    private void SetBodyColor(Color color)
    {
        foreach (Transform segment in _segments)
        {
            SpriteRenderer renderer = segment.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.color = color;
        }
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        // Make the new segment's color the same as the snake's color
        SpriteRenderer renderer = segment.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.color = this.GetComponent<SpriteRenderer>().color;

        _segments.Add(segment);
    }

    public void Death()
    {
        this._isAlive = false;

        gameManager.Death();
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

        SetBodyColor(abilities[_currentAbility].BodyColor);

        gameManager.Restart();
    }
}
