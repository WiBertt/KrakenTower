using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using WibertStudio;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlayerMove))]
    [RequireComponent(typeof(PlayerJump))]
    [RequireComponent(typeof(PlayerDash))]
    [RequireComponent(typeof(PlayerWallSlide_WallJump))]
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;

        #region Variables
        #region Base paramenters
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("Gravity that is active while grounded and ascending")]
        public float BaseGravityScale = 1;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("Gravity that is active while descending")]
        public float FallGravityScale = 3;
        [BoxGroup("Base Parameters")]
        [PropertyTooltip("The max speed that the player can fall")]
        public float MaxFallSpeed = -50f;
        private float initialFallSpeed;
        #endregion
       
        #region Collision check variables
        #region Ground check
        // Ground check variables
        [TabGroup("Ground Check")]
        [PropertyTooltip("Layers to consider as ground")]
        [SerializeField] private LayerMask groundCheckLayerMask;
        [TabGroup("Ground Check")]
        [PropertyTooltip("Offset for left ray cast")]
        [SerializeField] private Vector2 leftGroundCheckOffSet;
        [TabGroup("Ground Check")]
        [PropertyTooltip("Offset for middle ray cast")]
        [SerializeField] private Vector2 middleGroundCheckOffSet;
        [TabGroup("Ground Check")]
        [PropertyTooltip("Offset for right ray cast")]
        [SerializeField] private Vector2 rightGroundCheckOffset;
        [TabGroup("Ground Check")]
        [PropertyTooltip("How far the ray is cast")]
        [SerializeField] private float groundCheckDistance;

        private bool isOnLeftGround()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + leftGroundCheckOffSet.x, transform.position.y + leftGroundCheckOffSet.y), -transform.up, groundCheckDistance, groundCheckLayerMask))
                return true;
            else
                return false;
        }

        private bool isOnMiddleGround()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + middleGroundCheckOffSet.x, transform.position.y + middleGroundCheckOffSet.y), -transform.up, groundCheckDistance, groundCheckLayerMask))
                return true;
            else
                return false;
        }

        private bool isOnRightGround()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + rightGroundCheckOffset.x, transform.position.y + rightGroundCheckOffset.y), -transform.up, groundCheckDistance, groundCheckLayerMask))
                return true;
            else
                return false;
        }

        private bool isGrounded()
        {
            if (isOnLeftGround() || isOnMiddleGround() || isOnRightGround())
                return true;
            else
                return false;
        }
        #endregion
        #region Ceiling check
        // Ceiling check variablies
        [TabGroup("Ceiling Check")]
        [PropertyTooltip("Layers to consider as ceiling")]
        [SerializeField] private LayerMask ceilingCheckLayerMask;
        [TabGroup("Ceiling Check")]
        [PropertyTooltip("Offset for left ray cast")]
        [SerializeField] private Vector2 leftCeilingCheckOffSet;
        [TabGroup("Ceiling Check")]
        [PropertyTooltip("Offset for middle ray cast")]
        [SerializeField] private Vector2 middleCeilingCheckOffSet;
        [TabGroup("Ceiling Check")]
        [PropertyTooltip("Offset for right ray cast")]
        [SerializeField] private Vector2 rightCeilingCheckOffSet;
        [TabGroup("Ceiling Check")]
        [PropertyTooltip("How far the ray is cast")]
        [SerializeField] private float ceilingCheckCastDistance;

        private bool isOnLeftCeiling()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + leftCeilingCheckOffSet.x, transform.position.y + leftCeilingCheckOffSet.y), transform.up, ceilingCheckCastDistance, ceilingCheckLayerMask))
                return true;
            else
                return false;
        }

        private bool isOnMiddleCeiling()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + middleCeilingCheckOffSet.x, transform.position.y + middleCeilingCheckOffSet.y), transform.up, ceilingCheckCastDistance, ceilingCheckLayerMask))
                return true;
            else
                return false;
        }

        private bool isOnRightCeiling()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + rightCeilingCheckOffSet.x, transform.position.y + rightCeilingCheckOffSet.y), transform.up, ceilingCheckCastDistance, ceilingCheckLayerMask))
                return true;
            else
                return false;
        }
        private bool isOnCeiling()
        {
            if (isOnLeftCeiling() || isOnMiddleCeiling() || isOnRightCeiling())
                return true;
            else
                return false;
        }
        #endregion
        #region Left wall Check
        [TabGroup("Right Wall Check")]
        [TabGroup("Left Wall Check")]
        [PropertyTooltip("Layers to consider as wall")]
        [SerializeField] private LayerMask wallCheckLayerMask;
        [Space(5)]

        // Left wall check variables
        [TabGroup("Left Wall Check")]
        [PropertyTooltip("Offset for top ray cast")]
        [SerializeField] private Vector2 topLeftWallCheckOffSet;
        [TabGroup("Left Wall Check")]
        [PropertyTooltip("Offset for bottom ray cast")]
        [SerializeField] private Vector2 bottomLeftWallCheckOffSet;
        [TabGroup("Left Wall Check")]
        [PropertyTooltip("How far the ray is cast")]
        [SerializeField] private float leftWallCheckCastDistance;

        private bool isOnTopLeftWall()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + topLeftWallCheckOffSet.x, transform.position.y + topLeftWallCheckOffSet.y), -transform.right, rightWallCheckCastDistance, wallCheckLayerMask))
                return true;
            else
                return false;
        }

        private bool isOnBottomLeftWall()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + bottomLeftWallCheckOffSet.x, transform.position.y + bottomLeftWallCheckOffSet.y), -transform.right, rightWallCheckCastDistance, wallCheckLayerMask))
                return true;
            else
                return false;
        }
        private bool isOnLeftWall()
        {
            if (isOnTopLeftWall() || isOnBottomLeftWall())
                return true;
            else
                return false;
        }
        #endregion
        #region Right wall check
        // Right wall check variables
        [TabGroup("Right Wall Check")]
        [SerializeField] private Vector2 topRightWallCheckOffSet;
        [TabGroup("Right Wall Check")]
        [SerializeField] private Vector2 bottomRightWallCheckOffSet;
        [TabGroup("Right Wall Check")]
        [SerializeField] private float rightWallCheckCastDistance;
        private bool isOnTopRightWall()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + topRightWallCheckOffSet.x, transform.position.y + topRightWallCheckOffSet.y), transform.right, rightWallCheckCastDistance, wallCheckLayerMask))
                return true;
            else
                return false;
        }
        private bool isOnBottomRightWall()
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + bottomRightWallCheckOffSet.x, transform.position.y + bottomRightWallCheckOffSet.y), transform.right, rightWallCheckCastDistance, wallCheckLayerMask))
                return true;
            else
                return false;
        }
        private bool isOnRightWall()
        {
            if (isOnTopRightWall() || isOnBottomRightWall())
                return true;
            else
                return false;
        }
        #endregion
        #endregion

        #region Logic variables and Debug variables
        // logic
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public bool CanAttack = true;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public bool CanFlipSprite = true;
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public bool DoesPlayerHaveControl = true;
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public bool IsFacingRight = true;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        private bool isGroundAttributesSet;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        private bool isAirAttributesSet;

        //debug
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public string HorizontalMoveSpeed;
        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public string VerticalMoveSpeed;
        #endregion
        #endregion

        #region Getters/Setters
        #region References
        public Rigidbody2D Rb { get; private set; }
        public Player Player { get; private set; }
        public PlayerAnimator PlayerAnimator { get; set; }
        public PlayerMove PlayerMove { get; private set; }
        public PlayerJump PlayerJump { get; private set; }
        public PlayerWallSlide_WallJump PlayerWallSlide { get; private set; }
        public PlayerDash PlayerDash { get; private set; }
        #endregion
        #region Collision checks
        public bool IsOnLeftGround { get { return isOnLeftGround(); } }
        public bool IsOnMiddleGround { get { return isOnMiddleGround(); } }
        public bool IsOnRightGround { get { return isOnRightGround(); } }
        public bool IsGrounded { get { return isGrounded(); } }
        public bool IsOnLeftCeiling { get { return isOnLeftCeiling(); } }
        public bool IsOnMiddleCeiling { get { return isOnMiddleCeiling(); } }
        public bool IsOnRightCeiling { get { return isOnRightCeiling(); } }
        public bool IsOnCeiling { get { return isOnCeiling(); } }
        public bool IsOnTopRightWall { get { return isOnTopRightWall(); } }
        public bool IsOnBottomRightWall { get { return isOnBottomRightWall(); } }
        public bool IsOnTopLeftWall { get { return isOnTopLeftWall(); } }
        public bool IsOnBottomLeftWall { get { return isOnBottomLeftWall(); } }
        public bool IsOnLeftWall { get { return isOnLeftWall(); } }
        public bool IsOnRightWall { get { return isOnRightWall(); } }
        #endregion
        #endregion      

        private void Awake()
        {
            instance = this;
            SetReferencesAndInitialValues();
        }

        private void SetReferencesAndInitialValues()
        {
            //References
            Rb = GetComponent<Rigidbody2D>();
            PlayerAnimator = GetComponent<PlayerAnimator>();
            PlayerMove = GetComponent<PlayerMove>();
            PlayerJump = GetComponent<PlayerJump>();
            PlayerWallSlide = GetComponent<PlayerWallSlide_WallJump>();
            PlayerDash = GetComponent<PlayerDash>();
            Player = ReInput.players.GetPlayer(0);
        }

        private void Start() { }

        private void Update()
        {
            Flip();
            CheckVariablesBasedOnGroundedState();
            FallVelocityClamp();         
        }

        private void Flip()
        {
            if (!DoesPlayerHaveControl)
                return;

            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                Player.GetAxis("Move Horizontal").Equals(0);
            float moveHorizontal = Player.GetAxis("Move Horizontal");

            if (CanFlipSprite)
            {
                if (moveHorizontal > .1f)
                {
                    IsFacingRight = true;
                    transform.localScale = Vector3.one;
                }
                else if (moveHorizontal < -.1f)
                {
                    IsFacingRight = false;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }

        // sets appropriate parameters based off grounded state
        private void CheckVariablesBasedOnGroundedState()
        {
            if (IsGrounded && !isGroundAttributesSet)
                SetGroundAttributes();
            else if (!IsGrounded && !isAirAttributesSet)
                SetAirAttributes();
        }

        // clamps the y velocity to not exceed a certain value
        private void FallVelocityClamp()
        {
            if (Rb.velocity.y < MaxFallSpeed)
                Rb.velocity = new Vector2(Rb.velocity.x, MaxFallSpeed);
        }

        private void SetGroundAttributes()
        {
            isAirAttributesSet = false;
            isGroundAttributesSet = true;

            PlayerMove.SetGroundValues();
            PlayerJump.ResetJumpAttributes();
            PlayerJump.CoyoteJump = true;
            PlayerAnimator.StopCoroutine("HardLandingCoroutine");
            PlayerAnimator.CheckLandAnimation();
            SetGravity(BaseGravityScale);
            PlayerWallSlide.ResetWallSlideAttributes();
        }

        private void SetAirAttributes()
        {
            isGroundAttributesSet = false;
            isAirAttributesSet = true;

            PlayerMove.SetAirValues();
            PlayerJump.IsJumpBufferActive = false;
            PlayerAnimator.StartCoroutine("HardLandingCoroutine");
            PlayerWallSlide.StartCoroutine("WallSlideCountDown");
        }

        private void FixedUpdate()
        {
            CheckYVelocity();
            GetVelocitys();
        }

        // checks the y velocity and changes gravity scale based off value
        private void CheckYVelocity()
        {
        if (PlayerJump.isInApexModifier)
            return;

            if (Rb.velocity.y < -1 || !PlayerJump.IsJumpPressed && PlayerJump.HasJumped && PlayerJump.ApplyForceOnJumpRelease && PlayerJump.HasApexModifier || !PlayerJump.IsJumpPressed && PlayerJump.HasJumped && PlayerJump.ApplyForceOnJumpRelease && PlayerJump.IsApexModifierComplete)
                SetGravity(FallGravityScale);
        }

        // gets x and y velocity values
        private void GetVelocitys()
        {
            HorizontalMoveSpeed = Mathf.Abs(Rb.velocity.x).ToString("F2");
            VerticalMoveSpeed = Mathf.Abs(Rb.velocity.y).ToString("F2");
        }

        /// <summary>
        /// manually flips the sprite in the opposite direction
        /// </summary>
        public void ManualFlip()
        {
            if (IsFacingRight)
            {
                IsFacingRight = false;
                transform.localScale = new Vector3(-1, 1, 1);
                return;
            }
            else
            {
                IsFacingRight = true;
                transform.localScale = Vector3.one;
            }

        }
        
        public void SetGravity(float amt)
        {
            Rb.gravityScale = amt;
        }
  
        private void OnDrawGizmos()
        {
            //Ground check
            if (!isOnLeftGround())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + leftGroundCheckOffSet.x, transform.position.y + leftGroundCheckOffSet.y), -transform.up * groundCheckDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + leftGroundCheckOffSet.x, transform.position.y + leftGroundCheckOffSet.y), -transform.up * groundCheckDistance);
            }

            if (!isOnMiddleGround())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + middleGroundCheckOffSet.x, transform.position.y + middleGroundCheckOffSet.y), -transform.up * groundCheckDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + middleGroundCheckOffSet.x, transform.position.y + middleGroundCheckOffSet.y), -transform.up * groundCheckDistance);
            }

            if (!isOnRightGround())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + rightGroundCheckOffset.x, transform.position.y + rightGroundCheckOffset.y), -transform.up * groundCheckDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + rightGroundCheckOffset.x, transform.position.y + rightGroundCheckOffset.y), -transform.up * groundCheckDistance);
            }



            //Ceiling check
            if (!isOnLeftCeiling())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + leftCeilingCheckOffSet.x, transform.position.y + leftCeilingCheckOffSet.y), transform.up * ceilingCheckCastDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + leftCeilingCheckOffSet.x, transform.position.y + leftCeilingCheckOffSet.y), transform.up * ceilingCheckCastDistance);
            }

            if (!isOnMiddleCeiling())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + middleCeilingCheckOffSet.x, transform.position.y + middleCeilingCheckOffSet.y), transform.up * ceilingCheckCastDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + middleCeilingCheckOffSet.x, transform.position.y + middleCeilingCheckOffSet.y), transform.up * ceilingCheckCastDistance);
            }

            if (!isOnRightCeiling())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + rightCeilingCheckOffSet.x, transform.position.y + rightCeilingCheckOffSet.y), transform.up * ceilingCheckCastDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + rightCeilingCheckOffSet.x, transform.position.y + rightCeilingCheckOffSet.y), transform.up * ceilingCheckCastDistance);
            }



            //Left wall check
            if (!isOnTopLeftWall())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + topLeftWallCheckOffSet.x, transform.position.y + topLeftWallCheckOffSet.y), -transform.right * leftWallCheckCastDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + topLeftWallCheckOffSet.x, transform.position.y + topLeftWallCheckOffSet.y), -transform.right * leftWallCheckCastDistance);
            }

            if (!isOnBottomLeftWall())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + bottomLeftWallCheckOffSet.x, transform.position.y + bottomLeftWallCheckOffSet.y), -transform.right * leftWallCheckCastDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + bottomLeftWallCheckOffSet.x, transform.position.y + bottomLeftWallCheckOffSet.y), -transform.right * leftWallCheckCastDistance);
            }

            // Top right wall check
            if (!isOnTopRightWall())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + topRightWallCheckOffSet.x, transform.position.y + topRightWallCheckOffSet.y), transform.right * rightWallCheckCastDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + topRightWallCheckOffSet.x, transform.position.y + topRightWallCheckOffSet.y), transform.right * rightWallCheckCastDistance);
            }

            // Bottom right wall check
            if (!isOnBottomRightWall())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Vector2(transform.position.x + bottomRightWallCheckOffSet.x, transform.position.y + bottomRightWallCheckOffSet.y), transform.right * rightWallCheckCastDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + bottomRightWallCheckOffSet.x, transform.position.y + bottomRightWallCheckOffSet.y), transform.right * rightWallCheckCastDistance);
            }
        }
    }
