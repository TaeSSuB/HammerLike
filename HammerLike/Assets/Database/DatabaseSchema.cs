using BennyKok.NotionAPI;
using System;

[Serializable]
public class DatabaseSchema
{
    public NumberProperty armor;
    public MultiSelectProperty _type;
    public NumberProperty detectRange;
    public NumberProperty knockbackPower;
    public NumberProperty movementSpeed;
    public NumberProperty healthPoint;
    public CheckboxProperty Build;
    public NumberProperty attackSpeed;
    public NumberProperty attackRange;
    public NumberProperty knockbackResistance;
    public NumberProperty index;
    public TextProperty _koreanName;
    public NumberProperty attackPower;
    public MultiSelectProperty _category;
    public NumberProperty weight;
    public TitleProperty entityName;
}