﻿using System.ComponentModel;

namespace CapFrameX.ViewModel
{
    public enum EComparisonContext
    {
		[Description("Date and time")]
		DateTime = 1,
		[Description("CPU")]
		CPU = 2,
		[Description("Graphic card")]
		GPU = 3,
		[Description("Context from comment")]
		Custom = 4
	}
}