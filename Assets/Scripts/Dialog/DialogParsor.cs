using System;
using System.Collections.Generic;
using UnityEngine;

// CSV 파일 한 번에 파싱해서 DialogLine 리스트와 DialogSelectOption 리스트를 반환
// Resources 폴더의 CSV 파일을 읽음

public class DialogParser : MonoBehaviour
{
    // CSV에서 빈 값 처리 유틸
    // idx가 row.Length보다 작은 경우 row[idx] 반환, 그렇지 않은 경우 "" 반환
    private static string Safe(string[] row, int idx) => (idx < row.Length) ? row[idx].Trim() : "";

    // Dialog.csv를 파싱해 List<DialogLine>로 반환
    // (EventID, DialogID, CharacterName, Text, SelectID, SkipAndMoveTo, Description)
    public static List<DialogLine> ParseDialogCSV(string resourcePath = "Dialog")
    {
        TextAsset csv = Resources.Load<TextAsset>(resourcePath);
        if (csv == null) return new List<DialogLine>(); // 파일 없을 경우 일단 빈 리스트 반환

        var lines = csv.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries); // 줄 단위 분할
        List<DialogLine> l = new List<DialogLine>(); // L파싱된 DialogLine들을 모아둘 리스트

        int lastEventID = -1; // 마지막 EventID 저장
        int lastDialogID = -1; // 마지막 DialogID 저장

        for (int i = 1; i < lines.Length; i++) // 0은 헤더이므로 i = 1 부터
        {
            var row = lines[i].Split(';'); // 각 행을 ; 로 분리해 row 에 저장

            DialogLine d = new DialogLine(); // 인스턴스 생성

            // EventID 처리
            // 빈칸인 경우 이전 값 그대로 씀
            string eventStr = Safe(row, 0);
            if (int.TryParse(eventStr, out int eID)) lastEventID = eID;
            d.EventID = lastEventID;             

            // DialogID 처리
            // 빈칸인 경우 이전 값 그대로 씀
            string dialogStr = Safe(row, 1);
            if (int.TryParse(dialogStr, out int dID)) lastDialogID = dID; 
            d.DialogID = lastDialogID;                            

            d.CharacterName = Safe(row, 2);
            d.Text = Safe(row, 3);
            d.SelectID = Safe(row, 4);

            // SkipAndMoveTo 처리
            string skipStr = Safe(row, 5);
            if (string.IsNullOrEmpty(skipStr)) d.SkipAndMoveTo = null; // 빈칸인 경우 널값
            else if (int.TryParse(skipStr, out int skip)) d.SkipAndMoveTo = skip;
            else d.SkipAndMoveTo = null;

            l.Add(d); // 완성된 DialogLine을 리스트에 추가
        }

        return l; // 전체 라인 리스트 반환
    }

    // DialogSelect.csv를 파싱해 모든 DialogSelectOption을 반환
    // (SelectID, DialogID, Text)
    public static List<DialogSelectOption> ParseDialogSelectCSV(string resourcePath = "DialogSelect")
    {
        TextAsset csv = Resources.Load<TextAsset>(resourcePath);
        if (csv == null) return new List<DialogSelectOption>();

        var lines = csv.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        List<DialogSelectOption> s = new List<DialogSelectOption>();

        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Split(';');
            if (row.Length < 3) continue;

            DialogSelectOption opt = new DialogSelectOption();
            opt.SelectID = Safe(row, 0);
            if (int.TryParse(Safe(row, 1), out int dID)) opt.DialogID = dID;
            opt.Text = Safe(row, 2);

            s.Add(opt);
        }

        return s;
    }

    // 최종: DialogEvent를 EventID로 묶어서 반환
    public static Dictionary<int, DialogEvent> BuildEventBundles(string eventCsv = "Dialog", string selectCsv = "DialogSelect")
    {
        var lines = ParseDialogCSV(eventCsv);
        var selects = ParseDialogSelectCSV(selectCsv);

        var dict = new Dictionary<int, DialogEvent>();

        // Lines >> EventID 기준으로 묶음 생성
        // 각 DialogLine을 그 EventID에 해당하는 DialogEvent의 Lines 리스트에 추가
        foreach (var l in lines)
        {
            if (!dict.ContainsKey(l.EventID)) dict[l.EventID] = new DialogEvent { EventID = l.EventID };
            dict[l.EventID].Lines.Add(l);
        }

        // Selects >> SelectID 기준으로 묶음 생성: 모든 Event에 공통
        foreach (var s in selects)
        {
            // 각 Event에 select들을 복사해두는 방식: 단일 DB에서 공유 가능
            // 쉽게 하기 위해 일단 모든 번들에 Selects 동일 복사 즉 전체 이벤트에서 동일한 SelectID 쓰도록
            foreach (var d in dict)
            {
                if (!d.Value.Selects.ContainsKey(s.SelectID)) d.Value.Selects[s.SelectID] = new List<DialogSelectOption>();
                d.Value.Selects[s.SelectID].Add(s);
            }
        }

        // 각 Event의 Lines를 DialogID 오름차순으로 정렬
        foreach (var d in dict) d.Value.Lines.Sort((a, b) => a.DialogID.CompareTo(b.DialogID));

        return dict;
    }
}