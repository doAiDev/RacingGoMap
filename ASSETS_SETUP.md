# 에셋 세팅 가이드 — Kenney Racing Pack

## 다운로드

**Kenney Racing Pack** — 커스텀 이용 가능, 커머셜 가능, Public Domain (CC0)

```
https://kenney.nl/assets/racing-pack
```

다운로드 후 `Assets/Art/` 폴더를 만들고 안에 푸세요.

---

## 폴더 구조

```
Assets/
├── Art/
│   ├── Cars/          ← 자동차 스프라이트 (car_black.png 등)
│   ├── Tracks/        ← 도로 타일 (track_*.png)
│   └── Environment/   ← 나무, 돌, 관중석, 장애물
├── Scripts/
└── ScriptableObjects/
    ├── Cars/
    └── Circuits/
```

---

## 차량 스프라이트 설정

1. `Assets/Art/Cars/` 안의 PNG를 **Sprite (2D)** 로 임포트
2. `Assets/ScriptableObjects/Cars/` 안에 **CarData** 생성
3. `Car Sprite` 필드에 연결

| carId | 스프라이트 | purchaseCost | bumpForce |
|---|---|---|---|
| `car_basic`  | car_red.png   | 0    | 3  |
| `car_bully`  | car_blue.png  | 500  | 6  |
| `car_muscle` | car_yellow.png| 1000 | 8  |
| `car_tank`   | car_black.png | 2000 | 12 |

---

## 서킷 만들기 (Tilemap 방식)

### 1. Tile Palette 설정

- **Window → 2D → Tile Palette** 열기
- 새 Palette 생성 후 `Assets/Art/Tracks/` 스프라이트를 드래그

### 2. 시작 Tilemap 구성

```
Hierarchy:
├── CircuitGrid
│   ├── Background    (Tilemap - 풀 배경)
│   ├── Road          (Tilemap - 도로)
│   └── Walls         (Tilemap + TilemapCollider2D - 벅)
├── Checkpoints
│   ├── CP_01         (BoxCollider2D trigger, Checkpoint.cs, index=0)
│   ├── CP_02         (index=1)
│   └── FinishLine    (BoxCollider2D trigger, isFinishLine=true)
└── StartGrid       (RaceStartGrid.cs, Slots 5개)
```

### 3. 벽 충돌 설정

- `Walls` Tilemap 에 `TilemapCollider2D` + `CompositeCollider2D` 추가
- `CompositeCollider2D` → `Used By Composite` 체크
- `Rigidbody2D` → `Body Type: Static`

### 4. Layer / Tag

| 레이어 | 대상 |
|---|---|
| `Car`   | 모든 차량 |
| `Wall`  | 트랙 벽 |
| `Track` | 도로 타일맵 |

---

## 서킷 종류 (3개 추천)

| ID | 이름 | 특징 |
|---|---|---|
| `circuit_city`  | 시티 서킷 | 직선 위주, 다른 속도로 담배기 쿨순 |
| `circuit_grass` | 초원 서킷 | 구불길 많음, 박치기 유리 |
| `circuit_port`  | 항구 서킷 | 좌우 대칭, 직괁 구간 있음 |

---

## DynamicJoystick UI 배치

```
Canvas (Screen Space Overlay)
└── JoystickArea (Image, 화면 전체 커버, Alpha=0)
    ├── JoystickRoot (Image - 외부 원, 140x140px)
    │   └── Handle      (Image - 내부 핸들, 70x70px)
    └── DynamicJoystick.cs 컴포넌트 연결
        ├── JoystickRoot → 위 JoystickRoot
        └── Handle       → 위 Handle
```

- **터치한 자리**에 조이스틱이 자동 이동
- 손 떼면 자동 숨김
- `Active Fraction = 0.5` → 화면 왼쪽 50%에서만 작동
- 탑탑특공대 스타일과 동일!
