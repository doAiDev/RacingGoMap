# 너랑나랑 레이싱 고맵 🏎️

Unity 2D 탑뷰 멀티플레이어 레이싱 게임

## 게임 흐름

```
로그인 화면 → 메인 메뉴 → 방 찾기(매칭) → 레이스 → 결과
                       ↘ 개러지
```

## 핵심 기능

- **매칭**: 선착순 자동 배정, 최소 2명 / 최대 5명, 2명 이상 시 10초 후 자동 시작
- **레이스**: 랜덤 서킷 배정, 3초 카운트다운, 전 차량 동일 속도, 화면 중앙 조이스틱 조작
- **박치기**: 차량 충돌 시 물리 반발력 적용
- **2랩 완주** → 1등 우승 → 포인트 획득
- **개러지**: 포인트로 박치기 힘 높은 차량 구매

## 개발 환경

- Unity 2022.3 LTS
- C# / Unity Netcode for GameObjects
- Unity Gaming Services (Relay + Lobby) — 오프라인 시뮬레이션 폴백 내장

## 설치

1. Unity Hub → Unity 2022.3 LTS 설치
2. 이 저장소 클론
3. Unity Hub에서 프로젝트 열기 (`Add project from disk`)
4. Package Manager에서 패키지 설치 확인
5. (멀티플레이 온라인 시) [Unity Dashboard](https://cloud.unity.com)에서 Project ID 연결

## 씬 목록

| 씬 | 설명 |
|---|---|
| `LoginScene` | 비회원/회원 선택 |
| `MainMenuScene` | 방 찾기 / 개러지 버튼 |
| `MatchmakingScene` | 매칭 대기 화면 |
| `GarageScene` | 차량 구매·장착 |
| `RaceScene` | 실제 레이스 |
| `ResultScene` | 결과 및 포인트 획득 |

## 씬 설정

### LoginScene
- `AppBootstrap` 오브젝트 (씬 최초 로드 시 싱글톤들 초기화)
- `LoginUI` 컴포넌트 + 버튼 2개 연결

### RaceScene
- `RaceManager`, `CountdownController` 오브젝트
- `RaceStartGrid` — 출발 위치 Transform 배열 (5개)
- `Checkpoint` 오브젝트들 — 마지막은 `isFinishLine = true`
- `VirtualJoystick` UI 캔버스 (화면 하단 중앙)
- `RaceHUD` 캔버스, `CameraController` 배치

## ScriptableObject 생성 위치

- 우클릭 → Create → RacingGoMap → **Car Data** / **Car Database** / **Circuit Data** / **Circuit Manager**

### 기본 차량 설정 예시
| carId | carName | purchaseCost | bumpForce | isDefault |
|---|---|---|---|---|
| `car_basic` | 기본 카트 | 0 | 3 | ✅ |
| `car_bully` | 불리 카트 | 500 | 6 | ❌ |
| `car_tank` | 탱크 카트 | 1500 | 10 | ❌ |
