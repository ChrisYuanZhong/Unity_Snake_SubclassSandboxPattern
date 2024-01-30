using UnityEngine;

public class Jump : Ability
{
    public Jump(Snake snake) : base(snake) { }

    public int jumpDistance = 5;

    public override Color BodyColor { get { return new Color(0f, 0.4360409f, 1f); } }

    public override void Activate()
    {
        if (snake == null)
            return;

        SetColor(Color.yellow);

        Vector2Int position = new Vector2Int((int)snake.transform.position.x, (int)snake.transform.position.y);
        Vector2Int direction = new Vector2Int((int)snake._direction.x, (int)snake._direction.y);
        Vector2Int newPosition = position + (direction * jumpDistance);
        
        MoveHead(newPosition);

        // Check if the snake is out of bounds
        if (Mathf.Abs(newPosition.x) > snake.gameManager.mapSize.x / 2 || Mathf.Abs(newPosition.y) > snake.gameManager.mapSize.y / 2)
        {
            snake.Death();
        }
    }

    public override void Deactivate()
    {
        SetColor(BodyColor);
    }
}
