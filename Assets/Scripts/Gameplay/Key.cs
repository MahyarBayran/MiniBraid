using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    // time rewind support
    List<Vector3> pickupPositions;
    List<KeyTimeState> states;
    bool isRewinding;
    bool hasOwner;
    bool isBroken;

    [SerializeField]
    GameObject knight;

    [SerializeField]
    bool timeRewindable;

    [SerializeField]
    Sprite brokenKeySprite;

    [SerializeField]
    Sprite intactKeySprite;

    Transform initialParent;

    Vector3 localPos;

    Rigidbody2D rb2d;

    BoxCollider2D col2d;
   
    public bool IsBroken
    {
        get { return isBroken; }
    }

    void Awake()
    {
        states = new List<KeyTimeState>();
        pickupPositions = new List<Vector3>();

        isRewinding = false;
        isBroken = false;
        hasOwner = false;

        // setup events
        // EventManager.AddChestOpenedListener(GetBroken);
    }

    // Start is called before the first frame update
    void Start()
    {
        CapsuleCollider2D collider2D = knight.GetComponent<CapsuleCollider2D>();
        float halfColHeight = collider2D.size.y / 2;
        float colOffSetY = collider2D.offset.y;
        float keyPositionOffsetY = halfColHeight + colOffSetY + 0.4f;
        localPos = keyPositionOffsetY * Vector3.up;

        initialParent = transform.parent;

        spriteRenderer = GetComponent<SpriteRenderer>();
        GetFixed();

        rb2d = GetComponent<Rigidbody2D>();
        col2d = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRewindable)
        {
            // check for rewinds
            if (Input.GetButtonDown("Fire1"))
                StartRewind();
            if (Input.GetButtonUp("Fire1"))
                StopRewind();

            if (isRewinding)
            {
                PopState();
            }
            else
            {
                PushState();
            }
        }
    }

    public void GetBroken()
    {
        isBroken = true;
        spriteRenderer.sprite = brokenKeySprite;
        pickupPositions.Add(transform.position);
    }

    void GetFixed()
    {
        isBroken = false;
        spriteRenderer.sprite = intactKeySprite;
    }

    public void SetOwner()
    {
        if (!hasOwner && !isBroken)
        {
            pickupPositions.Add(transform.position);
            transform.parent = knight.transform;
            transform.localPosition = localPos;
            hasOwner = true;
            rb2d.isKinematic = true;
            col2d.enabled = false;

            // sound effect
            if (timeRewindable)
            {
                AudioManager.PlayOneShot(AudioClipName.KeyPickUp);
            }
            else
            {
                AudioManager.PlayOneShotNonRewindable(AudioClipName.KeyPickUp);
            }
        }
    }

    public void RemoveOwner()
    {
        transform.parent = initialParent;
        hasOwner = false;

        int lastID = pickupPositions.Count - 1;
        if (lastID > -1)
        {
            transform.position = pickupPositions[lastID];
            pickupPositions.RemoveAt(lastID);
        }
        

        rb2d.isKinematic = false;
        col2d.enabled = true;
    }

    void StartRewind()
    {
        isRewinding = true;
    }

    void StopRewind()
    {
        isRewinding = false;
    }

    void PushState()
    {
        KeyTimeState kts = new KeyTimeState();

        kts.isBroken = isBroken;
        kts.hasOwner = hasOwner;

        states.Add(kts);
    }

    void PopState()
    {
        int lastIndex = states.Count - 1;
        if (lastIndex > -1)
        {
            KeyTimeState kts = states[lastIndex];
            states.RemoveAt(lastIndex);

            if (isBroken && !kts.isBroken)
            {
                GetFixed();
            }
            else if (!isBroken && kts.isBroken)
            {
                GetBroken();
            }

            if (hasOwner && !kts.hasOwner)
            {
                RemoveOwner();
            }
            else if (!hasOwner && kts.hasOwner)
            {
                SetOwner();
            }
        }
    }
}
