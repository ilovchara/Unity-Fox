using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    //����ֵ
    public int maxHealth = 5;
    //����˺�ʱ��
    public float timeInvincible = 2.0f;

    public int health { get { return currentHealth; } }
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    // �� - ��
    float horizontal;
    float vertical;

    //����
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    // Ԥ�Ƽ�
    public GameObject projectilePrefab;

    private AudioSource audioSource;



    // ִֻ��һ��
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        //��ʼ��
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // ��ִ֡��
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer < 0 )
            {
                isInvincible = false;
            }
        }
        // ʵ���� - ��c�����ӵ�
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
        // ���ƶԻ�
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (hit.collider != null)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = transform.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }
    // �ı�Ѫ��
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible) { return; }

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }


        //ʹ�� Mathf.Clamp() ����ȷ������ֵ��ָ����Χ�ڣ�0 �� maxHealth ֮�䣩
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        // ��ӡ
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    // �����ӵ�����
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
