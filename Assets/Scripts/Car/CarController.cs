using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum LaneChangeType //FreeLaneChange oyuncuya özgür bir şekilde şeriti değiştirebilmesini sağlıyor
    {
        StrictLaneChange,
        FreeLaneChange
    }
    private Animator animator;
    private string currentstate = "Idle";
    public LaneChangeType laneChangeType;
    #region NotShowenOnEditor
    [HideInInspector] public Rigidbody rigidBody;
    private float verticalright,currentbreakforce,currentsteerangle,driftfactor;
    private bool IsBreaking;
    private bool fixingangle = false;
    [HideInInspector]public bool IsChangingStrip;
    [HideInInspector]public bool IsCarTouching;
    [HideInInspector]public bool IsCarStoppedTouching;
    [HideInInspector]public bool CanUseNitro = false;
    [HideInInspector]public bool IsUsingNitro = false;
    [HideInInspector]public bool IsFinishedUsingNitro = false;
    [HideInInspector]public bool OnTouch;
    public int CarID;
    private bool IsCarLane;
    [HideInInspector]public float zCoordinate;
    private GameObject wheeltransforms;
    private GameObject wheelcolliders;
    private WheelCollider[] wheel;
    private GameObject [] wheeltransform;
    private WheelFrictionCurve forwardfriction,sidewaysfriction;
    [HideInInspector]public float horizontalright;
    [HideInInspector]public float maxspeed;
    [HideInInspector]public float minspeed;
    [HideInInspector]public float speed;
    [HideInInspector]public int GearLevel;
    public int lane;
    float timer;
    #endregion
    public float topspeed;
    public float[] gears;
    private float motorForce;
    public float StartMotorForce = 1000;
    [SerializeField]private float breakForce = 1000;
    [SerializeField]private float Grip;
    [SerializeField]public int MaxGearLevel = 4;
    private float maxsteerangle;
    private bool engineLerp;
    private float engineLerpValue;
    public GameManager gameManager;
    private Upgrades upgrades;
    [SerializeField]private CarAI[] carAIScript;
    [SerializeField]private InputManager inputManager;
    private string[] Upgrades;

    //Todo :
    //Yolları ayarla bir yarış yap.
    //3-2-1 başla ve oyun başlar
    //Kare siyah beyaz yarış çizgisi(bayrak)
    //Nitro sistemi
    //Partikal sistemi
    //Zorluk sistemi(sen nitroyu alana kadar rakiplerde olmayacak sonra olacak)
    //Kutucukları hareket ettir bizim yönümüze doğru
    //Yükseltmeler
    //Görsel olarak gözüken tek nitro.
    

    private void Start()
    {
        GetObjects();
    }
    private void GetObjects() 
    {
        rigidBody = GetComponent<Rigidbody>();
        wheelcolliders = transform.Find("WheelsCollider").gameObject;
        wheeltransforms = transform.Find("WheelsTransform").gameObject;
        upgrades = GetComponent<Upgrades>();
        wheel = new WheelCollider[4];
        wheeltransform = new GameObject[4];
        wheel[0] = wheelcolliders.transform.Find("0").gameObject.GetComponent<WheelCollider>();
        wheel[1] = wheelcolliders.transform.Find("1").gameObject.GetComponent<WheelCollider>();
        wheel[2] = wheelcolliders.transform.Find("2").gameObject.GetComponent<WheelCollider>();
        wheel[3] = wheelcolliders.transform.Find("3").gameObject.GetComponent<WheelCollider>();
        wheeltransform[0] = wheeltransforms.transform.Find("0").gameObject;
        wheeltransform[1] = wheeltransforms.transform.Find("1").gameObject;
        wheeltransform[2] = wheeltransforms.transform.Find("2").gameObject;
        wheeltransform[3] = wheeltransforms.transform.Find("3").gameObject;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(transform.tag == "Player") 
        {
            CanUseNitro = true;
        }
        gears[MaxGearLevel] = topspeed;
        if(laneChangeType == LaneChangeType.FreeLaneChange) 
        {
            maxsteerangle = 1;
        }
        motorForce = StartMotorForce;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {  
        if(gameManager.IsGameStarted) 
        {
            GetInput(verticalright,horizontalright);
            HandleMotor(); 
            HandleSteering();
            UpdateWheels();
            if(laneChangeType == LaneChangeType.StrictLaneChange) 
                HandleStripChange();
        }
        speed = CalculateCarSpeed();
        zCoordinate = CalculateZCoordinate();
        UsingNitro();
        if(transform.tag == "Player") 
        {
            DebugLog();
        }
        
    }
    private void ChangeAnimationState(string newstate)
    {
        if(currentstate == newstate) return;
        animator.Play(newstate);
        currentstate = newstate;
    }
    public void GetInput(float vertical,float horizontal) 
    {
        if(transform.tag == "Player") 
        {
            //Debug.Log(inputManager.UpPressed);
            //verticalright = Input.GetAxis("Vertical");
            if(inputManager.UpPressed) 
            {
                animator.SetBool("Gas",true);
                animator.SetBool("CanBeFixedGas", false);
                animator.SetBool("CanBeFixedBreak", false);
                if(verticalright < 1)
                {
                    verticalright += Time.deltaTime;
                }
            }
            else if(!inputManager.UpPressed && animator.GetBool("Break") != true)
            {
                animator.SetBool("Gas",false);
                animator.SetBool("CanBeFixedGas", true);
            }
            else if(!inputManager.UpPressed)
            {
                verticalright = 0;
            }
            if(inputManager.DownPressed) 
            {
                animator.SetBool("CanBeFixedGas", false);
                animator.SetBool("CanBeFixedBreak", false);
                animator.SetBool("Break",true);
                animator.SetBool("Gas",false);
                wheel[2].brakeTorque = breakForce;
                wheel[3].brakeTorque = breakForce;
            }
            else if(!inputManager.DownPressed && animator.GetBool("Gas") != true)
            {
                animator.SetBool("Break",false);
                animator.SetBool("Gas",false);
                animator.SetBool("CanBeFixedBreak", true);
            }
        }
        else
        {
            verticalright = vertical;
            if(verticalright > 1) 
            {
                animator.SetBool("Gas",true);
                animator.SetBool("Break",false);
            }
            else if(verticalright < 0) 
            {
                animator.SetBool("Gas",false);
                animator.SetBool("CanBeFixedGas",false);
                animator.SetBool("Break",false);
                
            }
            horizontalright = horizontal;
        }
        if(rigidBody.velocity.z <= 5 && verticalright < 0) 
        {
            verticalright = 0;
        }
        //Aracın geriye gitmesini engelliyor geriye doğru gittiğinde vertical değerini aşağı indiren her türlü kodu elimine ediyor.
        //Vitesi artırma kodu max vitesten fazlaysa vites atmıyor bu kodu yakında farklı bir yere yaz. 
        if(transform.tag == "Player") 
        {
            if(inputManager.GearUpgraded && GearLevel <= MaxGearLevel && !IsChangingStrip) 
            {
                inputManager.GearUpgraded = false;
                ChangeGearLevel(true);
                verticalright = 0;
                gameManager.ChangeGear();
            }
            if(inputManager.GearDowngraded && GearLevel > 0) 
            {
                inputManager.GearDowngraded = false;
                ChangeGearLevel(false);
                verticalright = 0;
                gameManager.ChangeGear();
            }
            if(laneChangeType == LaneChangeType.StrictLaneChange) 
            {
                if(inputManager.LeftPressed) 
                {
                    ChangeLane("left");
                }
                if(inputManager.RightPressed)
                {
                    ChangeLane("right");
                }
            }
            else if(laneChangeType == LaneChangeType.FreeLaneChange) 
            {
                //Addforce yerine velocityi hatırlattığın için saol
                //horizontalright = Input.GetAxis("Horizontal");
                //Debug.Log("Current State: " + currentstate);
                if(inputManager.RightPressed) 
                {
                    LockPosition(true);
                    animator.SetBool("Left",true);
                    animator.SetBool("Right",false);
                    animator.SetBool("CanBeFixedRight",false);
                    animator.SetBool("CanBeFixedLeft",false);
                    if(animator.GetCurrentAnimatorStateInfo(0).IsName("LeftIdle")) 
                    {
                        rigidBody.velocity = new Vector3(12,rigidBody.velocity.y,rigidBody.velocity.z);
                    } 
                    
                }
                else if( !inputManager.RightPressed && animator.GetBool("Right") != true) 
                {
                    animator.SetBool("CanBeFixedRight",true);
                    animator.SetBool("Right",false);
                    animator.SetBool("Left",false);
                }
               
                if(inputManager.LeftPressed) 
                {
                    //rigidBody.AddForce(-75000,0,0);
                    LockPosition(true);
                    animator.SetBool("Right",true);
                    animator.SetBool("Left",false);
                    animator.SetBool("CanBeFixedLeft",false);
                    animator.SetBool("CanBeFixedRight",false);
                    if(animator.GetCurrentAnimatorStateInfo(0).IsName("RightIdle")) 
                    {
                        rigidBody.velocity = new Vector3(-12,rigidBody.velocity.y,rigidBody.velocity.z);
                    }
                    
                }
                else if(!inputManager.LeftPressed && animator.GetBool("Left") != true) 
                {
                    animator.SetBool("CanBeFixedLeft",true);
                    animator.SetBool("Right",false);
                    animator.SetBool("Left",false);
                }
               
                if(!inputManager.LeftPressed && !inputManager.RightPressed) 
                {
                    LockPosition(false);
                }
            }
            if(Input.GetKeyDown(KeyCode.Z) && CanUseNitro) 
            {
                IsUsingNitro = true;
            }
        }
        if(speed < minspeed && GearLevel > 1 && transform.tag != "Player") 
        {
            ChangeGearLevel(false);
            if(transform.tag == "Player") 
            {
                gameManager.ChangeGear();
            }
        }
        //Vitesi aşağı çekiyor
        SteerFix();
    }
    private void SetEngineLerp(float num) 
    {
        engineLerp = true;
        engineLerpValue = num;
    }
    private void LerpEngine() 
    {
        if(engineLerp && !IsUsingNitro) 
        {
            //Debug.Log("ELV : " + engineLerpValue);
            motorForce = Mathf.Lerp(motorForce,0,8 *Time.deltaTime);
            engineLerp = speed <= engineLerpValue ? false : true;
            if(speed > maxspeed + 50) 
            {
                motorForce = 1;
            }
        }
    }
    public void ChangeLane(string side) 
    {
        if(side == "right" && lane < gameManager.MaxLane && !IsChangingStrip) 
        {
            lane++;
            IsChangingStrip = true;
        }
        else if(side == "left" && lane > 0 && !IsChangingStrip) 
        {
            lane--;
            IsChangingStrip = true;
        }
    }
    public void ChangeGearLevel(bool IsGearUpgraded) 
    {
        if(IsGearUpgraded) 
        {
            GearLevel++;
        }
        else 
        {
            GearLevel--;
        }
    }
    private void LaneCheck() 
    {
        if(transform.position.x > 0 && transform.position.x < 3.7f && !IsChangingStrip ) 
        {  
            lane = 0;
        }
        else if(transform.position.x > 3.7f && !IsChangingStrip) 
        {
            lane = 1;
        }

    }
    private void HandleStripChange() 
    {
        //Derler ki aslında bu yazdığın kodun yapay zekadan farkı yoktur, onlara de ki elbet o şerit değişitirmenin ve yapay zekanın ayrımını yapabilir, o yazanların en iyisi en mükemmelidir.
        //PS : Cidden nedense burayı yazmak en zorlandığım yerdi.
        //PS2 : Evet yapay zekayı buna benzer bir mantıkta yapabilirim ama onun yerine yapay zekanın sadece düz gitmesini sağlayacağım ve buraya belli şartlarda girmesini :D
        //Not : Boncuk teşekkürler.
        LaneCheck();
        Vector3 destinedposition = new Vector3(lane * 5.45f,transform.position.y,transform.position.z),gopositon = (destinedposition - transform.position).normalized;
        if(lane == 0) 
        {
            destinedposition = new Vector3(-0.74f,transform.position.y,transform.position.z);
        }
        float distancetotarget = Vector3.Distance(transform.position,destinedposition);
        float reachedtargetdistance =  1f;
        float angledir = Vector3.SignedAngle(transform.forward,gopositon,Vector3.up);
        if(transform.position.x != distancetotarget - reachedtargetdistance && IsCarStoppedTouching && transform.tag != "Player") 
        {
            if(lane == 0) 
            {
                lane++;
                IsChangingStrip = true;
                IsCarStoppedTouching = false;
            }
            else if(lane == 1) 
            {
                lane--;
                IsChangingStrip = true;
                IsCarStoppedTouching = false;
            }
        }
        if(!IsCarTouching) 
        {
            LockPosition(IsChangingStrip);
        }
        SteerFix();
        if(IsChangingStrip) 
        {
            timer += Time.deltaTime;
        }
        else 
        {
            timer = 0;
        }
        float maxrotate;
        maxrotate = MaxRotateFix();
        if(distancetotarget > reachedtargetdistance && IsChangingStrip) 
        {
            if(angledir > 0) 
            {
                if(transform.rotation.y < maxrotate) 
                {
                    //horizontalright = 0.6f;
                    if(horizontalright < 0.32f)
                        horizontalright += Time.deltaTime / 1.15f;
                    if(distancetotarget < 0.75f) 
                    {
                        horizontalright = 0;
                    }
                }
                else
                {
                    //Debug.Log("B2");
                    horizontalright = 0;
                }
            }
            else 
            {
                
                if(transform.rotation.y > -maxrotate) 
                {
                    //Debug.Log("c");
                    //horizontalright = -0.6f;
                    if(horizontalright > -0.32f)
                        horizontalright -= Time.deltaTime / 1.15f;
                }
                else 
                {
                    //Debug.Log("c2");
                    horizontalright = 0;
                }
            }
        }
        else
        {
            if(transform.rotation.y > 0.002 ) 
            {
                
                //Debug.Log("e");
                //horizontalright = -0.3f;
                if(horizontalright > -0.25f)
                    horizontalright -= Time.deltaTime / 0.5f;
            }
            else if(transform.rotation.y < -0.002) 
            {
                //Debug.Log("f");
                if(horizontalright < 0.25f)
                    horizontalright += Time.deltaTime / 0.5f;
                //horizontalright = 0.3f;
            }
            else if(transform.rotation.y == 0)
            {
                //Debug.Log("g");
                horizontalright = 0;
            }
           
            //Eulerangles 0 dan 360 ya kadar çalşır. Sola döndüğünde 359 dan başlar. Sağa döndüğünde 0 dan
            //Eğer değiştirirseniz yine test etmeyi unutmayın
            if(transform.eulerAngles.y >= 0 && transform.eulerAngles.y < 0.05) 
            {
                IsChangingStrip = false;
            }
        }
    }   
    private void SteerFix() 
    {
        //Maxsteerangle tekerliğin dönüşünü ayarlıyor eğer fazla olursa araba fazla kayıyor ve bu da yeniden tracke girmeyi denemesine neden oluyor.
        //Eğer bir nedenden dolayı burayı artırmak istersen maxrotationu da buraya göre ayarlaman gerekiyor. Eğer azaltırsan max rotationu artır artırırsan max rotationu azalt. Test etmeyi unutma farklı bir çözüm yok.
        if(speed > 0 && speed < 32 ) 
        {
            maxsteerangle = 22f;
        }
        else if(speed > 32 && speed < 45 ) 
        {
            maxsteerangle = 16.05f;
        }
        else if (speed > 45 && speed < 65) 
        {
            maxsteerangle = 14.95f;
        }
        else if(speed > 65 && speed < 100) 
        {
            maxsteerangle = 12.25f;
        }
        else if(speed > 100 && speed  < 110) 
        {
            maxsteerangle = 9.05f;
        }
        else if(speed > 110 && speed < 130) 
        {
            maxsteerangle = 8.35f;
        }
        else if(speed > 130 && speed < 150) 
        {
            maxsteerangle = 7.15f;
        }
        else if(speed > 150 && speed < 180) 
        {
            maxsteerangle = 6.90f;
        }
        else if(speed > 180 && speed < 220) 
        {
            maxsteerangle = 5.55f;
        }
        else if(speed > 220 && speed < 250) 
        {
            maxsteerangle = 4.25f;
        }
        else if(speed > 250 && speed < 280) 
        {
            maxsteerangle = 3.95f;
        }
        else 
        {
            maxsteerangle = 3.75f;
        }
        //maxsteerangle = 30;
    }
    private float MaxRotateFix() 
    {
        //Oyunda ki transform.rotation 0 dan 360 yerine yada 0 180 ve -180 yerine 0 ila 1 ve -1 ila 0 a gidiyor yani bunun matematiğiyle uğraşmadım ama düzgün çalışıyor denemelerimde
        //Rotation 0.0871558 = Oyun motorunda 10. Buradan hesaplama yapabilirsiniz
        float maxrotate;
        if(speed > 0 && speed < 32) 
        {
            maxrotate = 0.064558f;
        }
        else if(speed >= 32 && speed < 60) 
        {
            maxrotate = 0.03511f;
        }
        else if(speed >= 60 && speed < 90) 
        {
            maxrotate = 0.0321f;
        } 
        else if(speed >= 90 && speed < 130) 
        {
            maxrotate = 0.0271f;
        }
        else if(speed >= 130 && speed < 150) 
        {
            maxrotate = 0.0257f;
        }
        else if(speed >= 150 && speed < 180) 
        {
            maxrotate = 0.0235f;
        }
        else if(speed >= 180 && speed < 220) 
        {
            maxrotate = 0.0215f;
        }
        else if(speed >= 220 && speed < 270)
        {
            maxrotate = 0.0195f;
        }
        else if(speed >= 270 && speed < 320) 
        {
            maxrotate = 0.01755f;
        }
        else if(speed >= 320 && speed < 370) 
        {
            maxrotate = 0.0155f;
        }
        else if(speed >= 370 && speed < 400) 
        {
            maxrotate = 0.0125f;
        }
        else if(speed >= 400 && speed < 450) 
        {
            maxrotate = 0.0115f;
        }
        else if(speed >= 450 && speed < 500) 
        {
            maxrotate = 0.0095f;
        }
        else 
        {
            maxrotate = 0.0075f;
        }
        return maxrotate;
    }
    public void LockPosition(bool IsLocking) 
    {
        //Eğer şerit değiştirmiyorsa yana doğru hareketi engelliyor
        //Ve rotasyonu engelliyor.
        if(!IsLocking) 
        {
            rigidBody.constraints = RigidbodyConstraints.FreezePositionX;
        }
        else 
        {
            rigidBody.constraints = RigidbodyConstraints.None;
        }
    }
    public void LockRotation(bool IsLocking) 
    {
        if(!IsLocking ) 
        {
            rigidBody.freezeRotation = true;
        }
        else 
        {
            rigidBody.freezeRotation = false;
        }
    }
    private void HandleSteering() 
    {
        currentsteerangle = maxsteerangle * horizontalright;
        wheel[0].steerAngle = currentsteerangle;
        wheel[1].steerAngle = currentsteerangle;
        if(transform.eulerAngles.y < 5 && transform.eulerAngles.y > -5 && !IsChangingStrip && laneChangeType == LaneChangeType.StrictLaneChange) 
        {
            transform.rotation = new Quaternion(transform.rotation.x,0,0,transform.rotation.w);
        }
    }
    private void HandleMotor() 
    {
        LerpEngine();
        UpdateDrag();
        #region GearSpeed
        maxspeed = gears[GearLevel];
        if(GearLevel > 0) 
        {
            minspeed = gears[GearLevel - 1] - 10;
        }
        #endregion
        for(int i = 0; i < wheel.Length; i++) 
        {
            wheel[i].motorTorque = (verticalright * motorForce);
        }
    }
    private void UpdateDrag() 
    {
        if(transform.tag == "Player") 
        {
            if(!inputManager.UpPressed)
            {
                rigidBody.drag = 0.06f;
            }
            else
            {
                rigidBody.drag = 0.01f;
            }
        }
        if(transform.tag != "Player") 
        {
            if(verticalright != 0 && !IsChangingStrip) 
            {
                rigidBody.drag = 0.01f;
            }
            else if(verticalright == 0 && !IsChangingStrip)
            {
                rigidBody.drag = 0.1f;
            }
        }
        else if(IsChangingStrip) 
        {
            rigidBody.drag = 0.08f;
        }   
        UpdateDragForSameLane(); 
    } 
    private void UpdateDragForSameLane() 
    {
        //Not 1: Bu ideal yöntem olmayabilir.
        //Not 2: Her yapay zeka araç için ayrı ayrı oyunun editöründen koymanız gerekiyor. Sonra bu döngü onları teker teker bulup bizim çizgimizle aynı çizgide mi diye bakacak.
        //Not 3: Yapay zeka için ayrı olarak kontrol etmek gerecek.
        for(int i = 0;i < carAIScript.Length; i++) 
        {
            if(transform.tag == "Player" && lane == carAIScript[i].lane && !IsChangingStrip) 
            {
                float TargetZDistance =  zCoordinate - carAIScript[i].zCoordinate;
                //Debug.Log("Car:" + zCoordinate);
                //Debug.Log("Car AI:" + carAIScript[i].zCoordinate);
                if(zCoordinate > carAIScript[i].zCoordinate && TargetZDistance < 20)  
                {
                    rigidBody.drag = 0.23f;
                }
                else if(zCoordinate < carAIScript[i].zCoordinate && TargetZDistance > -20 ) 
                {
                    Debug.Log("ABC DRAG NEED TO BE CHANGED");
                    carAIScript[i].rigidBody.drag = 0.23f;
                    carAIScript[i].FixVariables();
                }
                Debug.Log("Targetted z coordinate " + (zCoordinate - carAIScript[i].zCoordinate));
            }
        }
    }
    private void UsingNitro() 
    {
        if(IsUsingNitro)
        {
            rigidBody.AddForce(transform.forward * upgrades.boostForce);
            upgrades.StockNitro--;
        }
        if(upgrades.StockNitro <= 0) 
        {
            IsUsingNitro = false;
            CanUseNitro = false;
            IsFinishedUsingNitro = true;
        }
        if(upgrades.StockNitro > 0) 
        {
            if(speed > 75 && transform.tag == "Player")  
            {
                CanUseNitro = true;
            }
            else if(transform.tag == "Player" && speed < 75) 
            {
                CanUseNitro = false;
            }
        }
    }
    private void UpdateWheels() 
    {
        UpdateSingleWheel(wheel[0],wheeltransform[0]);
        UpdateSingleWheel(wheel[1],wheeltransform[1]);
        UpdateSingleWheel(wheel[2],wheeltransform[2]);
        UpdateSingleWheel(wheel[3],wheeltransform[3]);
    } 
    private void UpdateSingleWheel(WheelCollider wheelCollider,GameObject wheelTransform) 
    {
        Vector3 pose;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pose,out rot);
        wheelTransform.transform.rotation = rot;
        wheelTransform.transform.position = pose;
    }
    private float CalculateCarSpeed() 
    {
        float speed = rigidBody.velocity.z;
        speed *= 3.6f;
        if(speed > maxspeed) 
        {
            SetEngineLerp(maxspeed);    
        }
        else 
        {
            motorForce = StartMotorForce;
        }
        return speed;
    }
    private float CalculateZCoordinate() 
    {
        zCoordinate = transform.position.z;
        return zCoordinate;
    }
    private void DebugLog() 
    {
        gameManager.LaneText.text = "Lane: " + lane;
        gameManager.ChangingTimeText.text = "Changing Time: " + timer;
        gameManager.SteerAngleText.text = "SteerAngle = " + currentsteerangle;
        gameManager.IsChangingStripText.text = "IsChangingStrip = " + IsChangingStrip;
        gameManager.OnTouchText.text = "OnTouch = " + OnTouch;
        gameManager.HorizontalText.text = "Horizontal = " + horizontalright;
        gameManager.NitroText.text = "Nitro = " + upgrades.StockNitro;
        gameManager.CanUseNitroText.text = "CanUseNitro = " + CanUseNitro;
        gameManager.GearText.text = "Gear = " + GearLevel;
        gameManager.MotorForceText.text = "MotorForce = " + motorForce;
        gameManager.MinSpeedText.text = "MinSpeed = " + minspeed;
        gameManager.MaxSpeedText.text = "MaxSpeed = " + maxspeed; 

    }
}
