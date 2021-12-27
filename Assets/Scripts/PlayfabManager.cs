using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PlayfabManager : MonoBehaviour {
    public InputField InputEmail;
    public InputField InputPassword;
    public InputField InputNickName;

    public InputField InputData_1;
    public InputField InputData_2;
    public InputField InputData_3;

    public Text TextLog;

    public GameObject PageLogin;
    public GameObject PageMain;

    public string MyId;

    private void Start() {
        this.PageLogin.SetActive(true);
        this.PageMain.SetActive(false);
    }

    public void ButtonLogin() {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) {
            PlayFabSettings.staticSettings.TitleId = "42";
        }

        var request = new LoginWithEmailAddressRequest { Email = this.InputEmail.text, Password = this.InputPassword.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => {
            string log = "로그인 성공";
            Debug.Log(log);
            this.TextLog.text = log;
            this.MyId = result.PlayFabId;
            this.PageLogin.SetActive(false);
            this.PageMain.SetActive(true);
        }, (error) => {
            string log = "로그인 실패";
            Debug.Log(log);
            this.TextLog.text = log;
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void ButtonRegister() {
        var request = new RegisterPlayFabUserRequest {
            Email = this.InputEmail.text,
            Password = this.InputPassword.text,
            Username = this.InputNickName.text,
            DisplayName = this.InputNickName.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => {
            string log = "회원가입 성공";
            Debug.Log(log);
            this.TextLog.text = log;
        }, (error) => {
            string log = "회원가입 실패";
            Debug.Log(log);
            this.TextLog.text = log;
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void SetStat() {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate{StatisticName = "Input_1", Value = int.Parse(this.InputData_1.text)},
                new StatisticUpdate{StatisticName = "Input_2", Value = int.Parse(this.InputData_2.text)},
                new StatisticUpdate{StatisticName = "Input_3", Value = int.Parse(this.InputData_3.text)}
            }
        }, (result) => {
            this.TextLog.text = "데이터 저장 성공";
        }, (error) => {
            this.TextLog.text = "데이터 저장 실패";
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void GetStat() {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), (result) => {
            string log = "";
            foreach (var stat in result.Statistics) {
                log += string.Format("{0} : {1} \n", stat.StatisticName, stat.Value);
            }
            Debug.Log(log);
            this.TextLog.text = log;
        }, (error) => {
            this.TextLog.text = "데이터 로드 실패";
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void SetData() {
        var request = new UpdateUserDataRequest() {
            Data = new Dictionary<string, string>() {
                { "A", "AA" },
                { "B", "BB" }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => {
            string log = "데이터 저장 성공";
            this.TextLog.text = log;
            print(log);
        }, (error) => {
            string log = "데이터 저장 실패";
            this.TextLog.text = log;
            print(log);
        });
    }

    public void GetData() {
        var request = new GetUserDataRequest() { PlayFabId = MyId };
        PlayFabClientAPI.GetUserData(request, (result) => {
            this.TextLog.text = "";
            foreach (var eachData in result.Data) {
                this.TextLog.text += eachData.Key + " : " + eachData.Value.Value + "\n";
            }
        }, (error) => {
            string log = "데이터 불러오기 실패";
            this.TextLog.text = log;
            print(log);
        });
    }

    public void GetLeaderboard() {
        var request = new GetLeaderboardRequest {
            StartPosition = 0,
            StatisticName = "Input_1",
            MaxResultsCount = 20,
            ProfileConstraints = new PlayerProfileViewConstraints() { ShowLocations = true, ShowDisplayName = true }
        };
        PlayFabClientAPI.GetLeaderboard(request, (result) => {
            this.TextLog.text = "";
            for (int i = 0; i < result.Leaderboard.Count; i++) {
                var curBoard = result.Leaderboard[i];
                this.TextLog.text += curBoard.Profile.Locations[0].CountryCode.Value + " / " + curBoard.DisplayName + " / " + curBoard.StatValue + "\n";
            }
        },
        (error) => {
            string log = "데이터 불러오기 실패";
            this.TextLog.text = log;
            print(log);
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void AddGold() {
        var request = new AddUserVirtualCurrencyRequest() { VirtualCurrency = "GD", Amount = 50 };
        PlayFabClientAPI.AddUserVirtualCurrency(request, (result) => {
            string log = string.Format("돈 얻기 성공! 현재 돈 : " + result.Balance);
            print(log);
            this.TextLog.text = log;
        }, (error) => {
            string log = "돈 얻기 실패";
            print(log);
            this.TextLog.text = log;
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void SubtractGold() {
        var request = new SubtractUserVirtualCurrencyRequest() { VirtualCurrency = "GD", Amount = 50 };
        PlayFabClientAPI.SubtractUserVirtualCurrency(request, (result) => {
            string log = string.Format("돈 빼기 성공! 현재 돈 : " + result.Balance);
            print(log);
            this.TextLog.text = log;
        }, (error) => {
            string log = "돈 빼기 실패";
            print(log);
            this.TextLog.text = log;
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void PurchaseItem() {
        var request = new PurchaseItemRequest() { CatalogVersion = "Main", ItemId = "potion", VirtualCurrency = "GD", Price = 100 };
        PlayFabClientAPI.PurchaseItem(request, (result) => {
            string log = "아이템 구입 성공";
            print(log);
            this.TextLog.text = log;
        }, (error) => {
            string log = "아이템 구입 실패";
            print(log);
            this.TextLog.text = log;
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void ConsumeItem() {
        var request = new ConsumeItemRequest { ConsumeCount = 1, ItemInstanceId = "69BDFB1D6961B4B1" };
        PlayFabClientAPI.ConsumeItem(request, (result) => {
            string log = "아이템 사용 성공";
            print(log);
            this.TextLog.text = log;
        }, (error) => {
            string log = "아이템 사용 실패";
            print(log);
            this.TextLog.text = log;
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void GetInventory() {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (result) => {
            string log = "현재금액 : " + result.VirtualCurrency["GD"];
            print(log);
            this.TextLog.text = log;
            for (int i = 0; i < result.Inventory.Count; i++) {
                var Inven = result.Inventory[i];
                log = Inven.DisplayName + " / " + Inven.UnitCurrency + " / " + Inven.UnitPrice + " / " + Inven.ItemInstanceId + " / " + Inven.RemainingUses;
                print(log);
                this.TextLog.text = log;
            }
        }, (error) => {
            string log = "인벤토리 불러오기 실패";
            print(log);
            this.TextLog.text = log;
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void GetCatalogItem() {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest() { CatalogVersion = "Main" }, (result) => {
            string log = "";
            for (int i = 0; i < result.Catalog.Count; i++) {
                var Catalog = result.Catalog[i];
                log += Catalog.ItemId + " / " + Catalog.DisplayName + " / " + Catalog.Description + " / " +
                    Catalog.VirtualCurrencyPrices["GD"] + " / " + Catalog.Consumable.UsageCount + "\n";
            }
            print(log);
            this.TextLog.text = log;
        },
        (error) => {
            string log = "상점 불러오기 실패";
            print(log);
            this.TextLog.text = log;
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void StartCloudHelloWorld() {
        var dicc = new Dictionary<string, object>();
        dicc.Add("name", "kiki");
        dicc.Add("power", "koko");
        string json = "{\"name\": \"kiki\" }";
        PlayFabCloudScriptAPI.ExecuteFunction(new PlayFab.CloudScriptModels.ExecuteFunctionRequest {
            FunctionName = "HttpExample",
            FunctionParameter = dicc,
            GeneratePlayStreamEvent = false
        }, (result) => {
            string log = "성공";
            print(log);
            this.TextLog.text = log;
            Debug.Log(result.FunctionResult.ToString());
        }, (error) => {
            string log = "실패";
            print(log);
            this.TextLog.text = log;
            Debug.LogError(error.GenerateErrorReport());
        }); ;
    }

    public void CloudIncrement() {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest {
            FunctionName = "IncrementReadOnlyUserData"
        }, (result) => {
            string log = "CloudScript call successful";
            print(log);
            this.TextLog.text = log;
        }, (error) => {
            string log = "CloudScript call failed";
            print(log);
            this.TextLog.text = log;
            Debug.Log(error.GenerateErrorReport());
        });
    }
}
