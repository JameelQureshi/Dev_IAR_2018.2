using System;
using System.Collections.Generic;

public class UIGauge : BaseElement
{
	public bool flushOthers;
	public bool alignParentSize;
	public string title;
	public string titleColor;
	public string unit;
	float gaugeValue;
	public string parentContainerId;
	public List<ChartRanges> chartRanges;
	public String delimiter = "@";
}
