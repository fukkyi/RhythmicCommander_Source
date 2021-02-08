using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateController : MonoBehaviour
{
    [SerializeField]
    private CreateTimeController timeController = null;
    [SerializeField]
    private CreateMusicController musicController = null;
    [SerializeField]
    private ChartDataController chartDataController = null;
    [SerializeField]
    private CreateChartController chartController = null;
    [SerializeField]
    private CreateMeasureController measureController = null;
    [SerializeField]
    private CreateNotesController notesController = null;
    [SerializeField]
    private DialogController dialogController = null;

    [SerializeField]
    public Transform lanePrarent = null;
    [SerializeField]
    public GameObject[] laneObj = null;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        chartDataController.Init();
        musicController.Init();
        chartController.Init();
        timeController.Init();
        measureController.Init();
        notesController.Init(laneObj);
    }

    // Update is called once per frame
    void Update()
    {
        timeController.UpdateTime();
        measureController.MoveMeasure();
        notesController.InputManage();
        dialogController.UpdateMange();
    }
}
