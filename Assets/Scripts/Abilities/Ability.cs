using UnityEngine;

public abstract class Ability
{
    public Ability(Snake snake)
    {
        this.snake = snake;
    }

    protected Snake snake;

    public virtual Color BodyColor { get { return new Color(0f, 0.4360409f, 1f); } }

    public abstract void Activate();

    public virtual void Deactivate() { }

    protected void SetColor(Color color)
    {
        if (snake != null)
        {
            foreach (Transform segment in snake._segments)
            {
                SpriteRenderer renderer = segment.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.color = color;
            }
        }
    }

    protected void SetSpeedMultiplier(float multiplier)
    {
        if (snake != null)
            snake._speedMultiplier = multiplier;
    }

    protected void MoveHead(Vector2Int position)
    {
        if (snake != null)
            snake.transform.position = new Vector3(position.x, position.y, 0);
    }
}
