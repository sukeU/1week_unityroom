using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    Animator animator;
    public float speed=3.0f;
    Rigidbody2D rb;
    public bool IsGround;//�ڒn���Ă��邩�ǂ���
    public bool IsWall;//�ǂɐڐG���Ă��邩�ǂ���
    public bool IsGrabPoint;//�L���b�`�|�C���g�ɃI�u�W�F�N�g�����邩�ǂ���
    public bool IsStack;//�u���ꏊ�ɃI�u�W�F�N�g�����邩�ǂ���
    public bool IsGrab;//�I�u�W�F�N�g��͂�ł��邩�ǂ���
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
    private void Update()//FixedUpdate����GetKeyDown�̏�������肭�����Ȃ����߁A���͏�����Update�ōs��
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
    /// �v���C���[�̈ړ����܂Ƃ߂郁�\�b�h
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
            if (Input.GetAxisRaw("Vertical") > 0)//�W�����v
            {
                SoundManager.Instance.PlaySeByName("jump");
                Ymove = 10.0f;
                IsGround = false;
            }
        }
        else
        {
            Ymove -= 0.5f;//�d��
        }
        if (IsWall&&Ymove<0)
        {
            Ymove = -4.0f;
            Xmove = 0.0f;
        }
      
        rb.velocity = new Vector2(Xmove*speed, Mathf.Clamp(Ymove, -5f, 10f));
    }
    /// <summary>
    /// �O���u�|�C���g�ɂ���I�u�W�F�N�g���q�I�u�W�F�N�g�ɂ���
    /// </summary>
    void Grab()
    {
        if (grabbableObj != null|| GrabPointObj == null) return;//���Ɏ����Ă��邩�A�O���u�|�C���g�ɖ����Ƃ�����0��
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
            GrabPointObj.transform.position = new Vector2(transform.position.x,transform.position.y+1f);//���̏�Ɏ����Ă�������
            GrabPointObj = null;

        }
    }
    void Put()//Cube��u������
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
            //�����Ă�������ɂ���Ēu��������ω�������@floor�Ő؂�̂Ă��Ă��邽�߁A���̂悤�Ȃ��Ƃ��N����
            if (transform.localScale.x > 0)//�E�����̏ꍇ
            {
                PutX = Xmass + 1.5f;
            }else if (transform.localScale.x < 0)//�������̏ꍇ
            {
                PutX = Xmass - 0.5f;
            }
            grabbableObj.GetComponent<Collider2D>().isTrigger = false;
            grabbableObj.transform.localScale= new Vector3(1.0f, 1.0f, 1.0f);
            grabbableObj.transform.position = new Vector2(PutX, PutY);//�u���ꏊ
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
