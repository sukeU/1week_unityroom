using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    Animator animator;
    public float speed=3.0f;
    Rigidbody2D rb;
    public bool IsGround;//接地しているかどうか
    public bool IsWall;//壁に接触しているかどうか
    public bool IsGrabPoint;//キャッチポイントにオブジェクトがあるかどうか
    public bool IsStack;//置く場所にオブジェクトがあるかどうか
    public bool IsGrab;//オブジェクトを掴んでいるかどうか
    [SerializeField] public GameObject GrabPointObj;
    [SerializeField]private GrabbableObj grabbableObj;
    [SerializeField] private int Xmass;
    [SerializeField] private int Ymass;
    [SerializeField] private bool ready;

    // Start is called before the first frame update
    void Start()
    {
        ready = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    
    }
    private void Update()//FixedUpdateだとGetKeyDownの処理が上手くいかないため、入力処理はUpdateで行う
    {
        Put();
        Grab();
        ReloadKey();
        EscapeKey();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Xmass = (int)Mathf.Floor(transform.position.x);
        Ymass = (int)Mathf.Floor(transform.position.y);
        Movement();

    }
    /// <summary>
    /// プレイヤーの移動をまとめるメソッド
    /// </summary>
    void Movement()
    {
        float Xmove = Input.GetAxisRaw("Horizontal");
        if (Xmove < 0)
        {
            transform.localScale=new Vector3(-1, 1, 1);
        }
        else if(Xmove>0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        float Ymove = rb.velocity.y;
        if (IsGround)
        {
            Ymove = 0.0f;
            if (Input.GetAxisRaw("Vertical") > 0)//ジャンプ
            {
                SoundManager.Instance.PlaySeByName("jump");
                Ymove = 10.0f;
                IsGround = false;
            }
        }
        else
        {
            Ymove -= 0.5f;//重力
        }
        if (IsWall&&Ymove<0)
        {
            Ymove = -4.0f;
            Xmove = 0.0f;
        }
      
        rb.velocity = new Vector2(Xmove*speed, Mathf.Clamp(Ymove, -5f, 10f));
    }
    /// <summary>
    /// グラブポイントにあるオブジェクトを子オブジェクトにする
    /// </summary>
    void Grab()
    {
        if (grabbableObj != null|| GrabPointObj == null) return;//既に持っているか、グラブポイントに無いとき利他0ン
        if (Input.GetKeyDown(KeyCode.Space) && GrabPointObj.layer != 9 && ready)
        {
            ready = false;
            StartCoroutine("animCoolDown");
            SoundManager.Instance.PlaySeByName("grab");
            animator.SetBool("Grab", true);
            IsGrab = true;
            grabbableObj=GrabPointObj.GetComponent<GrabbableObj>();
            grabbableObj.setParent(this.transform);
            grabbableObj.gameObject.layer = 9;
            GrabPointObj.GetComponent<Collider2D>().isTrigger = true;
            GrabPointObj.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
            GrabPointObj.transform.position = new Vector2(transform.position.x,transform.position.y+1f);//頭の上に持っていく処理
            GrabPointObj = null;

        }
    }
    void Put()//Cubeを置く処理
    {
        if (grabbableObj == null) return;
        if (Input.GetKeyDown(KeyCode.Space)&&!IsStack&&ready)
        {
            ready = false;
            StartCoroutine("animCoolDown");
            SoundManager.Instance.PlaySeByName("put");
            animator.SetBool("Put", true);
            animator.SetBool("Grab", false);
            StartCoroutine("waitAnim");
            if (grabbableObj.GetComponent<CoreCube>() != null)
            {
                grabbableObj.gameObject.layer = 0;
            }
            else
            {
                grabbableObj.gameObject.layer = 7;
            }
            IsGrab = false;
            float PutX = Xmass, PutY= Ymass+0.5f;
            grabbableObj.setParent(null);
            //向いている方向によって置く距離を変化させる　floorで切り捨てしているため、このようなことが起こる
            if (transform.localScale.x > 0)//右向きの場合
            {
                PutX = Xmass + 1.5f;
            }else if (transform.localScale.x < 0)//左向きの場合
            {
                PutX = Xmass - 0.5f;
            }
            grabbableObj.GetComponent<Collider2D>().isTrigger = false;
            grabbableObj.transform.localScale= new Vector3(1.0f, 1.0f, 1.0f);
            grabbableObj.transform.position = new Vector2(PutX, PutY);//置く場所
            grabbableObj = null;

          
        }
       
    }
    IEnumerator waitAnim()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Put", false);
    }
    IEnumerator animCoolDown()
    {
        yield return new WaitForSeconds(0.4f);
        ready = true;
    }
    public void Board(Transform t)
    {
        transform.SetParent(t);
    }
    public void ReloadKey()
    {
        if(Input.GetKeyDown(KeyCode.R))SceneManagerScript.Reload();
    }

    public void EscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManagerScript.ChangeStage("Title");
            SoundManager.Instance.StopBgm();
            SoundManager.Instance.PlayBgmByName("bgm_techsynth_rhythm");
        }
    }
}
