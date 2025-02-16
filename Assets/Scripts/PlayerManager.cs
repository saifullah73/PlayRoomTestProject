using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playroom;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    
    private Rigidbody rb;
    public Animator animator;
    private bool isGrounded;
    public PlayroomKit.Player playroomplayer  { get; private set; }
    public string playroomId { get; private set; }
    public int playerIdx { get; private set; }

    private bool isMyPlayer;
    public Transform playerModel;
    public Transform cameraLookAt;

    private CameraManger cameraManger;

    private bool Jumping,running,hasWon = false;

    private bool raceEnd;
    private bool raceStarted;


    public void Init(PlayroomKit.Player player,string playroomId, int playerIdx, CameraManger cameraManger ,bool isMyPlayer)
    {
        this.playroomId = playroomId;
        this.playerIdx = playerIdx;
        this.isMyPlayer = isMyPlayer;
        this.playroomplayer = player;
        raceEnd = false;
        raceStarted = false;
        Jumping = false;
        running = false;
        hasWon = false;
        if (isMyPlayer){
            cameraManger.Init(playerModel,cameraLookAt);
        }
    }

    public void ResetAnims(){
        
        animator.SetTrigger("reset");
        animator.SetBool("running",false);
        animator.SetBool("jumping", false);
        animator.ResetTrigger("win");
        animator.ResetTrigger("lost");
        //animator.ResetTrigger("reset");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (raceEnd || !raceStarted) return;
        if (!isMyPlayer){
            
        }else{
            Move();
            CheckGrounded();
        }        
    }

    void Update()
    {
        if (raceEnd || !raceStarted) return;
        if (!isMyPlayer) {
            ReadAndApplyState();
        }else{
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                if (isGrounded)
                {
                    Jumping = true;
                    animator.SetBool("jumping", true);
                    Jump();
                }
            }
            UpdateStates();
        }
        
    }


    private void ReadAndApplyState(){
        Vector3 pos = playroomplayer.GetState<Vector3>("position");
        Vector3 forward = playroomplayer.GetState<Vector3>("forward");
        bool newrunning = playroomplayer.GetState<bool>("running");
        bool newJumping = playroomplayer.GetState<bool>("jumping");
        bool newhasWon = playroomplayer.GetState<bool>("hasWon");

        rb.MovePosition(pos);
        playerModel.forward = forward;


        if (!running && newrunning){
            running = true;
            animator.SetBool("running",true);
        }else if (running && !newrunning){
            running = false;
            animator.SetBool("running",false);
        }

        if (!Jumping && newJumping){
            Jumping = true;
            animator.SetBool("jumping",true);
        }else if (Jumping && !newJumping){
            Jumping = false;
            animator.SetBool("jumping",false);
        }

    }


    void Move()
    {
        float moveInput = Input.GetAxis("Vertical");
        float strafeInput = Input.GetAxis("Horizontal");
        
        Vector3 moveDirection = transform.forward * moveInput + transform.right * strafeInput;
        
        bool moveInputRecieved = moveDirection.magnitude > 0.1f;
        if (moveInputRecieved)
        {
            rb.MovePosition(transform.position + moveDirection.normalized * moveSpeed * Time.deltaTime);
            playerModel.forward = moveDirection.normalized;
        }else {

        }

        if (!running && moveInputRecieved ){
            running = true;
            animator.SetBool("running",true);
        }else if (running && !moveInputRecieved){
            running = false;
            animator.SetBool("running",false);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    void CheckGrounded()
    {
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.5f, groundLayer);
        isGrounded = rb.velocity.y == 0f;
        if (Jumping && isGrounded)
        {
            Jumping = false;
            animator.SetBool("jumping", false);
        }

    }

    public void EndRace(bool won){
        animator.SetBool("running",false);
        animator.SetBool("jumping", false);
        
        if (won){
            hasWon = true;
            animator.SetTrigger("win");
        }else{
            hasWon = false;
            animator.SetTrigger("lost");
        }


        raceEnd = true;
    }

    public void StartRace(){
        raceStarted = true;
    }

    void UpdateStates(){
        if (playroomplayer == null) return;
        playroomplayer.SetState("position",transform.position);
        playroomplayer.SetState("forward",playerModel.forward.normalized);
        playroomplayer.SetState("running",running);
        playroomplayer.SetState("jumping",Jumping);
        playroomplayer.SetState("hasWon",hasWon);
    }
}
