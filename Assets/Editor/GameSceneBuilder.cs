using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using RacingGoMap.Core;
using RacingGoMap.UI;
using RacingGoMap.Car;
using RacingGoMap.Race;

/// <summary>
/// 유니티 에디터 메뉴 → Racing → 씬 전체 생성
/// 한 번의 클릭으로 모든 씬과 Car 프리팹을 자동 생성합니다.
/// </summary>
public static class GameSceneBuilder
{
    const string kScenes  = "Assets/Scenes";
    const string kPrefabs = "Assets/Prefabs";

    // ── Pastel palette ────────────────────────────────────────────
    static Color C(float r, float g, float b, float a = 1f) => new Color(r, g, b, a);

    static readonly Color BgLogin       = C(0.95f, 0.90f, 1.00f);
    static readonly Color BgMainMenu    = C(0.88f, 0.96f, 0.88f);
    static readonly Color BgMatchmaking = C(0.85f, 0.90f, 0.98f);
    static readonly Color BgGarage      = C(0.98f, 0.95f, 0.85f);
    static readonly Color BgResult      = C(1.00f, 0.95f, 0.85f);

    static readonly Color BtnGreen  = C(0.35f, 0.78f, 0.45f);
    static readonly Color BtnBlue   = C(0.40f, 0.60f, 0.90f);
    static readonly Color BtnOrange = C(1.00f, 0.58f, 0.18f);
    static readonly Color BtnPurple = C(0.68f, 0.45f, 0.92f);
    static readonly Color BtnRed    = C(0.88f, 0.30f, 0.30f);
    static readonly Color BtnGray   = C(0.72f, 0.72f, 0.72f);

    // ── Menu items ────────────────────────────────────────────────

    [MenuItem("Racing/🏁 씬 전체 생성 (Setup All Scenes)")]
    public static void BuildAll()
    {
        EnsureFolders();
        CarPrefabSetup();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        BuildScene("LoginScene",      SetupLogin);
        BuildScene("MainMenuScene",   SetupMainMenu);
        BuildScene("MatchmakingScene",SetupMatchmaking);
        BuildScene("GarageScene",     SetupGarage);
        BuildScene("RaceScene",       SetupRace);
        BuildScene("ResultScene",     SetupResult);

        UpdateBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "너랑나랑 레이싱 고맵 ✅",
            "씬 생성 완료!\n\n" +
            "• Assets/Scenes/ — 6개 씬\n" +
            "• Assets/Prefabs/Car.prefab — 차량 프리팹\n\n" +
            "LoginScene 에서 Play를 눌러 게임을 시작하세요.",
            "확인");
    }

    [MenuItem("Racing/씬 개별/Login")]       static void MenuLogin()      => BuildScene("LoginScene",      SetupLogin);
    [MenuItem("Racing/씬 개별/MainMenu")]    static void MenuMainMenu()   => BuildScene("MainMenuScene",   SetupMainMenu);
    [MenuItem("Racing/씬 개별/Matchmaking")] static void MenuMatchmaking()=> BuildScene("MatchmakingScene",SetupMatchmaking);
    [MenuItem("Racing/씬 개별/Garage")]      static void MenuGarage()     => BuildScene("GarageScene",     SetupGarage);
    [MenuItem("Racing/씬 개별/Race")]        static void MenuRace()       => BuildScene("RaceScene",       SetupRace);
    [MenuItem("Racing/씬 개별/Result")]      static void MenuResult()     => BuildScene("ResultScene",     SetupResult);
    [MenuItem("Racing/Car 프리팹 생성")]     public static void MenuCarPrefab() { EnsureFolders(); CarPrefabSetup(); AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }

    // ── Scene setup functions ─────────────────────────────────────

    static void SetupLogin(Scene _)
    {
        MakeCamera(BgLogin);
        MakeEventSystem();
        new GameObject("AppBootstrap").AddComponent<AppBootstrap>();

        var (canvasGO, _) = MakeCanvas("LoginCanvas");
        var bg = MakePanel(canvasGO.transform, "Bg", BgLogin);
        FullRect(bg);

        var title = MakeTMP(bg.transform, "Title", "너랑나랑\n레이싱 고맵", 52f, C(0.35f, 0.10f, 0.65f));
        Anch(title, 0.10f, 0.62f, 0.90f, 0.94f); title.fontStyle = FontStyles.Bold;

        var sub = MakeTMP(bg.transform, "Sub", "みんなでレーシング", 18f, C(0.60f, 0.40f, 0.80f));
        Anch(sub, 0.15f, 0.55f, 0.85f, 0.63f);

        var guestBtn  = MakeBtn(bg.transform, "GuestButton",  "비회원으로 시작", BtnGreen,  28f); Anch(guestBtn,  0.15f, 0.38f, 0.85f, 0.53f);
        var memberBtn = MakeBtn(bg.transform, "MemberButton", "회원 로그인",     BtnGray,   24f); Anch(memberBtn, 0.15f, 0.22f, 0.85f, 0.36f);
        memberBtn.GetComponent<Button>().interactable = false;

        var statusTMP = MakeTMP(bg.transform, "StatusText", "", 18f, C(0.50f, 0.30f, 0.70f));
        Anch(statusTMP, 0.05f, 0.11f, 0.95f, 0.20f);

        var spinnerGO = new GameObject("LoadingSpinner");
        spinnerGO.transform.SetParent(bg.transform, false);
        spinnerGO.AddComponent<Image>().color = C(0.70f, 0.50f, 0.90f, 0.90f);
        var sRT = spinnerGO.GetComponent<RectTransform>();
        sRT.anchorMin = sRT.anchorMax = new Vector2(0.50f, 0.06f);
        sRT.sizeDelta = new Vector2(32f, 32f);
        spinnerGO.SetActive(false);

        var ui = canvasGO.AddComponent<LoginUI>();
        Set(ui, "_guestButton",    guestBtn.GetComponent<Button>());
        Set(ui, "_memberButton",   memberBtn.GetComponent<Button>());
        Set(ui, "_statusText",     statusTMP);
        Set(ui, "_loadingSpinner", spinnerGO);
    }

    static void SetupMainMenu(Scene _)
    {
        MakeCamera(BgMainMenu);
        MakeEventSystem();

        var (canvasGO, _) = MakeCanvas("MainMenuCanvas");
        var bg = MakePanel(canvasGO.transform, "Bg", BgMainMenu);
        FullRect(bg);

        var title = MakeTMP(bg.transform, "Title", "너랑나랑 레이싱 고맵", 40f, C(0.15f, 0.45f, 0.15f));
        Anch(title, 0.05f, 0.72f, 0.95f, 0.93f); title.fontStyle = FontStyles.Bold;

        var findBtn   = MakeBtn(bg.transform, "FindRoomButton", "🔍  방 찾기", BtnOrange, 30f); Anch(findBtn,   0.15f, 0.50f, 0.85f, 0.66f);
        var garageBtn = MakeBtn(bg.transform, "GarageButton",   "🚗  개러지",  BtnPurple, 30f); Anch(garageBtn, 0.15f, 0.32f, 0.85f, 0.48f);

        var info = MakePanel(bg.transform, "InfoPanel", C(1f, 1f, 1f, 0.45f));
        Anch(info, 0.04f, 0.04f, 0.96f, 0.19f);

        var nameTMP   = MakeTMP(info.transform, "PlayerNameText", "플레이어", 22f, C(0.20f, 0.40f, 0.20f), TextAlignmentOptions.Left);
        Anch(nameTMP,   0.04f, 0.35f, 0.55f, 1.00f);
        var pointsTMP = MakeTMP(info.transform, "PointsText", "0 P", 22f, C(0.80f, 0.50f, 0.10f), TextAlignmentOptions.Right);
        Anch(pointsTMP, 0.55f, 0.35f, 0.96f, 1.00f); pointsTMP.fontStyle = FontStyles.Bold;

        var ui = canvasGO.AddComponent<MainMenuUI>();
        Set(ui, "_findRoomBtn",    findBtn.GetComponent<Button>());
        Set(ui, "_garageBtn",      garageBtn.GetComponent<Button>());
        Set(ui, "_playerNameText", nameTMP);
        Set(ui, "_pointsText",     pointsTMP);
    }

    static void SetupMatchmaking(Scene _)
    {
        MakeCamera(BgMatchmaking);
        MakeEventSystem();

        var (canvasGO, _) = MakeCanvas("MatchmakingCanvas");
        var bg = MakePanel(canvasGO.transform, "Bg", BgMatchmaking);
        FullRect(bg);

        var title = MakeTMP(bg.transform, "Title", "매치메이킹", 38f, C(0.20f, 0.30f, 0.70f));
        Anch(title, 0.10f, 0.78f, 0.90f, 0.95f); title.fontStyle = FontStyles.Bold;

        var statusTMP = MakeTMP(bg.transform, "StatusText", "플레이어를 찾는 중", 26f, C(0.30f, 0.40f, 0.60f));
        Anch(statusTMP, 0.05f, 0.58f, 0.95f, 0.77f);

        var countTMP = MakeTMP(bg.transform, "PlayerCountText", "0 / 5", 32f, C(0.50f, 0.30f, 0.80f));
        Anch(countTMP, 0.25f, 0.48f, 0.75f, 0.60f); countTMP.fontStyle = FontStyles.Bold;

        var dotGO = new GameObject("DotContainer");
        var dotRT = dotGO.AddComponent<RectTransform>();
        dotGO.transform.SetParent(bg.transform, false);
        Anch(dotRT, 0.30f, 0.41f, 0.70f, 0.49f);

        var cdPanel = MakePanel(bg.transform, "CountdownPanel", C(0.20f, 0.25f, 0.52f, 0.88f));
        Anch(cdPanel, 0.15f, 0.37f, 0.85f, 0.59f); cdPanel.SetActive(false);
        var cdTMP = MakeTMP(cdPanel.transform, "CountdownText", "10초 후 시작!", 28f, Color.white);
        FullRect(cdTMP);

        var cancelBtn = MakeBtn(bg.transform, "CancelButton", "취소", BtnRed, 24f);
        Anch(cancelBtn, 0.25f, 0.09f, 0.75f, 0.22f);

        var ui = canvasGO.AddComponent<MatchmakingUI>();
        Set(ui, "_statusText",      statusTMP);
        Set(ui, "_playerCountText", countTMP);
        Set(ui, "_countdownPanel",  cdPanel);
        Set(ui, "_countdownText",   cdTMP);
        Set(ui, "_cancelButton",    cancelBtn.GetComponent<Button>());
        Set(ui, "_dotContainer",    dotGO.transform);
    }

    static void SetupGarage(Scene _)
    {
        MakeCamera(BgGarage);
        MakeEventSystem();

        var (canvasGO, _) = MakeCanvas("GarageCanvas");
        var bg = MakePanel(canvasGO.transform, "Bg", BgGarage);
        FullRect(bg);

        // Header row
        var hdr = MakeTMP(bg.transform, "Header", "개러지", 40f, C(0.55f, 0.35f, 0.05f), TextAlignmentOptions.Left);
        Anch(hdr, 0.05f, 0.88f, 0.60f, 0.98f); hdr.fontStyle = FontStyles.Bold;
        var ptsTMP = MakeTMP(bg.transform, "PointsText", "0 P", 26f, C(0.80f, 0.50f, 0.00f), TextAlignmentOptions.Right);
        Anch(ptsTMP, 0.60f, 0.88f, 0.98f, 0.98f); ptsTMP.fontStyle = FontStyles.Bold;

        // Left: scroll list
        var listOuter = MakePanel(bg.transform, "CarListOuter", C(1f, 1f, 1f, 0.35f));
        Anch(listOuter, 0.02f, 0.08f, 0.38f, 0.87f);
        var (_, contentRT) = MakeScrollList(listOuter.transform, "CarList");

        // Right: preview panel
        var preview = MakePanel(bg.transform, "PreviewPanel", C(1f, 1f, 1f, 0.55f));
        Anch(preview, 0.40f, 0.08f, 0.98f, 0.87f);

        var prevImgGO = new GameObject("PreviewImage");
        prevImgGO.transform.SetParent(preview.transform, false);
        var prevImg = prevImgGO.AddComponent<Image>(); prevImg.color = C(0.85f, 0.85f, 0.85f);
        Anch(prevImgGO.GetComponent<RectTransform>(), 0.10f, 0.57f, 0.90f, 0.97f);

        var carNameTMP  = MakeTMP(preview.transform, "CarNameText",  "차량을 선택하세요", 26f, C(0.30f, 0.30f, 0.30f));
        Anch(carNameTMP, 0.05f, 0.49f, 0.95f, 0.58f); carNameTMP.fontStyle = FontStyles.Bold;

        var bumpTMP  = MakeTMP(preview.transform, "BumpForceText", "", 18f, C(0.50f, 0.30f, 0.10f));
        Anch(bumpTMP, 0.05f, 0.39f, 0.95f, 0.50f);

        var priceTMP = MakeTMP(preview.transform, "PriceText", "", 22f, C(0.70f, 0.50f, 0.00f));
        Anch(priceTMP, 0.05f, 0.28f, 0.95f, 0.39f); priceTMP.fontStyle = FontStyles.Bold;

        var buyBtn  = MakeBtn(preview.transform, "BuyButton",   "구매", BtnOrange, 24f); Anch(buyBtn,  0.05f, 0.16f, 0.50f, 0.28f); buyBtn.SetActive(false);
        var eqpBtn  = MakeBtn(preview.transform, "EquipButton", "장착", BtnGreen,  24f); Anch(eqpBtn,  0.52f, 0.16f, 0.95f, 0.28f); eqpBtn.SetActive(false);
        var fbkTMP  = MakeTMP(preview.transform, "BuyFeedback", "", 18f, C(0.20f, 0.60f, 0.20f));
        Anch(fbkTMP, 0.05f, 0.07f, 0.95f, 0.17f);

        var backBtn = MakeBtn(bg.transform, "BackButton", "← 돌아가기", BtnGray, 22f);
        Anch(backBtn, 0.02f, 0.01f, 0.36f, 0.08f);

        var ui = canvasGO.AddComponent<GarageUI>();
        Set(ui, "_listContent",   contentRT);
        Set(ui, "_previewImg",    prevImg);
        Set(ui, "_carNameText",   carNameTMP);
        Set(ui, "_bumpForceText", bumpTMP);
        Set(ui, "_priceText",     priceTMP);
        Set(ui, "_buyButton",     buyBtn.GetComponent<Button>());
        Set(ui, "_equipButton",   eqpBtn.GetComponent<Button>());
        Set(ui, "_buyFeedback",   fbkTMP);
        Set(ui, "_pointsText",    ptsTMP);
        Set(ui, "_backButton",    backBtn.GetComponent<Button>());
    }

    static void SetupRace(Scene _)
    {
        // ── Perspective camera (quarter-view) ───────────────────
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        camGO.AddComponent<AudioListener>();
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic   = false;
        cam.fieldOfView    = 60f;
        cam.nearClipPlane  = 0.3f;
        cam.farClipPlane   = 200f;
        cam.backgroundColor = C(0.56f, 0.82f, 0.48f);
        cam.clearFlags     = CameraClearFlags.SolidColor;
        var camCtrl = camGO.AddComponent<CameraController>();

        MakeEventSystem();

        // ── Logic objects ───────────────────────────────────────
        var trackBuilder = new GameObject("ProceduralTrackBuilder").AddComponent<ProceduralTrackBuilder>();
        var raceManager  = new GameObject("RaceManager").AddComponent<RaceManager>();
        new GameObject("CountdownController").AddComponent<CountdownController>();

        // ── HUD Canvas ──────────────────────────────────────────
        var (hudCanvasGO, _) = MakeCanvas("HUDCanvas");

        var hudPanel = new GameObject("HUDPanel");
        hudPanel.transform.SetParent(hudCanvasGO.transform, false);
        FullRect(hudPanel.AddComponent<RectTransform>());

        var lapTMP  = MakeTMP(hudPanel.transform, "LapText",     "LAP 1 / 2", 28f, Color.white, TextAlignmentOptions.Left);
        Anch(lapTMP,  0.02f, 0.88f, 0.42f, 0.99f); Outline(lapTMP);

        var posTMP  = MakeTMP(hudPanel.transform, "PositionText", "P1", 38f, Color.white, TextAlignmentOptions.Right);
        Anch(posTMP,  0.58f, 0.88f, 0.98f, 0.99f); posTMP.fontStyle = FontStyles.Bold; Outline(posTMP);

        var timeTMP = MakeTMP(hudPanel.transform, "LapTimeText",  "00:00.00", 22f, Color.white);
        Anch(timeTMP, 0.35f, 0.90f, 0.65f, 0.99f); Outline(timeTMP);

        var cdPanel = MakePanel(hudPanel.transform, "CountdownPanel", C(0f, 0f, 0f, 0.38f));
        Anch(cdPanel, 0.28f, 0.35f, 0.72f, 0.65f); cdPanel.SetActive(false);
        var cdTMP   = MakeTMP(cdPanel.transform, "CountdownText", "3", 100f, Color.white);
        FullRect(cdTMP); cdTMP.fontStyle = FontStyles.Bold;

        var raceHUD = hudPanel.AddComponent<RaceHUD>();
        Set(raceHUD, "_countdownPanel", cdPanel);
        Set(raceHUD, "_countdownText",  cdTMP);
        Set(raceHUD, "_lapText",        lapTMP);
        Set(raceHUD, "_positionText",   posTMP);
        Set(raceHUD, "_lapTimeText",    timeTMP);

        // ── Result overlay (inside HUD canvas, hidden) ──────────
        var resultPanel = MakePanel(hudCanvasGO.transform, "ResultPanel", C(0.08f, 0.08f, 0.12f, 0.88f));
        FullRect(resultPanel); resultPanel.SetActive(false);

        var rTitle = MakeTMP(resultPanel.transform, "ResultTitle", "🏁 레이스 결과", 44f, Color.white);
        Anch(rTitle, 0.10f, 0.80f, 0.90f, 0.95f); rTitle.fontStyle = FontStyles.Bold;

        var rContentGO = new GameObject("ResultListContent");
        var rContentRT = rContentGO.AddComponent<RectTransform>();
        rContentGO.transform.SetParent(resultPanel.transform, false);
        Anch(rContentRT, 0.12f, 0.38f, 0.88f, 0.80f);
        var rVLG = rContentGO.AddComponent<VerticalLayoutGroup>();
        rVLG.spacing = 8f; rVLG.childForceExpandWidth = true;
        rVLG.childForceExpandHeight = false; rVLG.childControlHeight = false;

        var earnedTMP = MakeTMP(resultPanel.transform, "PointsEarnedText", "+100 P 획득!", 32f, C(1f, 0.8f, 0.2f));
        Anch(earnedTMP, 0.10f, 0.27f, 0.90f, 0.39f); earnedTMP.fontStyle = FontStyles.Bold;

        var playBtn = MakeBtn(resultPanel.transform, "PlayAgainButton", "다시 플레이", BtnGreen, 26f); Anch(playBtn, 0.08f, 0.10f, 0.48f, 0.26f);
        var menuBtn = MakeBtn(resultPanel.transform, "MainMenuButton",  "메인 메뉴",   BtnBlue,  26f); Anch(menuBtn, 0.52f, 0.10f, 0.92f, 0.26f);

        var resultUI = resultPanel.AddComponent<ResultUI>();
        Set(resultUI, "_listContent",       rContentRT);
        Set(resultUI, "_pointsEarnedText",  earnedTMP);
        Set(resultUI, "_playAgainButton",   playBtn.GetComponent<Button>());
        Set(resultUI, "_mainMenuButton",    menuBtn.GetComponent<Button>());

        // ── Joystick canvas (top layer) ─────────────────────────
        var joyCanvasGO = new GameObject("JoystickCanvas");
        var joyCanvas   = joyCanvasGO.AddComponent<Canvas>();
        joyCanvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        joyCanvas.sortingOrder = 10;
        var joyScaler = joyCanvasGO.AddComponent<CanvasScaler>();
        joyScaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        joyScaler.referenceResolution = new Vector2(1080f, 1920f);
        joyScaler.matchWidthOrHeight  = 0.5f;
        joyCanvasGO.AddComponent<GraphicRaycaster>();

        // Full-screen transparent panel catches all pointer events
        var joyPanel = new GameObject("JoystickTouchPanel");
        joyPanel.transform.SetParent(joyCanvasGO.transform, false);
        var joyPanelImg = joyPanel.AddComponent<Image>();
        joyPanelImg.color = Color.clear;
        joyPanelImg.raycastTarget = true;
        FullRect(joyPanel.GetComponent<RectTransform>());

        // Joystick background circle (floats to touch position)
        var joyRoot = new GameObject("JoystickRoot");
        joyRoot.transform.SetParent(joyCanvasGO.transform, false);
        joyRoot.AddComponent<Image>().color = C(1f, 1f, 1f, 0.22f);
        var joyRootRT = joyRoot.GetComponent<RectTransform>();
        joyRootRT.anchorMin = joyRootRT.anchorMax = new Vector2(0.5f, 0.5f);
        joyRootRT.sizeDelta = new Vector2(140f, 140f);
        joyRoot.SetActive(false);

        // Inner handle
        var handleGO = new GameObject("Handle");
        handleGO.transform.SetParent(joyRoot.transform, false);
        handleGO.AddComponent<Image>().color = C(1f, 1f, 1f, 0.55f);
        var handleRT = handleGO.GetComponent<RectTransform>();
        handleRT.anchorMin = handleRT.anchorMax = new Vector2(0.5f, 0.5f);
        handleRT.sizeDelta = new Vector2(72f, 72f);

        var joystick = joyPanel.AddComponent<DynamicJoystick>();
        Set(joystick, "_joystickRoot", joyRootRT);
        Set(joystick, "_handle",       handleRT);

        // ── RaceSceneController ─────────────────────────────────
        var rsc = new GameObject("RaceSceneController").AddComponent<RaceSceneController>();
        var carPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{kPrefabs}/Car.prefab");

        Set(rsc, "_raceManager",  raceManager);
        Set(rsc, "_trackBuilder", trackBuilder);
        Set(rsc, "_hud",          raceHUD);
        Set(rsc, "_resultUI",     resultUI);
        Set(rsc, "_camera",       camCtrl);
        if (carPrefab != null)
            Set(rsc, "_carPrefab", carPrefab);
        else
            Debug.LogWarning("[GameSceneBuilder] Car.prefab not found — assign _carPrefab on RaceSceneController manually.");
    }

    static void SetupResult(Scene _)
    {
        MakeCamera(BgResult);
        MakeEventSystem();

        var (canvasGO, _) = MakeCanvas("ResultCanvas");
        var bg = MakePanel(canvasGO.transform, "Bg", BgResult);
        FullRect(bg);

        var title = MakeTMP(bg.transform, "Title", "레이스 결과", 44f, C(0.50f, 0.30f, 0.10f));
        Anch(title, 0.10f, 0.82f, 0.90f, 0.96f); title.fontStyle = FontStyles.Bold;

        var contentGO = new GameObject("ResultListContent");
        var contentRT = contentGO.AddComponent<RectTransform>();
        contentGO.transform.SetParent(bg.transform, false);
        Anch(contentRT, 0.08f, 0.38f, 0.92f, 0.82f);
        var vlg = contentGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8f; vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false; vlg.childControlHeight = false;
        contentGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var earnedTMP = MakeTMP(bg.transform, "PointsEarnedText", "+100 P 획득!", 34f, C(0.80f, 0.50f, 0.00f));
        Anch(earnedTMP, 0.10f, 0.27f, 0.90f, 0.38f); earnedTMP.fontStyle = FontStyles.Bold;

        var playBtn = MakeBtn(bg.transform, "PlayAgainButton", "다시 플레이", BtnGreen, 26f); Anch(playBtn, 0.08f, 0.09f, 0.48f, 0.24f);
        var menuBtn = MakeBtn(bg.transform, "MainMenuButton",  "메인 메뉴",   BtnBlue,  26f); Anch(menuBtn, 0.52f, 0.09f, 0.92f, 0.24f);

        var resultUI = canvasGO.AddComponent<ResultUI>();
        Set(resultUI, "_listContent",      contentRT);
        Set(resultUI, "_pointsEarnedText", earnedTMP);
        Set(resultUI, "_playAgainButton",  playBtn.GetComponent<Button>());
        Set(resultUI, "_mainMenuButton",   menuBtn.GetComponent<Button>());
    }

    // ── Car prefab ────────────────────────────────────────────────

    static void CarPrefabSetup()
    {
        EnsureFolder(kPrefabs);

        var carGO = new GameObject("Car");

        var sr = carGO.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 5;

        var rb = carGO.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f; rb.drag = 2f; rb.angularDrag = 5f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = carGO.AddComponent<CircleCollider2D>();
        col.radius = 0.38f;

        var cc = carGO.AddComponent<CarController>();
        var ccSO = new SerializedObject(cc);
        ccSO.FindProperty("_spriteRenderer").objectReferenceValue = sr;
        ccSO.ApplyModifiedPropertiesWithoutUndo();

        carGO.AddComponent<BumpHandler>();
        carGO.AddComponent<HitShake>();
        carGO.AddComponent<LapTracker>();

        // Trail child object
        var trailGO = new GameObject("Trail");
        trailGO.transform.SetParent(carGO.transform, false);
        var trail = trailGO.AddComponent<TrailRenderer>();
        trail.time              = 0.35f;
        trail.startWidth        = 0.15f;
        trail.endWidth          = 0f;
        trail.minVertexDistance = 0.05f;
        trail.material          = new Material(Shader.Find("Sprites/Default"));
        trail.startColor        = C(1f, 1f, 1f, 0.5f);
        trail.endColor          = C(1f, 1f, 1f, 0f);
        trail.sortingOrder      = 4;
        trailGO.AddComponent<SpeedTrail>();

        string path = $"{kPrefabs}/Car.prefab";
        PrefabUtility.SaveAsPrefabAsset(carGO, path);
        Object.DestroyImmediate(carGO);
        Debug.Log($"[GameSceneBuilder] Car prefab saved → {path}");
    }

    // ── Scene builder wrapper ─────────────────────────────────────

    static void BuildScene(string name, System.Action<Scene> setup)
    {
        EnsureFolder(kScenes);
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        setup(scene);
        string path = $"{kScenes}/{name}.unity";
        EditorSceneManager.SaveScene(scene, path);
        Debug.Log($"[GameSceneBuilder] ✅ {name}");
    }

    // ── UI factory helpers ────────────────────────────────────────

    static Camera MakeCamera(Color bg, bool ortho = true, float size = 10f)
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        go.AddComponent<AudioListener>();
        var cam = go.AddComponent<Camera>();
        cam.orthographic     = ortho;
        cam.orthographicSize = size;
        cam.backgroundColor  = bg;
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.nearClipPlane    = 0.3f;
        cam.farClipPlane     = 1000f;
        return cam;
    }

    static EventSystem MakeEventSystem()
    {
        var go = new GameObject("EventSystem");
        go.AddComponent<StandaloneInputModule>();
        return go.AddComponent<EventSystem>();
    }

    static (GameObject go, Canvas canvas) MakeCanvas(string name)
    {
        var go     = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight  = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return (go, canvas);
    }

    static GameObject MakePanel(Transform parent, string name, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<Image>().color = col;
        return go;
    }

    static TextMeshProUGUI MakeTMP(Transform parent, string name, string text,
        float size, Color col,
        TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text             = text;
        tmp.fontSize         = size;
        tmp.color            = col;
        tmp.alignment        = align;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    static GameObject MakeBtn(Transform parent, string name, string label, Color bg, float size = 26f)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<Image>().color = bg;
        var btn  = go.AddComponent<Button>();
        var cols = btn.colors;
        cols.highlightedColor = Color.Lerp(bg, Color.white, 0.22f);
        cols.pressedColor     = Color.Lerp(bg, Color.black, 0.12f);
        btn.colors = cols;

        var lGO = new GameObject("Label");
        lGO.transform.SetParent(go.transform, false);
        var tmp = lGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = size;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        FullRect(lGO.GetComponent<RectTransform>());
        return go;
    }

    static (ScrollRect scroll, RectTransform content) MakeScrollList(Transform parent, string name)
    {
        var sGO = new GameObject(name);
        sGO.transform.SetParent(parent, false);
        sGO.AddComponent<Image>().color = Color.clear;
        var sRT  = sGO.GetComponent<RectTransform>(); FullRect(sRT);
        var sr   = sGO.AddComponent<ScrollRect>();

        var vpGO = new GameObject("Viewport");
        var vpRT = vpGO.AddComponent<RectTransform>();
        vpGO.transform.SetParent(sGO.transform, false); FullRect(vpRT);
        vpGO.AddComponent<Image>().color = Color.clear;
        vpGO.AddComponent<Mask>().showMaskGraphic = false;

        var cGO  = new GameObject("Content");
        var cRT  = cGO.AddComponent<RectTransform>();
        cGO.transform.SetParent(vpGO.transform, false);
        cRT.anchorMin = new Vector2(0f, 1f); cRT.anchorMax = new Vector2(1f, 1f);
        cRT.pivot     = new Vector2(0.5f, 1f);
        cRT.offsetMin = cRT.offsetMax = Vector2.zero;
        cRT.sizeDelta = new Vector2(0f, 300f);
        var vlg = cGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing   = 6f; vlg.padding = new RectOffset(6, 6, 6, 6);
        vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false; vlg.childControlHeight = false;
        cGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content   = cRT; sr.viewport  = vpRT;
        sr.horizontal = false; sr.vertical = true;
        return (sr, cRT);
    }

    // ── RectTransform shortcuts ───────────────────────────────────

    static void FullRect(RectTransform rt)
    { rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero; }

    static void FullRect(Component c) => FullRect(c.GetComponent<RectTransform>());
    static void FullRect(GameObject g) => FullRect(g.GetComponent<RectTransform>());

    static void Anch(RectTransform rt, float x0, float y0, float x1, float y1)
    { rt.anchorMin = new Vector2(x0, y0); rt.anchorMax = new Vector2(x1, y1); rt.offsetMin = rt.offsetMax = Vector2.zero; }

    static void Anch(Component c, float x0, float y0, float x1, float y1) => Anch(c.GetComponent<RectTransform>(), x0, y0, x1, y1);
    static void Anch(GameObject g, float x0, float y0, float x1, float y1) => Anch(g.GetComponent<RectTransform>(), x0, y0, x1, y1);

    static void Outline(TextMeshProUGUI t) { t.outlineWidth = 0.18f; t.outlineColor = new Color32(0, 0, 0, 200); }

    // ── SerializedObject field writer ─────────────────────────────

    static void Set(Object target, string field, Object value)
    {
        var so   = new SerializedObject(target);
        var prop = so.FindProperty(field);
        if (prop == null)
        {
            Debug.LogWarning($"[GameSceneBuilder] Field '{field}' not found on {target.GetType().Name}");
            return;
        }
        prop.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ── Build Settings ────────────────────────────────────────────

    static void UpdateBuildSettings()
    {
        var list = new List<EditorBuildSettingsScene>();
        foreach (var n in new[] { "LoginScene", "MainMenuScene", "MatchmakingScene", "GarageScene", "RaceScene", "ResultScene" })
        {
            string p = $"{kScenes}/{n}.unity";
            if (File.Exists(p))
                list.Add(new EditorBuildSettingsScene(p, true));
            else
                Debug.LogWarning($"[GameSceneBuilder] Scene not found: {p}");
        }
        EditorBuildSettings.scenes = list.ToArray();
        Debug.Log($"[GameSceneBuilder] Build Settings updated: {list.Count} scenes");
    }

    static void EnsureFolders() { EnsureFolder(kScenes); EnsureFolder(kPrefabs); }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path)?.Replace('\\', '/') ?? "Assets";
        string folder = Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, folder);
    }
}
