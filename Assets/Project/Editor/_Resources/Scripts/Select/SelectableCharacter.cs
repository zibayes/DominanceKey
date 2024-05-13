using System.Collections.Generic;
using UnityEngine;
using VariableInventorySystem.Sample;

public class SelectableCharacter : MonoBehaviour {
    public PlayerController playerController;
    public Rigidbody selfRigidbody;

    public SpriteRenderer unselectedSprite;
    public SpriteRenderer selectedSprite;
    public SpriteRenderer currentSprite;

    public Texture portrait;
    public string personName;
    public string nationality;
    public string specialization;
    public List<InventoryItem> start_inventory_items = new List<InventoryItem>();
    public List<ItemCellData> inventory_items = new List<ItemCellData>();
    public ItemCellData currentWeapon = null;
    public ItemCellData currentGrenade = null;
    public float maxHealth = 100;
    public float health = 100;
    public float power = 100;
    public float maxPower = 100;
    public int player;

    public bool randomizeName = true;
    static public string[] firstnamesUS = "James\nJohn\nRobert\nMichael\nWilliam\nDavid\nRichard\nCharles\nJoseph\nThomas\nChristopher\nDaniel\nPaul\nMark\nDonald\nGeorge\nKenneth\nSteven\nEdward\nBrian\nRonald\nAnthony\nJason\nMatthew\nGary\nTimothy\nJose\nLarry\nJeffrey\nFrank\nScott\nEric\nStephen\nAndrew\nRaymond\nGregory\nJoshua\nJerry\nDennis\nWalter\nPatrick\nPeter\nHarold\nDouglas\nHenry\nCarl\nArthur\nRyan\nRoger\nJoe\nJuan\nJack\nAlbert\nJonathan\nJustin\nTerry\nGerald\nKeith\nSamuel\nWillie\nRalph\nLawrence\nNicholas\nRoy\nBenjamin\nBruce\nBrandon\nAdam\nHarry\nFred\nWayne\nBilly\nSteve\nLouis\nJeremy\nAaron\nRandy\nHoward\nEugene\nCarlos\nRussell\nBobby\nVictor\nMartin\nErnest\nPhillip\nTodd\nJesse\nCraig\nAlan\nShawn\nClarence\nSean\nPhilip\nChris\nJohnny\nEarl\nJimmy\nAntonio\nDanny\nBryan\nTony\nLuis\nMike\nStanley\nLeonard\nNathan\nDale\nManuel\nRodney\nCurtis\nNorman\nAllen\nMarvin\nVincent\nGlenn\nJeffery\nTravis\nJeff\nChad\nJacob\nLee\nMelvin\nAlfred\nKyle\nFrancis\nBradley\nJesus\nHerbert\nFrederick\nRay\nJoel\nEdwin\nDon\nEddie\nRicky\nTroy\nRandall\nBarry\nAlexander\nBernard\nMario\nLeroy\nFrancisco\nMarcus\nMicheal\nTheodore\nClifford\nMiguel\nOscar\nJay\nJim\nTom\nCalvin\nKevin\nChristian\n".Split('\n');
    static public string[] lastnamesUS = "Smith\nJohnson\nPalmer\nWilliams\nJones\nBrown\nDavis\nMiller\nWilson\nMoore\nTaylor\nAnderson\nThomas\nJackson\nWhite\nHarris\nMartin\nThompson\nGarcia\nMartinez\nRobinson\nClark\nRodriguez\nLewis\nLee\nWalker\nHall\nAllen\nYoung\nHernandez\nKing\nWright\nLopez\nHill\nScott\nGreen\nAdams\nBaker\nGonzalez\nNelson\nCarter\nMitchell\nPerez\nRoberts\nTurner\nPhillips\nCampbell\nParker\nEvans\nEdwards\nCollins\nStewart\nSanchez\nMorris\nRogers\nReed\nCook\nMorgan\nBell\nMurphy\nBailey\nRivera\nCooper\nRichardson\nCox\nHoward\nWard\nTorres\nPeterson\nGray\nRamirez\nJames\nWatson\nBrooks\nKelly\nSanders\nPrice\nBennett\nWood\nBarnes\nRoss\nHenderson\nColeman\nJenkins\nPerry\nPowell\nLong\nPatterson\nHughes\nFlores\nWashington\nButler\nSimmons\nFoster\nGonzales\nBryant\nAlexander\nRussell\nGriffin\nDiaz\nHayes\nCrosby\nGallaher\n".Split('\n');
    static public string[] firstnamesGer = "Achim\nAdolf\nAlbrecht\nAlexander\nAndreas\nArnold\nArnulf\nAxel\nAugustus\nBenjamin\nBernd\nBernhard\nBerthold\nBruno\nCarsten\nChristian\nDaniel\nDavid\nDennis\nDieter\nDietrich\nDirk\nDominik\nEgon\nElmar\nEmmerich\nErhard\nErich\nErnst\nErwin\nEugen\nFelix\nFlorian\nFrank\nFranz\nFriedhelm\nFriedrich\nFritz\nGebhard\nGeorg\nGerhard\nGernot\nGotthold\nGunther\nHans\nHarald\nHartmut\nHartwig\nHelge\nHelger\nHelmut\nHerbert\nHolger\nHugbert\nHugo\nIgnaz\nIngemar\nJan\nJens\nJohann\nJohannes\nJonas\nJorg\nJurgen\nKarl\nKarsten\nKaspar\nKevin\nKlaus\nKonrad\nKristian\nKristof\nKurt\nLaurenz\nLenz\nLeon\nLeonhard\nLothar\nLudwig\nLukas\nLuither\nManfred\nMarcel\nMarko\nMario\nMarius\nMarkus\nMartin\nMatthaus\nMathias\nMax\nMaximilian\nMeinhard\nMichael\nNikolaus\nNorbert\nOlof\nOswin\nPatrick\nPaul\nPeter\nPhilipp\nRaimund\nReiner\nRalf\nRandolf\nReinhard\nReinhold\nRene\nRichard\nRobert\nRudolf\nRupert\nSebastian\nSiegfried\nSimon\nStefan\nSteffen\nSven\nTheodor\nTim\nTimo\nTobias\nTorsten\nUdo\nUlf\nUlrich\nUrban\nUrs\nUwe\nVeit\nViktor\nVinzenz\nVolkard\nVolker\nWaldemar\nWalther\nWendel\nWenzel\nWernher\nWilfried\nWilhelm\nWolfgang\nWolfram\nTomas\nOtto\n".Split('\n');
    static public string[] lastnamesGer = "Adler\nAltmann\nArzt\nBader\nBauer\nBaum\nBaumann\nBecker\nBerlin\nBorg\nBrauer\nBraun\nBrenner\nBreuer\nBullwinkel\nCullen\nDannenberg\nDick\nDresdner\nEisen\nEisenhauer\nEisenhower\nErzberger\nFarber\nFischer\nFleischer\nFrank\nFuchs\nGebauer\nGerber\nGold\nGross\nGrossmann\nGruen\nHalpern\nHammer\nHartmann\nHellmann\nHermann\nHoffmann\nHolz\nHuber\nJung\nKaiser\nKaltenbach\nKeller\nKellerman\nKessler\nKlein\nKlutz\nKnopf\nKoch\nKoehler\nKoenig\nKramer\nKrauss\nKrieg\nKrieger\nKrueger\nKunstler\nKurz\nKuster\nLange\nLederer\nLedermann\nLehrer\nLesser\nLichtermann\nLichtman\nLowenthal\nLustig\nMaurer\nMeer\nMehler\nMehlinger\nMehlmann\nMelman\nMeltzer\nMetzger\nMeyer\nMuller\nNachtmann\nNagel\nNeumann\nPeters\nPfannnenschmidt\nPollack\nPostmann\nPuttkammer\nRabe\nRader\nReifsneider\nReifsnyder\nReiter\nRichter\nRockower\nRoth\nRothbart\nSaltz\nSaltzmann\nSandler\nSchafer\nSchatz\nSchenker\nScherer\nSchlesinger\nSchlosser\nSchluter\nSchmidt\nSchmuckler\nSchmuker\nSchneide\nSchneider\nSchreiber\nSchreiner\nSchroeder\nSchubert\nSchuhmann\nSchulmann\nSchultheis\nSchultz\nSchumacher\nSchuster\nSchwartz\nSchwarzkopf\nSchweitzer\nShapiro\nSilber\nSilberg\nStahl\nStamm\nStammler\nStein\nSteinhauer\nSternberg\nStock\nStroh\nStudebaker\nStump\nSussman\nUnruh\nWagner\nWaldschmidt\nWalter\nWeber\nWechsler\nWeiss\nWerner\nWolf\nZimmermann\nZumwald\nVolker\nLembek\n".Split('\n');

    void Update()
    {
        // if (health <= 0) // TODO: удалять из списка selectableChars
        //    Destroy(gameObject.transform.parent.gameObject);
    }

    private void Start() {
        selectedSprite.enabled = false;
        currentSprite.enabled = false;

        if (randomizeName)
        {
            if (nationality == "american")
                personName = firstnamesUS[Random.Range(0, firstnamesUS.Length)] + " " + lastnamesUS[Random.Range(0, lastnamesUS.Length)];
            if (nationality == "german")
                personName = firstnamesGer[Random.Range(0, firstnamesGer.Length)] + " " + lastnamesGer[Random.Range(0, lastnamesGer.Length)];
        }

        for (int i = 0; i < start_inventory_items.Count; i++)
        {
            inventory_items.Add(new ItemCellData(start_inventory_items[i]));
        }

        // Fill weapon mag on start
        for (int i = 0; i < inventory_items.Count; i++)
        {
            if (inventory_items[i].type == "weapon")
            {
                inventory_items[i].currentAmmo = inventory_items[i].magSize;
            }
        }

        if (player != playerController.selectManager.player)
            unselectedSprite.enabled = false;
        else
            unselectedSprite.enabled = true;

        foreach (Collider collider in playerController.colliders)
            if (collider.gameObject != playerController.gameObject)
                collider.enabled = true;
            else
                collider.enabled = false;

        // Вставка оружия со старта - первое попавшееся!!!
        for (int i = 0; i < inventory_items.Count; i++)
        {
            if (inventory_items[i].type == "weapon" && currentWeapon == null)
            {
                currentWeapon = inventory_items[i];

                var weaponHolder = playerController.weaponHolder;
                if (weaponHolder.childCount > 0)
                    Destroy(weaponHolder.GetChild(0).gameObject);
                var currentModel = Instantiate(currentWeapon.model, weaponHolder.transform);
                currentModel.transform.parent = weaponHolder.transform;
                var collectable = currentModel.GetComponent<Collectable>();
                playerController.SetWeapon();
                collectable.collider.enabled = false;
                collectable.rigidbody.isKinematic = true;
                collectable.outline.enabled = false;
                collectable.inventoryItem.enabled = false;
                collectable.enabled = false;
                break;
            } 
            // else if (inventory_items[i].type == "grenade" && currentWeapon == null)
        }

        StartCoroutine(playerController.UIController.WeaponChanging(true, this, 0f));
    }

    //Turns off the sprite renderer
    public void TurnOffSelector()
    {
        selectedSprite.enabled = false;
        unselectedSprite.enabled = true;
        currentSprite.enabled = false;
    }

    //Turns on the sprite renderer
    public void TurnOnSelector()
    {
        selectedSprite.enabled = true;
        unselectedSprite.enabled = false;
        currentSprite.enabled = false;
    }

    public void TurnOnCurrent()
    {
        selectedSprite.enabled = false;
        unselectedSprite.enabled = false;
        currentSprite.enabled = true;
    }

    public void TurnOffAll()
    {
        selectedSprite.enabled = false;
        unselectedSprite.enabled = false;
        currentSprite.enabled = false;
    }
}
