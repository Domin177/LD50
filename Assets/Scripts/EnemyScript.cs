using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;
using Pathfinding;
using Weapons;

public class EnemyScript : MonoBehaviour
{
    [Tooltip("How fast can enemy move as default")][SerializeField] 
    private float movementSpeed = 2.0f;

    [SerializeField][Tooltip("How much experience can player get")]
    private float experiencePerKill;

    [SerializeField][Tooltip("What items can enemy drops after kill")]
    private List<GameObject> dropItems;

    private List<GameObject> _droppableItems;
    [SerializeField]
    private GameObject letter;

    private PlayerScript _playerScript;
    private ShipScript _shipScript;

    private GameObject _target;

    [SerializeField]
    private float health = 50f;

    private float _maxHealth;

    [SerializeField] private float damage = 5f;

    [SerializeField] private float attackDelay = 5f;

    private float _nextAttack;

    private float _nextWaypointDistance = 0.3f;

    private Path _path;
    private int _currentWaypoint;
    private bool _reachedEndOfPath;
    private bool _dropped;

    private Animator _animator;

    private Seeker _seeker;
    private Rigidbody2D _rb;
    private Transform _enemyGFX;
    private SoundScript _soundScript;

    private Transform _healthBar;

    private bool _attacking;
    private bool _attackingFence;

    private FenceScript _fenceScript;

    private bool _pathPossible = true;

    [SerializeField] private bool attackingPlayer;
    
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Attacking = Animator.StringToHash("Attacking");

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        _target = GameObject.Find("Ship");
        _playerScript = player.GetComponent<PlayerScript>();
        _shipScript = _target.GetComponent<ShipScript>();
        _healthBar = transform.Find("HealthBar");
        _maxHealth = health;
        _enemyGFX = transform.Find("EnemyGFX");
        _animator = _enemyGFX.gameObject.GetComponent<Animator>();
        _soundScript = GameObject.Find("Sound").GetComponent<SoundScript>();

        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();

        if (attackingPlayer)
        {
            _target = _playerScript.gameObject;
        }

        UpdateDropItems();

        FindPath();

        InvokeRepeating(nameof(CheckPath), 0f, .5f);
    }

    private void FindPath()
    {
        if (_pathPossible)
        {
            _seeker.StartPath(_rb.position, _target.transform.position, OnPathComplete);
            GraphNode node1 = AstarPath.active.GetNearest(_rb.position, NNConstraint.Default).node;
            GraphNode node2 = AstarPath.active.GetNearest(_target.transform.position, NNConstraint.Default).node;
            _pathPossible = PathUtilities.IsPathPossible(node1, node2);
        }
    }
    
    private void CheckPath()
    {
        if ((!_attacking && !_attackingFence) || attackingPlayer)
        {
            FindPath();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalVariables.Running || GlobalVariables.GameOver) return;
        UpdatePositionAI();
    }
    
    private Vector2 GetBasicDirection()
    {
        return ((Vector2)_target.transform.position - (Vector2)transform.position).normalized;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
        }
    }

    private void UpdatePositionAI()
    {
        if (_attackingFence)
        {
            DoAttackFence();
            return;
        }
        if (_attacking)
        {
            DoAttack();
            if (!attackingPlayer) return;
        }
        if (_path == null) return;

        Vector2 direction;
        Vector2 position = _rb.position;
        if (_pathPossible)
        {
            if (_currentWaypoint >= _path.vectorPath.Count)
            {
                _reachedEndOfPath = true;
                _animator.SetBool(Running, false);
                if (attackingPlayer)
                {
                    _animator.SetBool(Attacking, true);
                    _attacking = true;
                    DoAttack();
                }

                return;
            }
            else
            {
                _reachedEndOfPath = false;
                _animator.SetBool(Running, true);
                _animator.SetBool(Attacking, false);
                _attacking = false;
            }
            
            direction = ((Vector2)_path.vectorPath[_currentWaypoint] - position).normalized;
            
            float distance = Vector2.Distance(position, _path.vectorPath[_currentWaypoint]);
        
            transform.Translate(direction * (movementSpeed * Time.deltaTime));
        
            if (distance < _nextWaypointDistance)
            {
                _currentWaypoint++;
            }
        }
        else
        {
            _animator.SetBool(Running, true);
            direction = GetBasicDirection();
            if (attackingPlayer)
            {
                if (Math.Abs(Vector3.Distance(_playerScript.gameObject.transform.position, position)) > 0.35f)
                {
                    _attacking = false;
                }
            }
            transform.Translate(direction * (movementSpeed * Time.deltaTime));
        }

        Vector3 scale = _enemyGFX.transform.localScale;
        if (direction.x > 0)
        {
            _enemyGFX.transform.localScale = new Vector3(Math.Abs(scale.x), scale.y, scale.z);
        } else if (direction.x < 0)
        {
            _enemyGFX.transform.localScale = new Vector3(Math.Abs(scale.x) * -1, scale.y, scale.z);
        }
    }

    private void DoAttack()
    {
        if (Time.time > _nextAttack)
        {
            _nextAttack = Time.time + attackDelay;
            
            _soundScript.PlayShipHitSound();
            
            if (attackingPlayer)
            {
                _playerScript.AttackOneMe(transform.position, damage);
            }
            else
            {
                _shipScript.AttackOneMe(damage);
            }
        }
    }

    public void StopFenceAttack()
    {
        _attackingFence = false;
        _animator.SetBool(Attacking, false);
        _animator.SetBool(Running, true);
        _fenceScript = null;
    }
    
    private void DoAttackFence()
    {
        if (Time.time > _nextAttack)
        {
            _nextAttack = Time.time + attackDelay;
            
            _soundScript.PlayShipHitSound();

            _fenceScript.AttackOneMe(damage);
        }
    }

    private void DropRandomItem()
    {
        Vector3 pos = transform.position;
        if (Random.Range(0f, 1f) > 0.2f && !_dropped)
        {
            _dropped = true;
            Instantiate(_droppableItems[Random.Range(0, _droppableItems.Count)], new Vector3(pos.x - 0.1f, pos.y - 0.1f, pos.z), Quaternion.identity);
        }
        Instantiate(letter, new Vector3(pos.x + 0.1f, pos.y + 0.1f, pos.z), Quaternion.identity);
        
    }

    private void UpdateDropItems()
    {
        List<GameObject> drops = new List<GameObject>();
        
        for (int i = 0; i < dropItems.Count; i++)
        {
            if (i == 0)
            {
                if (_playerScript.GetLevel() >= MachineGun.AvailableFromLevel) drops.Add(dropItems[i]);
            } 
            else if (i == 1)
            {
                if (_playerScript.GetLevel() >= Shotgun.AvailableFromLevel) drops.Add(dropItems[i]);
            }
            else
            {
                drops.Add(dropItems[i]);
            }
        }

        _droppableItems = drops;
    }

    private void StartAttacking()
    {
        _animator.SetBool(Attacking, true);
        _animator.SetBool(Running, false);
        _attacking = true;
        _currentWaypoint = 0;
        _path = null;
    }
    
    private void StartAttackingFence()
    {
        _animator.SetBool(Attacking, true);
        _animator.SetBool(Running, false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            float damage = other.gameObject.GetComponent<BulletScript>().damage;
            health -= damage;
            
            _soundScript.PlayHitSound();

            if (health > 0)
            {
                Vector3 scale = _healthBar.localScale;
                _healthBar.localScale = new Vector3(Utils.normalize(health, _maxHealth, 0.3f), scale.y, scale.z);
            }
            
            
            if (health <= 0)
            {
                _playerScript.AddExperience(experiencePerKill);
                DropRandomItem();
                Destroy(gameObject);
            }
        }

        if (other.gameObject.CompareTag("Fence"))
        {
            _attackingFence = true;
            _animator.SetBool(Attacking, true);
            _animator.SetBool(Running, false);
            _fenceScript = other.gameObject.GetComponent<FenceScript>();
            _fenceScript.AddAttacker(this);
        }

        if (_attackingFence) return;
        
        if (attackingPlayer)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartAttacking();
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Target"))
            {
                StartAttacking();
            }
        }
    }
    
    public float GetDamage()
    {
        return damage;
    }
}
