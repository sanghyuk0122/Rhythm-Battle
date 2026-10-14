using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public struct NoteData
{
    public int lane;      // 0 ~ laneCount-1
    public float hitTime; // 노래 시작 기준, 판정선에 닿아야 하는 시간(초)
}

public class ChartData
{
    public float bpm = 120f;
    public int div = 4;        // 한 박자(beat)를 몇 줄로 쪼갤지 (4 = 16분음표)
    public float offset = 0f;  // 첫 줄의 시작 시간(초)
    public List<NoteData> notes = new List<NoteData>();

    public static ChartData Parse(string text)
    {
        var chart = new ChartData();
        var rows = new List<string>();
        bool inChart = false;

        foreach (string raw in text.Split('\n'))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("//")) continue;

            if (!inChart)
            {
                if (line == "#CHART") { inChart = true; continue; }

                string[] kv = line.Split('=');
                if (kv.Length != 2) continue;
                string key = kv[0].Trim().ToUpper();
                string val = kv[1].Trim();

                if (key == "BPM") chart.bpm = float.Parse(val, CultureInfo.InvariantCulture);
                else if (key == "DIV") chart.div = int.Parse(val);
                else if (key == "OFFSET") chart.offset = float.Parse(val, CultureInfo.InvariantCulture);
            }
            else rows.Add(line);
        }

        float secPerRow = 60f / chart.bpm / chart.div;

        for (int i = 0; i < rows.Count; i++)
        {
            for (int lane = 0; lane < rows[i].Length; lane++)
            {
                if (rows[i][lane] == '1')
                {
                    chart.notes.Add(new NoteData
                    {
                        lane = lane,
                        hitTime = chart.offset + i * secPerRow
                    });
                }
            }
        }

        chart.notes.Sort((a, b) => a.hitTime.CompareTo(b.hitTime));
        return chart;
    }
}
