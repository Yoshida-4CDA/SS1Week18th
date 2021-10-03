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

    public GameObject playerImage;

    [SerializeField] DungeonGenerator dungeonGenerator;

    // メソッドを追加
    public void HandleUpdate(ObjectPosition player)
    {
        ShowObjects(enemyImage, enemies, enemiesObj);
        ShowPlayerObject(player);
    }

    private Vector2Int ShowPlayerObject(ObjectPosition player)
    {
        Vector2Int p = player.Grid;
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
            Transform child = objs.transform.GetChild(i);
            Vector2Int p = child.GetComponent<ObjectPosition>().Grid;
            Transform img = imgs.transform.GetChild(i);
            if (map.Get(p.x, p.y) == 1 && child.gameObject.activeSelf)
            {
                img.GetComponent<RectTransform>().anchoredPosition = new Vector2(pw * p.x, ph * -p.y);
                img.gameObject.SetActive(true);
            }
            else
            {
                img.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        RectTransform rect = roadImage.GetComponent<RectTransform>();
        pw = rect.sizeDelta.x;
        ph = rect.sizeDelta.y;
    }

    /*
    * 指定した位置をマッピングする:valueはDungeonのデータ
    */
    public void Mapping(int x, int y, int value)
    {
        if (map.Get(x,y) > 0)
        {
            return;
        }
        map.Set(x, y, value);
        if (value == 1)
        {
            GameObject road = Instantiate(roadImage, roads.transform);
            road.GetComponent<RectTransform>().anchoredPosition = new Vector2(pw * x, ph * -y);
        }
    }

    // TODO:playerの現在入っている部屋を取得
    // 部屋のindex座標, 大きさがわかれば良い
    // 部屋を配置してスケールを大きくすることで実現
    public void Mapping(ObjectPosition room)
    {
        int sx = room.RoomGrind.x;// + room.range.left;
        int sy = room.RoomGrind.y;// + room.range.top;
        if (map.Get(sx, sy) > 0) return;
        int ex = sx + room.range.width - 1;
        int ey = sy + room.range.height - 1;
        for (int x = sx; x <= ex; x++)
        {
            for (int y = sy; y <= ey; y++)
            {
                map.Set(x, y, 1);
            }
        }
        GameObject road = Instantiate(roadImage, roads.transform);
        road.GetComponent<RectTransform>().anchoredPosition = new Vector2(pw * (sx), ph * -sy);
        road.GetComponent<RectTransform>().localScale = new Vector3(room.range.width, room.range.height, 1);

        //通路の表示
        for (int x = sx; x <= ex; x++)
        {
            Mapping(x, sy - 1, dungeonGenerator.MapData2D.Get(x, sy-1) + 1);
            Mapping(x, ey + 1, dungeonGenerator.MapData2D.Get(x, ey + 1) + 1);
        }
        for (int y = sy; y <= ey; y++)
        {
            Mapping(sx - 1, y, dungeonGenerator.MapData2D.Get(sx - 1, y) + 1);
            Mapping(ex + 1, y, dungeonGenerator.MapData2D.Get(ex + 1, y) + 1);
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