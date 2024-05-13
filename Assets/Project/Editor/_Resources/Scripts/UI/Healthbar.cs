using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public SelectManager selectManager;
    public Slider mainHealthBar;
    public Slider mainPowerBar;
    public Slider healthSlider;
    public Image scale;
    public Slider easeHealthSlider;
    public SelectableCharacter character;
    public float lerpSpeed = 0.15f;
    public GameObject visualHealthbar;
    public GameObject visualEaseHealthbar;
    public GameObject visualBackground;
    public float timeToShow = 1f;
    public float timeToShowOver = 0f;
    // Start is called before the first frame update
    void Start()
    {
        // gameObject.SetActive(false);
        selectManager = GameObject.Find("SelectingBox").GetComponent<SelectManager>();
        healthSlider = transform.Find("Healthbar").GetComponent<Slider>();
        visualHealthbar = transform.Find("Healthbar").transform.Find("Scale").gameObject;
        scale = healthSlider.transform.Find("Scale").GetComponent<Image>();
        easeHealthSlider = transform.Find("EaseHealthbar").GetComponent<Slider>();
        visualEaseHealthbar = transform.Find("EaseHealthbar").transform.Find("Scale").gameObject;
        visualBackground = transform.Find("EaseHealthbar").transform.Find("Background").gameObject;
        character = transform.parent.transform.Find("Selection").GetComponent<SelectableCharacter>();
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
        transform.SetParent(GameObject.Find("Canvas").transform);
    }

    // Update is called once per frame
    void Update()
    {
        var currentHealth = Mathf.Clamp(character.health / character.maxHealth * 100, 0, 100);

        if (mainHealthBar == null && GameObject.Find("UnitInfo") != null)
            mainHealthBar = GameObject.Find("Health&PowerBar").transform.Find("HealthBar").GetComponent<Slider>();
        if (selectManager.selectedArmy.Any())
        {
            if (character == selectManager.selectedArmy[0])
            {
                if (mainHealthBar.value != currentHealth)
                    mainHealthBar.value = currentHealth;
            }
        }

        if (mainPowerBar == null && GameObject.Find("UnitInfo") != null)
            mainPowerBar = GameObject.Find("Health&PowerBar").transform.Find("PowerBar").GetComponent<Slider>();
        if (selectManager.selectedArmy.Any())
        {
            if (character == selectManager.selectedArmy[0])
            {
                var currentPower = Mathf.Clamp(character.power / character.maxPower * 100, 0, 100);
                if (mainPowerBar.value != currentPower)
                    mainPowerBar.value = currentPower;
            }
        }

        if (currentHealth != healthSlider.value)
        {
            visualHealthbar.SetActive(true);
            visualEaseHealthbar.SetActive(true);
            visualBackground.SetActive(true);
            healthSlider.value = currentHealth;
            timeToShowOver = Time.time + timeToShow;
        }

        if (visualHealthbar.activeSelf)
        {
            transform.position = Camera.main.WorldToScreenPoint(character.transform.position) - Vector3.up * 10;
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed);
            if (healthSlider.value > 60 && healthSlider.value <= 100)
                scale.color = Color.green;
            if (healthSlider.value <= 60 && healthSlider.value >= 35)
                scale.color = Color.yellow;
            if (healthSlider.value < 35)
                scale.color = Color.red;
            if (character.health <= 0)
                Destroy(gameObject);
        }

        if (Time.time > timeToShowOver)
        {
            visualHealthbar.SetActive(false);
            visualEaseHealthbar.SetActive(false);
            visualBackground.SetActive(false);
        }

        /*
            var healthbarTmp = Instantiate(healthbar, Camera.main.WorldToScreenPoint(target.transform.position) - Vector3.up * 3, Quaternion.LookRotation(Vector3.forward));
            healthbarTmp.transform.SetParent(GameObject.Find("Canvas").transform);
            healthSlider = healthbarTmp.transform.Find("Healthbar").GetComponent<Slider>();
            easeHealthSlider = healthbarTmp.transform.Find("EaseHealthbar").GetComponent<Slider>();
            healthSlider.value = Mathf.Clamp((target.health / target.maxHealth) * 100, 0, 100);
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed);
            Destroy(healthbarTmp, 1.2f);
        */
    }
}
