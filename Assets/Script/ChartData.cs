using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class NoteData
{
    public int lane;
    public float hitTime;
}

public class ChartData
{
    public float bpm = 120f;
    public int div = 2;
    public float offset = 0f;     // 초 단위
    public List<NoteData> notes = new List<NoteData>();

    public static ChartData Parse(string text)
    {
        var chart = new ChartData();
        var ci = CultureInfo.InvariantCulture;   // 소수점을 항상 '.'으로 읽음
        bool inChart = false;
        int row = 0;
        var rows = new List<string>();

        foreach (string raw in text.Split('\n'))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("//")) continue;

            if (line.StartsWith("#CHART")) { inChart = true; continue; }

            if (!inChart)
            {
                int eq = line.IndexOf('=');
                if (eq < 0) continue;
                string key = line.Substring(0, eq).Trim().ToUpper();
                string val = line.Substring(eq + 1).Trim();
                if (key == "BPM") chart.bpm = float.Parse(val, ci);
                else if (key == "DIV") chart.div = int.Parse(val, ci);
                else if (key == "OFFSET") chart.offset = float.Parse(val, ci);
            }
            else
            {
                if (line.Length < 4) continue;
                rows.Add(line);
            }
        }

        // 한 줄의 길이(초) = 60 / (BPM * DIV)
        double secPerRow = 60.0 / ((double)chart.bpm * chart.div);

        foreach (string line in rows)
        {
            for (int lane = 0; lane < 4; lane++)
            {
                if (line[lane] == '1')
                    chart.notes.Add(new NoteData
                    {
                        lane = lane,
                        hitTime = (float)(chart.offset + row * secPerRow)
                    });
            }
            row++;
        }

        float last = chart.notes.Count > 0 ? chart.notes[chart.notes.Count - 1].hitTime : 0f;
        Debug.Log($"차트 로드: BPM={chart.bpm}, DIV={chart.div}, OFFSET={chart.offset}, 노트={chart.notes.Count}개, 마지막 노트={last:0.0}초");
        return chart;
    }
}