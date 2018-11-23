using UnityEngine;

namespace FPController
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class FirstPersonController : MonoBehaviour
    {
        /*
         * Variables.
         */

        private CapsuleCollider m_collider;
        private PhysicMaterial m_physicMaterial;
        private Rigidbody m_rigidbody;
        private Camera m_camera;
        private Vector3 m_cameraRotation;
        private Vector3 m_previousForce;
        private Vector3 m_lastGround;
        private Vector3 m_targetPosition;
        private float m_targetSpeed = 5;
        private float m_walkSpeed = 5;
        private float m_runMultiplier = 1.3f;
        private float m_crouchMultiplier = 0.6f;
        private float m_lookSpeed = 2;
        private float m_jumpForce = 50;
        private float m_radius = 0.35f;
        private float m_height = 1.75f;
        private float m_maxSlopeAngle = 45f;
        private bool m_running = false;
        private bool m_crouching = false;

        /*
         * MonoBehaviour Functions.
         */

        private void Awake()
        {
            Reset();
            m_camera.transform.parent = null;
            m_cameraRotation = m_camera.transform.eulerAngles;
        }

        private void Update()
        {
            UpdateCamera();
            MouseMove(MouseHorizontal, MouseVertical);
            UpdateInput();
        }

        private void FixedUpdate()
        {
            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        private void OnDisable()
        {
            if(m_camera != null)
            {
                m_camera.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if(m_camera != null)
            {
                m_camera.gameObject.SetActive(true);
                SetCamera();
            }
        }

        private void Reset()
        {
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
            m_camera.name = string.Format("{0} {1}", this.name, m_camera.name);
        }

        /*
         * Public Functions.
         */

        public void Move(float _horizontal, float _vertical)
        {
            Move(new Vector3(_horizontal * m_targetSpeed, 0, _vertical * m_targetSpeed));
        }

        public void MouseMove(float _horizontal, float _vertical)
        {
            MouseMove(new Vector2(_horizontal * m_lookSpeed, _vertical * m_lookSpeed));
        }

        public void CrouchDown()
        {
            if(CanCrouch)
            {
                ChangeHeight(m_height / 2);
                m_targetSpeed = m_walkSpeed * m_crouchMultiplier;
                m_crouching = true;
            }
        }

        public void CrouchUp()
        {
            if(m_crouching)
            {
                ChangeHeight(m_height);
                m_targetSpeed = m_walkSpeed;
                m_crouching = false;
            }
        }

        public void StartRunning()
        {
            if(CanRun)
            {
                m_targetSpeed = m_walkSpeed * m_runMultiplier;
                m_running = true;
            }
        }

        public void StopRunning()
        {
            if(m_running)
            {
                m_running = false;
                m_targetSpeed = m_walkSpeed;
            }
        }

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

        private void UpdateCamera()
        {
            if(Vector3.Distance(transform.position, m_camera.transform.position) > m_height * 2)
            {
                SetCamera();
            }

            var velocity = m_rigidbody.velocity * 0.2f;
            var target = m_targetPosition + velocity;
            m_camera.transform.position = Vector3.Lerp(m_camera.transform.position, target, 5 * Time.deltaTime);
        }

        private void SetCamera()
        {
            m_camera.transform.position = CameraOffset;
        }

        private void Move(Vector3 _input)
        {
            var force = m_previousForce;

            if(Grounded)
            {
                m_lastGround = m_previousForce;
                force = transform.TransformDirection(_input);
            }
            else
            {
                var temp = new Vector3
                {
                    x = Mathf.Clamp(force.x + (_input.x / 2), -m_targetSpeed, m_targetSpeed),
                    z = m_lastGround.z > 0
                        ? Mathf.Clamp(force.z + (_input.z / 2), 0, m_targetSpeed)
                        : Mathf.Clamp(force.z + (_input.z / 2), -m_targetSpeed, 0)
                };

                force = m_lastGround == Vector3.zero ? _input : temp;
                if(_input.x == 0) force.x = Mathf.Lerp(force.x, 0, 2 * Time.deltaTime);
                if(_input.z == 0) force.z = Mathf.Lerp(force.z, 0, 2 * Time.deltaTime);
                force = transform.TransformDirection(force);
            }

            var velocity = m_rigidbody.velocity;
            var change = force - velocity;
            change.y = 0;
            m_rigidbody.AddForce(change, ForceMode.Impulse);

            ClampVelocity();
            StickToSlope(_input);

            m_previousForce = transform.InverseTransformDirection(force);
            m_targetPosition = CameraOffset;
        }

        private void ClampVelocity()
        {
            var y = m_rigidbody.velocity.y;
            var velocity = m_rigidbody.velocity;
            velocity.y = 0;
            velocity = Vector3.ClampMagnitude(velocity, m_targetSpeed);
            velocity.y = y;
            m_rigidbody.velocity = velocity;
        }

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

        private void MouseMove(Vector2 _input)
        {
            if(Cursor.lockState != CursorLockMode.Locked)
                return;

            transform.Rotate(new Vector3(0, _input.x, 0));
            m_cameraRotation.x = Mathf.Clamp(m_cameraRotation.x + -_input.y, -90, 90);
            m_cameraRotation = new Vector3(m_cameraRotation.x, transform.rotation.eulerAngles.y, transform.eulerAngles.z);
            m_camera.transform.eulerAngles = m_cameraRotation;
        }

        private void UpdateInput()
        {
            var crouch = KeyCode.LeftControl;
            var run = KeyCode.LeftShift;

            if(Input.GetButtonDown("Jump"))
            {
                Jump();
            }

            if(Input.GetKeyDown(run))
            {
                StartRunning();
            }
            else if(Input.GetKeyUp(run))
            {
                StopRunning();
            }

            if(Input.GetKeyDown(crouch))
            {
                CrouchDown();
            }
            else if(Input.GetKeyUp(crouch))
            {
                CrouchUp();
            }
        }

        private void ChangeHeight(float _height)
        {
            m_collider.height = _height;
            m_collider.center = Vector3.up * (m_collider.height / 2);
        }

        private void ChangeFriction(float _ammount, PhysicMaterialCombine _combine)
        {
            m_physicMaterial.frictionCombine = _combine;
            m_physicMaterial.staticFriction = _ammount;
        }

        /*
         * Accessors.
         */

        private Vector3 CameraOffset
        {
            get
            {
                return transform.position + (Vector3.up * (m_collider.height - 0.1f));
            }
        }

        private float MouseHorizontal
        {
            get
            {
                return Input.GetAxis("Mouse X") * m_lookSpeed;
            }
        }

        private float MouseVertical
        {
            get
            {
                return Input.GetAxis("Mouse Y") * m_lookSpeed;
            }
        }

        private bool Grounded
        {
            get
            {
                var hit = new RaycastHit();
                var start = new Vector3(transform.position.x, transform.position.y + (m_radius * 1.1f), transform.position.z);
                return Physics.SphereCast(start, m_radius, Vector3.down, out hit, 0.05f);
            }
        }

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

        private bool CanRun
        {
            get
            {
                return (!m_crouching);
            }
        }

        public bool CanCrouch
        {
            get
            {
                return (!m_running);
            }
        }
    }
}