using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public static PlayerResources instance;

    public int money = 100;
    public int restos = 0;

    void Awake()
    {
        instance = this;
    }

    public bool CanAfford(int costMoney, int costRestos)
    {
        return money >= costMoney && restos >= costRestos;
    }

    public bool Spend(int costMoney, int costRestos)
    {
        if (!CanAfford(costMoney, costRestos))
            return false;

        money -= costMoney;
        restos -= costRestos;

        return true;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void AddRestos(int amount)
    {
        restos += amount;
    }
}