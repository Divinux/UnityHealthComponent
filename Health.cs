using UnityEngine;

//=======================//
// How to Inflict Damage //
//=======================//
// Check for a 'Health' component on the 'GameObject' you are thinking about damaging.
// If it has one, construct a new 'DamageInfo' object, fill out all the fields, and call that health component's 'TakeDamage( DamageInfo damageInfo )' method.
// If damage was dealt, the method will return true. Otherwise, it will return false.
// Do not recycle 'DamageInfo' objects.

//==========//
// Messages //
//==========//
// This component introduces a few new damage-related messages for your objects.

// void OnIncomingDamage( DamageInfo damageInfo )
//  - Called on the victim when something is attempting to damage it.
//  - Setting the 'rejected' field of 'damageInfo' to false will prevent the damage from being dealt.

// void OnTakeDamage( DamageInfo damageInfo )
//  - Called on the victim immediately after it has taken damage and its health has been reduced.
//  - Will still be called on the killing blow.

// void OnKilled( DamageInfo damageInfo )
//  - Called on the victim after 'OnTakeDamage' when its health has been reduced to zero or less.

// void OnDamageDealt( DamageReport damageReport )
//  - Called on the attacker after the victim has called 'OnTakeDamage'.

// void OnKilledOther( DamageReport damageReport )
//  - Called on the attacker after the victim has called 'OnKilled'.


// Contains information about a damage event.
public class DamageInfo
{
	// May later include a field to indicate the damage type(s).
	public float damage;			// The amount of damage that will be subtracted from the health. Subject to modifiers like armor, weakness, resistance, invulnerability, etc.
	public GameObject inflictor;	// The object used to inflict the damage, e.g. the sword.
	public GameObject attacker;		// The object responsible for the damage, e.g. the player holding the sword.
	public Vector3 force;			// The force of this damage, both direction and magnitude. It's up to the object taking the damage to interpret this in a meaningful way.
	public bool rejected = false;	// Set this field to true in the 'OnIncomingDamage' message to prevent the damage from being dealt.
}

// Used when reporting back to an attacker about damage it has dealt to another object.
public class DamageReport
{
	public GameObject victim;
	public DamageInfo damageInfo;
}


public class Health : MonoBehaviour
{
	public int fullHealth;
	public int health;
	
	public bool alive
	{
		get { return health > 0; }
	}
	
	public bool TakeDamage (DamageInfo damageInfo)
	{
		if (!alive)
			return false;

		SendMessage( "OnIncomingDamage", damageInfo, SendMessageOptions.DontRequireReceiver );	// Allow other components to modify the damage.

		if (damageInfo.rejected)
			return false;

		health -= (int)damageInfo.damage;

		SendMessage( "OnTakeDamage", damageInfo, SendMessageOptions.DontRequireReceiver );
		if ( damageInfo.attacker )
		{
			damageInfo.attacker.SendMessage( "OnDamageDealt", new DamageReport{ victim = gameObject, damageInfo = damageInfo }, SendMessageOptions.DontRequireReceiver );
		}

		if (!alive)
		{
			SendMessage( "OnKilled", damageInfo, SendMessageOptions.DontRequireReceiver );
			if ( damageInfo.attacker )
			{
				damageInfo.attacker.SendMessage( "OnKilledOther", new DamageReport{ victim = gameObject, damageInfo = damageInfo }, SendMessageOptions.DontRequireReceiver );
			}
		}

		return true;
	}
}
