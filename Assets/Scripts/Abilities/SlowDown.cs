using UnityEngine;

public class SlowDown : Ability
{
    public SlowDown(Snake snake) : base(snake) { }

    public override Color BodyColor { get { return Color.green; } }

    public override void Activate()
    {
        SetSpeedMultiplier(0.3f);
        SetColor(new Color(0.5f, 1f, 0f));
    }

    public override void Deactivate()
    {
        SetSpeedMultiplier(1.0f);
        SetColor(BodyColor);
    }
}
