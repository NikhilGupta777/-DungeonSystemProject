using System.Collections;
using System.Collections.Generic;
using AnderSystems.GoodMotion.cam;
using AnderSystems.GoodMotion;
using GameSettings;
using AnderSystems;
using UnityEngine;

public class GM_Movement : MonoBehaviour
{
    public bool IsFreezed { get; set; }
    public bool Controls { get; set; } = true;
    public static GM_Movement LocalPlayer;
    public GameConfigurations MainSettings;
    public Rigidbody rb { get; set; }
    [SerializeField]
    public PlayerPhysics.physics playerPhysics;

    [SerializeField]
    public Movement.movement playerMovement;

    [SerializeField]
    public Animations.AnimationsSetup animationSetup;
    
    [SerializeField]
    public GameplayCam.Params GameplayCamParams;

    [SerializeField]
    public Polishing.SoundsParticles SoundsAndParticles;

    //Main Player Controls
    float OrbitResetTime;
    void ControlOrbit()
    {
        if (GameplayCam.GetGameplayCamState() != GameplayCam.GameplayCamState.Custom)
        {
            if (playerMovement.OnRun)
            {
                GameplayCam.SetGameplayCamState(GameplayCam.GameplayCamState.Run);
            }
            else
            {
                if (playerMovement.OnSprint)
                {
                    GameplayCam.SetGameplayCamState(GameplayCam.GameplayCamState.Sprint);
                }
                else
                {
                    GameplayCam.SetGameplayCamState(GameplayCam.GameplayCamState.Idle);
                }
            }
        } else
        {
            if(GameplayCam.GetGameplayCamState() == GameplayCam.GameplayCamState.Default)
            {
                GameplayCam.SetGameplayCamState(GameplayCam.GameplayCamState.Idle);
            }
        }

        if (Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.CameraMoveX) != 0 ||
            Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.CameraMoveY) != 0 ||
            playerMovement.OnAim)
        {
            GameplayCam.SetOrbitAxisMouse(Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.CameraMoveX),
                -Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.CameraMoveY), GameplayCamParams.MaxY,
                GameplayCamParams.MinY);
            OrbitResetTime = 0;
        } else
        {
            if (OrbitResetTime >= 5)
            {
                Vector3 pos = (GameplayCam.GetOrbit().position + (transform.forward)) - (Utility.GetTransformVelocity(this.transform) / 2f);
                GameplayCam.SetOrbitLookAt(pos, 0.01f * (Movement.GetPlayerMovInputs().magnitude + (rb.velocity.magnitude / 5)));
            } else
            {
                OrbitResetTime += 0.1f;
            }
        }
    }
    private float RunValue;
    void Run()
    {
        playerMovement.OnWalk = Inputs.GetButton(GameSettings.GameConfigurations.MainSettings.Controls.WalkHorizontal) ||
            Inputs.GetButton(GameSettings.GameConfigurations.MainSettings.Controls.WalkVertical);

        if (Inputs.GetButtonUp(GameConfigurations.MainSettings.Controls.Run) && playerMovement.OnWalk)
        {
            playerMovement.OnSprint = !playerMovement.OnSprint;
        }

        if (Inputs.GetButton(GameConfigurations.MainSettings.Controls.Run) && playerMovement.OnWalk &&
            Mathf.Abs(GroundAngle) <= 20)
        {
            RunValue += 0.1f;
        } else
        {
            RunValue = 0;
            playerMovement.OnRun = false;
        }

        if (RunValue >= 1)
        {
            playerMovement.OnRun = Inputs.GetButton(GameConfigurations.MainSettings.Controls.Run);
        }
    }
    void Move()
    {
        Vector3 move = new Vector3 (0, 0,Mathf.Abs(Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkVertical)) +
            Mathf.Abs(Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkHorizontal))).normalized;
        if (move != Vector3.zero)
        {
            if (!playerMovement.UseRootMotion)
            {
                Movement.MoveEntityTo(this.transform, move + transform.position, playerMovement.MoveSpeed, 0, Space.Self);
            }
            if (!playerMovement.OnAim)
            {
                Movement.EntityLookAtRelative(this.transform, Movement.GetPlayerMovInputsRelative(this.transform), 3, GameplayCam.GetOrbit(), -90);
            }
        }

        if (playerMovement.OnAim)
        {
            Movement.EntityLookAtRelative(this.transform, transform.position + Vector3.right, 8, GameplayCam.GetOrbit(), -90);
            GameplayCam.SetGameplayCamParams(GameplayCamParams.AimCam, 8);
        } else
        {
            if (GameplayCam.GetGameplayCamState() == GameplayCam.GameplayCamState.Custom)
            {
                GameplayCam.SetGameplayCamState(GameplayCam.GameplayCamState.Default);
            }
        }
    }
    void ControlPhysics()
    {
        PlayerPhysics.PlacePlayerOnGround(this.transform, playerPhysics, true);
    }

    public float GroundAngle { get; set; }
    float DirectionWeight;
    float Direction;
    bool OnWallCollision;
    void Animate()
    {
        GroundAngle = Mathf.Lerp(GroundAngle, Mathf.Clamp((Utility.GetGroundAngle(this.transform, (transform.forward / 1.5f), playerPhysics.GroundLayers,
            playerPhysics.DetectionDistance * 2f)) *
            Mathf.Abs(Movement.GetPlayerMovInputs().normalized.magnitude), -90,90),
            3 * Time.deltaTime);


        if (Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkVertical) != 0 ||
            Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkHorizontal) != 0)
        {
            if (!playerMovement.OnAim)
            {
                Direction = Mathf.Lerp(Direction,
                    Mathf.DeltaAngle(this.transform.eulerAngles.y,
                    Movement.GetEntityLookAtRelative(this.transform, Movement.GetPlayerMovInputsRelative(this.transform), GameplayCam.GetOrbit(), -90).eulerAngles.y),
                    8 * Time.deltaTime);
            } else
            {
                if(Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkVertical) > 0 ||
                    Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkHorizontal) != 0)
                {
                    Direction = Mathf.DeltaAngle(this.transform.eulerAngles.y,
                    Movement.GetEntityLookAtRelative(this.transform, Movement.GetPlayerMovInputsRelative(this.transform), GameplayCam.GetOrbit(), -90).eulerAngles.y);
                } else
                {
                    Direction = Mathf.LerpAngle(Direction, 180, 8 * Time.deltaTime);
                }
            }
        } else
        {
            Direction = Mathf.Lerp(Direction, 0, 8 * Time.deltaTime);
        }


        Animations.BaseAnimations(animationSetup, PlayerPhysics.DetectGround(this.transform, playerPhysics),
            playerMovement.OnWalk, playerMovement.OnSprint, playerMovement.OnRun, Direction, GroundAngle, rb.velocity.y, playerMovement.OnAim);
    }
    void StartPolishing()
    {
        Polishing.ExecuteParticles(SoundsAndParticles, playerMovement.OnRun, playerPhysics.GroundLayers);

        Animations.ExtraWallAnimation(animationSetup, Animations.GetWallParam(this.transform, Contact), OnWallCollision);
    }

    Vector3 LastPos;
    void FallOfWorld()
    {
        if (this.transform.position.y <= -1000)
        {
            this.transform.position = LastPos;
        }
        else
        {
            if (PlayerPhysics.DetectGround(this.transform, playerPhysics))
            {
                LastPos = PlayerPhysics.DetectGroundRay(this.transform, playerPhysics).point;
            }
        }
    }


    //Call voids
    void Awake()
    {
        LocalPlayer = this;
        GameConfigurations.MainSettings = MainSettings;
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        InvokeRepeating("FallOfWorld", 1, 1);
    }
    void Update()
    {
        if (IsFreezed)
            return;
        if (Controls)
        {
            Run();
            playerMovement.OnAim = Inputs.GetButton(GameConfigurations.MainSettings.Controls.Aim);
        }

    }
    void FixedUpdate()
    {
        GameplayCam.Execute(GameplayCamParams);
        ControlOrbit();
        if (!IsFreezed)
        {
            ControlPhysics();
            Animate();
            if (Controls)
            {
                Move();
            }
        }
        //Movement.EntityLookAtRelative(this.transform, Movement.GetPlayerMovInputs(), playerMovement.ThurnSpeed, CameraManagment.ActiveCam.Orbit, -90);
    }
    void LateUpdate()
    {
        StartPolishing();
    }
    void OnAnimatorIK()
    {
        if (!animationSetup.IK.UseIK)
            return;
        if (PlayerPhysics.DetectGround(this.transform, playerPhysics))
        {
            Animations.ExecuteInverseKinematics(animationSetup);
        }
    }

    ContactPoint Contact;
    private void OnCollisionStay(Collision collision)
    {
        OnWallCollision = true;
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            Contact = collision.contacts[i];
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        OnWallCollision = false;
    }
}
