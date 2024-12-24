using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Chunk
{
    public Vector3 indexes = Vector3.zero;
    public Transform obj;
    public bool creatingStars;
    public bool starsCreated;
    public bool destroyed;
}
public class ChunkManager : MonoBehaviour
{
    public int chunkSize = 150;
    public int distance = 1;
    public int steps = 100;
    List<Chunk> chunks = new List<Chunk>();
    IEnumerator CreateStars(Chunk chunk)
    {
        if (chunk.destroyed)
        {
            yield break;
        }
        int count = 20;
        float st = (255 / 20);
        chunk.creatingStars = true;
        MeshRenderer mr = chunk.obj.gameObject.GetComponent<MeshRenderer>();
        mr.materials[0].SetColor("_TintColor", new Color32(255, 0, 0, 255));
        while (count > 0)
        {
            --count;
            if (mr == null || mr.materials[0] == null)
            {
                yield break;
            }
            byte bt = (byte)(count * st);
            mr.materials[0].SetColor("_TintColor", new Color32(255, (byte)(255 - bt), 0, 255));
            yield return new WaitForSeconds(0.1f);
        }
        if (!chunk.destroyed)
        {
            mr.materials[0].SetColor("_TintColor", new Color32(0, 255, 0, 255));
            chunk.starsCreated = true;
        }
    }
    public void UpdateChunks(Vector3 pos)
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            chunks[i].destroyed = true;
        }
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                for (int z = -distance; z <= distance; z++)
                {
                    Vector3 indx = pos + chunkSize * new Vector3(x, y, z);
                    Chunk chunk = chunks.Find(x => x.indexes == indx);
                    if (chunk == null)
                    {
                        chunk = new Chunk();
                        chunk.indexes = indx;
                        chunk.obj = GameObject.Instantiate(GamePrefabsManager.LoadPrefab<Transform>("TestCube"));
                        chunk.obj.gameObject.layer = 6;
                        chunk.obj.gameObject.SetActive(true);
                        chunk.obj.localScale = new Vector3(chunkSize, chunkSize, chunkSize);
                        chunk.obj.transform.SetParent(SpaceManager.galaxyContainer.transform);
                        chunk.obj.transform.localPosition = indx;
                        StartCoroutine(CreateStars(chunk));
                        chunks.Add(chunk);
                    }
                    else
                    {
                        chunk.destroyed = false;
                        chunk.obj.transform.localPosition = indx;
                    }
                }
            }
        }
        for (int i = chunks.Count - 1; i >= 0; i--)
        {
            if (chunks[i].destroyed)
            {
                MeshRenderer mr = chunks[i].obj.gameObject.GetComponent<MeshRenderer>();
                mr.materials[0].SetColor("_TintColor", new Color32(0, 0, 0, 255));
                GameObject.Destroy(chunks[i].obj.gameObject);
                chunks.RemoveAt(i);
            }
        }
        for (int i = 0; i < chunks.Count; i++)
        {
            //Debug.Log($"{chunks[i].indexes}");
        }
    }
}
