using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ISS.Common
{
    public enum CapacityGroup
    {
        [Description("Cut")]
        Cut,

        [Description("Src")]
        Src,

        [Description("Sew")]
        Sew,

        [Description("Tex")]
        Tex,
    }

    public enum PlaningBox
    {
        APS = 0,
        AVYX = 1,
        ISS = 2,
        NET = 3,
        CWC = 4,
        MTLA = 5
    }
    public enum WrkOrderDueDate
    {
       [Description("B/D")]
        BD = 0,
         [Description("Cut")]
        Cut = 1,
         [Description("Sew")]
        Sew = 2,
         [Description("Atr")]
         Atr = 3,
         [Description("DC")]
        DC = 4

    }
    public enum ProductionStatus
    {
        [Description("R")]
        Released,
        [Description("L")]
        Locked,
        [Description("S")]
        Suggested,
        [Description("P")]
        CapacitySuggested
    }

    public enum CapacityType
    {
        [Description("WIP")]
        WIP,
        [Description("IN")]
        IN,
        [Description("PLAN")]
        PLAN,
        [Description("WOS")]
        WOS
    }

    public enum CapacityGroupAll
    {
        [Description("Cut")]
        Cut,

        [Description("Src")]
        Src,

        [Description("Sew")]
        Sew,

        [Description("Aprt")]
        Aprt,

        [Description("Tex")]    
        Tex,

        [Description("Atr")]
        Atr,

        [Description("Emb")]
        Emb,

        [Description("Sp")]
        Sp,

        [Description("Heat")]
        Heat,

        [Description("Flock")]
        Flock,

        [Description("Gw")]
        Gw
    }

    public enum ProductionCategories
    {
        [Description("ATR")]
        ATR,

        [Description("PKG")]
        PKG,

        [Description("SEW")]
        SEW,

        [Description("CUT")]
        CUT,

        [Description("DBF")]
        DBF,

        [Description("KNT")]
        KNT
    }

    public enum OrderStatus
    {
        [Description("S")]
        Suggested,

        [Description("R")]
        Released,

        [Description("L")]
        Locked
    }

    public enum SpreadTypes
    {
        [Description("BSD")]     
        BothSpread,

        [Description("T")]
        Trim,

        [Description("BO")]
        Body
    }

    public enum DemandSourceTypes
    {
        [Description("C")]
        CustomerOrder,

        [Description("E")]        
        Event,

        // Forecast
        [Description("F")]
        FCST,

        [Description("ST")]
        StockTarget,

        [Description("MB")]
        Maxbuild,

        [Description("TI")]
        TIL
    }

    public enum KAProgramSource
    {
        /// <summary>
        /// Process to OneSource
        /// </summary>
        [Description("Process to OneSource")]
        ISS2165,

        /// <summary>
        /// Process to AVYX
        /// </summary>
        [Description("Process to AVYX")]
        ISS2166

    }
}
