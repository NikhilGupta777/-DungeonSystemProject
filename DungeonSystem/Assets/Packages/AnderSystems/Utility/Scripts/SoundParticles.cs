using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainUtility
{
    public static Terrain DetectTerrain(Transform Target, LayerMask Layers)
    {
        Terrain OutputTerrain;
        RaycastHit hit;
        Physics.Linecast(Target.position + Vector3.up, Target.position + (Vector3.down * 1000), out hit, Layers);
        hit.collider.TryGetComponent<Terrain>(out OutputTerrain);
        return OutputTerrain;
    }
    public static float[] GetTextureMix(Vector3 worldPos, Terrain terrain)
    {
        // returns an array containing the relative mix of textures
        // on the main terrain at this world position.
        // The number of values in the array will equal the number
        // of textures added to the terrain.
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        // calculate which splat map cell the worldPos falls within (ignoring y)
        int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
        // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        // extract the 3D array data to a 1D array:
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
        for (int n = 0; n < cellMix.Length; n++)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }
        return cellMix;
    }
    public static Texture GetMainTexture(Vector3 worldPos, Terrain terrain)
    {
        // returns the zero-based index of the most dominant texture
        // on the main terrain at this world position.
        float[] mix = GetTextureMix(worldPos, terrain);
        float maxMix = 0;
        int maxIndex = 0;
        // loop through each mix value and find the maximum
        for (int n = 0; n < mix.Length; n++)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return terrain.terrainData.terrainLayers[maxIndex].diffuseTexture;
    }
}

[CreateAssetMenu(fileName = "New ''Particle Sound''", menuName = "Create new ''Particle Sound''")]
public class SoundParticles : ScriptableObject
{
    public GameObject[] Particles;
    public GameObject[] ParticlesVariant;
    public float ValueToSpawn;
    [System.Serializable]
    public class options
    {
        public bool Rotation,YRotationOnly, ReParent, DestroyOther, OnlyOnce;
    }
    [SerializeField]
    public options Options;
    [Header("SpawnOptions")]
    public string[] ObjectTags;
    public Texture2D[] Textures;

    public enum WhoParticle
    {
        ParticleA,ParticleB
    }


    public static GameObject SpawnParticle(SoundParticles Particle, GameObject Current, Transform Target, int ParticleIndex, bool Variant, LayerMask Layers, bool PlaceOnGround)
    {
        GameObject CurrentParticle = default;
        Texture texture = default;

        for (int i = 0; i < Particle.Textures.Length; i++)
        {
            texture = Particle.Textures[i];
        }

        bool CanSpawn = false;

        RaycastHit hit;
        if (Physics.Linecast(Target.transform.position + Vector3.up, Target.transform.position - (Vector3.up * 1000), out hit, Layers))
        {
            for (int i = 0; i < Particle.ObjectTags.Length; i++)
            {
                if (hit.collider.GetComponent<Terrain>() == null)
                {
                    CanSpawn = hit.collider.gameObject.tag == Particle.ObjectTags[i];
                } else
                {
                    if (Particle.Textures.Length > 0)
                    {
                        CanSpawn = TerrainUtility.GetMainTexture(Target.transform.position, TerrainUtility.DetectTerrain(Target.transform, Layers)) ==
                        texture;
                    }
                }
            }
        }

        if (CanSpawn)
        {
            //Check if "DestroyOther" is enable
            if (Particle.Options.DestroyOther)
            {
                if (Current)
                {
                    //Destroy the currentParticle
                    Destroy(Current.gameObject);
                }
            }

            // Check if "OnlyOnce" option is enable or Not
            if ((Particle.Options.OnlyOnce && (Current == null) || !Particle.Options.OnlyOnce))
            {
                if (Variant)
                {
                    //Instantiate Particle
                    if (Particle.Options.Rotation)
                    {
                        CurrentParticle = Instantiate(Particle.ParticlesVariant[ParticleIndex],
                            Target.position, Target.rotation);
                    } else
                    {
                        CurrentParticle = Instantiate(Particle.ParticlesVariant[ParticleIndex],
                         Target.position, Quaternion.identity);
                    }
                }
                else
                {
                    //Instantiate Particle
                    if (Particle.Options.Rotation)
                    {
                        CurrentParticle = Instantiate(Particle.Particles[ParticleIndex],
                            Target.position, Target.rotation);
                    }
                    else
                    {
                        CurrentParticle = Instantiate(Particle.Particles[ParticleIndex],
                         Target.position, Quaternion.identity);
                    }
                }
            }

            // Check if "ReParent" option is enable or Not
            if (Particle.Options.ReParent)
            {
                //Set Parent
                CurrentParticle.transform.parent = Target;
            }



            if (PlaceOnGround)
            {
                CurrentParticle.transform.position = hit.point;
                CurrentParticle.transform.up = hit.normal;
            }

            if (Particle.Options.YRotationOnly)
            {
                CurrentParticle.transform.eulerAngles = new Vector3(0, Target.eulerAngles.y, 0);
            }


            return CurrentParticle;
        } else
        {
            return null;
        }
    }
    public static GameObject SpawnParticle(SoundParticles Particle, Vector3 Position, Quaternion Rotation, int ParticleIndex)
    {
        return Instantiate(Particle.Particles[ParticleIndex], Position, Rotation);
    }

    public static GameObject SpawnParticle(SoundParticles Particle, Vector3 Position, Vector3 Normal, int ParticleIndex)
    {
        GameObject _particle = Instantiate(Particle.Particles[ParticleIndex], Position, Quaternion.identity);
        _particle.transform.up = Normal;
        return _particle;
    }

    public static GameObject SpawnParticle(SoundParticles Particle, Vector3 Position, Vector3 Normal, float Value, int ParticleIndex)
    {
        if (Value < Particle.ValueToSpawn)
            return null;
        GameObject _particle = Instantiate(Particle.Particles[ParticleIndex], Position, Quaternion.identity);
        _particle.transform.up = Normal;
        return _particle;
    }
}
