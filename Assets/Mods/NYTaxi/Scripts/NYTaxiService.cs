using System.Collections;
using BigAmbitions.DayNightCycle;
using BigAmbitions.SaveSystem.Legacy;
using GleyTrafficSystem;
using Helpers;
using Parking.UndergroundParking;
using Timemachine;
using UI;
using UnityEngine;
using Vehicles.Taxis;

namespace NYTaxi
{
    /// <summary>
    /// Диспетчерская служба: промотка ожидания машины и запуск родного
    /// режима такси на карте. Живёт на скрытом GameObject, потому что
    /// CityMap.SetTaxiMode принимает только активный MonoBehaviour-ITaxi.
    /// </summary>
    public class NYTaxiService : MonoBehaviour, ITaxi
    {
        public static NYTaxiService Instance { get; private set; }

        /// <summary>
        /// Сервис живёт на объекте, созданном при загрузке города. Если тот по
        /// какой-то причине пропал, поднимаем его заново, чтобы звонок не молчал.
        /// </summary>
        public static NYTaxiService EnsureInstance()
        {
            if (Instance != null)
                return Instance;
            Debug.LogWarning("[NYTaxi] Service instance was missing, recreating");
            return new GameObject("NYTaxiService").AddComponent<NYTaxiService>();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public static bool IsPlayerOutside()
        {
            // TaxiSystem может быть ещё не поднят — обращение к нему уронило бы звонок
            bool traveling = InstanceBehavior<TaxiSystem>.IsInitialized
                             && TaxiSystem.IsTraveling;
            return !BuildingManager.IsInsideBuilding
                   && !UndergroundParkingManager.IsInsideParking
                   && !VehicleHelper.IsInsideVehicle()
                   && !traveling
                   && !InstanceBehavior<UIs>.Instance.timeMachine.isRunning;
        }

        public void BeginPickup(int waitMinutes)
        {
            StartCoroutine(WaitForTaxiThenOpenMap(waitMinutes));
        }

        private IEnumerator WaitForTaxiThenOpenMap(int waitMinutes)
        {
            // Кадр на закрытие телефона после звонка
            yield return null;
            TimeMachine timeMachine = InstanceBehavior<UIs>.Instance.timeMachine;
            Timestamp goal = TimeHelper.Now().AddMinutes(waitMinutes);
            timeMachine.StartTimeMachine(goal, disableCancel: true, "nytaxi:waiting",
                showBlur: true, useConstantSpeed: true);
            // isRunning сбрасывается в начале остановки, а isBlockingUi — только
            // через секунду, после чего CityMap снова активна и Toggle() работает
            yield return new WaitWhile(
                () => timeMachine.isRunning || timeMachine.isBlockingUi);
            yield return null;
            // Игрок не мог сдвинуться во время промотки, но перестрахуемся
            if (!IsPlayerOutside())
            {
                Debug.LogWarning("[NYTaxi] Player is no longer outside, ride cancelled");
                yield break;
            }
            CityMap cityMap = InstanceBehavior<CityManager>.Instance.cityMap;
            // SetTaxiMode переключает карту, поэтому уже открытую сначала закрываем
            if (CityMap.IsOpen)
                cityMap.Close();
            cityMap.SetTaxiMode(this);
        }

        private static VehicleComponent FindTaxiPrefab()
        {
            TaxiController streetTaxi = Object.FindObjectOfType<TaxiController>();
            if (streetTaxi != null)
                return streetTaxi.GetVehiclePrefab();
            PermanentTaxiController permanentTaxi =
                Object.FindObjectOfType<PermanentTaxiController>();
            if (permanentTaxi != null)
                return permanentTaxi.GetVehiclePrefab();
            Debug.LogWarning("[NYTaxi] No taxi prefab source found in scene");
            return null;
        }

        public void DriveAway()
        {
        }

        public VehicleComponent GetVehiclePrefab()
        {
            return FindTaxiPrefab();
        }

        public VehicleComponent InstantiateVehicle(Waypoint waypoint)
        {
            VehicleComponent prefab = FindTaxiPrefab();
            if (prefab == null)
                return null;
            return TrafficManager.Instance.LoadVehicle(prefab.gameObject, waypoint);
        }

        public float GetTimeMultiplier()
        {
            return 1f;
        }

        public string GetHappinessModifierName()
        {
            return "ba:happinessmodifier_taxi";
        }

        public void OnTravelFinished()
        {
            // Ванильный код считает поездки только у TaxiController /
            // PermanentTaxiController — учитываем свои сами
            SaveGameManager.Current.achievementsData.taxiRides++;
        }
    }
}
