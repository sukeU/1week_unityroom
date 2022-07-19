using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreCube : GrabbableObj
{
    [SerializeField]Player player;
    int layerMask;
    private float rayDistance=1.0f;
    Vector2 rayPosition;
    bool R, L, U, D;
    [SerializeField]List<GameObject> cubes = new List<GameObject>();
    Pattern pattern = Pattern.none;
    [SerializeField]Sprite[] Cores=new Sprite[5];
    SpriteRenderer spriteRenderer;
    bool Wait = false;
    enum Pattern
    {
        none,
        Moveup,
        Roundtrip,
        MoveRight,
        MoveLeft,

    }
    //ステート管理
    public class StateBase
    {
        public virtual void OnStart(CoreCube owner) { }

        public virtual void OnUpdate(CoreCube owner) { }

        public virtual void OnEnd(CoreCube owner) { }
    }
    private class Waiting : StateBase
    {
        public override void OnStart(CoreCube owner) {
            owner.layerMask = LayerMask.GetMask("Cubes");
            if (owner.cubes.Count > 0) owner.cubes.Clear();
        }

        public override void OnUpdate(CoreCube owner) {
            owner.Fusion();

        }
        //触れているオブジェクトを全部取得する
        public override void OnEnd(CoreCube owner) {
            owner.rayPosition = owner.transform.position;
            Ray2D rayRight = new Ray2D(owner.rayPosition, Vector2.right);
            Ray2D rayLeft = new Ray2D(owner.rayPosition, Vector2.left);
            Ray2D rayUp = new Ray2D(owner.rayPosition, Vector2.up);
            Ray2D rayDown = new Ray2D(owner.rayPosition, Vector2.down);
            if (owner.rayHitCheck(rayUp)) owner.U = true; else owner.U = false;
            if (owner.rayHitCheck(rayRight)) owner.R = true; else owner.R = false;
            if (owner.rayHitCheck(rayLeft)) owner.L = true; else owner.L = false;
            if (owner.rayHitCheck(rayDown)) owner.D = true; else owner.D = false;

        }
    }
    public class Running : StateBase
    {
        Vector2 startPosition, targetPosition;
        private Vector2 velocity = Vector2.zero;
        private float time = 1.0f;
        float elapsedTime;
        Ray2D ray;
        Vector3 rayOffset1, rayOffset2;
      


        public override void OnStart(CoreCube owner) {
            owner.layerMask = LayerMask.GetMask(new string[] { "Stage","Cubes", "Cores" });
            startPosition = owner.transform.position;
            elapsedTime = 0.0f;
            owner.gameObject.layer = 9;//RunningCoresの9
            foreach (var cube in owner.cubes)
            {
                cube.gameObject.layer = 9;//RunningCoresの9
                cube.transform.parent = owner.transform;
            }
            if (owner.U && !owner.R && !owner.L && !owner.D)
            {
                owner.pattern = Pattern.Moveup;
            }else if (!owner.U && owner.R && owner.L && !owner.D)
            {
                owner.pattern = Pattern.Roundtrip;
            }else if(owner.U && !owner.R && owner.L && !owner.D)
            {
                owner.pattern = Pattern.MoveRight;
            }else if(owner.U && owner.R && !owner.L && !owner.D)
            {
                owner.pattern = Pattern.MoveLeft;
            }



            switch (owner.pattern)
            {
                case Pattern.none:
                    owner.ChangeState(owner.waiting);
                    break;
                case Pattern.Moveup:
                    SoundManager.Instance.PlaySeByName("move");
                    ray = new Ray2D(owner.rayPosition, Vector2.up);
                    owner.rayDistance = 1.5f;
                    owner.spriteRenderer.sprite = owner.Cores[1];
                    targetPosition = new Vector2(owner.transform.position.x, owner.transform.position.y + 5.0f);
                    break;
                case Pattern.Roundtrip:
                    SoundManager.Instance.PlaySeByName("move");
                    owner.rayDistance = 0.5f;
                    owner.spriteRenderer.sprite = owner.Cores[2];
                    rayOffset1 = new Vector3(1.0f, 0.0f, 0.0f);
                    rayOffset2 = new Vector3(-1.0f, 0.0f, 0.0f);
                    if (owner.RunningHitCheck(ray, rayOffset1))
                    {
                        owner.ChangeState(owner.waiting);
                    }
                    if (owner.RunningHitCheck(ray, rayOffset2))
                    {
                        owner.ChangeState(owner.waiting);
                    }
                    //playerがcoreよりも上に居る時
                    if (owner.transform.position.y < owner.player.transform.position.y)
                    {
                        ray = new Ray2D(owner.rayPosition, Vector2.up);
                     
                       
                        targetPosition = new Vector2(owner.transform.position.x, owner.transform.position.y+3.0f);
                    }
                    else
                    {
                        ray = new Ray2D(owner.rayPosition, Vector2.down);
                       
                        targetPosition = new Vector2(owner.transform.position.x, owner.transform.position.y - 3.0f);
                    }
                   
                   
                    break;
                case Pattern.MoveRight:
                    rayOffset1 = new Vector3(0.0f, 1.0f, 0.0f);
                    SoundManager.Instance.PlaySeByName("move");
                    ray = new Ray2D(owner.rayPosition, Vector2.right);
                    owner.rayDistance = 0.5f;
                    owner.spriteRenderer.sprite = owner.Cores[3];
                    targetPosition = new Vector2(owner.transform.position.x+10.0f, owner.transform.position.y);
                    break;
                case Pattern.MoveLeft:
                    SoundManager.Instance.PlaySeByName("move");
                    ray = new Ray2D(owner.rayPosition, Vector2.left);
                    owner.rayDistance = 0.5f;
                    owner.spriteRenderer.sprite = owner.Cores[4];
                    targetPosition = new Vector2(owner.transform.position.x - 10.0f, owner.transform.position.y);
                    break;
            }

            if (owner.RunningHitCheck(ray))
            {
                SoundManager.Instance.PlaySeByName("movestop");
                owner.ChangeState(owner.waiting);
            }
        }

        public override void OnUpdate(CoreCube owner)
        {
           
            elapsedTime += Time.deltaTime;
            switch (owner.pattern)//パターンによって動きを変える
            {
                case Pattern.none:
                    owner.ChangeState(owner.waiting);
                    break;
                case Pattern.Moveup:
                    if (owner.transform.Find("chara")) { owner.rayDistance = 2.5f;  }
                    else owner.rayDistance = 1.5f;
                    owner.transform.position = Vector2.SmoothDamp(owner.transform.position, targetPosition, ref velocity, time);
                    
                    if (elapsedTime > 4.0f) { owner.transform.position = targetPosition; owner.ChangeState(owner.waiting); }
                    break;
                case Pattern.Roundtrip:
                    if (owner.transform.Find("chara")&&ray.direction==Vector2.up)owner.rayDistance = 1.5f;
                    else owner.rayDistance = 0.5f;

                    if (owner.RunningHitCheck(ray,rayOffset1))
                    {
                        owner.ChangeState(owner.waiting);
                    }
                    if (owner.RunningHitCheck(ray, rayOffset2))
                    {
                        owner.ChangeState(owner.waiting);
                    }

                    if (elapsedTime < 4.0f)
                    {
                        owner.transform.position = Vector2.SmoothDamp(owner.transform.position, targetPosition, ref velocity, time);
                    }else if (4.0f<=elapsedTime && elapsedTime < 8.0f) {
                        if (targetPosition.y > startPosition.y)
                        {
                            
                           if(ray.direction.y>0.0f) ray = new Ray2D(owner.rayPosition, Vector2.down);
                            owner.transform.position = Vector2.SmoothDamp(owner.transform.position, new Vector2(startPosition.x, startPosition.y - 3.0f), ref velocity, time * 2.0f);
                        }
                        else
                        {
                            if (ray.direction.y <= 0.0f) ray = new Ray2D(owner.rayPosition, Vector2.up);
                            owner.transform.position = Vector2.SmoothDamp(owner.transform.position, new Vector2(startPosition.x, startPosition.y + 3.0f), ref velocity, time * 2.0f);
                        }
                       
                    }
                    else if (8.0f<=elapsedTime&&elapsedTime<12.0f)
                    {
                        if(ray.direction.y > 0.0f) ray = new Ray2D(owner.rayPosition, Vector2.down); else ray = new Ray2D(owner.rayPosition, Vector2.up);
                        owner.transform.position = Vector2.SmoothDamp(owner.transform.position, startPosition, ref velocity, time);
                    }
                    else
                    {
                        owner.transform.position = startPosition;
                        owner.ChangeState(owner.waiting);
                    }
                    break;
                case Pattern.MoveRight:
                case Pattern.MoveLeft:
                    if (owner.RunningHitCheck(ray, rayOffset1))
                    {
                       // Debug.DrawRay(owner.transform.position + rayOffset1, Vector2.right, Color.red);
                        owner.ChangeState(owner.waiting);
                    }
                    owner.transform.position = Vector2.SmoothDamp(owner.transform.position, targetPosition, ref velocity, time);
                    if (elapsedTime > 4.0f) { owner.transform.position = targetPosition; SoundManager.Instance.PlaySeByName("movestop"); owner.ChangeState(owner.waiting); }
                    break;

            }//移動中に当たったら
          
            if (owner.RunningHitCheck(ray))
            {
                SoundManager.Instance.PlaySeByName("movestop");
                owner.ChangeState(owner.waiting);
               
               
            }


        }

        public override void OnEnd(CoreCube owner) {
            owner.spriteRenderer.sprite = owner.Cores[0];
            owner.gameObject.layer = 10;//Coresの10
            owner.rayDistance = 1.0f;
            startPosition = Vector2.zero;
            targetPosition = Vector2.zero;
            velocity = Vector2.zero;
            owner.transform.position = new Vector2((int)Mathf.Floor(owner.transform.position.x) + 0.5f, (int)Mathf.Floor(owner.transform.position.y) + 0.5f);
            foreach (var cube in owner.cubes)
            {
                cube.gameObject.layer = 7;//Cubesの7
                cube.transform.parent = null;
                cube.transform.position = new Vector2((int)Mathf.Floor(cube.transform.position.x) + 0.5f, (int)Mathf.Floor(cube.transform.position.y) + 0.5f);

            }
         
            owner.pattern = Pattern.none;
        }

 
    }

   

    private Waiting waiting = new Waiting();
    public Running running = new Running();
    public StateBase _currentState; // 現在のステート

    private void ChangeState(StateBase nextState)
    {
        _currentState.OnEnd(this);
        _currentState =nextState;
        _currentState.OnStart(this);
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        layerMask = LayerMask.GetMask( "Cubes" );
        _currentState = waiting;
        _currentState.OnStart(this);
        player = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<Player>();
    }
    private void Update()
    {
        _currentState.OnUpdate(this);
      //  Debug.Log(_currentState);
    }
    //CubeにRayが衝突したときの処理
    bool rayHitCheck(Ray2D ray)
    {
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayDistance, layerMask);
        if (hit.collider && transform.parent == null)
        {
            cubes.Add(hit.collider.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }
    bool RunningHitCheck(Ray2D ray)
    {
        //引数がrayじゃないのはオブジェクトが移動したときもついてくるようにするため
        RaycastHit2D hit = Physics2D.Raycast(transform.position, ray.direction, rayDistance, layerMask);
        Debug.DrawRay(transform.position, ray.direction, Color.red, rayDistance);
        if (hit.collider)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool RunningHitCheck(Ray2D ray,Vector3 offset)
    {
        //引数がrayじゃないのはオブジェクトが移動したときもついてくるようにするため
        RaycastHit2D hit = Physics2D.Raycast(transform.position+offset, ray.direction, rayDistance, layerMask);
        Debug.DrawRay(transform.position+offset, ray.direction, Color.red, rayDistance);
        if (hit.collider)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// コアを起動させるための関数
    /// </summary>
    void Fusion()
    {
        if (Input.GetAxisRaw("Vertical") < 0&&!player.IsGrab&&Wait==false)
        {
            Wait = true;
            StartCoroutine("WaitRunning");
            ChangeState(running);
        }
    }
    IEnumerator WaitRunning()
    {
        yield return new WaitForSeconds(1.0f);
        Wait = false;
    }
}
