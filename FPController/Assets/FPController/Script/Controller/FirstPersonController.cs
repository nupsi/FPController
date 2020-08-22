using UnityEngine;

namespace FPController
{
    /// <summary>
    /// First Person Controller using Rigidbody for movement.
    /// Call Move(h, v) and MouseMove(h, v) to apply movement.
    /// Use Pause(p) and Freeze(f) to stop and resume the controller.
    /// Other movement functions: Jump(), CrouchDown(), CrouchUp(), StartRunning() and StopRunning().
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class FirstPersonController : MonoBehaviour
    {
        /*
         * Variables.
         */

        /// <summary>
        /// Controllers Capsule Collider component.
        /// Cached for changing Physics Material and height.
        /// </summary>
        private CapsuleCollider m_collider;

        /// <summary>
        /// Physics Material for Capsule Collider.
        /// Used to manage friction between charachter and ground to prevent sliding along slopes.
        /// </summary>
        private PhysicMaterial m_physicMaterial;

        /// <summary>
        /// Controllers Rigidbody component.
        /// Used for applying movement.
        /// </summary>
        private Rigidbody m_rigidbody;

        /// <summary>
        /// Cached First Person camera.
        /// Once the camera is cached on Awake() its parent transform is cleared.
        /// </summary>
        private Camera m_camera;

        /// <summary>
        /// Camera rotation as euler angle.
        /// </summary>
        private Vector3 m_cameraRotation;

        /// <summary>
        /// Previous force added in local space to Rigidbody during Move().
        /// </summary>
        private Vector3 m_previousForce;

        /// <summary>
        /// Previous force added in local space to Rigidbody while grounded during Move().
        /// Used for calculating movement during air time.
        /// </summary>
        private Vector3 m_lastGround;

        /// <summary>
        /// Target position for camera.
        /// Expected position for camera based on rigidbodys position and velocity (Updated on FixedUpdate).
        /// Camera is moved towards this position to achieve smooth movement (Moved on Update).
        /// </summary>
        private Vector3 m_targetPosition;

        /// <summary>
        /// Target speed for rigidbodys magnitude.
        /// </summary>
        private float m_targetSpeed = 5;

        /// <summary>
        /// Preset for settings used for controller.
        /// </summary>
        [SerializeField]
        private FirstPersonPreset m_settings;

        /// <summary>
        /// Is there request to stand up after crouching.
        /// </summary>
        private bool m_requestingStandUp;

        /// <summary>
        /// Is the controller currently paused.
        /// </summary>
        private bool m_paused;

        /*
         * MonoBehaviour Functions.
         */

        protected void Awake()
        {
            Reset();
            //Clear cameras parent.
            //Camera is cached during Reset().
            m_camera.transform.parent = null;
            //Store cameras original rotation.
            m_cameraRotation = m_camera.transform.eulerAngles;
        }

        protected void Update()
        {
            //Update camera on each frame to lerp it to its target position.
            UpdateCamera();
        }

        protected void FixedUpdate()
        {
            if(!m_paused)
            {
                //Check if standing up is requested and possible.
                StandUp();
            }
            //Set target position for camera.
            m_targetPosition = CameraOffset;
        }

        protected void OnDisable()
        {
            if(m_camera != null)
            {
                //Deactive camera with controller.
                //NOTE: Cameras parent is cleared in Awake().
                m_camera.gameObject.SetActive(false);
            }
        }

        protected void OnEnable()
        {
            if(m_camera != null)
            {
                //Active camera with controller.
                //NOTE: Cameras parent is cleared in Awake().
                m_camera.gameObject.SetActive(true);
                //Update cameras position after it's activated.
                SetCamera();
            }
        }

        protected void Reset()
        {
            //Cache and reset required components.
            this.name = Settings.Name;
            this.tag = Settings.Tag;

            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.freezeRotation = true;
            m_rigidbody.mass = 10f;

            m_collider = GetComponent<CapsuleCollider>();
            m_collider.radius = Settings.Radius;
            m_collider.material = m_physicMaterial = new PhysicMaterial
            {
                name = string.Format("{0}PhysicMaterial", name),
                dynamicFriction = 0,
                staticFriction = 0,
                frictionCombine = PhysicMaterialCombine.Minimum
            };

            ChangeHeight(Settings.Height);

            m_targetSpeed = Settings.WalkSpeed;

            m_camera = GetComponentInChildren<Camera>();
            m_camera.fieldOfView = 70;
            m_camera.nearClipPlane = 0.01f;
            m_camera.farClipPlane = 250f;
            m_camera.transform.localPosition = Vector3.up * (Settings.Height - 0.1f);
            m_camera.name = string.Format("{0} {1}", this.name, "Camera");
        }

#if UNITY_EDITOR

        [SerializeField]
        private bool m_drawRays = false;

        private Vector3 debug_previousPosition;

        protected void OnDrawGizmos()
        {
            if(m_drawRays && UnityEditor.EditorApplication.isPlaying)
            {
                var radius = 0.05f;
                var root = transform.position;
                var top = root + Vector3.up * m_collider.height;
                var cam = m_targetPosition;
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(root, radius);
                Gizmos.DrawWireSphere(top, radius);
                Gizmos.DrawLine(root, top);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(CameraOffset, Vector3.one * radius);
                Gizmos.DrawLine(CameraOffset, m_camera.transform.position);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(cam, radius);
                Gizmos.DrawLine(cam, m_camera.transform.position);
                Gizmos.DrawWireCube(m_camera.transform.position, (Vector3.one * radius) * 0.9f);

                Gizmos.color = Color.red;
                var velocity = m_rigidbody.velocity * 0.2f;
                var target = m_targetPosition + velocity;
                Gizmos.DrawWireSphere(target, radius);
                Gizmos.DrawLine(target, m_camera.transform.position);

                Debug.DrawLine(transform.position, debug_previousPosition, Color.blue, 2f);
                debug_previousPosition = transform.position;
            }
        }

#endif

        /*
         * Public Functions.
         */

        /// <summary>
        /// Moves the rigidbody component with given horizontal and vertical input.
        /// By default the input values are clamped between -1 and 1 and multiplied by the target speed.
        /// </summary>
        /// <param name="_horizontal">Horizontal Input.</param>
        /// <param name="_vertical">Vertical Input.</param>
        /// <param name="_clamp">Clamp the input values between -1 and 1.</param>
        public void Move(float _horizontal, float _vertical, bool _clamp = true)
        {
            if(_clamp)
            {
                _horizontal = Mathf.Clamp(_horizontal, -1, 1);
                _vertical = Mathf.Clamp(_vertical, -1, 1);
            }

            Move(new Vector3(_horizontal * m_targetSpeed, 0, _vertical * m_targetSpeed));
        }

        /// <summary>
        /// Rotates the controller with given horizontal and vertical input.
        /// </summary>
        /// <param name="_horizontal">Horizontal Input.</param>
        /// <param name="_vertical">Vertical Input.</param>
        public void MouseMove(float _horizontal, float _vertical)
        {
            MouseMove(new Vector2(_horizontal, _vertical));
        }

        /// <summary>
        /// Makes the controller crouch down if allowed.
        /// <see cref="CanCrouch"/>
        /// </summary>
        public void CrouchDown()
        {
            if(CanCrouch && !m_paused)
            {
                ChangeHeight(Settings.Height / 2);
                m_targetSpeed = Settings.WalkSpeed * Settings.CrouchMultiplier;
                Crouching = true;
                m_requestingStandUp = false;
            }
        }

        /// <summary>
        /// Makes the controller stands up after crouching.
        /// <see cref="CrouchDown"/>
        /// </summary>
        public void CrouchUp()
        {
            if(Crouching)
            {
                //Set Request for standing up.
                //StandUp() will check if standing up is possible.
                m_requestingStandUp = true;
            }
        }

        /// <summary>
        /// Makes the conttroller run if allowed.
        /// <see cref="CanRun"/>
        /// </summary>
        public void StartRunning()
        {
            if(CanRun && !m_paused)
            {
                m_targetSpeed = Settings.WalkSpeed * Settings.RunMultiplier;
                Running = true;
            }
        }

        /// <summary>
        /// Makes the controller walk if running.
        /// <see cref="StartRunning"/>
        /// </summary>
        public void StopRunning()
        {
            if(Running)
            {
                Running = false;
                m_targetSpeed = Settings.WalkSpeed;
            }
        }

        /// <summary>
        /// Makes the controller jump if on the ground.
        /// <see cref="Grounded"/>
        /// <see cref="m_jumpForce"/>
        /// </summary>
        public void Jump()
        {
            if(Grounded)
            {
                m_rigidbody.AddForce(0, Settings.JumpForce, 0, ForceMode.VelocityChange);
            }
        }

        /// <summary>
        /// Freeze the controller by preventing user input and making the rigidbody kinematic.
        /// </summary>
        /// <param name="freeze">Freeze/Unfreeze.</param>
        public void Freeze(bool freeze)
        {
            m_rigidbody.isKinematic = freeze;
            Pause(freeze);
        }

        /// <summary>
        /// Pause the controller. Prevent Move() and Look() input.
        /// Rigidbody is still active.
        /// </summary>
        /// <param name="pause">Pause/Unpause.</param>
        public void Pause(bool pause)
        {
            Move(Vector3.zero);
            m_previousForce = Vector3.zero;
            m_paused = pause;
            if(m_rigidbody.isKinematic && !pause)
            {
                Debug.LogWarning("Unpausing freezed First Person Controller! Use Freeze(false) instead.", gameObject);
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
            if(Vector3.Distance(transform.position, m_camera.transform.position) > Settings.Height * 1.3f
                || Mathf.Round(m_rigidbody.velocity.magnitude * 10) / 10 <= 0.1f)
            {
                SetCamera();
                return;
            }

            //Estimate offset with current velocity.
            var velocity = m_rigidbody.velocity * 0.2f;
            //Create target position based off velocity and position.
            var target = m_targetPosition + velocity;
            //Move camera towards target position by lerping.
            m_camera.transform.position = Vector3.Lerp(m_camera.transform.position, target, 5 * Time.deltaTime);
        }

        /// <summary>
        /// Resets controllers camera to the right local position.
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
            if(m_rigidbody == null)
                return;

            if(m_paused)
                _input = Vector3.zero;

            //Convert current input into force.
            var force = Grounded
                ? GroundControl(_input)
                : AirControl(_input, m_previousForce);
            force.y = 0;

            //Store rigidbodys current velocity.
            var velocity = m_rigidbody.velocity;
            velocity.y = 0;

            //Calculate change between current and target velocity.
            var change = Vector3.ClampMagnitude(force, m_targetSpeed) - Vector3.ClampMagnitude(velocity, m_targetSpeed);
            //Add missing force to rigidbody.
            m_rigidbody.AddForce(change, ForceMode.Impulse);

            //Update Capsule Colliders friction to allow/prevent sliding along surfaces.
            UpdateFriction(_input);
            StickToGround();
            //Store previous force.
            m_previousForce = transform.InverseTransformDirection(force);
        }

        /// <summary>
        /// Calculates force for the controller when moving on the ground.
        /// </summary>
        /// <param name="_input">Input for movement.</param>
        /// <returns>New force.</returns>
        private Vector3 GroundControl(Vector3 _input)
        {
            m_lastGround = m_previousForce;
            if(_input == Vector3.zero)
            {
                //Use current velocity to slow down the movement.
                return -m_rigidbody.velocity;
            }
            return transform.TransformDirection(_input);
        }

        /// <summary>
        /// Calculates force for the controller when moving in the air.
        /// </summary>
        /// <param name="_input">Input for movement.</param>
        /// <param name="_force">Previous force.</param>
        /// <returns>New force.</returns>
        private Vector3 AirControl(Vector3 _input, Vector3 _force)
        {
            //Previous force affecting how a new force is applied.
            _force = m_lastGround == Vector3.zero
                ? _input
                : _force + (_input * 0.5f);
            //Lerping force towards zero if input is not given (slowing the charachter down).
            if(_input.x == 0)
            {
                _force.x = Mathf.Lerp(_force.x, 0, 2 * Time.fixedDeltaTime);
            }
            if(_input.z == 0)
            {
                _force.z = Mathf.Lerp(_force.z, 0, 2 * Time.fixedDeltaTime);
            }
            //Convert new force to global space.
            return transform.TransformDirection(_force);
        }

        /// <summary>
        /// Changes Physics Materials fricting based on the ground angle.
        /// Input is required to prevent high friction during movement.
        /// </summary>
        /// <param name="_input">Input for movement.</param>
        private void UpdateFriction(Vector3 _input)
        {
            if(!Grounded)
            {
                ZeroFriction();
                return;
            }

            if(SurfaceAngle > 0 && SurfaceAngle <= Settings.MaxSlopeAngle)
            {
                if(_input == Vector3.zero)
                {
                    MaxFriction(Settings.MaxFriction);
                    return;
                }
                else
                {
                    m_rigidbody.AddForce(Vector3.down * Physics.gravity.y, ForceMode.Force);
                }
            }
            ZeroFriction();
        }

        /// <summary>
        /// Fix controllers velocity to move along slope or ground after surface angle changes.
        /// </summary>
        private void StickToGround()
        {
            RaycastHit hit;
            if(Physics.SphereCast(GroundSphereCenter, SafeRadius, Vector3.down, out hit, 0.075f))
            {
                if(SurfaceAngle < Settings.MaxSlopeAngle)
                {
                    m_rigidbody.velocity -= Vector3.Project(m_rigidbody.velocity, hit.normal);
                }
            }
        }

        /// <summary>
        /// Rotates the controller with given input
        /// </summary>
        /// <param name="_input">Input.</param>
        private void MouseMove(Vector2 _input)
        {
            if(Cursor.lockState != CursorLockMode.Locked || m_paused)
                return;

            transform.Rotate(new Vector3(0, _input.x, 0));
            m_cameraRotation.x = Mathf.Clamp(m_cameraRotation.x + -_input.y, -90, 90);
            m_cameraRotation = new Vector3(m_cameraRotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
            m_camera.transform.eulerAngles = m_cameraRotation;
        }

        /// <summary>
        /// Changes Controllers height to given height.
        /// </summary>
        /// <param name="_height">New height.</param>
        private void ChangeHeight(float _height)
        {
            m_collider.height = _height;
            m_collider.center = Vector3.up * (m_collider.height / 2);
        }

        /// <summary>
        /// Checks if standing up is requested and is it possible.
        /// </summary>
        private void StandUp()
        {
            if(m_requestingStandUp)
            {
                if(CanStandUp)
                {
                    ChangeHeight(Settings.Height);
                    m_targetSpeed = Settings.WalkSpeed;
                    Crouching = false;
                    m_requestingStandUp = false;
                }
            }
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

        /// <summary>
        /// Changes Physic Materials Static friction to zero.
        /// </summary>
        private void ZeroFriction()
        {
            ChangeFriction(0, PhysicMaterialCombine.Minimum);
        }

        /// <summary>
        /// Changes Physic Materials Static friction to given ammount.
        /// </summary>
        private void MaxFriction(float _ammount)
        {
            ChangeFriction(_ammount, PhysicMaterialCombine.Maximum);
        }

        /*
         * Accessors.
         */

        /// <summary>
        /// First person controller settings.
        /// </summary>
        public FirstPersonPreset Settings
        {
            get
            {
                return m_settings ?? (m_settings = ScriptableObject.CreateInstance<FirstPersonPreset>());
            }

            set
            {
                m_settings = value;
            }
        }

        /// <summary>
        /// Is the controller currently standing on ground.
        /// </summary>
        public bool Grounded
        {
            get
            {
                return Physics.SphereCast(new Ray(GroundSphereOffset, Vector3.down), SafeRadius, 0.1f);
            }
        }

        /// <summary>
        /// Is the controller currently running.
        /// </summary>
        public bool Running { get; protected set; }

        /// <summary>
        /// Is the controller currently crouching.
        /// </summary>
        public bool Crouching { get; protected set; }

        /// <summary>
        /// Is Running allowed.
        /// </summary>
        private bool CanRun
        {
            get
            {
                return !Crouching;
            }
        }

        /// <summary>
        /// Is Crouching allowed.
        /// </summary>
        private bool CanCrouch
        {
            get
            {
                return !Running;
            }
        }

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
        /// Returns surface angle below controller.
        /// </summary>
        private float SurfaceAngle
        {
            get
            {
                var angle = 0f;
                RaycastHit hit;
                if(Physics.SphereCast(GroundSphereOffset, Settings.Radius, Vector3.down, out hit, 0.05f))
                {
                    angle = Vector3.Angle(Vector3.up, hit.normal);
                }
                return Mathf.Round(angle);
            }
        }

        /// <summary>
        /// Can Controller stand up from crouching.
        /// </summary>
        private bool CanStandUp
        {
            get
            {
                return Crouching
                    ? !Physics.SphereCast(new Ray(GroundSphereCenter, Vector3.up), SafeRadius, Settings.Height - (Settings.Radius * 2))
                    : true;
            }
        }

        /// <summary>
        /// Center of the bottom 'sphere' of the capsule collider.
        /// </summary>
        private Vector3 GroundSphereCenter
        {
            get
            {
                return new Vector3(transform.position.x, transform.position.y + Settings.Radius, transform.position.z);
            }
        }

        /// <summary>
        /// Center of the bottom 'sphere' of the capsule collider with offset.
        /// </summary>
        private Vector3 GroundSphereOffset
        {
            get
            {
                return GroundSphereCenter + (Vector3.up * (Settings.Radius * 0.1f));
            }
        }

        private float SafeRadius
        {
            get
            {
                return Settings.Radius * 0.95f;
            }
        }
    }
}