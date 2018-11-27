using UnityEngine;

namespace FPController
{
    /// <summary>
    /// First Person Controller using Rigidbody for movement.
    /// Call Move(h, v) and MouseMove(h, v) to apply movement.
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
        /// Controllers Rigidbody component.
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
        /// Previous force added in local space to Rigidbody while grounded during Move().
        /// Used for calculating movement during air time.
        /// <see cref="Move(Vector3)"/>
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
        private FirsPersonPreset m_settings;

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
            //Time.timeScale = 0.2f;
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
                //Deactive camera with controller.
                //NOTE: Cameras parent is cleared in Awake().
                m_camera.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
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

        private void Reset()
        {
            //Cache and reset required components.

            this.name = Settings.Name;
            this.tag = Settings.Tag;

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
            m_collider.radius = Settings.Radius;
            m_collider.material = m_physicMaterial;
            ChangeHeight(Settings.Height);

            m_camera = GetComponentInChildren<Camera>();
            m_camera.fieldOfView = 70;
            m_camera.nearClipPlane = 0.01f;
            m_camera.farClipPlane = 250f;
            m_camera.transform.localPosition = Vector3.up * (Settings.Height - 0.1f);
            m_camera.name = string.Format("{0} {1}", this.name, "Camera");
        }

#if UNITY_EDITOR
        private Vector3 debug_previousPosition;

        private void OnDrawGizmos()
        {
            if(UnityEditor.EditorApplication.isPlaying)
            {
                var radius = 0.05f;
                var root = transform.position;
                var top = root + Vector3.up * Settings.Height;
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

                //Prevent slope movement debug.
                return;

                //Movement prediction
                if(m_rigidbody.velocity.magnitude == 0)
                    return;

                var start = transform.position + Vector3.up * Settings.Radius;
                var direction = m_rigidbody.velocity.normalized;
                var fwd = start + (direction * Settings.Radius);
                var dw1 = start + (direction * Settings.Radius * 0.5f);
                var dw2 = (start + (direction * (Settings.Radius * 1)));
                Gizmos.color = Color.blue;
                var hit = new RaycastHit();
                var c = Vector3.zero;
                var ca = 0f;
                if(Physics.Raycast(start, Vector3.down, out hit, 0.5f))
                {
                    c = hit.point;
                    ca = Vector3.Angle(Vector3.up, hit.normal);
                    Gizmos.DrawWireSphere(c, radius);
                    Gizmos.DrawLine(transform.position, c);

                    /*
                     * Rest of the raycasts if controller close to ground.
                     */

                    var n = Vector3.zero;
                    var f = Vector3.zero;
                    var na = 0f;
                    var fa = 0f;

                    if(Physics.Raycast(dw1, Vector3.down, out hit))
                    {
                        n = hit.point;
                        na = Vector3.Angle(Vector3.up, hit.normal);
                        Gizmos.DrawLine(c, n);
                        Gizmos.DrawWireSphere(n, radius);
                        Gizmos.DrawLine(transform.position, n);
                    }

                    Gizmos.color = Color.cyan;
                    if(Physics.Raycast(dw2, Vector3.down, out hit))
                    {
                        f = hit.point;
                        fa = Vector3.Angle(Vector3.up, hit.normal);
                        Gizmos.DrawLine(n, f);
                        Gizmos.DrawWireSphere(f, radius);
                        Gizmos.DrawLine(transform.position, f);
                    }

                    var gd = Mathf.Round(Vector3.Distance(transform.position, c) * 100f) / 100f;
                    UnityEditor.Handles.Label(n - (n - f), "Yd: " + (n.y - f.y) + "\nSA: " + fa);
                    UnityEditor.Handles.Label(c - (c - n), "Yd: " + (c.y - n.y) + "\nSA: " + na);
                    UnityEditor.Handles.Label(c, "SA: " + ca + "\nd: " + gd);
                }
            }
        }

#endif

        /*
         * Public Functions.
         */

        /// <summary>
        /// Move Controllers Rigidbody component with given horizontal and vertical input.
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
            MouseMove(new Vector2(_horizontal, _vertical));
        }

        /// <summary>
        /// Makes charachter crouch down if allowed.
        /// <see cref="CanCrouch"/>
        /// </summary>
        public void CrouchDown()
        {
            if(CanCrouch)
            {
                ChangeHeight(Settings.Height / 2);
                m_targetSpeed = Settings.WalkSpeed * Settings.CrouchMultiplier;
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
                ChangeHeight(Settings.Height);
                m_targetSpeed = Settings.WalkSpeed;
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
                m_targetSpeed = Settings.WalkSpeed * Settings.RunMultiplier;
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
                m_targetSpeed = Settings.WalkSpeed;
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
                m_rigidbody.AddForce(0, Settings.JumpForce, 0, ForceMode.VelocityChange);
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
            if(Vector3.Distance(transform.position, m_camera.transform.position) > Settings.Height * 2)
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

            //Store rigidbodys current velocity.
            var velocity = m_rigidbody.velocity;
            //Calculate change between current and target velocity.
            var change = force - velocity;
            //Y velocity should be controller by jump and gravity.
            change.y = 0;
            //Add missing force to rigidbody.
            m_rigidbody.AddForce(change, ForceMode.Impulse);

            //Clamp velocity to limit max speed.
            ClampVelocity();
            //Update Capsule Colliders friction to allow/prevent sliding along surfaces.
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
        /// Input is required to prevent high friction during movement.
        /// </summary>
        /// <param name="_input">Input for movement.</param>
        private void StickToSlope(Vector3 _input)
        {
            if(!Grounded)
            {
                ZeroFriction();
                return;
            }

            if(SurfaceAngle <= Settings.MaxSlopeAngle)
            {
                if(_input == Vector3.zero)
                {
                    MaxFriction(20f);
                    return;
                }
            }
            ZeroFriction();
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
        /// Changes Controllers height to given height.
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

        /// <summary>
        /// First person controller settings.
        /// </summary>
        public FirsPersonPreset Settings
        {
            get
            {
                return m_settings ?? (m_settings = ScriptableObject.CreateInstance<FirsPersonPreset>());
            }

            set
            {
                m_settings = value;
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
        /// Is the charachter currently standing on ground.
        /// </summary>
        private bool Grounded
        {
            get
            {
                var hit = new RaycastHit();
                var start = new Vector3(transform.position.x, transform.position.y + (Settings.Radius * 1.1f), transform.position.z);
                return Physics.SphereCast(start, Settings.Radius, Vector3.down, out hit, 0.05f);
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
                var start = new Vector3(transform.position.x, transform.position.y + (Settings.Radius * 1.1f), transform.position.z);
                if(Physics.SphereCast(start, Settings.Radius, Vector3.down, out hit, 0.05f))
                {
                    angle = Vector3.Angle(Vector3.up, hit.normal);
                }
                return Mathf.Round(angle);
            }
        }
    }
}