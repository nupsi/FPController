using UnityEngine;

namespace FPController
{
    /// <summary>
    /// First Person Controller using Rigidbody for movement.
    /// Call Move(h, v) and MouseMove(h, v) to apply movement.
    /// Other movement functions: CrouchDown(), CrouchUp(), StartRunning() and StopRunning().
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class FirstPersonController : MonoBehaviour
    {
        /*
         * Variables.
         */

        /// <summary>
        /// Charachters Capsule Collider component.
        /// Cached for changing Physics Material and height.
        /// <see cref="ChangeHeight(float)"/>
        /// <see cref="ChangeFriction(float, PhysicMaterialCombine)"/>
        /// <see cref="m_physicMaterial"/>
        /// </summary>
        private CapsuleCollider m_collider;

        /// <summary>
        /// Physics Material for Capsule Collider.
        /// Used to manage friction between charachter and ground to prevent sliding along slopes.
        /// <see cref="ChangeFriction(float, PhysicMaterialCombine)"/>
        /// <see cref="m_collider"/>
        /// </summary>
        private PhysicMaterial m_physicMaterial;

        /// <summary>
        /// Charachters Rigidbody component.
        /// Used for applying movement.
        /// <see cref="Move(float, float)"/>
        /// <see cref="Move(Vector3)"/>
        /// <see cref="StickToSlope(Vector3)"/>
        /// </summary>
        private Rigidbody m_rigidbody;

        /// <summary>
        /// Cached First Person camera.
        /// Once the camera is cached on Awake() its parent transform is cleared.
        /// <see cref="UpdateCamera"/>
        /// <see cref="SetCamera"/>
        /// <see cref="CameraOffset"/>
        /// </summary>
        private Camera m_camera;

        /// <summary>
        /// Camera rotation as euler angle.
        /// <see cref="MouseMove(Vector2)"/>
        /// </summary>
        private Vector3 m_cameraRotation;

        /// <summary>
        /// Previous force added in local space to Rigidbody during Move().
        /// <see cref="Move(Vector3)"/>
        /// </summary>
        private Vector3 m_previousForce;

        /// <summary>
        /// Previous force added in local space while grounded during Move().
        /// <see cref="Move(Vector3)"/>
        /// </summary>
        private Vector3 m_lastGround;

        /// <summary>
        /// Target position for camera.
        /// Expected position for camera based on rigidbodys position and velocity.
        /// Camera is lerped to this position to achieve smooth movement.
        /// </summary>
        private Vector3 m_targetPosition;

        /// <summary>
        /// Target speed for rigidbodys magnitude.
        /// </summary>
        private float m_targetSpeed = 5;

        /// <summary>
        /// Charachters target walk speed.
        /// </summary>
        private float m_walkSpeed = 5;

        /// <summary>
        /// Charachters target run speed (walk * run multiplier).
        /// </summary>
        private float m_runMultiplier = 1.3f;

        /// <summary>
        /// Charachters target crouching speed (walk * crouch multiplier).
        /// </summary>
        private float m_crouchMultiplier = 0.6f;

        /// <summary>
        /// Cameras look speed (sensitivy).
        /// </summary>
        private float m_lookSpeed = 2;

        /// <summary>
        /// Force added to make rigidbody jump.
        /// </summary>
        private float m_jumpForce = 50;

        /// <summary>
        /// Charachters (Capsule Colliders) radius.
        /// </summary>
        private float m_radius = 0.35f;

        /// <summary>
        /// Charachters (Capsule Colliders) height.
        /// Used for standing after crouching.
        /// </summary>
        private float m_height = 1.75f;

        /// <summary>
        /// Maximum angle for standable slope.
        /// <see cref="StickToSlope(Vector3)"/>
        /// </summary>
        private float m_maxSlopeAngle = 45f;

        /// <summary>
        /// Is charachter currently running.
        /// <see cref="StartRunning"/>
        /// <see cref="StopRunning"/>
        /// <see cref="CanRun"/>
        /// </summary>
        private bool m_running = false;

        /// <summary>
        /// Is charachter currently crouching.
        /// <see cref="CrouchDown"/>
        /// <see cref="CrouchUp"/>
        /// <see cref="CanCrouch"/>
        /// </summary>
        private bool m_crouching = false;

        /*
         * MonoBehaviour Functions.
         */

        private void Awake()
        {
            Reset();
            //Clear cameras parent.
            //Camera is cached during Reset().
            m_camera.transform.parent = null;
            //Store cameras original rotation.
            m_cameraRotation = m_camera.transform.eulerAngles;
        }

        private void Update()
        {
            //Update camera on each frame to lerp it to its target position.
            UpdateCamera();
        }

        private void OnDisable()
        {
            if(m_camera != null)
            {
                //Deactive camera with charachter.
                //NOTE: Cameras parent is cleared in Awake().
                m_camera.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if(m_camera != null)
            {
                //Active camera with charachter.
                //NOTE: Cameras parent is cleared in Awake().
                m_camera.gameObject.SetActive(true);
                //Update cameras position after it's activated.
                SetCamera();
            }
        }

        private void Reset()
        {
            //Cache and reset required components.

            this.name = "Player";
            this.tag = "Player";

            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.freezeRotation = true;
            m_rigidbody.mass = 10f;
            m_physicMaterial = new PhysicMaterial
            {
                name = string.Format("{0}PhysicMaterial", name),
                dynamicFriction = 0,
                staticFriction = 0,
                frictionCombine = PhysicMaterialCombine.Minimum
            };

            m_collider = GetComponent<CapsuleCollider>();
            m_collider.radius = m_radius;
            m_collider.material = m_physicMaterial;
            ChangeHeight(m_height);

            m_camera = GetComponentInChildren<Camera>();
            m_camera.fieldOfView = 70;
            m_camera.nearClipPlane = 0.01f;
            m_camera.farClipPlane = 250f;
            m_camera.transform.localPosition = Vector3.up * (m_height - 0.1f);
            m_camera.name = string.Format("{0} {1}", this.name, "Camera");
        }

        /*
         * Public Functions.
         */

        /// <summary>
        /// Move Charachters Rigidbody component with given horizontal and vertical input.
        /// </summary>
        /// <param name="_horizontal">Horizontal Input.</param>
        /// <param name="_vertical">Vertical Input.</param>
        public void Move(float _horizontal, float _vertical)
        {
            Move(new Vector3(_horizontal * m_targetSpeed, 0, _vertical * m_targetSpeed));
        }

        /// <summary>
        /// Rotate Charachter with given horizontal and vertical input.
        /// </summary>
        /// <param name="_horizontal">Horizontal Input.</param>
        /// <param name="_vertical">Vertical Input.</param>
        public void MouseMove(float _horizontal, float _vertical)
        {
            MouseMove(new Vector2(_horizontal * m_lookSpeed, _vertical * m_lookSpeed));
        }

        /// <summary>
        /// Makes charachter crouch down if allowed.
        /// <see cref="CanCrouch"/>
        /// </summary>
        public void CrouchDown()
        {
            if(CanCrouch)
            {
                ChangeHeight(m_height / 2);
                m_targetSpeed = m_walkSpeed * m_crouchMultiplier;
                m_crouching = true;
            }
        }

        /// <summary>
        /// Makes chrachter stands up after crouching.
        /// <see cref="CrouchDown"/>
        /// </summary>
        public void CrouchUp()
        {
            if(m_crouching)
            {
                ChangeHeight(m_height);
                m_targetSpeed = m_walkSpeed;
                m_crouching = false;
            }
        }

        /// <summary>
        /// Makes charachter run if allowed.
        /// <see cref="CanRun"/>
        /// </summary>
        public void StartRunning()
        {
            if(CanRun)
            {
                m_targetSpeed = m_walkSpeed * m_runMultiplier;
                m_running = true;
            }
        }

        /// <summary>
        /// Makes charachter walk if running.
        /// <see cref="StartRunning"/>
        /// </summary>
        public void StopRunning()
        {
            if(m_running)
            {
                m_running = false;
                m_targetSpeed = m_walkSpeed;
            }
        }

        /// <summary>
        /// Makes charachter jump if on ground.
        /// <see cref="Grounded"/>
        /// <see cref="m_jumpForce"/>
        /// </summary>
        public void Jump()
        {
            if(Grounded)
            {
                m_rigidbody.AddForce(0, m_jumpForce, 0, ForceMode.Impulse);
            }
        }

        /*
         * Private Functions.
         */

        /// <summary>
        /// Update cameras position.
        /// Lerps camera towards target position.
        /// If camera is too far apart from its body, it's forced back to the right position.
        /// <see cref="SetCamera"/>
        /// <see cref="CameraOffset"/>
        /// </summary>
        private void UpdateCamera()
        {
            //Check distance between body and camera.
            if(Vector3.Distance(transform.position, m_camera.transform.position) > m_height * 2)
            {
                SetCamera();
            }

            //Estimate offset with current velocity.
            var velocity = m_rigidbody.velocity * 0.2f;
            //Create target position based off velocity and position.
            var target = m_targetPosition + velocity;
            //Move camera towards target position by lerping.
            m_camera.transform.position = Vector3.Lerp(m_camera.transform.position, target, 5 * Time.deltaTime);
        }

        /// <summary>
        /// Resets camera to the right local position.
        /// <see cref="CameraOffset"/>
        /// </summary>
        private void SetCamera()
        {
            m_camera.transform.position = CameraOffset;
        }

        /// <summary>
        /// Moves the rigidbody component with given force.
        /// </summary>
        /// <param name="_input">Input for movement.</param>
        private void Move(Vector3 _input)
        {
            //If charachter is on air, previous force is used to control velocity.
            var force = m_previousForce;

            if(Grounded)
            {
                //If charachter is grounded given input is used to control velocity.
                m_lastGround = m_previousForce;
                force = transform.TransformDirection(_input);
            }
            else
            {
                //If charachter is in air.
                //Creating temporary variable to modify air movement.
                var temp = new Vector3
                {
                    x = Mathf.Clamp(force.x + (_input.x / 2), -m_targetSpeed, m_targetSpeed),
                    z = m_lastGround.z > 0
                        ? Mathf.Clamp(force.z + (_input.z / 2), 0, m_targetSpeed)
                        : Mathf.Clamp(force.z + (_input.z / 2), -m_targetSpeed, 0)
                };
                //Previous force affecting how new force is applied.
                force = m_lastGround == Vector3.zero ? _input : temp;
                //Lerping force towards zero if input is not given (slowing the charachter down).
                if(_input.x == 0) force.x = Mathf.Lerp(force.x, 0, 2 * Time.deltaTime);
                if(_input.z == 0) force.z = Mathf.Lerp(force.z, 0, 2 * Time.deltaTime);
                //Convert new force to global space.
                force = transform.TransformDirection(force);
            }

            //Store current velocity.
            var velocity = m_rigidbody.velocity;
            //Calculate change between current and target velocity.
            var change = force - velocity;
            change.y = 0;
            //Add missing force to rigidbody.
            m_rigidbody.AddForce(change, ForceMode.Impulse);

            //Clamp velocity to limit max speed.
            ClampVelocity();
            //Update Capsule Colliders friction to allow/prevent sliding against surfaces.
            StickToSlope(_input);

            //Store previous force.
            m_previousForce = transform.InverseTransformDirection(force);
            //Set target position for camera.
            m_targetPosition = CameraOffset;
        }

        /// <summary>
        /// Clamps rigidbody velocity to target speed.
        /// </summary>
        private void ClampVelocity()
        {
            var y = m_rigidbody.velocity.y;
            var velocity = m_rigidbody.velocity;
            velocity.y = 0;
            velocity = Vector3.ClampMagnitude(velocity, m_targetSpeed);
            velocity.y = y;
            m_rigidbody.velocity = velocity;
        }

        /// <summary>
        /// Changes Physics Materials fricting based on the ground angle.
        /// </summary>
        /// <param name="_input"></param>
        private void StickToSlope(Vector3 _input)
        {
            if(!Grounded)
            {
                ChangeFriction(0, PhysicMaterialCombine.Minimum);
                return;
            }

            if(SurfaceAngle <= m_maxSlopeAngle)
            {
                if(_input == Vector3.zero)
                {
                    ChangeFriction(20, PhysicMaterialCombine.Maximum);
                    return;
                }
            }
            ChangeFriction(0, PhysicMaterialCombine.Minimum);
        }

        /// <summary>
        /// Rotates charachter with given input
        /// </summary>
        /// <param name="_input">Input.</param>
        private void MouseMove(Vector2 _input)
        {
            if(Cursor.lockState != CursorLockMode.Locked)
                return;

            transform.Rotate(new Vector3(0, _input.x, 0));
            m_cameraRotation.x = Mathf.Clamp(m_cameraRotation.x + -_input.y, -90, 90);
            m_cameraRotation = new Vector3(m_cameraRotation.x, transform.rotation.eulerAngles.y, transform.eulerAngles.z);
            m_camera.transform.eulerAngles = m_cameraRotation;
        }
        
        /// <summary>
        /// Changes charachters height to given height.
        /// </summary>
        /// <param name="_height">New height.</param>
        private void ChangeHeight(float _height)
        {
            m_collider.height = _height;
            m_collider.center = Vector3.up * (m_collider.height / 2);
        }

        /// <summary>
        /// Changes Physics Materials properties.
        /// </summary>
        /// <param name="_ammount">Static Friction ammount.</param>
        /// <param name="_combine">Physic Material Combine type.</param>
        private void ChangeFriction(float _ammount, PhysicMaterialCombine _combine)
        {
            m_physicMaterial.frictionCombine = _combine;
            m_physicMaterial.staticFriction = _ammount;
        }

        /*
         * Accessors.
         */

        /// <summary>
        /// Camera offset.
        /// Cameras local position in global position.
        /// </summary>
        private Vector3 CameraOffset
        {
            get
            {
                return transform.position + (Vector3.up * (m_collider.height - 0.1f));
            }
        }

        /// <summary>
        /// Is the charachter currently standing on ground.
        /// </summary>
        private bool Grounded
        {
            get
            {
                var hit = new RaycastHit();
                var start = new Vector3(transform.position.x, transform.position.y + (m_radius * 1.1f), transform.position.z);
                return Physics.SphereCast(start, m_radius, Vector3.down, out hit, 0.05f);
            }
        }

        /// <summary>
        /// Returns surface angle below charachter.
        /// </summary>
        private float SurfaceAngle
        {
            get
            {
                var angle = 0f;
                var hit = new RaycastHit();
                var start = new Vector3(transform.position.x, transform.position.y + (m_radius * 1.1f), transform.position.z);
                if(Physics.SphereCast(start, m_radius, Vector3.down, out hit, 0.05f))
                {
                    angle = Vector3.Angle(Vector3.up, hit.normal);
                }
                return Mathf.Round(angle);
            }
        }

        /// <summary>
        /// Can charachter run.
        /// </summary>
        private bool CanRun
        {
            get
            {
                return (!m_crouching);
            }
        }

        /// <summary>
        /// Can charachter crouch.
        /// </summary>
        public bool CanCrouch
        {
            get
            {
                return (!m_running);
            }
        }
    }
}