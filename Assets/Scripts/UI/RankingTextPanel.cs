using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingTextPanel : MonoBehaviour
{
    public TextMeshProUGUI rankingText;
    private bool isVisible = false;

    public int topCount = 50; // 보여줄 랭킹 수 (에디터에서 조정 가능)

    // 패널 켜기 (Show와 동시에 데이터 새로고침)
    public void OpenRanking()
    {
        Debug.Log("OpenRanking 호출됨"); 
        gameObject.SetActive(true);
        isVisible = true;
        ShowRanking();
    }

    // 패널 끄기
    public void CloseRanking()
    {
        gameObject.SetActive(false);
        isVisible = false;
    }

    // 버튼 토글 (On/Off 자동 전환)
    public void ToggleRanking()
    {
        if (isVisible)
            CloseRanking();
        else
            OpenRanking();
    }

    // 실제 랭킹 불러오기 및 출력
    public void ShowRanking()
    {
        if (!gameObject.activeInHierarchy) return;
        Debug.Log("ShowRanking() 호출됨"); 

        rankingText.text = "불러오는 중...";

        FirebaseManager.Instance.LoadTopRankings(topCount, (list) =>
        {
            Debug.Log("LoadTopRankings 콜백 진입");
            if (list == null || list.Count == 0)
            {
                rankingText.text = "랭킹 데이터 없음";
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("<size=120%><b> 랭킹 TOP " + topCount + "</b></size>\n");

            int rank = 1;
            foreach (var entry in list)
            {
                sb.AppendLine($"<b>{rank}위</b>  {entry.nickname}   <color=#FFD700>스테이지 {entry.maxClearedStage}</color> / 웨이브 {entry.maxClearedWave}");
                rank++;
            }
            rankingText.text = sb.ToString();
        });
    }
}