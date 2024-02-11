using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    [SerializeField] protected int maxLife = 4;
    protected int life;

    [SerializeField] protected Slider lifeSlider;
    
    [SerializeField] protected GameObject deadSprite;

    [SerializeField] protected GameObject hitVFX;
    
    protected Animator m_anim;
    protected Rigidbody2D m_rb;
    protected NavMeshAgent m_agent;

    
    void Awake()
    {
        life = maxLife;
        lifeSlider.maxValue = maxLife;
        lifeSlider.value = life;

        m_anim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
        m_agent = GetComponent<NavMeshAgent>();

        Init();
    }

    protected virtual void Init() { }


    public void TakeDamage(int damage, Vector3 knockback =new Vector3())
    {
        life -= damage;

        if(knockback!=null)
        {
            gameObject.GetComponent<EnemyBehaviourStateMachine>().KnockbackHandler(knockback);
        }

        if (life <= 0)
        {
            Die();
        }
        else
        {
            Hit();
        }
    }


    private void KnockbackCheck()
    {
        Debug.Log("knocker check");
        if (m_rb.velocity.magnitude < 1f)
        {
            m_rb.velocity = Vector2.zero;
            m_rb.isKinematic = true;
            m_agent.enabled = true;
        }
    }

    public virtual void Knocked(Vector3 knockback)
    {
        if (knockback == null || knockback == Vector3.zero)
        { return; }

        m_agent.enabled = false;
        m_rb.isKinematic = false;
        m_rb.AddForce(knockback, ForceMode2D.Impulse);

        InvokeRepeating("KnockbackCheck", 0.05f, 0.1f);
    }

    protected void Hit()
    {
        UpdateSlider();

        //TODO: ENEMY SLOW MODE
        //OnHitSlowMo?.Invoke();

        if (hitVFX != null)
        {
            Instantiate(hitVFX, transform);
        }

        AdditionalHit();
    }

    protected virtual void AdditionalHit() { }

    protected virtual void Die() { }

    protected void UpdateSlider()
    {
        if (life == maxLife)
            lifeSlider.gameObject.SetActive(false);
        else
            lifeSlider.gameObject.SetActive(true);

        lifeSlider.value = life;
    }

    public void FlipLifeSliderSprite(bool invert)
    {
        if(invert)
        {
            lifeSlider.transform.Rotate(0, 0, 0);
        }
        else
        {
            lifeSlider.transform.Rotate(0, 180, 0);
        }
    }
}
