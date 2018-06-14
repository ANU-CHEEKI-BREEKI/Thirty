using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class SquadTriggerInitiator : MonoBehaviour
{
    Squad squad;
    public Squad Squad { get { return squad; } }

    Rigidbody2D rb;
    Transform tr;
    CircleCollider2D cl;

	void Start ()
    {
        tr = transform;

        squad = tr.parent.GetComponent<Squad>();

        if(squad == null)
        {
            Debug.Log("Не туда засунул этот обьект. Надо внетрь сквада.");
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        cl = GetComponent<CircleCollider2D>();
        
	}
	
	void Update ()
    {
		if((Vector2)tr.position != squad.CenterSquad)
            rb.MovePosition(squad.CenterSquad);
	}

    private void OnDrawGizmos()
    {
        if (cl != null && tr != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(tr.position, cl.radius);
        }
    }
}
