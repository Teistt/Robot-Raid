using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    [SerializeField] protected int maxLife = 4;
    protected int life;

    [SerializeField] protected Slider lifeSlider;
    
    [SerializeField] protected GameObject deadSprite;

    [SerializeField] protected GameObject hitVFX;
    
    protected NavMeshManager m_navMesh;
    protected Animator m_anim;

    // Start is called before the first frame update
    void Awake()
    {
        life = maxLife;
        lifeSlider.maxValue = maxLife;
        lifeSlider.value = life;

        m_navMesh = GetComponent<NavMeshManager>();
        m_anim = GetComponent<Animator>();

        Init();
    }

    protected virtual void Init() { }

    public void GetHeal(int amount)
    {
        if (life < maxLife)
        {
            //Instantiate(healVFX, transform);
            life += amount;
        }
        UpdateSlider();

        AdditionalHeal();
    }

    protected virtual void AdditionalHeal() { }

    public void TakeDamage(int damage, Vector3 knockback =new Vector3())
    {
        life -= damage;

        if(knockback!=null)
        {
            m_navMesh.Knocked(knockback);
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

    protected void Hit()
    {
        UpdateSlider();

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
