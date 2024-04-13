using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    //生命值
    public int maxHealth = 5;
    //造成伤害时间
    public float timeInvincible = 2.0f;

    public int health { get { return currentHealth; } }
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    // 横 - 纵
    float horizontal;
    float vertical;

    //动画
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    // 预制件
    public GameObject projectilePrefab;

    private AudioSource audioSource;



    // 只执行一次
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        //初始化
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // 按帧执行
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
        // 实例化 - 按c发射子弹
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
        // 控制对话
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
    // 改变血量
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible) { return; }

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }


        //使用 Mathf.Clamp() 函数确保健康值在指定范围内（0 到 maxHealth 之间）
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        // 打印
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    // 发射子弹函数
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
