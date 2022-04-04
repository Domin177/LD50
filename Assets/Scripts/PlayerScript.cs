using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Weapons;

public class PlayerScript : MonoBehaviour
{
    [Tooltip("How fast can player move")][SerializeField] 
    private float movementSpeed = 4.5f;

    [SerializeField][Tooltip("Weapon hard point")]
    private GameObject weaponHardPoint;
    
    [SerializeField]
    private GameObject fence;

    [SerializeField] private List<Sprite> weaponsUpperParts;
    
    [SerializeField]
    private float health = 100f;

    private Dictionary<CollectableScript.CollectType, int> _ammoAmount = new Dictionary<CollectableScript.CollectType, int>();

    private float _maxHealth;

    private Weapon _weapon;
    private int _lettersAmount;
    private bool _sendEnabled;

    private Camera _camera;
    private bool _shooting;
    private int _level = 1;
    private int _weaponIndex = 0;
    private float _experience;
    private float _experienceForNextLevel = 100;
    private float _basicExperienceForNextLevel = 100;
    private float _experiencePerLetter = 20;

    private bool _building;
    private GameObject _actualFence;
    private Transform _upperPartPivot;
    private SpriteRenderer _upperPartSprite;
    private FenceScript _actualFenceScript;
    private Rigidbody2D _rb;
    private ShipScript _shipScript;
    private SoundScript _soundScript;

    private SquareLoaderScript _fenceLoader;
    private GameObject _fenceSquare;
    private readonly List<SquareData> _squares = new List<SquareData>();
    private readonly List<Weapon> _availableWeapons = new List<Weapon>();
    private readonly Dictionary<KeyCode, int> _weaponsMap = new Dictionary<KeyCode, int>();

    private Transform _healthBar;
    private Text _scoreText;
    private Text _messagesText;
    private Text _levelText;
    private Text _XPText;
    private Text _nextLevelXPText;
    
    private Animator _animator;
    private static readonly int Running = Animator.StringToHash("Running");

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _availableWeapons.Add(new Pistol());
        _availableWeapons.Add(new MachineGun());
        _availableWeapons.Add(new Shotgun());
        _weapon = _availableWeapons[0];
        _maxHealth = health;

        _ammoAmount.Add(CollectableScript.CollectType.PistolAmmo, -999);
        _ammoAmount.Add(CollectableScript.CollectType.RifleAmmo, 70);
        _ammoAmount.Add(CollectableScript.CollectType.ShotgunAmmo, 40);
        
        _weaponsMap.Add(KeyCode.Alpha1, 0);
        _weaponsMap.Add(KeyCode.Alpha2, 1);
        _weaponsMap.Add(KeyCode.Alpha3, 2);
        
        _squares.Add(new SquareData(GameObject.Find("Square1"), CollectableScript.CollectType.PistolAmmo));
        _squares.Add(new SquareData(GameObject.Find("Square2"), CollectableScript.CollectType.RifleAmmo));
        _squares.Add(new SquareData(GameObject.Find("Square3"), CollectableScript.CollectType.ShotgunAmmo));
        
        GameObject bottomBar = GameObject.Find("LeftBottomBar");
        _healthBar = bottomBar.transform.Find("HealthBarBG").Find("HealthBar");
        _scoreText = bottomBar.transform.Find("ScoreText").GetComponent<Text>();
        _messagesText = bottomBar.transform.Find("MessagesText").GetComponent<Text>();
        _levelText = bottomBar.transform.Find("LevelText").GetComponent<Text>();
        _XPText = GameObject.Find("LevelInfo").transform.Find("XPText").GetComponent<Text>();
        _nextLevelXPText = GameObject.Find("LevelInfo").transform.Find("NextLevelXPText").GetComponent<Text>();

        _fenceLoader = GameObject.Find("Square4").transform.Find("LoadingBar").GetComponent<SquareLoaderScript>();
        _fenceSquare = GameObject.Find("Square4").transform.Find("Selected").gameObject;

        _soundScript = GameObject.Find("Sound").GetComponent<SoundScript>();

        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _shipScript = GameObject.Find("Ship").GetComponent<ShipScript>();
        _upperPartPivot = transform.Find("Pivot");
        _upperPartSprite = _upperPartPivot.transform.Find("UpperPart").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalVariables.Running || GlobalVariables.GameOver) return;
        
        UpdatePosition();
        UpdateHardPointFacing();
        ChooseWeapon();
        UpdateRotation();
        CheckSending();
        UpdateTexts();
        if (_building)
        {
            PutFence();
        }
        else
        {
            Shoot();
        }
    }

    private void UpdatePosition()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(horizontalMovement, verticalMovement, 0);

        _animator.SetBool(Running, horizontalMovement != 0 || verticalMovement != 0);
        
        direction.x *= movementSpeed + 0.5f;
        direction.y *= movementSpeed + 0.5f;
        transform.Translate(direction * Time.deltaTime);
    }

    private void UpdateTexts()
    {
        _messagesText.text = "Messages: " + _lettersAmount;
        _levelText.text = "Level: " + _level;
        _scoreText.text = "Score: " + Stats.LettersSent;
        Vector3 scale = _healthBar.localScale;
        _healthBar.localScale = new Vector3(health / _maxHealth, scale.y, scale.z);
        _XPText.text = "XP: " + _experience;
        _nextLevelXPText.text = "Next level XP: " + _experienceForNextLevel;
    }

    private void UpdateRotation()
    {
        Vector3 scale = transform.localScale;
        
        Vector3 mouseScreenPosition = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.transform.position.z * -1));

        float modifier = 1f;
        if (mouseScreenPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(Math.Abs(scale.x), scale.y, scale.z);
        }
        else
        {
            transform.localScale = new Vector3(Math.Abs(scale.x) * -1, scale.y, scale.z);
            modifier = -1f;
        }
        
        Vector3 lookAt = mouseScreenPosition;

        float angleRad = Mathf.Atan2(lookAt.y - _upperPartPivot.transform.position.y, 1);
        float angleDeg = ((180 * modifier) / Mathf.PI) * angleRad;

        _upperPartPivot.transform.rotation = Quaternion.Euler(0, 0, angleDeg);
    }

    private void ChooseWeapon()
    {
        foreach (var weapon in _weaponsMap.Where(weapon => Input.GetKeyDown(weapon.Key)).Where(weapon => _availableWeapons.Count > weapon.Value))
        {
            var choosedWeapon = _availableWeapons[weapon.Value];
            if (!choosedWeapon.IsAvailable(_level)) return;


            _weapon = choosedWeapon;
            _upperPartSprite.sprite = weaponsUpperParts[weapon.Value];
            
            foreach (SquareData squareData in _squares)
            {
                squareData.SetSelected(false);
            }
            
            _squares[weapon.Value].SetSelected(true);
            _weaponIndex = weapon.Value;
            _soundScript.PlayChooseSound();
            
            if (_building)
            {
                Destroy(_actualFence);
                _building = false;
                _fenceSquare.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !_building && _fenceLoader.IsAvailable())
        {
            Vector3 position = _camera.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            _soundScript.PlayChooseSound();
            _actualFence = Instantiate(fence, position, Quaternion.identity);
            _actualFenceScript = _actualFence.GetComponent<FenceScript>();
            _fenceSquare.SetActive(true);
            _building = true;
            
        }
    }

    private void CheckSending()
    {
        if (!_sendEnabled) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Stats.LettersSent += _lettersAmount;
            _soundScript.PlayMessageSendSound();
            AddExperience(_lettersAmount * _experiencePerLetter);
            _lettersAmount = 0;
            _sendEnabled = false;
            _shipScript.Send();
        }
    }
    
    
    private void UpdateHardPointFacing()
    {
        Vector3 pos = _camera.WorldToScreenPoint(weaponHardPoint.transform.position);
        Vector3 dir = Input.mousePosition - pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        weaponHardPoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _shooting = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _shooting = false;
        }

        if (_shooting)
        {
            if (HasAmmo())
            {
                if (_weapon.Shoot(weaponHardPoint.transform, _level))
                {
                    _soundScript.PlayWeaponSound(_weaponIndex);
                    SubtractAmmo();
                }

            }
            else
            {
                //TODO write error
            }
            
        }
    }

    private bool HasAmmo()
    {
        int ammo = _ammoAmount[_weapon.GetAmmoType()];
        return ammo == -999 || ammo > 0;
    }

    private void SubtractAmmo()
    {
        if (_weapon.GetAmmoType() != CollectableScript.CollectType.PistolAmmo)
        {
            _ammoAmount[_weapon.GetAmmoType()] -= 1;
            if (_ammoAmount[_weapon.GetAmmoType()] < 0)
            {
                _ammoAmount[_weapon.GetAmmoType()] = 0;
            }
            _squares[_weaponIndex].SetAmmo(_ammoAmount[_weapon.GetAmmoType()]);
        }
    }

    private void PutFence()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _actualFenceScript.Put();
            _soundScript.PlayPutWallSound();
            _fenceLoader.SetCooldown();
            _fenceSquare.SetActive(false);
            _building = false;
        }
    }

    public int GetLevel()
    {
        return _level;
    }

    public void AddExperience(float xp)
    {
        _experience += xp;
        if (_experience >= _experienceForNextLevel)
        {
            float diff = _experienceForNextLevel - _experience;
            _level++;

            for (int i = 0; i < _availableWeapons.Count; i++)
            {
                if (_availableWeapons[i].IsAvailable(_level))
                {
                    SquareData sq = _squares[i];
                    sq.ShowWeaponSprite();
                    sq.SetAmmo(_ammoAmount[sq.GetCollectType()]);
                }
            }
            
            _experience += diff;
            _experienceForNextLevel = _basicExperienceForNextLevel * _level;

        }
    }

    public void AttackOneMe(Vector3 attackerPosition, float damage)
    {
        
        Vector2 direction = (transform.position - attackerPosition).normalized;
        Vector2 force = direction * 10000f * Time.deltaTime;
        
        _rb.AddForce(force);
        health -=  damage;

        if (health <= 0)
        {
            health = 0;
            GlobalVariables.GameOver = true;
        }
    }

    public void Collect(CollectableScript.CollectType collectType, int amount)
    {
        _soundScript.PlayPickSound();
        switch (collectType)
        {
            case CollectableScript.CollectType.RifleAmmo:
                _ammoAmount[CollectableScript.CollectType.RifleAmmo] += amount;
                AddAmmo(CollectableScript.CollectType.RifleAmmo);
                break;
            case CollectableScript.CollectType.ShotgunAmmo:
                _ammoAmount[CollectableScript.CollectType.ShotgunAmmo] += amount;
                AddAmmo(CollectableScript.CollectType.ShotgunAmmo);
                break;
            case CollectableScript.CollectType.HealthPoint:
                health += amount;
                if (health > _maxHealth)
                {
                    health = _maxHealth;
                }
                break;
            case CollectableScript.CollectType.Letter:
                _lettersAmount += amount;
                break;
            case CollectableScript.CollectType.RepairKit:
                _shipScript.AddHealth(amount);
                break;
        }
    }

    private void AddAmmo(CollectableScript.CollectType collectType)
    {
        foreach (SquareData sq in _squares)
        {
            if (sq.GetCollectType() == collectType) sq.SetAmmo(_ammoAmount[collectType]);
        }
    }

    public void SendEnabled(bool enabled)
    {
        _sendEnabled = enabled;
    }

    public int LettersHolding()
    {
        return _lettersAmount;
    }

    private class SquareData
    {
        private readonly GameObject _weaponSprite;
        private readonly Text _ammoText;
        private readonly GameObject _selected;
        private readonly CollectableScript.CollectType _collectType;

        public SquareData(GameObject square, CollectableScript.CollectType collectType)
        {
            _weaponSprite = square.transform.Find("Type").gameObject;
            _ammoText = square.transform.Find("Ammo").GetComponent<Text>();
            _selected = square.transform.Find("Selected").gameObject;
            _collectType = collectType;
        }

        public void SetAmmo(int ammo)
        {
            if (_collectType != CollectableScript.CollectType.PistolAmmo)
            {
                _ammoText.text = ammo.ToString();
            }
        }

        public void SetSelected(bool selected)
        {
            _selected.SetActive(selected);
        }

        public void ShowWeaponSprite()
        {
            _weaponSprite.SetActive(true);
        }

        public CollectableScript.CollectType GetCollectType()
        {
            return _collectType;
        }
    }
}
