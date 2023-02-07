using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class IsoFollow : MonoBehaviour
    {
        public const float Angle = 30;
        public const float DistanceFromPlayer = 10;

        [SerializeField] private Transform pointToFollow;
        [SerializeField] private IsoDirection direction = IsoDirection.BackwardsLeft;
        [SerializeField] private float followSpeed = 5;
        // [SerializeField] private float smoothTime = 5f;
        [SerializeField] private float deadZone = 1;
        public IsoDirection Direction => direction;

        private Vector3 _delta;
        private Vector3 _velocity;

        private void Start()
        {
            // _pointToFollow = GameObject.FindWithTag("Player");
            var angleVector = new Vector3(Angle, (float)direction, 0);
            var cameraTransform = transform;
            cameraTransform.rotation = Quaternion.Euler(angleVector);
            _delta = -cameraTransform.forward * DistanceFromPlayer + Vector3.up * .5f;

            var position = pointToFollow.position;
            cameraTransform.position = position + _delta;
        }

        private void LateUpdate()
        {
            var playerPosition = pointToFollow.position;
            var cameraPosition = transform.position;
            var deltaPosition = playerPosition + _delta;
            var distanceToDelta = Vector3.Distance(cameraPosition, deltaPosition);
            
            
            
            if (distanceToDelta > deadZone)
            {
                var deltaVector = deltaPosition - cameraPosition;
                var goalPosition = cameraPosition + deltaVector * (distanceToDelta - deadZone);
                transform.position = Vector3.Lerp(cameraPosition, goalPosition, Time.deltaTime * followSpeed);
                // transform.position = Vector3.SmoothDamp(cameraPosition, goalPosition, ref _velocity, smoothTime * Time.deltaTime);
            }
        }


        public enum IsoDirection
        {
            BackwardsLeft = -135,
            BackwardsRight = 135,
            ForwardLeft = -45,
            ForwardRight = 45
        }
    }
}