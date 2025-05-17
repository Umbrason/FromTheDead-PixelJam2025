using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerAmmunition ammunition;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite[] Images;
    [SerializeField] private Image template;
    [SerializeField] private RectTransform container;
    void Update()
    {
        backgroundImage.sprite = Images[Mathf.FloorToInt(Time.time / 4) % Images.Length];
    }

    void OnEnable()
    {
        ammunition.OnShootingQueueChanged += UpdateUI;
    }


    void Clear()
    {
        foreach (var icon in icons)
            Destroy(icon.gameObject);
        icons.Clear();
    }

    private readonly List<Image> icons = new();
    private void UpdateUI()
    {
        Clear();
        var projectiles = ammunition.ShootingQueue.Take(5);
        foreach (var projectile in projectiles.Reverse())
        {
            var icon = Instantiate(template, container);
            icon.sprite = projectile.UIIcon;
            icons.Add(icon);
        }
    }
}
