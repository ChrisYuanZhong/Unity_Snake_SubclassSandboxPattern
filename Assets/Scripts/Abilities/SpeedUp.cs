using UnityEngine;

public class SpeedUp : Ability
{
    public SpeedUp(Snake snake) : base(snake) { }

    public override Color BodyColor { get { return Color.red; } }

    public override void Activate()
    {
        SetSpeedMultiplier(2.5f);
        SetColor(new Color(1f, 0.5f, 0f));
    }

    public override void Deactivate()
    {
        SetSpeedMultiplier(1.0f);
        SetColor(BodyColor);
    }
}
