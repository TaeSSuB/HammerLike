using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Equipment/Weapon")]
public class Weapon : EquipItem
{    
    // �ʿ��� �߰����� ���� Ư��
    public GameObject prefab; // ���� ������

    public int damage; 
    public float weight;
    public float knockback;

    public ElementalType elementalType;
    public Skill unlockableSkill; // �߰� ��ų �ر�. Skill�� ������ ���ǵ� ScriptableObject�� �� �� �ֽ��ϴ�.

    // Weapon Ŭ������ ��� ����...
}
