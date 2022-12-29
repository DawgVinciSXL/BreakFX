using HarmonyLib;
using ReplayEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BreakFX
{
    public class BreakFXController : MonoBehaviour
    {
        public static BreakFXController Instance;
        private Dictionary<string, AudioClip> clipForName;
        public GameObject deckGameObject = null;
        public GameObject[] truckGameObjects = new GameObject[4];
        public GameObject[] wheelGameObjects = new GameObject[4];

        private AudioSource audioSource = null;
        private AudioClip snap = null;

        private Transform boardTransPos;
        public GameObject bbPrefab;
        private GameObject bbSpawner;

        private Material deckMaterial;
        private Material gripMaterial;
        private Material truckMaterial;
        private Material wheelMaterial;

        private bool randomNumberGenerated = false;
        private bool isBoardHidden = false;

        public bool needReset;
        public bool boardBroken;
        public bool wasInReplay;

        public Transform frontTrans = null;
        public Transform backTrans = null;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            audioSource = new GameObject().AddComponent<AudioSource>();
            audioSource.enabled = true;
            audioSource.minDistance = 10f;
            audioSource.maxDistance = 20f;
            audioSource.volume = 0.5f;
            audioSource.playOnAwake = false;
            DontDestroyOnLoad(audioSource);

            snap = AssetBundleHelper.BreakSFX;

            boardTransPos = PlayerController.Instance.boardController.boardTransform;
            bbPrefab = AssetBundleHelper.BoardPrefab;

            SoundManager instance = SoundManager.Instance;
            Traverse traverse = Traverse.Create(instance);
            clipForName = traverse.Field("clipForName").GetValue<Dictionary<string, AudioClip>>();
            bool flag = clipForName != null;
            if (flag)
            {
                Load();
            }
        }

        private void Load()
        {
            DeckSounds deckSounds = SoundManager.Instance.deckSounds;
            LoadClip(deckSounds.boardLand, snap);
            deckSounds.GenerateInitialEvents();
        }

        private void LoadClip(AudioClip[] clips, AudioClip clipToAdd)
        {
            Array.Resize(ref clips, clips.Length + 1);
            clips[clips.Length - 1] = clipToAdd;
            clipForName.Add(clipToAdd.name, clipToAdd);
        }

        private void Break()
        {
            if (boardBroken != true)
            {
                FindMaterials();
                HideBoard();
                isBoardHidden = true;
                PlayerController.Instance.ForceBail();
                Quaternion rot = Quaternion.FromToRotation(bbPrefab.transform.rotation.eulerAngles, boardTransPos.rotation.eulerAngles);
                bbSpawner = Instantiate(bbPrefab, boardTransPos.position, boardTransPos.rotation);

                backTrans = bbSpawner.transform.Find("Back");
                backTrans.gameObject.AddComponent<ObjectTracker>();
                backTrans.GetComponent<Renderer>().materials = new Material[]
                {
                deckMaterial,
                gripMaterial,
                truckMaterial,
                wheelMaterial
                };
                frontTrans = bbSpawner.transform.Find("Front");
                frontTrans.gameObject.AddComponent<ObjectTracker>();
                frontTrans.GetComponent<Renderer>().materials = new Material[]
                {
                deckMaterial,
                gripMaterial,
                truckMaterial,
                wheelMaterial
                };
                PlaySnapSFX();
                boardBroken = true;
            }
        }

        private void PlaySnapSFX()
        {
            SoundManager soundManager = SoundManager.Instance;
            soundManager.deckSounds.deckSource.PlayOneShot(snap, 0.5f);
            soundManager.DidPlayOneShot(soundManager.deckSounds.deckSource, snap, 0.5f);
        }

        private void StopSound()
        {
            audioSource.Stop();
        }

        private void Update()
        {
            CheckImpact();

            if (Input.GetKeyDown(Main.settings.BreakKey.keyCode))
            {
                Break();
            }

            bool flag = PlayerController.Instance.IsRespawning;
            if (flag)
            {
                if (isBoardHidden == true)
                {
                    randomNumberGenerated = false;
                    Destroy(bbSpawner.GetComponent<ObjectTracker>());
                    Destroy(bbSpawner);
                    UnhideBoard();
                    isBoardHidden = false;
                    boardBroken = false;
                    CharacterCustomizer rcc = ReplayEditorController.Instance.playbackController.characterCustomizer;
                    rcc.DeckParent.gameObject.SetActive(true);
                    rcc.TruckBaseParents[0].gameObject.SetActive(true);
                    rcc.TruckBaseParents[1].gameObject.SetActive(true);
                    rcc.TruckHangerParents[0].gameObject.SetActive(true);
                    rcc.TruckHangerParents[1].gameObject.SetActive(true);
                    rcc.WheelParents[0].gameObject.SetActive(true);
                    rcc.WheelParents[1].gameObject.SetActive(true);
                    rcc.WheelParents[2].gameObject.SetActive(true);
                    rcc.WheelParents[3].gameObject.SetActive(true);
                    needReset = false;
                }
            }
        }

        private void CheckImpact()
        {
            if (PlayerController.Instance.currentStateEnum == PlayerController.CurrentState.Impact)
            {
                float impact = Mathf.Abs(PlayerController.Instance.comController.COMRigidbody.velocity.y) * 1f;

                if (impact > 2 && impact < 4)
                {
                    System.Random random = new System.Random();
                    int rand = random.Next(0, 1000);
                    if (randomNumberGenerated == false)
                    {
                        randomNumberGenerated = true;
                        if (rand == 2)
                        {
                            Break();
                        }
                    }
                }

                else if (impact > 4 && impact < 7)
                {
                    System.Random random = new System.Random();
                    int rand = random.Next(0, 100);
                    if (randomNumberGenerated == false)
                    {
                        randomNumberGenerated = true;
                        if (rand == 2)
                        {
                            Break();
                        }
                    }
                }

                else if (impact > 7 && impact < 25)
                {
                    System.Random random = new System.Random();
                    int rand = random.Next(0, 15);
                    if (randomNumberGenerated == false)
                    {
                        randomNumberGenerated = true;
                        if (rand == 2)
                        {
                            Break();
                        }
                    }
                }

                else if (impact > 10)
                {
                    System.Random random = new System.Random();
                    int rand = random.Next(0, 10);
                    if (randomNumberGenerated == false)
                    {
                        randomNumberGenerated = true;
                        if (rand == 1)
                        {
                            Break();
                        }
                    }
                }
            }
            else
            {
                randomNumberGenerated = false;
            }
        }

        private void HideBoard()
        {
            CharacterCustomizer cc = PlayerController.Instance.characterCustomizer;
            cc.DeckParent.gameObject.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            cc.TruckBaseParents[0].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            cc.TruckBaseParents[1].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            cc.TruckHangerParents[0].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            cc.TruckHangerParents[1].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            cc.WheelParents[0].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            cc.WheelParents[1].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            cc.WheelParents[2].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            cc.WheelParents[3].gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        private void UnhideBoard()
        {
            CharacterCustomizer cc = PlayerController.Instance.characterCustomizer;
            cc.DeckParent.gameObject.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            cc.TruckBaseParents[0].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            cc.TruckBaseParents[1].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            cc.TruckHangerParents[0].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            cc.TruckHangerParents[1].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            cc.WheelParents[0].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            cc.WheelParents[1].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            cc.WheelParents[2].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            cc.WheelParents[3].gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }

        private void FindMaterials()
        {
            foreach (MeshRenderer mr in deckGameObject.GetComponentsInChildren<MeshRenderer>())
            {
                deckMaterial = mr.sharedMaterials[0];
                gripMaterial = mr.sharedMaterials[1];
            }

            foreach (MeshRenderer mr in truckGameObjects[0].GetComponentsInChildren<MeshRenderer>())
            {
                truckMaterial = mr.sharedMaterial;
            }

            foreach (MeshRenderer mr in wheelGameObjects[0].GetComponentsInChildren<MeshRenderer>())
            {
                wheelMaterial = mr.sharedMaterial;
            }
        }
    }
}