using UnityEngine;

public class AutoMapping : MonoBehaviour
{
    public GameObject roads;
    public GameObject enemies;
    public GameObject items;
    public GameObject roadImage;



    private float pw, ph;
    private DungeonMapData2D map;

    public GameObject enemiesObj;
    public GameObject itemsObj;
    public GameObject enemyImage;
    public GameObject itemImage;

    public ObjectPosition player;
    public GameObject playerImage;

    [SerializeField] DungeonGenerator dungeonGenerator;

    // メソッドを追加
    private void Update()
    {
        ShowObjects(enemyImage, enemies, enemiesObj);
        // ShowObjects(itemImage, items, itemsObj);
        ShowPlayerObject();
    }

    private Vector2 ShowPlayerObject()
    {
        Vector2Int p = new Vector2Int(
                dungeonGenerator.GetGridX(player.transform.position.x),
                dungeonGenerator.GetGridY(player.transform.position.y));

        playerImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(pw * p.x, ph * -p.y);
        return p;
    }

    private void ShowObjects(GameObject image, GameObject imgs, GameObject objs)
    {
        for (int i = imgs.transform.childCount; i < objs.transform.childCount; i++)
            Instantiate(image, imgs.transform);
        for (int i = objs.transform.childCount; i < imgs.transform.childCount; i++)
            Destroy(imgs.transform.GetChild(i).gameObject);
        for (int i = 0; i < objs.transform.childCount; i++)
        {
            // Pos2D p = objs.transform.GetChild(i).GetComponent<ObjectPosition>().grid;
            Vector2Int p = new Vector2Int(
                dungeonGenerator.GetGridX(objs.transform.GetChild(i).transform.position.x),
                dungeonGenerator.GetGridY(objs.transform.GetChild(i).transform.position.y));
            Transform img = imgs.transform.GetChild(i);
            img.GetComponent<RectTransform>().anchoredPosition = new Vector2(pw * p.x, ph * -p.y);
            img.gameObject.SetActive(true);

            //if (map.Get(p.x, p.y) == 1 || true)
            //{
            //}
            //else img.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        RectTransform rect = roadImage.GetComponent<RectTransform>();
        pw = rect.sizeDelta.x;
        ph = rect.sizeDelta.y;
    }

    /*
    * 指定した位置をマッピングする
    */
    public void Mapping(int x, int y, int value)
    {
        map.Set(x, y, value);
        if (value == 1)
        {
            GameObject road = Instantiate(roadImage, roads.transform);
            road.GetComponent<RectTransform>().anchoredPosition = new Vector2(pw * x, ph * -y);
        }
    }

    /*
    * 表示をリセットする
    */
    public void Reset(int width, int height)
    {
        for (int i = 0; i < roads.transform.childCount; i++)
            Destroy(roads.transform.GetChild(i).gameObject);
        for (int i = 0; i < enemies.transform.childCount; i++)
            Destroy(enemies.transform.GetChild(i).gameObject);
        for (int i = 0; i < items.transform.childCount; i++)
            Destroy(items.transform.GetChild(i).gameObject);
        map = new DungeonMapData2D(width, height);
        GetComponent<RectTransform>().sizeDelta = new Vector2(width * pw, height * ph);
    }

    /**
    * Z軸に対して反対の値を返す
    */
    private int ToMirrorX(int xgrid)
    {
        return map.Width - xgrid - 1;
    }
}