using UnityEngine;
using System.Collections.Generic;
using RacingGoMap.Car;
using RacingGoMap.Art;
using RacingGoMap.Circuit;

namespace RacingGoMap.Core
{
    public class RuntimeGameData : Singleton<RuntimeGameData>
    {
        public List<CarData>           Cars     { get; private set; }
        public List<Race.TrackWaypointData> Circuits { get; private set; }

        static readonly Color[] PlayerColors =
        {
            new(1f,    0.30f, 0.30f), // P1 빨강
            new(0.28f, 0.56f, 1f   ), // P2 파랑
            new(1f,    0.85f, 0.20f), // P3 노랑
            new(0.30f, 0.80f, 0.40f), // P4 초록
            new(0.70f, 0.30f, 1f   ), // P5 보라
        };

        protected override void Awake()
        {
            base.Awake();
            BuildCars();
            BuildCircuits();
        }

        // ────────────────────── 자동차 ──────────────────────

        void BuildCars()
        {
            Cars = new List<CarData>();

            Cars.Add(MakeCar(
                "car_speedster", "스피드스터", "가볍고 날렵한 기본 카트",
                new Color(1f, 0.30f, 0.30f),
                bumpForce: 3f, bumpRes: 1f, cost: 0, isDefault: true));

            Cars.Add(MakeCar(
                "car_bumperking", "범퍼킹", "넓고 단단한 강화 카트",
                new Color(0.28f, 0.56f, 1f),
                bumpForce: 7f, bumpRes: 2f, cost: 500, isDefault: false));

            Cars.Add(MakeCar(
                "car_tankbubble", "탱크버블", "둥근둥근 강력한 슈퍼 카트",
                new Color(1f, 0.85f, 0.20f),
                bumpForce: 12f, bumpRes: 3.5f, cost: 1500, isDefault: false));
        }

        static CarData MakeCar(string id, string name, string desc,
                               Color color, float bumpForce, float bumpRes,
                               int cost, bool isDefault)
        {
            var d = ScriptableObject.CreateInstance<CarData>();
            d.carId          = id;
            d.carName        = name;
            d.description    = desc;
            d.primaryColor   = color;
            d.bumpForce      = bumpForce;
            d.bumpResistance = bumpRes;
            d.purchaseCost   = cost;
            d.isDefault      = isDefault;
            d.carSprite      = CarSpriteBuilder.Build(color);
            return d;
        }

        // ────────────────────── 서킷 ──────────────────────

        void BuildCircuits()
        {
            Circuits = new List<Race.TrackWaypointData>
            {
                GrandLoop(),
                SnakePark(),
                TwinArc()
            };
        }

        static Race.TrackWaypointData GrandLoop() => new()
        {
            circuitId   = "circuit_grandloop",
            circuitName = "그랜드 루프",
            trackWidth  = 6f,
            totalLaps   = 2,
            waypoints   = new Vector2[]
            {
                // 하단 직선
                new(-12, -11), new(0, -11), new(12, -11),
                // 우하단 코너
                new(14, -9), new(15, -6),
                // 우측 직선
                new(15, 0), new(15, 6),
                // 우상단 코너
                new(14, 9), new(12, 10),
                // 상단 시케인 (S커브)
                new(7, 10), new(5f, 8.5f), new(1, 8),
                new(-1, 8), new(-5f, 8.5f), new(-7, 10),
                // 좌상단
                new(-12, 10),
                // 좌상단 코너
                new(-14, 9), new(-15, 6),
                // 좌측 직선
                new(-15, 0), new(-15, -6),
                // 좌하단 코너
                new(-14, -9), new(-12, -11),
            }
        };

        static Race.TrackWaypointData SnakePark() => new()
        {
            circuitId   = "circuit_snakepark",
            circuitName = "스네이크 파크",
            trackWidth  = 6.5f,
            totalLaps   = 2,
            waypoints   = new Vector2[]
            {
                // 하단
                new(-10, -12), new(0, -12), new(10, -12),
                // 우하단 코너
                new(14, -9), new(15, -4),
                // S-커브 올라가며 (우쪽)
                new(13, 2), new(9, 5), new(5, 7),
                // 상단
                new(1, 9), new(-3, 10), new(-7, 10),
                // S-커브 (좌쪽)
                new(-11, 8), new(-14, 4),
                // 좌쪽
                new(-15, 0),
                new(-14, -5), new(-11, -9),
                // 좌하단
                new(-7, -11), new(0, -12),
            }
        };

        static Race.TrackWaypointData TwinArc() => new()
        {
            circuitId   = "circuit_twinarc",
            circuitName = "트윈 아크",
            trackWidth  = 7f,
            totalLaps   = 2,
            waypoints   = new Vector2[]
            {
                // 하단 직선
                new(-8, -14), new(0, -14), new(8, -14),
                // 우측 큰 아크
                new(12, -12), new(15, -8), new(16, -3),
                new(16,  3), new(15,  8), new(12, 12),
                // 상단 직선
                new(8,  14), new(0,  14), new(-8, 14),
                // 좌측 큰 아크
                new(-12,  12), new(-15,  8), new(-16,  3),
                new(-16, -3), new(-15, -8), new(-12, -12),
            }
        };

        // ────────────────────── 쿼리 API ──────────────────────

        public CarData GetCarById(string id)
            => Cars.Find(c => c.carId == id);

        public CarData GetDefaultCar()
            => Cars.Find(c => c.isDefault) ?? Cars[0];

        public Race.TrackWaypointData GetRandomCircuit()
            => Circuits[Random.Range(0, Circuits.Count)];

        public Race.TrackWaypointData GetCircuitById(string id)
            => Circuits.Find(c => c.circuitId == id);

        public Color GetPlayerColor(int index)
            => index < PlayerColors.Length ? PlayerColors[index] : Color.white;
    }
}
