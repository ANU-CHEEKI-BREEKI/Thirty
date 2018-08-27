using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class StringFormats
{

    public const string floatNumber = "0.##";
    public const string floatNumberPercent = "0.##%";
    public const string floatSignNumber = "+0.##;-0.##";
    public const string floatSignNumberPercent = "+0.##%;-0.##%";

    public const string intNumber = "0.";
    public const string intNumberPercent = "0.%";
    public const string intSignNumber = "+0.;-0.";
    public const string intSignNumberPercent = "+0.%;-0.%";
    public const string intSeparatorNumber = "#,0";
    public const string intSeparatorSignNumber = "+#,0;-#,0";

    public static readonly NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " " };
    public static readonly NumberFormatInfo nfiSign = new NumberFormatInfo { NumberGroupSeparator = " ", PositiveSign = "+", NegativeSign = "-" };

}
