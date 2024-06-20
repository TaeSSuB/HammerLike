using BennyKok.NotionAPI;
using System;

[Serializable]
public class EntityDatabase
{
    public NumberProperty index;

    public CheckboxProperty Build;

    public TitleProperty entityName;
    public TextProperty _koreanName;

    public MultiSelectProperty _category;

    public NumberProperty weight;

    public NumberProperty armor;
    
    public MultiSelectProperty _type;
    
    public NumberProperty detectRange;
    
    public NumberProperty movementSpeed;
    public NumberProperty healthPoint;

    public NumberProperty attackPower;
    public NumberProperty attackRange;
    public NumberProperty attackSpeed;

    public NumberProperty knockbackPower;
    public NumberProperty knockbackResistance;
}

