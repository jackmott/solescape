using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;



[System.Serializable]
public class GameState : MonoBehaviour
{
    private static GameState instance;

    public int gameLength;
    public int yearsLeft;

    string planetName;
    public Planet planet;

    public Sun sun;


    public float energy;
    public float previousEnergy;
    public float pollution;
    public int population;
    public float iq;

    public float setiChance = 0;

    public float coalReserves;
    public float oilFactor;
    public float sunFactor;
    public float windFactor;
        

    public int populationKilled = 0;
    public int colonyShipsLaunched = 0;

    public float pollutionClearance;
    public float pollutionDeathAmount;

    public Hashtable researchTree;
    public Hashtable attainedResearch;
    public Hashtable availableResearch;

    public Hashtable buildings;
    public Hashtable availableBuildings;

    public List<PlanetInfo> planets;


    public Research pendingResearch = null;

    public bool pollutionLose = false;
    public bool timeLose = false;

    public bool showSeti = false;
    

    public bool scanning = false;
    public bool oilScan = false;
    public float scanCost = 1.0f;

    public Color[] oilColors;
    public Texture2D oilTex;

    StreamReader researchReader;

    public GameStats[] gameStats;

    void Awake()
    {

        instance = this;

        researchTree = new Hashtable();
        attainedResearch = new Hashtable();
        availableResearch = new Hashtable();

        buildings = new Hashtable();
        availableBuildings = new Hashtable();

        planets = new List<PlanetInfo>();

        GetData();

      


    }

    void Start()
    {
        GameObject perObj = (GameObject)GameObject.Find("Persistence");
        PlanetInfo pi;
        if (perObj == null)
        {
            pi = LoadPlanets()[0];
        }
        else
        {
            Persistence per = perObj.GetComponent<Persistence>();
            pi = per.pi;

        }

        planet.transform.localScale = new Vector3(pi.planetSize, pi.planetSize, pi.planetSize);
        planet.GeneratePlanetNoise(2048, 1024, pi);
        planet.BeginGame();
        gameStats = new GameStats[gameLength + 1];
        InvokeRepeating("UpdateState", 1f, 1.0f);
    }
   

    public static GameState Instance
    {
        get
        {
            while (instance == null)
            {
                System.Threading.Thread.Sleep(1000);
                print("GameState instance");
            }
            return instance;
        }
    }

    public void UpdateOilMap()
    {
        oilTex.SetPixels(oilColors);
        oilTex.Apply();
    }


    public void UpdateState()
    {
        GameStats stats = new GameStats();
        stats.buildings = new Dictionary<string, int>();
        stats.energy = energy;
        stats.pollution = pollution;
        stats.iq = iq;
        stats.population = population;

        
        for (int i = planet.placedBuildings.Count-1;i >=0;i--)
        {
            Building b = planet.placedBuildings[i];
            if (b.gameObject == null)
            if (!stats.buildings.ContainsKey(b.buildingName))
            {
                stats.buildings[b.buildingName] = 1;
            }
            else
            {
                int count = stats.buildings[b.buildingName];
                count++;
                stats.buildings[b.buildingName] = count;                
            }
        }

        gameStats[gameLength - yearsLeft] = stats;

        previousEnergy = energy;
        sun.UpdateSun();
        CheckLoseStates();
        CheckWinStates();
        if (scanning)
        {
            UpdateOilMap();
            if (HasEnoughEnergyFor(1))
            {
                AddEnergy(-1);
            }
            else
            {
                scanning = false;
                Notification.Instance.SetNotification("Scanning Halted\nInsufficient Energy");
            }

        }
        if (!timeLose && !pollutionLose)
        {
            yearsLeft -= 1;
            if (yearsLeft < 0) yearsLeft = 0;
            ProcessBuildings();
            UpdatePendingResearch();
            UpdateAvailableBuildings();
            CheckSeti();
        }

    }

    private void CheckSeti()
    {
        Research setiResearch = null;

        float r = Random.Range(0f, 1f);

        if (r < setiChance)
        {
            int rcount = availableResearch.Count;
            if (pendingResearch != null) rcount++;

            int rnum = Random.Range(0, rcount);
            int count = 0;
            foreach (DictionaryEntry e in availableResearch)
            {
                if (count == rnum)
                {
                    setiResearch = (Research)e.Value;
                    break;
                }
                count++;
            }
            if (setiResearch == null && pendingResearch != null) setiResearch = pendingResearch;
            if (setiResearch != null)
            {
                if (setiResearch == pendingResearch)
                {
                    CancelResearch();
                }
               
                RemoveAvailableResearch(setiResearch);               
                attainedResearch.Add(setiResearch.name, setiResearch);
                UpdateAvailableResearch();                
                Dialog.Instance.SetDialog("SETI Success!", "Your SETI program has discovered transmissions from an alien race and attained the following resarch:" + setiResearch.name, "Ok", true, false);
                setiChance = setiChance / 2;
            }

        }
    }


    private void CheckLoseStates()
    {
        if (yearsLeft <= 0)
        {
            if (colonyShipsLaunched > 0)
            {
                int finalScore = colonyShipsLaunched * 1000 - population - populationKilled;
                Dialog.Instance.SetDialog("Partial Victory", "Your sun explodes incenerating those left behind, but all is not lost. Those that escaped into the galaxy may yet live on!\nFina Score:" + finalScore,
                                                                "Try Again", true, true);
            }
            else
            {
                float totalEnergy = gameStats[gameStats.Length - 1].energy;
                Dialog.Instance.SetDialog("Complete Solar Destruction", "Your civilization was too slow to advance, and remains trapped on this lonely rock as your sun goes nova. All of your efforts have been for naught, and no trace of your civilization remains. Total Energy:" + totalEnergy,
                                                                "Try Again", true, true);
            }
        }
        else if (pollution >= pollutionDeathAmount)
        {
            Dialog.Instance.SetDialog("Environmental Apocalypse", "Your lust for power and technology has led you to pollute your planet beyond the point of no return. Your people choke to death on their own waste and hubris. Alien races will no doubt discover the remains of your sad civilization and use your example as a warning to others.",
                                                             "Try Again", true, true);
        }


    }

    private void CheckWinStates()
    {
        int finalScore = colonyShipsLaunched * 1000 + yearsLeft * 100 - populationKilled;
        if (colonyShipsLaunched > 0 && population == 0 && populationKilled == 0)
        {

            Dialog.Instance.SetDialog("ULTIMATE VICTORY!", "You saved your entire population with time to spare! Your incredible civilization will thrive amongst the stars of the galaxy!\nFina Score:" + finalScore,
                                                            "Play Again", true, true);
        }

        else if (colonyShipsLaunched > 0 && population == 0 && populationKilled > 0)
        {
            Dialog.Instance.SetDialog("Pyrrhic Victory", "Your entire population has escaped the wrath of it's sun, but not the wrath of it's leaders. Many innocents died in executing your vision. You did what you had to do, and your civilization lives on.\n",
                                                        "Play Again", true, true);
        }

    }

    //Accepts positive of negative energy, enforces minimum energy, returns delta.
    public float AddEnergy(float energyToAdd)
    {
        float oldEnergy = energy;
        energy = Mathf.Max(0, energy + energyToAdd);
        return energy - oldEnergy;

    }

    public bool HasEnoughEnergyFor(float forthis)
    {
        if (forthis <= energy)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetEnergy()
    {
        return energy;
    }

    //Accepts positive or negative pollution, enforces a minimum and returns delta.
    public float GetPollution()
    {
        return pollution;
    }

    public float AddPollution(float pollutionToAdd)
    {

        float oldPollution = pollution;
        pollution = Mathf.Max(0, pollution + pollutionToAdd);

        return pollution - oldPollution;

    }


    private void ProcessBuildings()
    {

        for (int i = planet.placedBuildings.Count-1;i >=0;i--)        
        {
            Building b = planet.placedBuildings[i];
            b.AffectState();
        }

    }

    public void RemoveAvailableResearch(Research r)
    {
        print("remove available research:"+r.name);
        availableResearch.Remove(r.name);
        GuiEventHandler.Instance.RemoveResearch(r);
    }

    public void AddAvailableResearch(Research r)
    {
        print("add available research:" + r.name);
        availableResearch.Add(r.name, r);
        GuiEventHandler.Instance.AddResearch(r);
    }

    public void AddAttainedResearch(Research r)
    {
        print("add attained research:" + r.name);
        attainedResearch.Add(r.name, r);
    }

    public void RemoveAttainedResearch(Research r)
    {
        print("remove attained research:" + r.name);
        attainedResearch.Remove(r.name);
    }

    public void BeginResearch(Research r)
    {
        print("Begin Research:" + r.name);
        RemoveAvailableResearch(r);
        pendingResearch = r;

    }

    public void CancelResearch()
    {
        print("Cancel Research:" + pendingResearch.name);
        pendingResearch.completion = 0;
        AddAvailableResearch(pendingResearch);
        pendingResearch = null;
    }

    private void UpdatePendingResearch()
    {

        if (pendingResearch != null)
        {
            if (pendingResearch.paused) return;

            if (energy >= pendingResearch.energyCost)
            {
                pendingResearch.insufficientEnergy = false;
                energy -= pendingResearch.energyCost;


                pendingResearch.incrementCompletion(population, iq);

                if (pendingResearch.completion >= pendingResearch.iqCost)
                {
                    attainedResearch.Add(pendingResearch.name, pendingResearch);
                    pendingResearch = null;
                    UpdateAvailableResearch();
                }
            }
            else
            {
                pendingResearch.insufficientEnergy = true;
            }
        }

    }

    private void UpdateAvailableBuildings()
    {
        foreach (DictionaryEntry e in buildings)
        {
            BuildingInfo bi = (BuildingInfo)e.Value;


            if (!availableBuildings.Contains(bi.buildingName))
            {

                bool valid = true;
                for (int i = 0; i < bi.researchDependencies.Length; i++)
                {
                    if (!attainedResearch.ContainsKey(bi.researchDependencies[i]))
                    {
                        valid = false;
                    }
                }
                if (valid)
                {

                    availableBuildings.Add(bi.buildingName, bi);
                    GuiEventHandler.Instance.AddBuilding(bi);
                    if (bi.researchDependencies.Length > 0)
                        Notification.Instance.SetNotification(bi.buildingName + " Now Available");
                    
                }
            }
        }

    }

    private void UpdateAvailableResearch()
    {

        foreach (DictionaryEntry e in researchTree)
        {
            Research r = (Research)e.Value;
            bool valid = true;

            for (int i = 0; i < r.researchDependencies.Length; i++)
            {
                if (!attainedResearch.ContainsKey(r.researchDependencies[i]))
                {
                    valid = false;
                }
            }
            if (valid)
            {
                if (!availableResearch.Contains(r.name) && !attainedResearch.Contains(r.name) )
                {
                    if (pendingResearch == null || pendingResearch.name != r.name)
                    {
                       AddAvailableResearch(r);
                    }

                }
            }
        }
    }



    private void GetData()
    {

        TextAsset research = Resources.Load<TextAsset>("research");
        TextAsset buildings = Resources.Load<TextAsset>("buildings");


        Stream researchStream = GenerateStreamFromString(research.text);
        Stream buildingStream = GenerateStreamFromString(buildings.text);


        LoadResearchTree(new StreamReader(researchStream));
        LoadBuildings(new StreamReader(buildingStream));


    }

    public static List<PlanetInfo> LoadPlanets()
    {

        TextAsset planetsFile = Resources.Load<TextAsset>("planets");
        Stream planetStream = GenerateStreamFromString(planetsFile.text);
        StreamReader reader = new StreamReader(planetStream);

        List<PlanetInfo> planets = new List<PlanetInfo>();

        int numCol = 19; //number of columns before dependency list


        string line = reader.ReadLine();
        line = reader.ReadLine(); //skip the header
        while (line != null)
        {

            string[] splitLine = line.Split(',');
            if (splitLine.Length < numCol)
            {
                print("invalid line in planets file");
            }

            PlanetInfo planetInfo = new PlanetInfo();

            planetInfo.planetName = splitLine[0].Trim();
            planetInfo.coalReserves = int.Parse(splitLine[1]);
            planetInfo.oilFactor = float.Parse(splitLine[2]);
            planetInfo.windFactor = float.Parse(splitLine[3]);
            planetInfo.sunFactor = float.Parse(splitLine[4]);
            planetInfo.rotationSpeed = float.Parse(splitLine[5]);
            planetInfo.gameLength = int.Parse(splitLine[6]);
            planetInfo.planetSize = int.Parse(splitLine[7]);
            planetInfo.population = int.Parse(splitLine[8]);
            planetInfo.iq = float.Parse(splitLine[9]);
            planetInfo.startEnergy = int.Parse(splitLine[10]);
            planetInfo.startPollution = int.Parse(splitLine[11]);
            planetInfo.pollutionClearance = int.Parse(splitLine[12]);
            planetInfo.maxPollution = int.Parse(splitLine[13]);
            planetInfo.windZones = int.Parse(splitLine[14]);
            planetInfo.octaves = int.Parse(splitLine[15]);
            planetInfo.gain = float.Parse(splitLine[16]);
            planetInfo.lacunarity = float.Parse(splitLine[17]);
            planetInfo.stretch = float.Parse(splitLine[18]);

            List<Color> colors = new List<Color>();
            List<float> ranges = new List<float>();
            for (int i = 0; i < splitLine.Length - numCol; i++)
            {
                string colorString = splitLine[i + numCol].Trim();
                if (colorString == "")
                {
                    break;
                }
                string[] colorArray = colorString.Split('|');
                float r = float.Parse(colorArray[0]);
                float g = float.Parse(colorArray[1]);
                float b = float.Parse(colorArray[2]);
                float a = 1;
                if (colorArray.Length > 3)
                {
                    a = float.Parse(colorArray[3]);
                }
                colors.Add(new Color(r, g, b, a));
                i++;

                if (i + numCol < splitLine.Length)
                {
                    string range = (string)(splitLine[i + numCol]).Trim();
                    if (!string.IsNullOrEmpty(range))
                    {
                        ranges.Add(float.Parse(range));
                    }
                }
            }
            planetInfo.colorRamp = new ColorRamp(colors.ToArray(), ranges.ToArray());
            planets.Add(planetInfo);
            print("added planet:" + planetInfo.planetName);
            line = reader.ReadLine();

        }
        return planets;
    }

    private void LoadBuildings(StreamReader reader)
    {
        int numCol = 10; //number of columns before dependency list

        string line = reader.ReadLine();
        line = reader.ReadLine(); //skip the header
        while (line != null)
        {

            string[] splitLine = line.Split(',');
            if (splitLine.Length < numCol)
            {
                print("invalid line in buildings file");
            }

            string buildingName = splitLine[0].Trim();
            string className = splitLine[1].Trim();
            float cost = float.Parse(splitLine[2]);
            float energy = float.Parse(splitLine[3]);
            float pollution = float.Parse(splitLine[4]);
            int population = int.Parse(splitLine[5]);
            float IQFactor = float.Parse(splitLine[6]);
            int type = int.Parse(splitLine[7]);
            string upgrade = splitLine[8].Trim();
            float upgradeCost = float.Parse(splitLine[9]);

            ArrayList depList = new ArrayList();

            for (int i = 0; i < splitLine.Length - numCol; i++)
            {
                string depStr = splitLine[i + numCol].Trim();
                if (depStr != "")
                {
                    depList.Add(depStr);
                }
            }



            BuildingInfo b = new BuildingInfo(buildingName, className, cost, energy, pollution, population, IQFactor, type, upgrade, upgradeCost, (string[])depList.ToArray(typeof(string)));
            buildings.Add(className, b);

            line = reader.ReadLine();
        }
        UpdateAvailableBuildings();
    }

    private void LoadResearchTree(StreamReader reader)
    {
        int numCol = 3; //number of columns before dependency list

        string line = reader.ReadLine();
        line = reader.ReadLine(); //skip the header
        while (line != null)
        {

            string[] splitLine = line.Split(',');
            if (splitLine.Length < numCol)
            {
                print("invalid line in research file");
            }

            string name = splitLine[0].Trim();
            float energyCost = float.Parse(splitLine[1]);
            float iqCost = float.Parse(splitLine[2]);

            ArrayList depList = new ArrayList();

            for (int i = 0; i < splitLine.Length - numCol; i++)
            {
                string depStr = splitLine[i + numCol].Trim();

                if (depStr != "")
                {
                    depList.Add(depStr);
                }
            }

            Research r = new Research(name, energyCost, iqCost, (string[])depList.ToArray(typeof(string)));
            researchTree.Add(name, r);

            line = reader.ReadLine();
        }
        UpdateAvailableResearch();
    }

    public static Stream GenerateStreamFromString(string s)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

}

