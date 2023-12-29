using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPiece : MonoBehaviour
{
    public enum ElementType
    {
        FIRE,
        ICE,
        WIND,
        LIGHT,
        DARK,
        ANY,
        COUNT
    }

    [System.Serializable]
    public struct ElementSprite
    {
        public ElementType element;
        public Sprite sprite;
    };

    public ElementSprite[] elementSprites;

    private ElementType element;

    public ElementType Element{
        get{return element;}
        set{SetElement(value);}
    } 

    public int NumElements
    {
        get{return elementSprites.Length;}
    }

    private SpriteRenderer sprite;
    private Dictionary<ElementType, Sprite> elementSpriteDict;

    void Awake()
    {
        sprite = transform.Find("piece").GetComponent<SpriteRenderer>();
        elementSpriteDict = new Dictionary<ElementType, Sprite>();
        for(int i = 0; i < elementSprites.Length; i++){
            elementSpriteDict.Add(elementSprites[i].element, elementSprites[i].sprite);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetElement(ElementType newElement)
    {
        element = newElement; 
        if(elementSpriteDict.ContainsKey(newElement)){
            sprite.sprite = elementSpriteDict[newElement];
        }
    }
}
