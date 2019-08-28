using UnityEngine;
using System.Collections;
using ChartAndGraph;

public class BarChartFeed : MonoBehaviour {
	void Start () {
        BarChart barChart = GetComponent<BarChart>();
        if (barChart != null)
        {
        }
    }

    public void setBars(float value, string category, Material material)
    {
        BarChart barChart = GetComponent<BarChart>();
        if (barChart != null)
        {
            barChart.DataSource.AddCategory(category, material);
            barChart.DataSource.SetValue(category, "Value", value);
            //barChart.DataSource.SlideValue(category, "Scholarship Program", value, 10f);
        }

    }
    private void Update()
    {
    }
}
