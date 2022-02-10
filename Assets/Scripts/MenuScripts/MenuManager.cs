using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] Menu[] menus;

    private void Awake()
    {
        Instance = this;
    }
    public void OpenMenu(string name)
    {
        for(int i = 0; i < menus.Length; i++)
        {
            if(name == menus[i].menuName)
            {
                menus[i].Open();
            } else
            {
                menus[i].Close();
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menu == menus[i])
            {
                menus[i].Open();
            }
            else
            {
                menus[i].Close();
            }
        }
    }
    public void CloseMenu(string name)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (name == menus[i].menuName)
            {
                menus[i].Close();
            }
        }
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }


}
