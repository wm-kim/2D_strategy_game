using UnityEngine;

namespace Minimax.Utilities
{
    public static class CameraEx
    {
        public static Vector2 CalculateFrustumSize(this Camera camera, float distanceFromCamera)
        {
            // 카메라의 Field of View와 Aspect Ratio를 가져옵니다.
            var fieldOfView = camera.fieldOfView;
            var aspectRatio = camera.aspect;

            // 클리핑 플레인의 높이를 계산합니다. (각도는 라디안 단위로 변환)
            var height = 2.0f * distanceFromCamera * Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad);

            // 클리핑 플레인의 너비를 계산합니다.
            var width = height * aspectRatio;

            return new Vector2(width, height);
        }
    }
}