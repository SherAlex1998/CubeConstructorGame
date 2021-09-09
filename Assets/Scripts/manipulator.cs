using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class manipulator : MonoBehaviour
{
    [SerializeField] Text text; // Для отладки
    public GameObject mainCube; // Кубик, бегающий по полю
    public GameObject cloneCube; // Кубик, который ставится на поле
    public GameObject cloneRoofCube; // Кубик, над которым нельзя ставить кубики
    public LayerMask surface;
    public Mesh cylinder;
    public Mesh cube;
    private GameObject temporaryObject; // Вспомогательные

    private Mesh mainMesh;
    private MeshRenderer meshRenderer;
    public Material green;
    public Material red;

    bool destroyMode = false;
    private int brickType = 0; // Для выбора кубика

    private RaycastHit hit;
    private RaycastHit temporaryHit;

    bool RayCastCheck()
    {
        return (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, surface.value));
    }

    bool CubePlaceCheck() // Можно ли установить кубик на эту позицию?
    {
        bool placeCheck = (Physics.Raycast(mainCube.transform.position, Vector3.down, out temporaryHit, 1f)) ||
            ((Physics.Raycast(mainCube.transform.position, Vector3.right, out temporaryHit, 1f)) && !(temporaryHit.transform.gameObject.tag == "Roof")) ||
            ((Physics.Raycast(mainCube.transform.position, Vector3.left, out temporaryHit, 1f)) && !(temporaryHit.transform.gameObject.tag == "Roof")) ||
            ((Physics.Raycast(mainCube.transform.position, Vector3.forward, out temporaryHit, 1f)) && !(temporaryHit.transform.gameObject.tag == "Roof")) ||
            ((Physics.Raycast(mainCube.transform.position, Vector3.up, out temporaryHit, 1f)) && !(temporaryHit.transform.gameObject.tag == "Roof")) ||
            ((Physics.Raycast(mainCube.transform.position, Vector3.back, out temporaryHit, 1f)) && !(temporaryHit.transform.gameObject.tag == "Roof"));
        if ((Physics.Raycast(mainCube.transform.position, Vector3.down, out temporaryHit, 1f)))
            placeCheck = placeCheck && !(temporaryHit.transform.gameObject.tag == "Roof");
        if (brickType == 1) placeCheck = placeCheck && !(Physics.Raycast(mainCube.transform.position, Vector3.up, 1f));
        return (placeCheck);
    } 

    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = green;
        mainMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) // Смена режима удаления и строительства
        {
            if (destroyMode)
            {
                meshRenderer.material = green;
                destroyMode = false;
            }
            else
            {
                meshRenderer.material = red;
                destroyMode = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !destroyMode) // Установка кубика
        {
            if (CubePlaceCheck())
            {
                if (brickType == 0) Instantiate(cloneCube, new Vector3(mainCube.transform.position.x, mainCube.transform.position.y, mainCube.transform.position.z), Quaternion.identity);
                if (brickType == 1) Instantiate(cloneRoofCube, new Vector3(mainCube.transform.position.x, mainCube.transform.position.y, mainCube.transform.position.z), Quaternion.identity);

            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && destroyMode) // Удаление кубика
        {
            temporaryObject = hit.transform.gameObject;
            if (temporaryObject.tag != "MainPLane") Destroy(temporaryObject);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) // Смена типа кубика
        {
            brickType++;
            if (brickType == 2) brickType = 0;
            if (brickType == 1)
            {
                GetComponent<MeshFilter>().sharedMesh = cylinder;
            }
            else
            {
                GetComponent<MeshFilter>().sharedMesh = cube;
            }
        }
    }

    void MainCubeRenderer() // Отрисовка кубика, бегающего по полю
    {
        if (RayCastCheck())
        {
            float CubeTypeHighUp = 0.5f;
            if (brickType == 0) CubeTypeHighUp = 0.5f;
            if (brickType == 1) CubeTypeHighUp = 1.0f;
            if (CubePlaceCheck() && !destroyMode) meshRenderer.material = green; else meshRenderer.material = red;
            Vector3 vec = new Vector3(Mathf.Round(hit.point.x), hit.point.y - hit.point.y % 1 + CubeTypeHighUp, Mathf.Round(hit.point.z));
            text.text = vec.x + " " + vec.y + " " + vec.z + "\n" + hit.point.x + " " + hit.point.y + " " + hit.point.z;
            transform.position = vec;

            if(destroyMode)
            {
                temporaryObject = hit.transform.gameObject;
                if (temporaryObject.tag != "MainPLane")
                {
                    GetComponent<MeshFilter>().sharedMesh = temporaryObject.GetComponent<MeshFilter>().sharedMesh;
                    transform.position = temporaryObject.transform.position;
                }
            }
        }
    }

    void FixedUpdate()
    {
        MainCubeRenderer();
    }
}