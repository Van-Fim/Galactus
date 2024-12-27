using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Star
{
    public int scale = 10;
    public Vector3 position = Vector3.zero;
    public Transform obj;
    public bool hidden = false;
    public void SetHiddenState(bool state)
    {
        hidden = state;
        if (obj != null)
        {
            obj.gameObject.SetActive(!state);
        }
    }
}
public class Chunk
{
    public Vector3 indexes = Vector3.zero;
    public bool creatingStars;
    public bool starsCreated;
    public bool destroyed;
    public List<Star> stars = new List<Star>();
}
public class ChunkManager : MonoBehaviour
{
    public int chunkSize = 50;
    public int distance = 1;
    public int steps = 100;
    List<Chunk> chunks = new List<Chunk>();
    IEnumerator CreateStars(Chunk chunk)
    {
        if (chunk.destroyed || chunk.creatingStars)
        {
            yield break;
        }
        int count = 1;
        chunk.creatingStars = true;
        while (count > 0)
        {
            --count;
            yield return new WaitForSeconds(0.1f);
            // Создаем звезду в чанке
            Star st = CreateNewStar(chunk);
            if (st != null)
            {
                chunk.stars.Add(st);
            }
        }
        if (!chunk.destroyed)
        {
            chunk.starsCreated = true;
        }
    }
    public Star CreateNewStar(Chunk chunk)
    {
        Vector3 chunkPosition = chunk.indexes;
        Star star = null;
        if (!chunk.destroyed)
        {
            star = GalaxyChunkController.CreateStar(chunkPosition);
            chunk.stars.Add(star);
        }
        return star;
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
                        StartCoroutine(CreateStars(chunk));
                        chunks.Add(chunk);
                    }
                    else
                    {
                        chunk.destroyed = false;
                    }
                }
            }
        }
        for (int i = chunks.Count - 1; i >= 0; i--)
        {
            if (chunks[i].destroyed)
            {
                // Удаляем созданные звезды
                for (int j = chunks[i].stars.Count - 1; j >= 0; j--)
                {
                    chunks[i].stars[j].SetHiddenState(true);
                }
                chunks[i].stars = null;
                chunks.RemoveAt(i);
            }
        }
    }
}
