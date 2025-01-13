using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
    public List<SpaceSystem> stars = new List<SpaceSystem>();
}
public class ChunkManager : MonoBehaviour
{
    public int chunkSize = 20;
    public int distance = 2;
    public int steps = 5;
    List<Chunk> chunks = new List<Chunk>();
    public static ChunkManager Create(int size = 50)
    {
        ChunkManager chunkManager = new GameObject().AddComponent<ChunkManager>();
        chunkManager.chunkSize = size;
        return chunkManager;
    }
    IEnumerator CreateStars(Chunk chunk)
    {
        if (chunk.destroyed || chunk.creatingStars)
        {
            yield break;
        }
        int seed = $"{LocalClient.galaxyId}{chunk.indexes}".GetHashCode();
        Random.InitState(seed);
        int chance = 1;
        chunk.creatingStars = true;
        float s = chunkSize / steps;
        int sts = steps - 1;
        int number = 0;
        for (int x = -sts; x <= sts; x++)
        {
            for (int y = -sts; y <= sts; y++)
            {
                for (int z = -sts; z <= sts; z++)
                {
                    seed = $"{LocalClient.galaxyId}{chunk.indexes}{new Vector3(x, y, z)}".GetHashCode();
                    Random.InitState(seed);
                    int r = Random.Range(0, 101);
                    if (r <= chance)
                    {
                        yield return new WaitForSeconds(0.1f);
                        Random.InitState(seed);
                        // Создаем звезду в чанке
                        SpaceSystem st = CreateNewStar(chunk, number);
                        if (st != null)
                        {
                            Vector3 pos = new Vector3(x * s, y * s, z * s);
                            Vector3 fpos = Random.insideUnitCircle * (s - (s / 10));
                            st.SetPosition(pos + fpos + chunk.indexes);
                        }
                        number++;
                    }
                }
            }
        }
        if (!chunk.destroyed)
        {
            chunk.starsCreated = true;
        }
    }
    public void CreateStarsFunc(Chunk chunk)
    {
        if (chunk.destroyed || chunk.creatingStars)
        {
            return;
        }
        int seed = $"{LocalClient.galaxyId}{chunk.indexes}".GetHashCode();
        Random.InitState(seed);
        int chance = 1;
        chunk.creatingStars = true;
        float s = chunkSize / steps;
        int sts = steps - 1;
        int number = 0;
        for (int x = -sts; x <= sts; x++)
        {
            for (int y = -sts; y <= sts; y++)
            {
                for (int z = -sts; z <= sts; z++)
                {
                    seed = $"{LocalClient.galaxyId}{chunk.indexes}{new Vector3(x, y, z)}".GetHashCode();
                    Random.InitState(seed);
                    int r = Random.Range(0, 101);
                    if (r <= chance)
                    {
                        Random.InitState(seed);
                        // Создаем звезду в чанке
                        SpaceSystem st = CreateNewStar(chunk, number);
                        if (st != null)
                        {
                            Vector3 pos = new Vector3(x * s, y * s, z * s);
                            Vector3 fpos = Random.insideUnitCircle * (s - (s / 10));
                            st.SetPosition(pos + fpos + chunk.indexes);
                        }
                        number++;
                    }
                }
            }
        }
        if (!chunk.destroyed)
        {
            chunk.starsCreated = true;
        }
    }
    public SpaceSystem CreateNewStar(Chunk chunk, int num)
    {
        Vector3 chunkPosition = chunk.indexes;
        SpaceSystem star = null;
        if (!chunk.destroyed)
        {
            star = GalaxyChunkController.CreateStar(chunkPosition);
            star.id = $"{star.galaxyId}{star.GetPosition()}".GetHashCode();
            chunk.stars.Add(star);
        }
        return star;
    }
    public void UpdateChunks(Vector3 pos, bool forceUpdate = false)
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
                        if (forceUpdate)
                        {
                            CreateStarsFunc(chunk);
                        }
                        else
                        {
                            StartCoroutine(CreateStars(chunk));
                        }
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
                    chunks[i].stars[j].Hide();
                }
                chunks[i].stars = null;
                chunks.RemoveAt(i);
            }
        }
    }
}
