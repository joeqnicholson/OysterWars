using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// An implementation of a PhysicsComponent for 2D physics.
/// </summary>
public sealed class PhysicsComponent2D : PhysicsComponent
{
	RaycastHit2D[] raycastHits = new RaycastHit2D[10];
	Collider2D[] overlappedColliders = new Collider2D[10];

    ContactPoint2D[] contactsBuffer = new ContactPoint2D[10];



    void OnTriggerStay2D( Collider2D other )
    {        
        bool found = false;

        Trigger Trigger = new Trigger();

        for( int i = 0 ; i < Triggers.Count ; i++ )
        {
            if( Triggers[i].gameObject != other.gameObject )
                continue;
            
            found = true;

            

            // Ignore old Triggers
            if( !Triggers[i].firstContact )
                continue;
            
            // Set the firstContact field to false
            Trigger = Triggers[i];
            Trigger.firstContact = false;
            Triggers[i] = Trigger;
            

            break;
            
        }

        // First contact
        if( !found )
        {            
            Trigger.Set( true , other );
            Triggers.Add( Trigger );
        }
        
    }

    void OnTriggerExit2D( Collider2D other )
    {
        for( int i = Triggers.Count - 1 ; i >= 0 ; i-- )
        {            
            if( Triggers[i].collider2D == other )
            {
                Triggers.RemoveAt( i );

                break;
            }
        }
    }
            
    
    
    void OnCollisionEnter2D( Collision2D collision )
    {
        int bufferHits = collision.GetContacts( contactsBuffer );
        
        // Add the contacts to the list
        for( int i = 0 ; i < bufferHits ; i++ )
        {
            ContactPoint2D contact = contactsBuffer[i];    
            
            Contact outputContact = new Contact();

            outputContact.Set( true , contact );
            
            Contacts.Add( outputContact );
        }    
    }

    void OnCollisionStay2D( Collision2D collision )
    {
        int bufferHits = collision.GetContacts( contactsBuffer );
        
        // Add the contacts to the list
        for( int i = 0 ; i < bufferHits ; i++ )
        {
            ContactPoint2D contact = contactsBuffer[i];    
            
            Contact outputContact = new Contact();

            outputContact.Set( false , contact );
            
            Contacts.Add( outputContact );
        }
    }


    public override void IgnoreLayerCollision( int layerA , int layerB , bool ignore )
    {
        Physics2D.IgnoreLayerCollision( layerA , layerB , ignore );
    }

    
    public override int Raycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, LayerMask layerMask, bool ignoreTrigger = true)
    {
        Physics2D.queriesHitTriggers = !ignoreTrigger;

        hits = Physics2D.RaycastNonAlloc(
			origin ,
			castDisplacement.normalized ,            
            raycastHits ,
            castDisplacement.magnitude ,
			layerMask
		);

        GetClosestHit( out hitInfo , castDisplacement , layerMask );

        return hits;
    }

    public override int BoxCast( out HitInfo hitInfo , Vector3 center , Vector3 size , Vector3 up , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true )
    {

        float castAngle = Vector2.SignedAngle( Vector2.up , up );

        Physics2D.queriesHitTriggers = !ignoreTrigger;

        hits = Physics2D.BoxCastNonAlloc(
            center ,
            size ,
            castAngle ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask 
        );
        
        GetClosestHit( out hitInfo , castDisplacement , layerMask );

        return hits;
    }
    
    public override int SphereCast( out HitInfo hitInfo , Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true )
    {    
        Physics2D.queriesHitTriggers = !ignoreTrigger;

        hits = Physics2D.CircleCastNonAlloc(
            center ,
            radius ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            layerMask 
        );

        
        GetClosestHit( out hitInfo , castDisplacement , layerMask );

        return hits;
    }

    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
  
    public override bool OverlapSphere( Vector3 center , float radius , LayerMask layerMask , bool ignoreTrigger = true )
    {        
        Physics2D.queriesHitTriggers = !ignoreTrigger;
        
        hits = Physics2D.OverlapCircleNonAlloc(
            center ,
            radius ,
            overlappedColliders ,
            layerMask
        );

        return hits != 0;
    }

 

    public override bool OverlapCapsule( Vector3 bottom , Vector3 top , float radius , LayerMask layerMask , bool ignoreTrigger = true )
    {  
        Vector3 bottomToTop = top - bottom;
        Vector3 center = bottom + 0.5f * bottomToTop;
        Vector2 size = new Vector2( 2f * radius , bottomToTop.magnitude + 2f * radius );

        float castAngle = Vector2.SignedAngle( bottomToTop.normalized , Vector2.up );

        Physics2D.queriesHitTriggers = !ignoreTrigger;
        
        hits = Physics2D.OverlapCapsuleNonAlloc(
            center ,
            size ,
            CapsuleDirection2D.Vertical ,
            castAngle ,
            overlappedColliders ,
            layerMask
        );
        
        return hits != 0;
    }

    // ---------------------------------------------------------------------------------------------------------------------------------

    

    void GetHitInfo( ref HitInfo hitInfo , RaycastHit2D raycastHit , Vector3 castDirection )
    {
        if( raycastHit.collider != null )
        {                    
            hitInfo.point = raycastHit.point;
            hitInfo.normal = raycastHit.normal;
            hitInfo.distance = raycastHit.distance;
            hitInfo.direction = castDirection;
            hitInfo.gameObject = raycastHit.transform.gameObject;
            hitInfo.transform = raycastHit.transform;
            hitInfo.collider2D = raycastHit.collider;
            hitInfo.rigidbody2D = raycastHit.rigidbody;     
        }
    }

    void GetClosestHit( out HitInfo hitInfo , Vector3 castDisplacement , LayerMask layerMask )
    {
        RaycastHit2D closestRaycastHit = new RaycastHit2D();
        closestRaycastHit.distance = Mathf.Infinity;

        hitInfo = new HitInfo();
        hitInfo.hit = false;

        for( int i = 0 ; i < hits ; i++ )
        {
            RaycastHit2D raycastHit = raycastHits[i];             

            if( raycastHit.distance == 0 )
                continue;

            if( raycastHit.transform == this.transform )
                continue;

            hitInfo.hit = true;

            if( raycastHit.distance < closestRaycastHit.distance )
                closestRaycastHit = raycastHit;

        }

        if( hitInfo.hit )
            GetHitInfo( ref hitInfo , closestRaycastHit , castDisplacement.normalized );        

    }

    
     
}

}
