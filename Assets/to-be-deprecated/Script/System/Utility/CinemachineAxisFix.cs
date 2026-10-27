using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
[SaveDuringPlay]
public class CinemachineAxisFix : CinemachineExtension
{
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            if (CameraFollow.Instance == null || CameraFollow.Instance.activeBound == null)
                return;

            Bounds bounds = CameraFollow.Instance.activeBound.bounds;

            Vector3 pos = state.CorrectedPosition;

            float distance = Mathf.Abs(pos.z - bounds.center.z);
            float fov = state.Lens.FieldOfView;
            float viewHeight = 2f * distance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            float viewWidth = viewHeight * state.Lens.Aspect;

            if (bounds.size.x < viewWidth && bounds.size.y < viewHeight) return;

            if (bounds.size.x < viewWidth)
            {
                pos.x = bounds.center.x;
                float halfHeight = viewHeight * 0.5f;
                pos.y = Mathf.Clamp(pos.y, bounds.min.y + halfHeight, bounds.max.y - halfHeight);
            }
            else if (bounds.size.y < viewHeight)
            {
                pos.y = bounds.center.y;
                float halfWidth = viewWidth * 0.5f;
                pos.x = Mathf.Clamp(pos.x, bounds.min.x + halfWidth, bounds.max.x - halfWidth);
            }

            state.PositionCorrection += pos - state.CorrectedPosition;
        }
    }
}