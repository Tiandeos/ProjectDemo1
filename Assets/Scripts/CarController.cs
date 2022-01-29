using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    internal enum LaneChangeType //FreeLaneChange oyuncuya özgür bir şekilde şeriti değiştirebilmesini sağlıyor
    {
        StrictLaneChange,
        FreeLaneChange
    }
    [SerializeField] private LaneChangeType laneChangeType;
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
    [HideInInspector]public int lane;
    float timer;
    #endregion
    public float topspeed;
    public float[] gears;
    [SerializeField]private float motorForce = 1000;
    [SerializeField]private float breakForce = 1000;
    [SerializeField]private float StockNitro = 500;
    [SerializeField]private int MaxGearLevel = 4;
    private float maxsteerangle;
    private bool engineLerp;
    private float engineLerpValue;
    public GameManager gameManager;
    [SerializeField]private CarAI[] carAIScript;
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
        wheeltransforms = transform.Find("WheelsTransforms").gameObject;
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
        if(transform.tag == "Player") 
        {
            CanUseNitro = true;
        }
    }
    private void Update()
    {  
        if(gameManager.IsGameStarted) 
        {
            GetInput(verticalright,horizontalright);
            HandleMotor(); 
            HandleSteering();
            UpdateWheels();
            HandleStripChange();
        }
        speed = CalculateCarSpeed();
        zCoordinate = CalculateZCoordinate();
        UsingNitro();
        
    }
    public void GetInput(float vertical,float horizontal) 
    {
        if(transform.tag == "Player") 
        {
            verticalright = Input.GetAxis("Vertical");
        }
        else if(transform.tag == "Opponent")
        {
            verticalright = vertical;
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
            if(Input.GetKeyDown(KeyCode.E) && GearLevel < 4 && speed > maxspeed - 5 && !IsChangingStrip) 
            {
                ChangeGearLevel(true);   
                gameManager.ChangeGear();
            }
            if(Input.GetKeyDown(KeyCode.A)) 
            {
                ChangeLane("left");
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                ChangeLane("right");
            }
            if(Input.GetKeyDown(KeyCode.Z) && CanUseNitro) 
            {
                IsUsingNitro = true;
            }
        }
        //Vitesi aşağı çekiyor
        if(speed < minspeed && GearLevel > 1) 
        {
            ChangeGearLevel(false);
            if(transform.tag == "Player") 
            {
                gameManager.ChangeGear();
            }
        }
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
                motorForce = 0;
                verticalright = 0;
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
    private void HandleStripChange() 
    {
        //Derler ki aslında bu yazdığın kodun yapay zekadan farkı yoktur, onlara de ki elbet o şerit değişitirmenin ve yapay zekanın ayrımını yapabilir, o yazanların en iyisi en mükemmelidir.
        //PS : Cidden nedense burayı yazmak en zorlandığım yerdi.
        //PS2 : Evet yapay zekayı buna benzer bir mantıkta yapabilirim ama onun yerine yapay zekanın sadece düz gitmesini sağlayacağım ve buraya belli şartlarda girmesini :D
        //Not : Boncuk teşekkürler.
        Vector3 destinedposition = new Vector3(lane * 4.5f,transform.position.y,transform.position.z),gopositon = (destinedposition - transform.position).normalized;
        if(lane == 0) 
        {
            destinedposition = new Vector3(0.5f,transform.position.y,transform.position.z);
        }
        float distancetotarget = Vector3.Distance(transform.position,destinedposition);
        float reachedtargetdistance =  0.41f;
        float angledir = Vector3.SignedAngle(transform.forward,gopositon,Vector3.up);
        DistanceFix(distancetotarget);
        if(transform.position.x != distancetotarget - reachedtargetdistance && IsCarStoppedTouching && transform.tag != "Player") 
        {
            if(lane == 0) 
            {
                lane++;
                IsChangingStrip = true;
                IsCarStoppedTouching = false;
                ResetVelocity(false);
            }
            else if(lane == 1) 
            {
                lane--;
                IsChangingStrip = true;
                IsCarStoppedTouching = false;
                ResetVelocity(false);
            }
        }
        if(!IsCarTouching) 
        {
            LockPosition(IsChangingStrip);
            LockRotation(IsChangingStrip);
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
        Debug.Log("Changing strip in: " + timer);
        float maxrotate;
        maxrotate = MaxRotateFix();
        //Debug.Log("Distancetotarget:" + distancetotarget);
        if(distancetotarget > reachedtargetdistance && IsChangingStrip) 
        {
            //Debug.Log("a");
            if(angledir > 0) 
            {
                if(transform.rotation.y < maxrotate) 
                {
                    //Debug.Log("b");
                    horizontalright = 0.75f;
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
                    horizontalright = -0.75f;
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
            if(transform.rotation.y > 0.003 ) 
            {
                //Debug.Log("e");
                horizontalright = -0.25f;
            }
            else if(transform.rotation.y < -0.003) 
            {
                //Debug.Log("f");
                horizontalright = 0.25f;
            }
            else if(transform.rotation.y == 0)
            {
                //Debug.Log("g");
                horizontalright = 0;
            }
           
            //Eulerangles 0 dan 360 ya kadar çalşır. Sola döndüğünde 359 dan başlar. Sağa döndüğünde 0 dan
            //Eğer değiştirirseniz yine test etmeyi unutmayın
            if(transform.eulerAngles.y >= 0 && transform.eulerAngles.y < 2) 
            {
                IsChangingStrip = false;
            }
        }
        if(distancetotarget < reachedtargetdistance) 
        {
            LockPosition(false);
        }
        
    }
    private void DistanceFix(float distancetotarget) 
    {

    }
    private void ResetVelocity(bool ResetNonAngularVelocity) 
    {
        rigidBody.angularVelocity = Vector3.zero;
        if(ResetNonAngularVelocity) 
        {
            rigidBody.velocity = Vector3.zero;
        }
    }
    private void SteerFix() 
    {
        //Maxsteerangle tekerliğin dönüşünü ayarlıyor eğer fazla olursa araba fazla kayıyor ve bu da yeniden tracke girmeyi denemesine neden oluyor.
        //Eğer bir nedenden dolayı burayı artırmak istersen maxrotationu da buraya göre ayarlaman gerekiyor. Eğer azaltırsan max rotationu artır artırırsan max rotationu azalt. Test etmeyi unutma farklı bir çözüm yok.
        /*if(speed > 0 && speed < 32 ) 
        {
            maxsteerangle = 30.25f;
        }
        else if(speed > 32 && speed < 45 ) 
        {
            maxsteerangle = 20.05f;
        }
        else if (speed > 45 && speed < 65) 
        {
            maxsteerangle = 18.95f;
        }
        else if(speed > 65 && speed < 100) 
        {
            maxsteerangle = 15.25f;
        }
        else if(speed > 100 && speed  < 110) 
        {
            maxsteerangle = 13.05f;
        }
        else if(speed > 110 && speed < 130) 
        {
            maxsteerangle = 11.35f;
        }
        else if(speed > 130 && speed < 150) 
        {
            maxsteerangle = 10.15f;
        }
        else if(speed > 150 && speed < 180) 
        {
            maxsteerangle = 8.90f;
        }
        else if(speed > 180 && speed < 220) 
        {
            maxsteerangle = 8.55f;
        }
        else if(speed > 220 && speed < 250) 
        {
            maxsteerangle = 7.55f;
        }
        else if(speed > 250 && speed < 280) 
        {
            maxsteerangle = 7.10f;
        }
        else 
        {
            maxsteerangle = 6.5f;
        }*/
        maxsteerangle = 30;
    }
    private float MaxRotateFix() 
    {
        //Oyunda ki transform.rotation 0 dan 360 yerine yada 0 180 ve -180 yerine 0 ila 1 ve -1 ila 0 a gidiyor yani bunun matematiğiyle uğraşmadım ama düzgün çalışıyor denemelerimde
        //Rotation 0.0871558 = Oyun motorunda 10. Buradan hesaplama yapabilirsiniz
        float maxrotate;
        if(speed > 0 && speed < 32) 
        {
            maxrotate = 0.0871558f;
        }
        else if(speed >= 32 && speed < 60) 
        {
            maxrotate = 0.04011f;
        }
        else if(speed >= 60 && speed < 90) 
        {
            maxrotate = 0.03621f;
        } 
        else if(speed >= 90 && speed < 130) 
        {
            maxrotate = 0.0331f;
        }
        else if(speed >= 130 && speed < 150) 
        {
            maxrotate = 0.0317f;
        }
        else if(speed >= 150 && speed < 180) 
        {
            maxrotate = 0.0282f;
        }
        else if(speed >= 180 && speed < 220) 
        {
            maxrotate = 0.0265f;
        }
        else if(speed >= 220 && speed < 270)
        {
            maxrotate = 0.0235f;
        }
        else if(speed >= 270 && speed < 320) 
        {
            maxrotate = 0.0205f;
        }
        else if(speed >= 320 && speed < 370) 
        {
            maxrotate = 0.0195f;
        }
        else if(speed >= 370 && speed < 400) 
        {
            maxrotate = 0.0175f;
        }
        else if(speed >= 400 && speed < 450) 
        {
            maxrotate = 0.0155f;
        }
        else 
        {
            maxrotate = 0.0105f;
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
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationZ;
        }
    }
    public void LockRotation(bool IsLocking) 
    {
        if(!IsLocking) 
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
        if(transform.eulerAngles.y < 5 && transform.eulerAngles.y > -5 && !IsChangingStrip) 
        {
            transform.rotation = new Quaternion(0,0,0,transform.rotation.w);
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
            minspeed = gears[GearLevel - 1];
        }
        #endregion
        for(int i = 0; i < wheel.Length; i++) 
        {
            wheel[i].motorTorque = (verticalright * motorForce);
        }
    }
    private void UpdateDrag() 
    {
        if(verticalright != 0 && !IsChangingStrip) 
        {
            rigidBody.drag = 0.01f;
        }
        else if(verticalright == 0 && !IsChangingStrip)
        {
            rigidBody.drag = 0.1f;
        }
        else if(IsChangingStrip) 
        {
            rigidBody.drag = 0.12f;
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
            rigidBody.AddForce(transform.forward * 10000); 
            StockNitro--;
        }
        if(StockNitro <= 0) 
        {
            IsUsingNitro = false;
            CanUseNitro = false;
            IsFinishedUsingNitro = true;
        }
        if(StockNitro > 0) 
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
        float speed = rigidBody.velocity.magnitude;
        speed *= 3.6f;
        if(speed > maxspeed) 
        {
            SetEngineLerp(maxspeed);    
        }
        else 
        {
            motorForce = 1000;
        }
        return speed;
    }
    private float CalculateZCoordinate() 
    {
        zCoordinate = transform.position.z;
        return zCoordinate;
    }
}