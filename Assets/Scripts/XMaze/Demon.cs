﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demon : MonoBehaviour
{

    public string doneMsg = "Final Score: ";
    public string preRew = "+";

    public int mode;

    public float visibleTime;
    public float selectTime;
    public float returnTime;
    public float totalTime;

    public int direction;
    public int trialNum = 0;

    public int[] contexts;
    public float[] holds;

    public int[] leftObjects;
    public int[] leftRewards;
    public int[] rightObjects;
    public int[] rightRewards;
    public float[] choiceDelays;
    public float[] rewardDelays;

    public float westXPos = 184.2314f;
    public float eastXPos = 304.2314f;
    public float yPos = 3f;
    public float zPos = 255f;

    public float east = 90f;
    public float west = 270f;

    public float holdTurnRate = 90f;
    public float returnRate = 180f;
    public float returnSpeedX = 20f;
    public float returnSpeedZ = 20f;

    public Text rewardText;
    public Text scoreText;

    SimpleMovement move;

    public float endTimer = 5f;

    public enum segments
    {
        Hallway,
        HoldA,
        Selection,
        Choice,
        Reward,
        Return,
        HoldB,
        EndRun
    };

    public segments segment;

    public GameObject contextN;
    public GameObject contextS;

    public GameObject objectNE;
    public GameObject objectSE;
    public GameObject objectSW;
    public GameObject objectNW;

    public GameObject goalNE;
    public GameObject goalSE;
    public GameObject goalSW;
    public GameObject goalNW;

    public FileWriter writer;
    public FileReader reader;

    public string[] contextList =
    {
        "Gray",
        "Wood",
        "Brick",
        "Stone",
        "Metal"
    };

    private float choiceStart;
    private float selectStart;
    private float delayStart;
    private bool vis;

    private int score;

    public int choice = 0;

    public float choiceAngle = 50f;

    private bool wasWhite = false;
    public bool isWhite = false;
    private bool doLog = false;

    private SoundLogger syncLogger;
    private SoundLogger frameLogger;
    private readonly string syncFileName = "soundoutput.xml";
    private readonly string frameFileName = "frames.xml";

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            reader = GameObject.Find("FileReader").GetComponent<FileReader>();
        }
        catch
        {
            Debug.Log("Start with Gray_Screen.");
        }

        reader.XMazeInit();
        writer = GameObject.Find("FileWriter").GetComponent<FileWriter>();
        writer.XMazeInit();

        syncLogger = new SoundLogger(syncFileName);
        frameLogger = new SoundLogger(frameFileName);

        segment = segments.Hallway;
        move = GetComponent<SimpleMovement>();
        rewardText.enabled = false;
        scoreText.text = 0.ToString();
        trialNum = 0;
        if(mode == 2)
        {
            SetContexts();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (doLog)
        {
            syncLogger.Log(Time.time.ToString());
            doLog = false;
        }
        frameLogger.Log(Time.time.ToString());
        if ((wasWhite && !isWhite) || (!wasWhite && isWhite))
        {
            wasWhite = !wasWhite;
            doLog = true; // log frame start time at start of next frame; assuming this is relatively close to display time
        }

        if (segment == segments.Hallway)
        {
            if(direction == 1 && transform.position.x >= eastXPos)
            {
                segment = segments.HoldA;
                choiceStart = Time.time;
                writer.WriteSegment();
                move.BeginHold(holds[trialNum]);
                transform.position = new Vector3(eastXPos, transform.position.y, zPos);
                ClearContexts();
            }
            else if(direction == 2 && transform.position.x <= westXPos)
            {
                segment = segments.HoldA;
                choiceStart = Time.time;
                writer.WriteSegment();
                move.BeginHold(holds[trialNum]);
                transform.position = new Vector3(westXPos, transform.position.y, zPos);
                ClearContexts();
            }
        }
        
        else if(segment == segments.HoldA)
        {
            Rotate(holdTurnRate);

            if(!move.IsHolding())
            {
                segment = segments.Selection;
                selectStart = Time.time;
                writer.WriteSegment();
                vis = true;
                move.BeginHold(999f);
                if(direction == 1)
                {
                    if(mode == 2){
                        objectNE.SendMessage("Sprite", leftObjects[trialNum]);
                        objectSE.SendMessage("Sprite", rightObjects[trialNum]);
                    }
                    else
                    {
                        goalNE.SendMessage("Render", leftObjects[trialNum]);
                        goalSE.SendMessage("Render", rightObjects[trialNum]);
                    }
                }
                else if(direction == 2)
                {
                    if(mode == 2){
                        objectSW.SendMessage("Sprite", leftObjects[trialNum]);
                        objectNW.SendMessage("Sprite", rightObjects[trialNum]);
                    }
                    else
                    {
                        goalSW.SendMessage("Render", leftObjects[trialNum]);
                        goalNW.SendMessage("Render", rightObjects[trialNum]);
                    }
                }
                else{Debug.Log("Direction machine broke.");}
            }
        }
        
        else if(segment == segments.Selection)
        {
            if(choice == 0)
            {
                move.BeginHold(999f);
                if(Input.GetAxis("Horizontal") < 0)
                {
                    choice = 1;
                    move.EndHold();
                    move.choiceAxis = -1f;
                }
                else if(Input.GetAxis("Horizontal") > 0)
                {
                    choice = 2;
                    move.EndHold();
                    move.choiceAxis = 1f;
                }
            }

            if(choice == 1 && direction == 1){
                if(transform.eulerAngles.y <= east - choiceAngle){
                    move.choiceAxis = 0f;
                }
            }
            else if(choice == 1 && direction == 2){
                if(transform.eulerAngles.y <= west - choiceAngle)
                {
                    move.choiceAxis = 0f;
                }
            }
            else if(choice == 2 && direction == 1){
                if(transform.eulerAngles.y >= east + choiceAngle)
                {
                    move.choiceAxis = 0f;
                }
            }
            else if(choice == 2 && direction == 2){
                if(transform.eulerAngles.y >= west + choiceAngle)
                {
                    move.choiceAxis = 0f;
                }
            }

            if(vis && Time.time - selectStart >= visibleTime)
            {
                ClearVisibility();
            }
            if(Time.time - selectStart >= selectTime)
            {
                GiveReward(null);
            }
        }

        else if(segment == segments.Choice)
        {
            if(Time.time - delayStart >= choiceDelays[trialNum])
            {
                segment = segments.Reward;
                writer.WriteSegment();
                scoreText.text = score.ToString(); 
                delayStart = Time.time;
                rewardText.enabled = true;
                move.BeginHold(999f);
            }
        }
        
        else if(segment == segments.Reward)
        {
            if(Time.time - delayStart >= rewardDelays[trialNum]) 
            {
		segment = segments.Return;
                writer.WriteSegment();
                rewardText.enabled = false;
                move.BeginHold(returnTime);
            }
        }

        else if(segment == segments.Return)
        {
            if(direction == 2)
            {
                if(transform.position.x > eastXPos)
                {
                    move.move.x -= returnSpeedX * Time.deltaTime;
                    if(transform.position.x + move.move.x < eastXPos)
                    {
                        move.move.x = eastXPos - transform.position.x;
                    }
                }
            }
            else if(direction == 1)
            {
                if(transform.position.x < westXPos)
                {
                    move.move.x += returnSpeedX * Time.deltaTime;
                    if(transform.position.x + move.move.x > eastXPos)
                    {
                        move.move.x = eastXPos - transform.position.x;
                    }
                }
            }
            else{Debug.Log("Direction machine broke.");}

            if(transform.position.z > zPos)
            {
                move.move.z -= returnSpeedZ * Time.deltaTime;
                if(transform.position.z + move.move.z < zPos)
                {
                    move.move.z = zPos - transform.position.z;
                }
            }
            else if(transform.position.z < zPos)
            {
                move.move.z += returnSpeedZ * Time.deltaTime;
                if(transform.position.z + move.move.z > zPos)
                {
                    move.move.z = zPos - transform.position.z;
                }
            }

            Rotate(returnRate);

            if(!move.IsHolding()){
                segment = segments.HoldB;
                writer.WriteSegment();
                move.BeginHold(999f);
                rewardText.enabled = false;
            }
        }
        
        else if(segment == segments.HoldB)
        {
            if(trialNum + 1 == contexts.Length)
            {
                rewardText.text = doneMsg + score.ToString();
                rewardText.enabled = true;
                scoreText.enabled = false;
            }
            if(Time.time - choiceStart >= totalTime)
            {
                try
                {
                    trialNum++;
                    SetContexts();
                    //Debug.Log("Contexts set");
                    move.EndHold();
                    segment = segments.Hallway;
                    writer.WriteSegment();
                }
                catch
                {
                    //Debug.Log("Caught");
                    segment = segments.EndRun;
                    writer.WriteSegment();
                }
            }
        }

        else if(segment == segments.EndRun)
        {
            if(endTimer <= 0)
            {
                //Debug.Log("Quit!");
                Application.Quit();
            }
            endTimer -= Time.deltaTime;
            //Debug.Log("Quitting in: " + endTimer.ToString());
        }
    }

    void GiveReward(GameObject obj)
    {
        if(segment == segments.Selection)
        {
            int reward = 0;
            int select = 0;
            if(mode == 2)
            {
                if(obj == objectNE)
                {
                    reward = leftRewards[trialNum];
                    select = 1;
                }
                else if(obj == objectSE)
                {
                    reward = rightRewards[trialNum];
                    select = 2;
                }
                else if(obj == objectSW)
                {
                    reward = leftRewards[trialNum];
                    select = 1;
                }
                else if(obj == objectNW)
                {
                    reward = rightRewards[trialNum];
                    select = 2;
                }
            }
            else
            {
                if(obj == objectNE)
                {
                    reward = leftObjects[trialNum];
                    select = 1;
                }
                else if(obj == objectSE)
                {
                    reward = rightObjects[trialNum];
                    select = 2;
                }
                else if(obj == objectSW)
                {
                    reward = leftObjects[trialNum];
                    select = 1;
                }
                else if(obj == objectNW)
                {
                    reward = rightObjects[trialNum];
                    select = 2;
                }
            }

            rewardText.text = preRew + reward.ToString();
            score += reward;
            writer.WriteSelect(select, reward, score);
            segment = segments.Choice;
            writer.WriteSegment();
            choice = 0;
            move.choiceAxis = 0f;
            delayStart = Time.time;
            move.BeginHold(999f);
            ClearVisibility();

            /*
            segment = segments.Choice;
            writer.WriteSegment();
            rewardText.text = preRew + reward.ToString();
            rewardText.enabled = true;
            score += reward;
            writer.WriteSelect(select, reward, score);
            scoreText.text = score.ToString();
            move.BeginHold(returnTime);
            choice = 0;
            move.choiceAxis = 0f;
            */

            if(direction == 1)
            {
                direction = 2;
            }
            else if(direction == 2)
            {
                direction = 1;
            }
            else{Debug.Log("Direction machine broke.");}
        }
    }

    void ClearVisibility()
    {
        int zero = 0;

        objectNE.SendMessage("Sprite", zero);
        objectSE.SendMessage("Sprite", zero);
        objectSW.SendMessage("Sprite", zero);
        objectNW.SendMessage("Sprite", zero);

        goalNE.SendMessage("Render", zero);
        goalSE.SendMessage("Render", zero);
        goalSW.SendMessage("Render", zero);
        goalNW.SendMessage("Render", zero);

        vis = false;
    }

    void Rotate(float rate)
    {
        if(direction == 1)
        {
            if(transform.eulerAngles.y < east || transform.eulerAngles.y > west)
            {
                move.rotate.y += Time.deltaTime * rate;
                if(transform.eulerAngles.y + move.rotate.y > east &&
                    transform.eulerAngles.y + move.rotate.y < west)
                {
                    move.rotate.y = east - transform.eulerAngles.y;
                }
            }
            else if(transform.eulerAngles.y > east)
            {
                move.rotate.y -= Time.deltaTime * rate;
                if(transform.eulerAngles.y + move.rotate.y < east)
                {
                    move.rotate.y = east - transform.eulerAngles.y;
                }
            }
        }
        else if(direction == 2)
        {
            if(transform.eulerAngles.y > west || transform.eulerAngles.y < east)
            {
                move.rotate.y -= Time.deltaTime * rate;
                if(transform.eulerAngles.y + move.rotate.y < west &&
                    transform.eulerAngles.y + move.rotate.y > east)
                {
                    move.rotate.y = west - transform.eulerAngles.y;
                }
            }
            else if(transform.eulerAngles.y < west)
            {
                move.rotate.y += Time.deltaTime * rate;
                if(transform.eulerAngles.y + move.rotate.y > west)
                {
                    move.rotate.y = west - transform.eulerAngles.y;
                }
            }
        }
        else{Debug.Log("Direction machine broke.");}
    }

    void SetContexts()
    {
        if(mode != 2){
            contexts[trialNum] = 0;
            return;
        }

            contextN.SendMessage(contextList[contexts[trialNum]]);
            contextS.SendMessage(contextList[contexts[trialNum]]);
    }

    void ClearContexts()
    {
        contextN.SendMessage(contextList[0]);
        contextS.SendMessage(contextList[0]);
    }

    void OnDestroy()
    {
        syncLogger.Dispose();
        frameLogger.Dispose();
    }
}
