using Rewired;
using UnityEngine;

namespace WibertStudio
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;

        // Rewired
        private int playerID = 0;
        

        #region Variables
        #region Base paramenters
        [Header("Base Paramenters")]
        [Tooltip("Gravity scale that will affect the player when reentering the grounded state")]
        [SerializeField] private float baseGravityScale = 1;
        [SerializeField] private float fallGravityScale = 3;
        [Tooltip("The max downward velocity that the player can be traveling at")]
        [Range(-100, 0)]
        [SerializeField] private float maxFallSpeed = -50f;
        [Tooltip("If true will flip sprite to face the movement direction")]
        private float initialFallSpeed;
        private bool canAttack = true;

        #endregion
        #region Collision check variables
        [Header("Ground Check Variables")]
        // Ground check variables
        [SerializeField] private LayerMask groundCheckLayerMask;
        [SerializeField] private Vector2 leftGroundCheckOffSet;
        [SerializeField] private Vector2 middleGroundCheckOffset;
        [SerializeField] private Vector2 rightGroundCheckOffset;
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
            if (Physics2D.Raycast(new Vector2(transform.position.x + middleGroundCheckOffset.x, transform.position.y + middleGroundCheckOffset.y), -transform.up, groundCheckDistance, groundCheckLayerMask))
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
        [Space(5)]

        [Header("Ceiling Check Variables")]
        // Ceiling check variablies
        [SerializeField] private LayerMask ceilingCheckLayerMask;
        [SerializeField] private Vector2 leftCeilingCheckOffSet;
        [SerializeField] private Vector2 middleCeilingCheckOffSet;
        [SerializeField] private Vector2 rightCeilingCheckOffSet;
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


        [Header("Wall Check Variables")]
        [SerializeField] private LayerMask wallCheckLayerMask;
        [Space(5)]

        // Left wall check variables
        [SerializeField] private Vector2 topLeftWallCheckOffSet;
        [SerializeField] private Vector2 bottomLeftWallCheckOffSet;
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
        [Space(5)]

        // Right wall check variables
        [SerializeField] private Vector2 topRightWallCheckOffSet;
        [SerializeField] private Vector2 bottomRightWallCheckOffSet;
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

        #region Getters/Setters
        #region References
        public Rigidbody2D Rb { get; private set; }
        public Player Player { get; private set; }
        public PlayerAnimator PlayerAnimator { get; set; }
        public PlayerMove PlayerMove { get; private set; }
        public PlayerJump PlayerJump { get; private set; }
        public PlayerWallSlideState PlayerWallSlide { get; private set; }
        public PlayerDash PlayerDash { get; private set; }
        #endregion
        #region Base parameters
        public float BaseGravityScale { get { return baseGravityScale; } }
        public float FallGravityScale { get { return fallGravityScale; } }
        public float MaxFallSpeed { get { return maxFallSpeed; } }
        public bool DoesPlayerHaveControl { get; set; } = true;
        public bool IsFacingRight { get; set; }
        public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
        public bool CanFlipSprite { get; set; } = true;
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

        private bool isGroundAttributesSet;
        private bool isAirAttributesSet;

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
            PlayerWallSlide = GetComponent<PlayerWallSlideState>();
            PlayerDash = GetComponent<PlayerDash>();
            Player = ReInput.players.GetPlayer(playerID);
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
        private void CheckVariablesBasedOnGroundedState()
        {
            if (IsGrounded && !isGroundAttributesSet)
                SetGroundAttributes();
            else if (!IsGrounded && !isAirAttributesSet)
                SetAirAttributes();
        }
        private void FixedUpdate()
        {
            CheckYVelocity();
        }

      

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

        private void SetGroundAttributes()
        {
            isAirAttributesSet = false;
            isGroundAttributesSet = true;

            PlayerMove.SetGroundValues();
            PlayerJump.ResetJumpAttributes();
            PlayerJump.CoyoteJump = true;
            PlayerAnimator.StopCoroutine("HardLandingCoroutine");
            PlayerAnimator.CheckLandAnimation();
            SetGravity(baseGravityScale);
            PlayerWallSlide.ResetWallSlideAttributes();
            print("set ground attributes");
        }

        private void SetAirAttributes()
        {
            isGroundAttributesSet = false;
            isAirAttributesSet = true;

            PlayerMove.SetAirValues();
            PlayerJump.IsJumpBufferActive = false;
            PlayerAnimator.StartCoroutine("HardLandingCoroutine");
            PlayerWallSlide.StartCoroutine("WallSlideCountDown");
            print("set air attributes");
        }

        public void SetGravity(float amt)
        {
            Rb.gravityScale = amt;
            print(amt);
        }

        private void FallVelocityClamp()
        {
            if (Rb.velocity.y < MaxFallSpeed)
                Rb.velocity = new Vector2(Rb.velocity.x, MaxFallSpeed);
        }
        private void CheckYVelocity()
        {
            if (Rb.velocity.y < -1 || !PlayerJump.IsJumpPressed && PlayerJump.HasJumped && PlayerJump.ApplyForceOnJumpRelease && PlayerJump.HasApexModifier || !PlayerJump.IsJumpPressed && PlayerJump.HasJumped && PlayerJump.ApplyForceOnJumpRelease && PlayerJump.IsApexModifierComplete)
                SetGravity(fallGravityScale);
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
                Gizmos.DrawRay(new Vector2(transform.position.x + middleGroundCheckOffset.x, transform.position.y + middleGroundCheckOffset.y), -transform.up * groundCheckDistance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Vector2(transform.position.x + middleGroundCheckOffset.x, transform.position.y + middleGroundCheckOffset.y), -transform.up * groundCheckDistance);
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
}
