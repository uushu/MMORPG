using SkillBridge.Message;
using UnityEngine;

public class RideController :MonoBehaviour
{
    public Transform ridePoint;
    public EntityController rider;
    public Vector3 offset;
    private Animator anim;
    

    
    void Start () {
       this.anim = this.GetComponent<Animator>();
    }
    void Update()
    {
        if (this.ridePoint == null || this.rider == null)
            return;
        this.rider.SetRidePosition(this.ridePoint.position + this.ridePoint.TransformDirection((this.offset)));
    }
    public void SetRider(EntityController entity)
    {
        this.rider = entity;
    }
    
    public void OnEntityEvent(EntityEvent entityEvent ,int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }

    }
    

   

   
}