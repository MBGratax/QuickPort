using UnityEngine;

namespace PresentationController
{
    public struct FrameInput
    {
        public float xAxis;
        public bool bJumpPressed;
        public bool bJumpReleased;
        public override string ToString()
        {
            return $"xAxis: {xAxis},\n JumpPressed: {bJumpPressed},\n JumpReleased: {bJumpReleased}";
        }
    }

    public interface IPlayerController
    {
        public Vector3 Velocity { get; }
        public FrameInput Input { get; }
        public bool JumpingThisFrame { get; }
        public bool LandingThisFrame { get; }
        public Vector3 RawMovement { get; }
        public bool Grounded { get; }
    }

    public struct RayRange
    {
        public readonly Vector2 Start;
        public readonly Vector2 End;
        public readonly Vector2 Direction;

        public RayRange(float x1, float y1, float x2, float y2, Vector2 direction)
        {
            Start = new Vector2(x1, y1);
            End = new Vector2(x2, y2);
            Direction = direction;
        }

    }

}
