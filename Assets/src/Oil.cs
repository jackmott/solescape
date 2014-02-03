using UnityEngine;
using System.Collections;

public class Oil : MonoBehaviour
{


    public bool scanning = false;

    

    GameObject target;
    GameObject scanner;
    GameObject oilCam;
    GameObject scanGlow;

    GameState state;

    // Use this for initialization
    void Start()
    {
        state = GameState.Instance;
        Texture2D oilTex = new Texture2D(640, 640, TextureFormat.ARGB32, false);

        Color[] colors = oilTex.GetPixels();

        Noise noise = new Noise();

        float offsetx = (float)Random.Range(-32f, 32f);
        float offsety = (float)Random.Range(-32f, 32f);
        

        float pi = 3.14159265359f;
        float twopi = pi * 2f;

        float min = 9999;
        float max = -999;

        for (int y = 0; y < oilTex.height; y++)
        {
            int row = oilTex.width * y;
            for (int x = 0; x < oilTex.width; x++)
            {
                float theta = twopi * (x / (float)oilTex.width);
                float phi = pi * (y / (float)oilTex.height);

                float x3d = Mathf.Cos(theta) * Mathf.Sin(phi);
                float y3d = Mathf.Sin(theta) * Mathf.Sin(phi);
                float z3d = -Mathf.Cos(phi);

                float color = noise.noise3(x3d * 2f + offsetx, y3d * 2f + offsety,z3d*2f );
               
                if (color > .3f / state.oilFactor) color = 1;
                colors[row + x] = new Color(color, 0, 0, 1f);

                
            }
        }

        print("oilmin:" + min + " oilmax:" + max);
        oilTex.SetPixels(colors);
        oilTex.Apply();
        renderer.material.mainTexture = oilTex;
        state.oilColors = colors;
        state.oilTex = oilTex;
    }

    void OnGUI()
    {

    }



    // Update is called once per frame
    void Update()
    {       
        if (state.oilScan)
        {


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {

                bool hitPlanet = false;
                RaycastHit planetHit = new RaycastHit();

                for (int i = 0; i < hits.Length; i++)
                {

                    if (hits[i].transform.gameObject == GameState.Instance.planet.transform.gameObject)
                    {
                        hitPlanet = true;
                        planetHit = hits[i];
                        break;
                    }
                }


                if (hitPlanet)
                {
                    
                    
                    if (GameState.Instance.scanning)
                    {

                        if (target != null)
                        {
                            print("destroy target");
                            Destroy(target);
                            target = null;
                        }
                        if (scanner == null)
                        {
                            scanner = (GameObject)Instantiate(Resources.Load("prefabs/oilscan/OilScanPlane"), planetHit.point, Quaternion.identity);
                        }

                        scanner.transform.position = planetHit.point;
                        scanner.transform.LookAt(transform);
                        scanner.transform.Translate(Vector3.forward * -3);
                        scanner.transform.Rotate(-90f, 0f, 0f, Space.Self);

                        if (scanGlow == null)
                        {
                            scanGlow = (GameObject)Instantiate(Resources.Load("prefabs/oilscan/ScanGlow"), planetHit.point, Quaternion.identity);
                        }
                        scanGlow.transform.position = planetHit.point;
                        scanGlow.transform.LookAt(transform);
                        scanGlow.transform.Translate(Vector3.forward * -3);
                        scanGlow.transform.Rotate(-180f, 0f, 0f, Space.Self);


                        if (oilCam == null)
                        {
                            oilCam = (GameObject)Instantiate(Resources.Load("prefabs/oilscan/OilCamera"), planetHit.point, Quaternion.identity);
                        }
                        oilCam.transform.position = planetHit.point;
                        oilCam.transform.LookAt(transform);
                        oilCam.transform.Translate(Vector3.forward * 30);

                        
                    }
                    else
                    {
                        if (scanner != null)
                        {
                            Destroy(scanner);
                            scanner = null;
                        }
                        if (scanGlow != null)
                        {
                            Destroy(scanGlow);
                            scanGlow = null;
                        }
                        if (oilCam != null)
                        {
                            Destroy(oilCam);
                            oilCam = null;
                        }
                        if (target == null)
                        {
                            print("instant target");
                            target = (GameObject)Instantiate(Resources.Load("prefabs/oilscan/OilTargetPlane"), planetHit.point, Quaternion.identity);
                        }

                        target.transform.position = planetHit.point;
                        target.transform.LookAt(transform);
                        target.transform.Translate(Vector3.forward * -4);
                        target.transform.Rotate(-90f, 0f, 0f, Space.Self);

                    }

                }
            }
        }
        else
        {
            if (target != null)
            {
                print("destroy target");
                Destroy(target);
            }
            target = null;
        }
    }
}
