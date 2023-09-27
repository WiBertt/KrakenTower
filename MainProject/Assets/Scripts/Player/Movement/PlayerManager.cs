using Com.LuisPedroFonseca.ProCamera2D;
using Rewired;
using UnityEngine;

namespace WibertStudio
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;
        PlayerBaseState currentState;

        // Possible states
        public PlayerGroundedState groundedState = new PlayerGroundedState();
        public PlayerInAirState inAirState = new PlayerInAirState();

        // Logic scripts
        private PlayerMove playerMove;
        private PlayerJump playerJump;

        // Rewired
        [SerializeField] private int playerID = 0;
        [SerializeField] private Player player;

        #region Variables
        #region References
        private Rigidbody2D rb;
        [SerializeField] private float cameraXOffset;
        [SerializeField] private float cameraYOffset;
        #endregion
        #region Base paramenters
        [Header("Base Paramenters")]
        [Tooltip("Gravity scale that will affect the player when reentering the grounded state")]
        [SerializeField] private float baseGravityScale = 1;
        [SerializeField] private float fallGravityScale = 3;
        [Tooltip("The max downward velocity that the player can be traveling at")]
        [Range(-100, 0)]
        [SerializeField] private float maxFallSpeed = -50f;
        [Tooltip("If true will flip sprite to face the movement direction")]
        [SerializeField] private bool canFlipSprite = true;
        private float initialFallSpeed;
        private float verticalInput;
        private bool canAttack = true;
        public enum direction
        {
            left,
            right,
        }

        private direction currentDirection;
        [Space()]
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
        public Rigidbody2D Rb { get { return rb; } }
        public PlayerAnimator PlayerAnimator { get; set; }
        public PlayerMove PlayerMove { get { return playerMove; } }
        public PlayerJump PlayerJump { get { return playerJump; } }
        public PlayerWallSlideState PlayerWallSlide { get; set; }
        public PlayerDash PlayerDash { get; set; }
        public float CameraXOffset { get { return cameraXOffset; } }
        public float CameraYOffset { get { return cameraYOffset; } }
        #endregion
        #region Base parameters
        public float BaseGravityScale { get { return baseGravityScale; } }
        public float FallGravityScale { get { return fallGravityScale; } }
        public float MaxFallSpeed { get { return maxFallSpeed; } }
        public bool DoesPlayerHaveControl { get; set; } = true;
        public bool IsFacingRight { get; set; }
        public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
        public bool CanFlipSprite { get { return canFlipSprite; } set { canFlipSprite = value; } }
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
        }
        private void Start()
        {
            SetReferencesAndInitialValues();

            //Sets starting state to avoid errors
            currentState = groundedState;
            currentState.EnterState(this);
        }

        private void SetReferencesAndInitialValues()
        {
            //References
            rb = GetComponent<Rigidbody2D>();
            PlayerAnimator = GetComponent<PlayerAnimator>();
            playerMove = GetComponent<PlayerMove>();
            playerJump = GetComponent<PlayerJump>();
            PlayerWallSlide = GetComponent<PlayerWallSlideState>();
            PlayerDash = GetComponent<PlayerDash>();
            player = ReInput.players.GetPlayer(playerID);
        }

        private void Update()
        {
            currentState.UpdateState();
            currentState.SwitchConditions();
            Flip();
            PlayerLook();
        }

        private void PlayerLook()
        {
            float horizontalInput = player.GetAxis("Move Horizontal");
            //if (!IsGrounded || horizontalInput != 0f)
            //{
            //    proCamera2D.OffsetY = 0;
            //    return;
            //}

            verticalInput = player.GetAxis("Look");

            //if (verticalInput > .8f)
            //  //  proCamera2D.OffsetY = cameraYOffset;
            //else if (verticalInput < -.8f)
            // //   proCamera2D.OffsetY = -cameraYOffset;
            //else if (verticalInput >= -.9f || verticalInput <= .9f)
            //   // proCamera2D.OffsetY = 0;
        }

        private void FixedUpdate()
        {
            currentState.FixedUpdateState();
        }

        private void Flip()
        {
            if (!DoesPlayerHaveControl)
                return;

            
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                player.GetAxis("Move Horizontal").Equals(0);
            float moveHorizontal = player.GetAxis("Move Horizontal");
            if (canFlipSprite)
            {
                if ( moveHorizontal> .1f)
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

        public void ManualFlip()
        {
            print("Here");
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

        public void SwitchState(PlayerBaseState state)
        {
            currentState.ExitState();
            currentState = state;
            currentState.EnterState(this);
        }

        public void SetGravity(string State)
        {
            switch (State)
            {
                case ("Base"):
                    break;
                case ("Falling"):
                    break;
            }
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
