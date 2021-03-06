﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerBase : NetworkBehaviour {

    public GameManager gameManager;
    public PlayerManager playerManager;
    NetManager netManager;

    public Mesh FarmTileMesh;
    public Material[] FarmTileMat;

    // Interface //

    public Sprite QuadroCao, QuadroPassaro, QuadroRato, QuadroGato;
    public Sprite SlotCao, SlotPassaro, SlotRato, SlotGato;
    public Sprite ExploradorCao, ExploradorPassaro, ExploradorRato, ExploradorGato;
    public Sprite ArqueiroCao, ArqueiroPassaro, ArqueiroRato, ArqueiroGato;
    public Sprite GuerreiroCao, GuerreiroPassaro, GuerreiroRato, GuerreiroGato;
    public Sprite FazendaCao, FazendaPassaro, FazendaRato, FazendaGato;
    public Sprite StatusCao, StatusPassaro, StatusRato, StatusGato;
    public Sprite QuantGuerreiroCao, QuantGuerreiroPassaro, QuantGuerreiroRato, QuantGuerreiroGato;
    public Sprite QuantFazendaCao, QuantFazendaPassaro, QuantFazendaRato, QuantFazendaGato;
    public Sprite QuantGoldCao, QuantGoldPassaro, QuantGoldRato, QuantGoldGato;
    public Sprite ObjeCao, ObjePassaro, ObjeRato, ObjeGato;
    public Sprite PassCao, PassPassaro, PassRato, PassGato;
    public Sprite ReliCao, ReliPassaro, ReliRato, ReliGato;
    public Sprite TempoCao, TempoPassaro, TempoRato, TempoGato;
    public Sprite OpcoesCao, OpcoesPassaro, OpcoesRato, OpcoesGato;

    Text UnitsAmmountText;

    Text TempoTxt;
    public GameObject Aguia_Explorer , Cao_Explorer, Rato_Explorer , Gato_Explorer;
    public GameObject Aguia_Warrior, Cao_Warrior , Rato_Warrior , Gato_Warrior;
    public GameObject Aguia_Archer, Cao_Archer , Rato_Archer , Gato_Archer;

    Button SpawnExplorerBtn, PassTurnButton, SpawnGuerreiroBtn, SpawnArqueiroBtn, SpawnFarmBtn;
    [SyncVar]
    public string test="test";
    [SyncVar]
    public int Gold;
    [SyncVar]
    public int Food;
    [SyncVar]
    public int PlayerBaseID;
    [SyncVar]
    public int curHealth;
    [SyncVar]
    public bool Occupied;
    [SyncVar]
    public bool Destroyed;
    [SyncVar]
    public int LastAttackingPlayerId ;
    public Text GoldText, FoodText;

    InputField InputText;

    public Vector3 WinningPlayerPos;

    public int GoldMineAmmount = 0;

    public Text ObjectiveText;
    [SyncVar]
    public int MyObjective;

    bool CanMoveCamera = false;

    string PlayerString;
    
    [SyncVar]
    float tempoTurno = 45;

    bool ReadyToPlay = false;

    public GameObject Farm;

    public bool GameOver = false;

    bool Initialized = false;

    public List<int> Reliquias; //1 = Vermelho ; 2 = Amarelo; 3 = Azul; 4 = Verde
    public Image RelicSlot1, RelicSlot2, RelicSlot3, RelicSlot4;

    public Sprite RedRelic, YellowRelic, BlueRelic, GreenRelic, EmptyRelic;

    public AudioClip NarratorYourTurn;
    public AudioClip[] VictoryVoiceLine;
    public AudioClip[] DefeatVoiceLine;

    public int UnitAmmount;

    List<GameObject> ConfirmationScreens = new List<GameObject>();

    [Command]
    public void Cmd_UpdatePlayerBaseID(int ID)
    {
        PlayerBaseID = ID - GameObject.Find("NetManager").GetComponent<NetworkManager>().numPlayers;
    }

    [ClientRpc]
    public void Rpc_UpdatePlayerBaseID(int ID)
    {
        //PlayerBaseID = ID;
        Debug.Log("RPC ID Update");
    }

    [Command]
    public void Cmd_UpdateGoldMineAmmount( bool isIncreasing)
    {
        if (isIncreasing)
        {
            GoldMineAmmount++;

        }
        else
        {
            GoldMineAmmount--;
        }
    }

    [Command]
    public void Cmd_AdicionaChat(string texto, int SendingPlayerID)
    {
        Text ChatOutput = GameObject.Find("TextChat").GetComponent<Text>();
        //Chat Global
        int PrivatePlayerID = 0;
        string newText = null;

        if (texto.Length > 2)
        {
            if (texto.Substring(0, 2) != "/w")
            {
                switch (PlayerBaseID)
                {
                    case 1:
                        newText = "\n <color=red>[Dogs]</color> " + texto;
                        break;
                    case 2:
                        newText = "\n <color=orange>[Birds]</color> " + texto;
                        break;
                    case 3:
                        newText = "\n <color=green>[Rats]</color> " + texto;
                        break;
                    case 4:
                        newText = "\n <color=blue>[Cats]</color> " + texto;
                        break;
                }
            }
            //Chat Particular
            else if(texto.Substring(0, 2) == "/w")
            {
                texto = texto.Remove(0, 3);
                string tempText = "";
                if(texto.Substring(0,4) == "Dogs" || texto.Substring(0, 4) == "dogs")
                {
                    tempText = "[Dogs]";
                    PrivatePlayerID = 1;
                    texto = texto.Remove(0, 4);
                }
                else if(texto.Substring(0, 5) == "Birds" || texto.Substring(0, 5) == "birds")
                {
                    tempText = "[Birds]";
                    PrivatePlayerID = 2;
                    texto = texto.Remove(0, 5);
                }
                else if (texto.Substring(0, 4) == "Rats" || texto.Substring(0, 4) == "rats")
                {
                    tempText = "[Rats]";
                    PrivatePlayerID = 3;
                    texto = texto.Remove(0, 4);
                }
                else if (texto.Substring(0, 4) == "Cats" || texto.Substring(0, 4) == "cats")
                {
                    tempText = "[Cats]";
                    PrivatePlayerID = 4;
                    texto = texto.Remove(0, 4);
                }


                switch (PlayerBaseID)
                {
                    case 1:
                        newText = "\n <color=purple>Whisper [Dogs] to " +tempText + ":" + texto + "</color>";
                        break;
                    case 2:
                        newText = "\n <color=purple>Whisper [Birds] to " + tempText + ":" + texto + "</color>";
                        break;
                    case 3:
                        newText = "\n <color=purple>Whisper [Rats] to " + tempText + ":" + texto + "</color>";
                        break;
                    case 4:
                        newText = "\n <color=purple>Whisper [Cats] to " + tempText + ":" + texto + "</color>";
                        break;
                }
            }
        }
        else
        {
            switch (PlayerBaseID)
            {
                case 1:
                    newText = "\n <color=red>[Dogs]</color> " + texto;
                    break;
                case 2:
                    newText = "\n <color=orange>[Birds]</color> " + texto;
                    break;
                case 3:
                    newText = "\n <color=green>[Rats]</color> " + texto;
                    break;
                case 4:
                    newText = "\n <color=blue>[Cats]</color> " + texto;
                    break;
            }
        }
        if(ChatOutput.cachedTextGenerator.lineCount > 30)
        {
            ChatOutput.text = ChatOutput.text.Substring(ChatOutput.text.IndexOf('\n') + 1);
        }

        Rpc_UpdateChat(newText , PrivatePlayerID, SendingPlayerID);
    }

    [ClientRpc]
    void Rpc_UpdateChat(string newText , int PrivatePlayerID, int SendingPlayerID)
    {

        if (PrivatePlayerID == 0)
        {
            GameObject.Find("TextChat").GetComponent<Text>().text += newText;

        }
        else
        {

            GameObject[] Bases = GameObject.FindGameObjectsWithTag("PlayerBase");
            for (int i = 0; i < Bases.Length; i++)
            {
                PlayerBase Script = Bases[i].GetComponent<PlayerBase>();
                if (Script.enabled == true)
                {
                    if (PrivatePlayerID == Script.PlayerBaseID || SendingPlayerID == Script.PlayerBaseID)
                    {
                        GameObject.Find("TextChat").GetComponent<Text>().text += newText;
                    }
                }
            }
        }
    }

    [ClientRpc]
    public void Rpc_UpdateGoldMineAmmount(bool isIncreasing)
    {
        if (isIncreasing)
        {
            GoldMineAmmount++;

        }
        else
        {
            GoldMineAmmount--;
        }
    }

    IEnumerator DelayedStart()
    {

        yield return new WaitForSeconds(2);

        GoldText = GameObject.Find("_Dinheiro").GetComponent<Text>();
        FoodText = GameObject.Find("_Comida").GetComponent<Text>();
        TempoTxt = GameObject.Find("_Tempo").GetComponent<Text>();
        ObjectiveText = GameObject.Find("_ObjectiveText").GetComponent<Text>();
        RelicSlot1 = GameObject.Find("Relic_Slot_1").GetComponent<Image>();
        RelicSlot2 = GameObject.Find("Relic_Slot_2").GetComponent<Image>();
        RelicSlot3 = GameObject.Find("Relic_Slot_3").GetComponent<Image>();
        RelicSlot4 = GameObject.Find("Relic_Slot_4").GetComponent<Image>();
        GameObject.Find("Chat").GetComponent<controlaChat>().PlayerOwner = this;
        InputText = GameObject.Find("ChatInputField").GetComponent<InputField>();
        UnitsAmmountText = GameObject.Find("_Soldado").GetComponent<Text>();

        gameManager = GameObject.Find("_GameManager").GetComponent<GameManager>();
        playerManager = GameObject.Find("_PlayerManager").GetComponent<PlayerManager>();

        gameManager.MyPlayerBase = this.gameObject;
        playerManager.PlayerID = PlayerBaseID;
        playerManager.UpdateVariables();
    
        if (PlayerBaseID == 4)

        {
            GameObject.Find("BasePersonagem").GetComponent<Image>().sprite = QuadroGato;
            GameObject.Find("Spawn").GetComponent<Image>().sprite = SlotGato;
            GameObject.Find("SpawnExplorer").GetComponent<Image>().sprite = ExploradorGato;
            GameObject.Find("SpawnArcher").GetComponent<Image>().sprite = ArqueiroGato;
            GameObject.Find("SpawnWarrior").GetComponent<Image>().sprite = GuerreiroGato;
            GameObject.Find("SpawnFarm").GetComponent<Image>().sprite = FazendaGato;
            GameObject.Find("PainelStatus").GetComponent<Image>().sprite = StatusGato;
            GameObject.Find("Soldado").GetComponent<Image>().sprite = QuantGuerreiroGato;
            GameObject.Find("Comida").GetComponent<Image>().sprite = QuantFazendaGato;
            GameObject.Find("Dinheiro").GetComponent<Image>().sprite = QuantGoldGato;
            GameObject.Find("ObjectivePanel").GetComponent<Image>().sprite = ObjeGato;
            GameObject.Find("PassTurnBtn").GetComponent<Image>().sprite = PassGato;
            GameObject.Find("ReliquiasSlot").GetComponent<Image>().sprite = ReliGato;
            GameObject.Find("Panel").GetComponent<Image>().sprite = TempoGato;
            GameObject.Find("Opcoes").GetComponent<Image>().sprite = OpcoesGato;

            // Mat.material = MatBaseCao;
            GameObject CameraRot = GameObject.Find("CameraRotator");
            CameraRot.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 310, this.transform.eulerAngles.z);
        }
        if (PlayerBaseID == 3)
        {
            GameObject.Find("BasePersonagem").GetComponent<Image>().sprite = QuadroRato;
            GameObject.Find("Spawn").GetComponent<Image>().sprite = SlotRato;
            GameObject.Find("SpawnExplorer").GetComponent<Image>().sprite = ExploradorRato;
            GameObject.Find("SpawnArcher").GetComponent<Image>().sprite = ArqueiroRato;
            GameObject.Find("SpawnWarrior").GetComponent<Image>().sprite = GuerreiroRato;
            GameObject.Find("SpawnFarm").GetComponent<Image>().sprite = FazendaRato;
            GameObject.Find("PainelStatus").GetComponent<Image>().sprite = StatusRato;
            GameObject.Find("Soldado").GetComponent<Image>().sprite = QuantGuerreiroRato;
            GameObject.Find("Comida").GetComponent<Image>().sprite = QuantFazendaRato;
            GameObject.Find("Dinheiro").GetComponent<Image>().sprite = QuantGoldRato;
            GameObject.Find("ObjectivePanel").GetComponent<Image>().sprite = ObjeRato;
            GameObject.Find("PassTurnBtn").GetComponent<Image>().sprite = PassRato;
            GameObject.Find("ReliquiasSlot").GetComponent<Image>().sprite = ReliRato;
            GameObject.Find("Panel").GetComponent<Image>().sprite = TempoRato;
            GameObject.Find("Opcoes").GetComponent<Image>().sprite = OpcoesRato;

            // Mat.material = MatBaseGato;
            GameObject CameraRot = GameObject.Find("CameraRotator");
            CameraRot.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 45, this.transform.eulerAngles.z);
        }
        if (PlayerBaseID == 2)
        {
            GameObject.Find("BasePersonagem").GetComponent<Image>().sprite = QuadroPassaro;
            GameObject.Find("Spawn").GetComponent<Image>().sprite = SlotPassaro;
            GameObject.Find("SpawnExplorer").GetComponent<Image>().sprite = ExploradorPassaro;
            GameObject.Find("SpawnArcher").GetComponent<Image>().sprite = ArqueiroPassaro;
            GameObject.Find("SpawnWarrior").GetComponent<Image>().sprite = GuerreiroPassaro;
            GameObject.Find("SpawnFarm").GetComponent<Image>().sprite = FazendaPassaro;
            GameObject.Find("PainelStatus").GetComponent<Image>().sprite = StatusPassaro;
            GameObject.Find("Soldado").GetComponent<Image>().sprite = QuantGuerreiroPassaro;
            GameObject.Find("Comida").GetComponent<Image>().sprite = QuantFazendaPassaro;
            GameObject.Find("Dinheiro").GetComponent<Image>().sprite = QuantGoldPassaro;
            GameObject.Find("ObjectivePanel").GetComponent<Image>().sprite = ObjePassaro;
            GameObject.Find("PassTurnBtn").GetComponent<Image>().sprite = PassPassaro;
            GameObject.Find("ReliquiasSlot").GetComponent<Image>().sprite = ReliPassaro;
            GameObject.Find("Panel").GetComponent<Image>().sprite = TempoPassaro;
            GameObject.Find("Opcoes").GetComponent<Image>().sprite = OpcoesPassaro;


            // Mat.material = MatBaseRato;
            GameObject CameraRot = GameObject.Find("CameraRotator");
            CameraRot.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 210, this.transform.eulerAngles.z);
        }
        if (PlayerBaseID == 1)
        {
            GameObject.Find("BasePersonagem").GetComponent<Image>().sprite = QuadroCao;
            GameObject.Find("Spawn").GetComponent<Image>().sprite = SlotCao;
            GameObject.Find("SpawnExplorer").GetComponent<Image>().sprite = ExploradorCao;
            GameObject.Find("SpawnArcher").GetComponent<Image>().sprite = ArqueiroCao;
            GameObject.Find("SpawnWarrior").GetComponent<Image>().sprite = GuerreiroCao;
            GameObject.Find("SpawnFarm").GetComponent<Image>().sprite = FazendaCao;
            GameObject.Find("PainelStatus").GetComponent<Image>().sprite = StatusCao;
            GameObject.Find("Soldado").GetComponent<Image>().sprite = QuantGuerreiroCao;
            GameObject.Find("Comida").GetComponent<Image>().sprite = QuantFazendaCao;
            GameObject.Find("Dinheiro").GetComponent<Image>().sprite = QuantGoldCao;
            GameObject.Find("ObjectivePanel").GetComponent<Image>().sprite = ObjeCao;
            GameObject.Find("PassTurnBtn").GetComponent<Image>().sprite = PassCao;
            GameObject.Find("ReliquiasSlot").GetComponent<Image>().sprite = ReliCao;
            GameObject.Find("Panel").GetComponent<Image>().sprite = TempoCao;
            GameObject.Find("Opcoes").GetComponent<Image>().sprite = OpcoesCao;

            // Mat.material = MatBaseAguia;
            GameObject CameraRot = GameObject.Find("CameraRotator");
            CameraRot.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 135, this.transform.eulerAngles.z);
        }

        SpawnExplorerBtn = GameObject.Find("SpawnExplorer").GetComponent<Button>();
        SpawnExplorerBtn.onClick.AddListener(() => Cmd_SpawnExplorer(PlayerBaseID, false));

        SpawnGuerreiroBtn = GameObject.Find("SpawnWarrior").GetComponent<Button>();
        SpawnGuerreiroBtn.onClick.AddListener(() => Cmd_SpawnGuerreiro(PlayerBaseID));

        SpawnArqueiroBtn = GameObject.Find("SpawnArcher").GetComponent<Button>();
        SpawnArqueiroBtn.onClick.AddListener(() => Cmd_SpawnArqueiro(PlayerBaseID));

        PassTurnButton = GameObject.Find("PassTurnBtn").GetComponent<Button>();
        PassTurnButton.onClick.AddListener(() => Cmd_PassTurn(PlayerBaseID));

        SpawnFarmBtn = GameObject.Find("SpawnFarm").GetComponent<Button>();
        SpawnFarmBtn.onClick.AddListener(() => Cmd_RequestBuildFarm());

        GameObject[] AllClearRelicBtns = GameObject.FindGameObjectsWithTag("RemoveRelic");

        AllClearRelicBtns[0].GetComponent<Button>().onClick.AddListener(() => OpenConfirmation(0));
        GameObject.Find("Confirm01").GetComponent<Button>().onClick.AddListener(() => RemoveRelic(0));
        GameObject.Find("Cancel01").GetComponent<Button>().onClick.AddListener(() => CloseConfirmation(0));
        ConfirmationScreens.Add(GameObject.Find("Confirmacao01"));
        ConfirmationScreens[0].SetActive(false);

        AllClearRelicBtns[1].GetComponent<Button>().onClick.AddListener(() => OpenConfirmation(1));
        GameObject.Find("Confirm02").GetComponent<Button>().onClick.AddListener(() => RemoveRelic(1));
        GameObject.Find("Cancel02").GetComponent<Button>().onClick.AddListener(() => CloseConfirmation(1));
        ConfirmationScreens.Add(GameObject.Find("Confirmacao02"));
        ConfirmationScreens[1].SetActive(false);

        AllClearRelicBtns[2].GetComponent<Button>().onClick.AddListener(() => OpenConfirmation(2));
        GameObject.Find("Confirm03").GetComponent<Button>().onClick.AddListener(() => RemoveRelic(3));
        GameObject.Find("Cancel03").GetComponent<Button>().onClick.AddListener(() => CloseConfirmation(2));
        ConfirmationScreens.Add(GameObject.Find("Confirmacao03"));
        ConfirmationScreens[2].SetActive(false);

        AllClearRelicBtns[3].GetComponent<Button>().onClick.AddListener(() => OpenConfirmation(3));
        GameObject.Find("Confirm04").GetComponent<Button>().onClick.AddListener(() => RemoveRelic(2));
        GameObject.Find("Cancel04").GetComponent<Button>().onClick.AddListener(() => CloseConfirmation(3));
        ConfirmationScreens.Add(GameObject.Find("Confirmacao04"));
        ConfirmationScreens[3].SetActive(false);

        GetComponent<SoundVolumeFix>().enabled = true;

        Cmd_SpawnExplorer(PlayerBaseID, true);

        if (isServer)
            DistributeObjectives();

        Destroy(GameObject.Find("Background"));
        //Cmd_ControlaTempoTurno();

        ReadyToPlay = true;
       // StartCoroutine("MatchStartVoiceLine");
    }

    public void OpenConfirmation(int id)
    {
        ConfirmationScreens[id].SetActive(true);
    }

    public void CloseConfirmation (int id)
    {
        ConfirmationScreens[id].SetActive(false);
    }

    void DistributeObjectives()
    {
        GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
        List<int> UsedIds = new List<int>();
        for (int i = 0; i < AllPlayers.Length; i++)
        {
            int id = UnityEngine.Random.Range(1, 9);
            if (UsedIds.Count != 9)
            {
                while (UsedIds.Contains(id))
                {
                    id = UnityEngine.Random.Range(1, 9);
                }
            }
            
            UsedIds.Add(id);
            AllPlayers[i].GetComponent<PlayerBase>().Rpc_RecieveObjective(id);

            if(AllPlayers[i].GetComponent<PlayerBase>().PlayerBaseID == 1 && id == 1)
            {
                DistributeObjectives();
                break;
            }
            if (AllPlayers[i].GetComponent<PlayerBase>().PlayerBaseID == 2 && id == 2)
            {
                DistributeObjectives();
                break;
            }
            if (AllPlayers[i].GetComponent<PlayerBase>().PlayerBaseID == 3 && id == 4)
            {
                DistributeObjectives();
                break;
            }
            if (AllPlayers[i].GetComponent<PlayerBase>().PlayerBaseID == 4 && id == 3)
            {
                DistributeObjectives();
                break;
            }
        }
    }

    [ClientRpc]
    void Rpc_RecieveObjective(int ID)
    {
        MyObjective = ID;
        ObjectiveText = GameObject.Find("_ObjectiveText").GetComponent<Text>();
        if (isLocalPlayer)
        {
            switch (MyObjective)
            {
                case 1:
                    ObjectiveText.text = "Eliminate the Red Player or gather 3 Green Relics";
                    break;
                case 2:
                    ObjectiveText.text = "Elminate the Yellow Player or gather 3 Red Relics";
                    break;
                case 3:
                    ObjectiveText.text = "Eliminate the Green Player or gather 3 Blue Relics";
                    break;
                case 4:
                    ObjectiveText.text = "Eliminate the Blue Player or gather 3 Yellow Relics";
                    break;
                case 5:
                    ObjectiveText.text = "Possess 10 Farms and 2 Gold Mines";
                    break;
                case 6:
                    ObjectiveText.text = "Gather 3 Green Relics OR Gather 3 Blue Relics";
                    break;
                case 7:
                    ObjectiveText.text = "Gather 3 Yellow Relics or Gather 3 Red Relics";
                    break;
                case 8:
                    ObjectiveText.text = "Gather 3 Green Relics OR Gather 3 Yellow Relics";
                    break;
            }
        }
    }

    void Start()
    {
        Cmd_UpdatePlayerBaseID(Convert.ToInt32(GetComponent<NetworkIdentity>().netId.Value));
        DontDestroyOnLoad(this.gameObject);

    }


    [Command]
    public void Cmd_PassDestroyedTurn(int TargetTurn)
    {
        gameManager.curTurn = TargetTurn;
    }

    [ClientRpc]
    public void Rpc_PassDestroyedTurn(int TargetTurn)
    {
        gameManager.curTurn = TargetTurn;
    }

    [ClientRpc]
    public void Rpc_UpdateWinningPlayer(Vector3 Pos)
    {
        WinningPlayerPos = Pos;
    }

    [Command]
    public void Cmd_UpdateWinningPlayer(Vector3 Pos , GameObject target)
    {
        target.GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(Pos);
    }

    public void RemoveRelic(int i)
    {
        if (isLocalPlayer)
        {
            Reliquias.RemoveAt(i);
            //Reliquias.Sort();
            UpdateRelicUI();

        }
    }


    void UpdateRelicUI()
    {
        if (isLocalPlayer)
        {
            //Clear all Relics
            RelicSlot1.sprite = EmptyRelic;
            RelicSlot2.sprite = EmptyRelic;
            RelicSlot3.sprite = EmptyRelic;
            RelicSlot4.sprite = EmptyRelic;

            //Add Relics
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (i == 0)
                {
                    if (Reliquias[i] == 1)
                    {
                        RelicSlot1.sprite = RedRelic;
                    }
                    else if (Reliquias[i] == 2)
                    {
                        RelicSlot1.sprite = YellowRelic;
                    }
                    else if (Reliquias[i] == 3)
                    {
                        RelicSlot1.sprite = BlueRelic;
                    }
                    else if (Reliquias[i] == 4)
                    {
                        RelicSlot1.sprite = GreenRelic;
                    }
                }
                if (i == 1)
                {
                    if (Reliquias[i] == 1)
                    {
                        RelicSlot2.sprite = RedRelic;
                    }
                    else if (Reliquias[i] == 2)
                    {
                        RelicSlot2.sprite = YellowRelic;
                    }
                    else if (Reliquias[i] == 3)
                    {
                        RelicSlot2.sprite = BlueRelic;
                    }
                    else if (Reliquias[i] == 4)
                    {
                        RelicSlot2.sprite = GreenRelic;
                    }
                }
                if (i == 2)
                {
                    if (Reliquias[i] == 1)
                    {
                        RelicSlot3.sprite = RedRelic;
                    }
                    else if (Reliquias[i] == 2)
                    {
                        RelicSlot3.sprite = YellowRelic;
                    }
                    else if (Reliquias[i] == 3)
                    {
                        RelicSlot3.sprite = BlueRelic;
                    }
                    else if (Reliquias[i] == 4)
                    {
                        RelicSlot3.sprite = GreenRelic;
                    }
                }
                if (i == 3)
                {
                    if (Reliquias[i] == 1)
                    {
                        RelicSlot4.sprite = RedRelic;
                    }
                    else if (Reliquias[i] == 2)
                    {
                        RelicSlot4.sprite = YellowRelic;
                    }
                    else if (Reliquias[i] == 3)
                    {
                        RelicSlot4.sprite = BlueRelic;
                    }
                    else if (Reliquias[i] == 4)
                    {
                        RelicSlot4.sprite = GreenRelic;
                    }
                }
            }
        }
    }

    //[Command]
    public void Cmd_GetReliquia()
    {
        if (Reliquias.Count <= 3)
        {
            int i = UnityEngine.Random.Range(1, 5);
            Reliquias.Add(i);
            UpdateRelicUI();

            CheckRelicCondition();
        }
    }

    [ClientRpc]
    public void Rpc_GetReliquia()
    {
        if (Reliquias.Count <= 3)
        {
            int i = UnityEngine.Random.Range(1, 5);
            Reliquias.Add(i);
            UpdateRelicUI();

            CheckRelicCondition();
        }
    }


    void CheckRelicCondition()
    {
        if (MyObjective == 1)
        {
            //Verdes
            int Verdes = 0;
            for(int i = 0; i < Reliquias.Count; i++)
            {
                if(Reliquias[i] == 4)
                {
                    Verdes++;
                }
            }
            if(Verdes == 3)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }
        }

        if (MyObjective == 2)
        {
            //Vermelhos
            int Vermelhos = 0;
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (Reliquias[i] == 1)
                {
                    Vermelhos++;
                }
            }
            if (Vermelhos == 3)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }
        }

        if (MyObjective == 3)
        {
            //Azuius
            int Azuis = 0;
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (Reliquias[i] == 3)
                {
                    Azuis++;
                }
            }
            if (Azuis == 3)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }
        }

        if (MyObjective == 4)
        {
            //Amarelos
            int Amarelos = 0;
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (Reliquias[i] == 2)
                {
                    Amarelos++;
                }
            }
            if (Amarelos == 3)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }
        }

        if (MyObjective == 6)
        {
            //Azuis
            int Azuis = 0;
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (Reliquias[i] == 3)
                {
                    Azuis++;
                }
            }
            if (Azuis == 3)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }

            //Verdes
            int Verdes = 0;
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (Reliquias[i] == 4)
                {
                    Verdes++;
                }
            }
            if (Verdes == 3)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }
        }

        if (MyObjective == 7)
        {
            //Amarelos
            int Amarelos = 0;
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (Reliquias[i] == 2)
                {
                    Amarelos++;
                }
            }
            if (Amarelos == 3)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }

            //Vermelhos
            int Vermelhos = 0;
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (Reliquias[i] == 1)
                {
                    Vermelhos++;
                }
            }
            if (Vermelhos == 3)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }
        }

        if (MyObjective == 8)
        {
            //Amarelos
            int Amarelos = 0;
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (Reliquias[i] == 2)
                {
                    Amarelos++;
                }
            }
            if (Amarelos == 3)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }

            //Verdes
            int Verde = 0;
            for (int i = 0; i < Reliquias.Count; i++)
            {
                if (Reliquias[i] == 3)
                {
                    Verde++;
                }
            }
            if (Verde == 4)
            {
                Cmd_WinMatch(this.gameObject);
                WinningPlayerPos = this.gameObject.transform.position;
                GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                for (int i = 0; i < AllPlayers.Length; i++)
                {
                    if (AllPlayers[i] != this.gameObject)
                    {

                        if (isServer)
                        {
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                            AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                        }
                        else {
                            Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                            Cmd_LooseMatch(AllPlayers[i]);
                        }
                    }
                }
            }
        }
    }

    public void GetRelic()
    {
        if (Reliquias.Count <= 3)
        {
            int i = UnityEngine.Random.Range(1, 5);
            Reliquias.Add(i);
            UpdateRelicUI();
            CheckRelicCondition();
        }
    }

    bool TurnStart = true;

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {

            if (Input.GetKeyDown(KeyCode.T))
            {
                Cmd_SpawnArqueiro(2);
            }
            if(Initialized == false)
            {
                if (isLocalPlayer)
                {
                    StartCoroutine("DelayedStart");
                    
                    Initialized = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!InputText.isFocused)
                {
                    InputText.Select();
                    InputText.ActivateInputField();
                }
            }

            if (GameOver == true)
            {
                Camera.main.transform.parent.transform.position = Vector3.Lerp(Camera.main.transform.parent.transform.position, new Vector3(WinningPlayerPos.x, 7.1f, WinningPlayerPos.z), Time.deltaTime * 2);
            }

            if (CanMoveCamera == true)
            {
                GameObject.Find("CameraRotator").transform.position = Vector3.Lerp(GameObject.Find("CameraRotator").transform.position, new Vector3(WinningPlayerPos.x, 0, WinningPlayerPos.z), Time.deltaTime * 2);
            }

            if (ReadyToPlay == true)
            {
                UnitsAmmountText.text = UnitAmmount.ToString();
                GoldText.text = Gold.ToString();
                GameObject[] AllFarms = GameObject.FindGameObjectsWithTag("Farm");
                FoodText.text = Food + "/" + AllFarms.Length + "/20";

                if (gameManager.curTurn == playerManager.MyTurn && Destroyed == true)
                {
                    if (isServer)
                    {
                        Rpc_PassDestroyedTurn(playerManager.MyTurn + 1);
                    }
                    else
                    {
                        Cmd_PassDestroyedTurn(playerManager.MyTurn + 1);
                    }

                }

                ControlaTempoTurno();


                if (gameManager.curTurn == playerManager.MyTurn && Destroyed == false)
                {
                    if (TurnStart == true)
                    {
                        AudioSource As = GetComponent<AudioSource>();
                        As.clip = NarratorYourTurn;
                        As.Play();
                        int GoldToGive = 1;
                        GameObject[] AllGoldMines = GameObject.FindGameObjectsWithTag("GoldMine");
                        for (int i = 0; i < AllGoldMines.Length; i++)
                        {
                            if (AllGoldMines[i].GetComponent<GoldMineManager>().PlayerOwner != null)
                            {
                                if (AllGoldMines[i].GetComponent<GoldMineManager>().PlayerOwner == this.gameObject)
                                {
                                    GoldToGive++;
                                }
                            }
                        }
                        Cmd_GiveGold(GoldToGive);
                        TurnStart = false;
                    }

                    PassTurnButton.interactable = true;

                    if (Occupied == false)
                    {
                        //Explorador
                        if (Gold >= 2)
                        {
                            SpawnExplorerBtn.interactable = true;
                            SpawnFarmBtn.interactable = true;
                        }
                        else
                        {
                            SpawnExplorerBtn.interactable = false;
                            SpawnFarmBtn.interactable = false;
                        }

                        //Guerreiro
                        if (Gold >= 5)
                        {
                            SpawnGuerreiroBtn.interactable = true;
                        }
                        else
                        {
                            SpawnGuerreiroBtn.interactable = false;
                        }

                        //Arqueiro
                        if (Gold >= 7)
                        {
                            SpawnArqueiroBtn.interactable = true;
                        }
                        else
                        {
                            SpawnArqueiroBtn.interactable = false;
                        }
                    }
                    else
                    {
                        SpawnExplorerBtn.interactable = false;
                        SpawnGuerreiroBtn.interactable = false;
                        SpawnArqueiroBtn.interactable = false;
                        SpawnFarmBtn.interactable = false;
                    }

                }
                else
                {
                    PassTurnButton.interactable = false;
                    SpawnExplorerBtn.interactable = false;
                    SpawnGuerreiroBtn.interactable = false;
                    SpawnArqueiroBtn.interactable = false;
                    SpawnFarmBtn.interactable = false;
                    TurnStart = true;
                }

                if (curHealth <= 0 && Destroyed == false)
                {
                    Cmd_DestroyKingdom();
                }
            }

            //Condições de vitória
            if (MyObjective == 5)
            {
                if (Food == 11 & GoldMineAmmount == 2)
                {
                    GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
                    for (int i = 0; i < AllPlayers.Length; i++)
                    {
                        if (AllPlayers[i] != this.gameObject)
                        {

                            if (isServer)
                            {
                                AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                                AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                            }
                            else {
                                Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                                Cmd_LooseMatch(AllPlayers[i]);
                            }
                        }
                    }

                    //Rpc_UpdateWinningPlayer(this.transform.position);
                    if (isLocalPlayer)
                    {
                        if (isServer)
                        {
                            //Rpc_UpdateWinningPlayer(this.transform.position);
                            WinMatch();
                        }
                        else
                        {
                            //Rpc_UpdateWinningPlayer(this.transform.position);
                            WinMatch();
                        }
                    }
                }
            }
        }
    }

    [Command]
    public void Cmd_GiveGold(int GoldToGive)
    {
        Gold += GoldToGive;
    }

    public void WinMatch()
    {
        StartCoroutine("VictoryLine");
        ObjectiveText.text = "YOU WON THE GAME!!";
        WinningPlayerPos = this.transform.position;
        StartCoroutine("RepositionCamera");
    }

    [Command]
    public void Cmd_WinMatch(GameObject target)
    {
        target.GetComponent<PlayerBase>().Rpc_WinMatch();
    }

    [ClientRpc]
    public void Rpc_WinMatch()
    {
        ObjectiveText.text = "YOU WON THE GAME!!";
        WinningPlayerPos = this.transform.position;
        StartCoroutine("RepositionCamera");
    }

    [Command]
    public void Cmd_LooseMatch(GameObject target)
    {
        target.GetComponent<PlayerBase>().Rpc_LooseMatch();
    }

    [ClientRpc]
    public void Rpc_LooseMatch()
    {
        if (isLocalPlayer)
        {
            StartCoroutine("DefeatLine");
            ObjectiveText.text = "YOU LOST THE GAME!!";
            StartCoroutine("RepositionCamera");
        }
    }

    public IEnumerator VictoryLine()
    {
        yield return new WaitForSeconds(2.5f);
        AudioSource As = GetComponent<AudioSource>();
        As.clip = VictoryVoiceLine[PlayerBaseID - 1];
        As.Play();
    }

    public IEnumerator DefeatLine()
    {
        yield return new WaitForSeconds(2.5f);
        AudioSource As = GetComponent<AudioSource>();
        As.clip = DefeatVoiceLine[PlayerBaseID - 1];
        As.Play();
    }

    public IEnumerator RepositionCamera()
    {
        yield return new WaitForSeconds(2);
        Camera.main.transform.parent.GetComponent<MoveCamera>().enabled = false;
        GameOver = true;
    }

    [Command]
    public void Cmd_DestroyKingdom()
    {
        Destroyed = true;
        var AllUnits = GameObject.FindGameObjectsWithTag("Unit");
        for(int i = 0; i < AllUnits.Length; i++)
        {
            if(AllUnits[i].gameObject.GetComponent<UnitManager>().PlayerOwner == this.gameObject)
            {
                AllUnits[i].GetComponent<UnitManager>().curHealth = 0;
            }
        }
        GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("PlayerBase");
        GameObject TargetPlayer = null;
        for(int i = 0; i < AllPlayers.Length; i++)
        {
            if(AllPlayers[i].GetComponent<PlayerBase>().PlayerBaseID == LastAttackingPlayerId)
            {
                TargetPlayer = AllPlayers[i];
            }
        }

        if(TargetPlayer.GetComponent<PlayerBase>().MyObjective == 1 && PlayerBaseID == 1)
        {
            Cmd_WinMatch(TargetPlayer);
            WinningPlayerPos = TargetPlayer.transform.position;
            for (int i = 0; i < AllPlayers.Length; i++)
            {
                if (AllPlayers[i] != TargetPlayer)
                {

                    if (isServer)
                    {
                        AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                        AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                    }
                    else {
                        Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                        Cmd_LooseMatch(AllPlayers[i]);
                    }
                }
            }
        }
        if (TargetPlayer.GetComponent<PlayerBase>().MyObjective == 2 && PlayerBaseID == 2)
        {
            Cmd_WinMatch(TargetPlayer);
            WinningPlayerPos = TargetPlayer.transform.position;

            for (int i = 0; i < AllPlayers.Length; i++)
            {
                if (AllPlayers[i] != TargetPlayer)
                {

                    if (isServer)
                    {
                        AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                        AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                    }
                    else {
                        Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                        Cmd_LooseMatch(AllPlayers[i]);
                    }
                }
            }
        }
        if (TargetPlayer.GetComponent<PlayerBase>().MyObjective == 3 && PlayerBaseID == 4)
        {
            Cmd_WinMatch(TargetPlayer);
            WinningPlayerPos = TargetPlayer.transform.position;

            for (int i = 0; i < AllPlayers.Length; i++)
            {
                if (AllPlayers[i] != TargetPlayer)
                {

                    if (isServer)
                    {
                        AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                        AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                    }
                    else {
                        Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                        Cmd_LooseMatch(AllPlayers[i]);
                    }
                }
            }
        }
        if (TargetPlayer.GetComponent<PlayerBase>().MyObjective == 4 && PlayerBaseID == 3)
        {
            Cmd_WinMatch(TargetPlayer);
            WinningPlayerPos = TargetPlayer.transform.position;

            for (int i = 0; i < AllPlayers.Length; i++)
            {
                if (AllPlayers[i] != TargetPlayer)
                {

                    if (isServer)
                    {
                        AllPlayers[i].GetComponent<PlayerBase>().Rpc_UpdateWinningPlayer(this.transform.position);
                        AllPlayers[i].GetComponent<PlayerBase>().Rpc_LooseMatch();
                    }
                    else {
                        Cmd_UpdateWinningPlayer(this.transform.position, AllPlayers[i]);
                        Cmd_LooseMatch(AllPlayers[i]);
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Unit")
            Occupied = true;

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Unit")
            Occupied = false;
    }

    [Command]
    public void Cmd_SwitchOccupied()
    {
        Occupied = false;
    }

    [ClientRpc]
    void Rpc_UpdateUnitAmmount(int Count)
    {
        UnitAmmount = Count;
    }

    [Command]
    public void Cmd_SpawnExplorer( int ExplorerID , bool isFirst )
    {
        GameObject[] AllUnits = GameObject.FindGameObjectsWithTag("Unit");
        int AmmountOfFriendlyUnits = 0;
        for(int i = 0; i < AllUnits.Length; i++)
        {
            if(AllUnits[i].GetComponent<UnitManager>().PlayerOwner == this.gameObject )
                AmmountOfFriendlyUnits++;
        }
        if (Food > AmmountOfFriendlyUnits && AmmountOfFriendlyUnits <= 5)
        {
            var Explorer = Cao_Explorer;
            if (isFirst == false)
            {
                if (Gold >= 2)
                {
                    Gold -= 2;
                    UnitAmmount++;
                    Rpc_UpdateUnitAmmount(UnitAmmount);

                    if (ExplorerID == 1)
                    {
                        Explorer = Cao_Explorer;
                    }
                    if (ExplorerID == 2)
                    {
                        Explorer = Aguia_Explorer;
                    }
                    if (ExplorerID == 3)
                    {
                        Explorer = Rato_Explorer;
                    }
                    if (ExplorerID == 4)
                    {
                        Explorer = Gato_Explorer;
                    }

                    GameObject go = (GameObject)Instantiate(Explorer, this.transform.position, Quaternion.identity);
                    NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
                    go.GetComponent<UnitManager>().PlayerOwner = this.gameObject;
                    if (isFirst == true)
                        go.GetComponent<UnitManager>().curActions = 3;

                    Rpc_SetObjectOwner(go);
                }
            }
            else
            {

                UnitAmmount++;
                Rpc_UpdateUnitAmmount(UnitAmmount);
                if (ExplorerID == 1)
                {
                    Explorer = Cao_Explorer;
                }
                if (ExplorerID == 2)
                {
                    Explorer = Aguia_Explorer;
                }
                if (ExplorerID == 3)
                {
                    Explorer = Rato_Explorer;
                }
                if (ExplorerID == 4)
                {
                    Explorer = Gato_Explorer;
                }

                GameObject go = (GameObject)Instantiate(Explorer, this.transform.position, Quaternion.identity);
                NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
                go.GetComponent<UnitManager>().PlayerOwner = this.gameObject;
                if (isFirst == true)
                    go.GetComponent<UnitManager>().curActions = 3;

                Rpc_SetObjectOwner(go);
            }
        }
    }

    [Command]
    public void Cmd_SpawnGuerreiro(int WarriorID)
    {
        GameObject[] AllUnits = GameObject.FindGameObjectsWithTag("Unit");
        int AmmountOfFriendlyUnits = 0;
        for (int i = 0; i < AllUnits.Length; i++)
        {
            if (AllUnits[i].GetComponent<UnitManager>().PlayerOwner == this.gameObject)
                AmmountOfFriendlyUnits++;
        }
        if (Food > AmmountOfFriendlyUnits && AmmountOfFriendlyUnits <= 5)
        {
            var Warrior = Cao_Warrior;

            if (Gold >= 5)
            {
                UnitAmmount++;
                Rpc_UpdateUnitAmmount(UnitAmmount);
                Gold -= 5;

                if (WarriorID == 1)
                {
                    Warrior = Cao_Warrior;
                }
                if (WarriorID == 2)
                {
                    Warrior = Aguia_Warrior;
                }
                if (WarriorID == 3)
                {
                    Warrior = Rato_Warrior;
                }
                if (WarriorID == 4)
                {
                    Warrior = Gato_Warrior;
                }

                GameObject go = (GameObject)Instantiate(Warrior, this.transform.position, Quaternion.identity);
                NetworkServer.SpawnWithClientAuthority(go, this.gameObject);
                go.GetComponent<UnitManager>().PlayerOwner = this.gameObject;

                Rpc_SetObjectOwner(go);
            }
        }
    }

    [ClientRpc]
    public void Rpc_UpdateFarmVisuals(GameObject Tile , GameObject Farm)
    {
        Material[] temp = new Material[1];
        Tile.transform.GetChild(0).GetComponent<MeshRenderer>().materials = temp;
        Tile.transform.GetChild(0).GetComponent<MeshFilter>().mesh = FarmTileMesh;
        switch (Tile.transform.GetChild(0).transform.tag)
        {
            case "GrassTile":
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[0];
                break;
            case "SandTile":
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[1];
                break;
            case "DirtTile":
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[2];
                break;
            case "SnowTile":
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[3];
                break;
            default:
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[0];
                break;
        }
        
        Farm.transform.parent = Tile.transform;
    }

    [Command]
    public void Cmd_BuildFarm(GameObject Target, GameObject Tile)
    {
        Material[] temp = new Material[1];
        Tile.transform.GetChild(0).GetComponent<MeshRenderer>().materials = temp;
        Tile.transform.GetChild(0).GetComponent<MeshFilter>().mesh = FarmTileMesh;
        switch (Tile.transform.GetChild(0).transform.tag)
        {
            case "GrassTile":
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[0];
                break;
            case "SandTile":
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[1];
                break;
            case "DirtTile":
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[2];
                break;
            case "SnowTile":
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[3];
                break;
            default:
                Tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = FarmTileMat[0];
                break;
        }

        GameObject go = (GameObject)Instantiate(Farm, Target.transform.position, Quaternion.identity);
        NetworkServer.Spawn(go);

        go.transform.parent = Tile.transform;

        Rpc_UpdateFarmVisuals(Tile, go);

        if (isServer)
        {
            go.GetComponent<FarmManager>().Rpc_SetInitialOwner(this.gameObject);

        }
        else
        {
            go.GetComponent<FarmManager>().Cmd_SetInitialOwner(this.gameObject);
        }

    }

    
    public void Cmd_RequestBuildFarm()
    {
        var AllUnits = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0; i < AllUnits.Length; i++)
        {
            if (AllUnits[i].gameObject.GetComponent<UnitManager>().PlayerOwner == this.gameObject)
            {
                if (AllUnits[i].GetComponent<UnitManager>().Selected == true)
                {

                    AllUnits[i].GetComponent<UnitManager>().Cmd_SpawnFarm();
                }
            }
        }
    }


    [Command]
    public void Cmd_SpawnArqueiro(int ArcherID)
    {
        GameObject[] AllUnits = GameObject.FindGameObjectsWithTag("Unit");
        int AmmountOfFriendlyUnits = 0;
        for (int i = 0; i < AllUnits.Length; i++)
        {
            if (AllUnits[i].GetComponent<UnitManager>().PlayerOwner == this.gameObject)
                AmmountOfFriendlyUnits++;
        }
        if (Food > AmmountOfFriendlyUnits && AmmountOfFriendlyUnits <= 5)
        {
            var Archer = Cao_Archer;

            if (Gold >= 7)
            {
                Gold -= 7;
                UnitAmmount++;
                Rpc_UpdateUnitAmmount(UnitAmmount);

                if (ArcherID == 1)
                {
                    Archer = Cao_Archer;
                }
                if (ArcherID == 2)
                {
                    Archer = Aguia_Archer;
                }
                if (ArcherID == 3)
                {
                    Archer = Rato_Archer;
                }
                if (ArcherID == 4)
                {
                    Archer = Gato_Archer;
                }

                GameObject go = (GameObject)Instantiate(Archer, this.transform.position, Quaternion.identity);
                NetworkServer.SpawnWithClientAuthority(go, this.gameObject);
                go.GetComponent<UnitManager>().PlayerOwner = this.gameObject;

                Rpc_SetObjectOwner(go);
            }
        }
    }

    [ClientRpc]
    public void Rpc_SetObjectOwner(GameObject LastCreatedObject)
    {
        LastCreatedObject.GetComponent<UnitManager>().PlayerOwner = this.gameObject;
    }

    [Command]
    public void Cmd_PassTurn(int ID)
    {
        GameManager TempGameManager = GameObject.Find("_GameManager").GetComponent<GameManager>();

        if (TempGameManager.curTurn == ID)
        {
            TempGameManager.curTurn++;
            tempoTurno = 45;
            Rpc_UpdateTempoTurno();
            TempGameManager.ActualCurTurn++;

            if (TempGameManager.curTurn > TempGameManager.MaxTurns)
                TempGameManager.curTurn = 1;

            //Aqui carrego a variável com todos os objetos da cena que possuem o Tag "Unit"
            GameObject[] AllFriendlyUnits = GameObject.FindGameObjectsWithTag("Unit");

            //Este For loop, é para identificar qual das unidades do jogador que está atualmente selecionado e salvá-lo na variável "SelectedUnit"
            for (int i = 0; i < AllFriendlyUnits.Length; i++)
            {
                AllFriendlyUnits[i].GetComponent<UnitManager>().curActions = AllFriendlyUnits[i].GetComponent<UnitManager>().MaxActions;
                AllFriendlyUnits[i].GetComponent<UnitManager>().HasAttacked = false;
            }

            Rpc_PassTurn();
        }

    }

    [Command]
    public void Cmd_ForcePassTurn()
    {
        GameManager TempGameManager = GameObject.Find("_GameManager").GetComponent<GameManager>();

        TempGameManager.curTurn++;
        tempoTurno = 45;
        Rpc_UpdateTempoTurno();

        if (TempGameManager.curTurn > TempGameManager.MaxTurns)
            TempGameManager.curTurn = 1;

        //Aqui carrego a variável com todos os objetos da cena que possuem o Tag "Unit"
        GameObject[] AllFriendlyUnits = GameObject.FindGameObjectsWithTag("Unit");

        //Este For loop, é para identificar qual das unidades do jogador que está atualmente selecionado e salvá-lo na variável "SelectedUnit"
        for (int i = 0; i < AllFriendlyUnits.Length; i++)
        {
            AllFriendlyUnits[i].GetComponent<UnitManager>().curActions = AllFriendlyUnits[i].GetComponent<UnitManager>().MaxActions;
            AllFriendlyUnits[i].GetComponent<UnitManager>().HasAttacked = false;
        }

        Rpc_PassTurn();

    }

    [ClientRpc]
    public void Rpc_PassTurn()
    {
        //tempoTurno = 45;
        //Gold++;
        GameObject[] AllFarms = GameObject.FindGameObjectsWithTag("Farm");
        for(int i = 0; i < AllFarms.Length; i++)
        {
            FarmManager FarmMg = AllFarms[i].GetComponent<FarmManager>();

            if (FarmMg.PlayerOwner == this.gameObject)
                FarmMg.Cmd_healUnit();
        }

        //Aqui carrego a variável com todos os objetos da cena que possuem o Tag "Unit"
        GameObject[] AllFriendlyUnits = GameObject.FindGameObjectsWithTag("Unit");

        //Este For loop, é para identificar qual das unidades do jogador que está atualmente selecionado e salvá-lo na variável "SelectedUnit"
        for (int i = 0; i < AllFriendlyUnits.Length; i++)
        {
            AllFriendlyUnits[i].GetComponent<UnitManager>().curActions = AllFriendlyUnits[i].GetComponent<UnitManager>().MaxActions;
            AllFriendlyUnits[i].GetComponent<UnitManager>().HasAttacked = false;
        }
        
    }

    [Command]
    void Cmd_ResetTimer()
    {
        tempoTurno = 45;
    }

    void ResetTimer()
    {
        tempoTurno = 45;
    }

    [Command]
    public void Cmd_MoveUnit(GameObject Obj, GameObject Tile)
    {

        Obj.GetComponent<UnitManager>().Rpc_MoveTowardsPoint(new Vector3(Tile.transform.position.x , Tile.transform.position.y + 0.15f , Tile.transform.position.z ));
        
        Rpc_MoveUnit(Obj, Tile);

    }

    [ClientRpc]
    public void Rpc_MoveUnit(GameObject Obj, GameObject Tile)
    {

        Obj.GetComponent<UnitManager>().Rpc_MoveTowardsPoint(new Vector3(Tile.transform.position.x, Tile.transform.position.y + 0.15f, Tile.transform.position.z));
        
    }

    [Command]
    public void Cmd_UpdateSteppingOnTile(GameObject tile , GameObject Obj)
    {
        
        tile.GetComponent<TileManager>().SteppingObject = Obj;
        Rpc_UpdateSteppingOnTile(tile, Obj);
    }

    [ClientRpc]
    public void Rpc_UpdateSteppingOnTile(GameObject tile, GameObject Obj)
    {
        
        tile.GetComponent<TileManager>().SteppingObject = Obj;
    }

    [ClientRpc]
    public void Rpc_UpdateTempoTurno()
    {
        GameObject[] AllBases = GameObject.FindGameObjectsWithTag("PlayerBase");
        for(int i = 0; i < AllBases.Length; i++)
        {
            AllBases[i].GetComponent<PlayerBase>().tempoTurno = 45;
        }
    }

    
    public void ControlaTempoTurno() {
        
        tempoTurno = tempoTurno - Time.deltaTime;
        TempoTxt.text = Convert.ToInt32(tempoTurno).ToString();
        if (isServer)
        {
            if (tempoTurno <= 0)
            {
                Cmd_ForcePassTurn();
                tempoTurno = 45;
                Rpc_UpdateTempoTurno();
            }
        }
    }
}