using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

    static public Hero S;
    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    [Header("Set Dynamically")]
    private GameObject lastTriggerGo = null;
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;
    [SerializeField]
    private float _shieldLevel = 1;

    private void Start()
    {
        if (S == null)
        {
            S = this;
        } 
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return weapons[i];
            }
        }
        return null;
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;
        if (go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        { 
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                
                foreach (Weapon w in weapons)
                {
                    w.SetType(pu.type);
                }
                
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

}
