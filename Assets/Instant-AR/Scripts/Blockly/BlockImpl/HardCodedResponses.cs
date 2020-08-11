using System;
using System.Collections.Generic;

public class HardCodedResponses
{
    public HardCodedResponses()
    {
    }

    public List<string> getDropdownOptionsHardCoded(object value)
    {
        List<string> listOptions = new List<string>();
        if (value.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference bRef = (BlocklyReference)value;
            if (bRef.type.Equals("text"))
            {
                string bValue = (string)bRef.value;
                if (bValue.StartsWith("ARQuery:", StringComparison.CurrentCulture))
                {
                    string queryNodePath = bValue.Substring(8);
                    if (queryNodePath.StartsWith("Green", StringComparison.CurrentCulture))
                    {
                        foreach (string optionText in getGreenStatus())
                        {
                            listOptions.Add(optionText);
                        }
                    }
                    else if (queryNodePath.StartsWith("Yellow", StringComparison.CurrentCulture))
                    {
                        foreach (string optionText in getYellowStatus())
                        {
                            listOptions.Add(optionText);
                        }
                    }
                    else if (queryNodePath.StartsWith("Red", StringComparison.CurrentCulture))
                    {
                        foreach (string optionText in getRedStatus())
                        {
                            listOptions.Add(optionText);
                        }
                    }
                }
                else
                {
                    if (bValue.Contains(";"))
                    {
                        foreach (string optionText in bValue.Split(';'))
                        {
                            listOptions.Add(optionText);
                        }
                    }
                    else
                    {
                        listOptions.Add(bValue);
                    }
                }
            }
        }
        return listOptions;
    }

    private string[] getGreenStatus()
    {
        string[] green = {
            "NE_1>AMC1>ISense0",
            "NE_1>AMC1>ISense2",
            "NE_1>AMC1>ISense3",
            "NE_1>AMC1>ISense4",
            "NE_1>AMC1>ISense5",
            "NE_1>AMC1>ISense6",
            "NE_1>AMC1>KU Fault_0",
            "NE_1>AMC1>KU Fault_1",
            "NE_1>AMC1>KU Fault_2",
            "NE_1>AMC1>KU Tsense_Int",
            "NE_1>AMC1>KU Vin_sns",
            "NE_1>AMC1>PWR_CTL_PGOOD",
            "NE_1>AMC1>SPB Plus +12V",
            "NE_1>AMC1>SPB Plus +1V35 ZQ DDR",
            "NE_1>AMC1>SPB Plus +1V8 KU AUX",
            "NE_1>AMC1>SPB Plus +2V5",
            "NE_1>AMC1>SPB Plus +3.3V",
            "NE_1>AMC1>SPB Plus +3V3 KU",
            "NE_1>AMC1>SPB Plus Hotswap",
            "NE_1>AMC1>SPB Plus KU Temp",
            "NE_1>AMC1>SPB Plus PCB Temp",
            "NE_1>AMC1>SPB Plus Watchdog",
            "NE_1>AMC1>SPB Plus ZQ Temp",
            "NE_1>AMC1>Tsense0",
            "NE_1>AMC1>Tsense1",
            "NE_1>AMC1>Tsense2",
            "NE_1>AMC1>Tsense5",
            "NE_1>AMC1>VSense0",
            "NE_1>AMC1>VSense1",
            "NE_1>AMC1>VSense2",
            "NE_1>AMC1>VSense3",
            "NE_1>AMC1>VSense4",
            "NE_1>AMC1>VSense5",
            "NE_1>AMC1>VSense6",
            "NE_1>AMC1>ZQ Fault_0",
            "NE_1>AMC1>ZQ Fault_1",
            "NE_1>AMC1>ZQ Fault_2",
            "NE_1>AMC1>ZQ Fault_3",
            "NE_1>AMC1>ZQ Tsense_Int",
            "NE_1>AMC1>ZQ Vin_sns",
            "NE_1>AMC10>AUXM DS75 Temp",
            "NE_1>AMC11>MSB +10V7",
            "NE_1>AMC11>MSB +12V",
            "NE_1>AMC11>MSB +3.3V",
            "NE_1>AMC11>MSB +3V7",
            "NE_1>AMC11>MSB +5V5",
            "NE_1>AMC11>MSB +8V5",
            "NE_1>AMC11>MSB Hotswap",
            "NE_1>AMC11>MSB Isense",
            "NE_1>AMC11>MSB PCB Temp 1",
            "NE_1>AMC11>MSB PCB Temp 2",
            "NE_1>AMC11>MSB Power Good",
            "NE_1>AMC11>MSB RFB_TEMP_RX1_RX2",
            "NE_1>AMC11>MSB RFB_TEMP_TX1_TX2",
            "NE_1>AMC11>MSB RFB_TEMP_TX3_TX4",
            "NE_1>AMC11>MSB RFIO_TEMP_RX",
            "NE_1>AMC11>MSB RFIO_TEMP_TCT",
            "NE_1>AMC11>MSB RFIO_TEMP_TX",
            "NE_1>AMC11>MSB Watchdog",
            "NE_1>AMC12>MSB +10V7",
            "NE_1>AMC12>MSB +12V",
            "NE_1>AMC12>MSB +3.3V",
            "NE_1>AMC12>MSB +3V7",
            "NE_1>AMC12>MSB +5V5",
            "NE_1>AMC12>MSB +8V5",
            "NE_1>AMC12>MSB Hotswap",
            "NE_1>AMC12>MSB Isense",
            "NE_1>AMC12>MSB PCB Temp 1",
            "NE_1>AMC12>MSB PCB Temp 2",
            "NE_1>AMC12>MSB Power Good",
            "NE_1>AMC12>MSB RFB_TEMP_RX1_RX2",
            "NE_1>AMC12>MSB RFB_TEMP_TX1_TX2",
            "NE_1>AMC12>MSB RFB_TEMP_TX3_TX4",
            "NE_1>AMC12>MSB Watchdog",
            "NE_1>AMC2>VT AMC702 HS",
            "NE_1>AMC2>VT AMC702 TCext1",
            "NE_1>AMC2>VT AMC702 TCext2",
            "NE_1>AMC2>VT AMC702 TCint1",
            "NE_1>AMC2>VT AMC702 TCint2",
            "NE_1>AMC2>VT AMC702 Tin",
            "NE_1>AMC2>VT AMC702 Tout",
            "NE_1>AMC4>VT AMC702 HS",
            "NE_1>AMC4>VT AMC702 TCext1",
            "NE_1>AMC4>VT AMC702 TCext2",
            "NE_1>AMC4>VT AMC702 TCint1",
            "NE_1>AMC4>VT AMC702 TCint2",
            "NE_1>AMC4>VT AMC702 Tin",
            "NE_1>AMC4>VT AMC702 Tout",
            "NE_1>AMC5>VT AMC702 HS",
            "NE_1>AMC5>VT AMC702 TCext1",
            "NE_1>AMC5>VT AMC702 TCext2",
            "NE_1>AMC5>VT AMC702 TCint1",
            "NE_1>AMC5>VT AMC702 TCint2",
            "NE_1>AMC5>VT AMC702 Tin",
            "NE_1>AMC5>VT AMC702 Tout",
            "NE_1>AMC7>XPC Board Temp",
            "NE_1>AMC7>XPC CPU Temp",
            "NE_1>AMC8>DCB +12V",
            "NE_1>AMC8>DCB +1V8",
            "NE_1>AMC8>DCB +1V8 KA",
            "NE_1>AMC8>DCB +3.3V",
            "NE_1>AMC8>DCB +3V3",
            "NE_1>AMC8>DCB +3V3 KA",
            "NE_1>AMC8>DCB Hotswap",
            "NE_1>AMC8>DCB Isense",
            "NE_1>AMC8>DCB KA Temp",
            "NE_1>AMC8>DCB KB Temp",
            "NE_1>AMC8>DCB PCB Temp",
            "NE_1>AMC8>DCB Watchdog",
            "NE_1>AMC8>DCB ZQ Temp",
            "NE_1>AMC8>KA LTC2974_Fault_0",
            "NE_1>AMC8>KA LTC2974_Fault_1",
            "NE_1>AMC8>KA LTC2974_Fault_2",
            "NE_1>AMC8>KA LTC2974_Fault_3",
            "NE_1>AMC8>KA LTC2974_Isense_0",
            "NE_1>AMC8>KA LTC2974_Isense_1",
            "NE_1>AMC8>KA LTC2974_Isense_2",
            "NE_1>AMC8>KA LTC2974_Isense_3",
            "NE_1>AMC8>KA LTC2974_Tsense_0",
            "NE_1>AMC8>KA LTC2974_Tsense_2",
            "NE_1>AMC8>KA LTC2974_Tsense_3",
            "NE_1>AMC8>KA LTC2974_Tsense_Int",
            "NE_1>AMC8>KA LTC2974_Vin_sns",
            "NE_1>AMC8>KA LTC2974_Vout_0",
            "NE_1>AMC8>KA LTC2974_Vout_1",
            "NE_1>AMC8>KA LTC2974_Vout_2",
            "NE_1>AMC8>KA LTC2974_Vout_3",
            "NE_1>AMC8>KB LTC2974_Fault_0",
            "NE_1>AMC8>KB LTC2974_Fault_1",
            "NE_1>AMC8>KB LTC2974_Fault_2",
            "NE_1>AMC8>KB LTC2974_Fault_3",
            "NE_1>AMC8>KB LTC2974_Isense_0",
            "NE_1>AMC8>KB LTC2974_Isense_1",
            "NE_1>AMC8>KB LTC2974_Isense_2",
            "NE_1>AMC8>KB LTC2974_Isense_3",
            "NE_1>AMC8>KB LTC2974_Tsense_0",
            "NE_1>AMC8>KB LTC2974_Tsense_2",
            "NE_1>AMC8>KB LTC2974_Tsense_3",
            "NE_1>AMC8>KB LTC2974_Tsense_Int",
            "NE_1>AMC8>KB LTC2974_Vin_sns",
            "NE_1>AMC8>KB LTC2974_Vout_0",
            "NE_1>AMC8>KB LTC2974_Vout_1",
            "NE_1>AMC8>KB LTC2974_Vout_2",
            "NE_1>AMC8>KB LTC2974_Vout_3",
            "NE_1>AMC8>PWR_CTL_PGOOD",
            "NE_1>AMC8>ZQ LTC2974_Fault_0",
            "NE_1>AMC8>ZQ LTC2974_Fault_1",
            "NE_1>AMC8>ZQ LTC2974_Fault_2",
            "NE_1>AMC8>ZQ LTC2974_Fault_3",
            "NE_1>AMC8>ZQ LTC2974_Isense_0",
            "NE_1>AMC8>ZQ LTC2974_Isense_1",
            "NE_1>AMC8>ZQ LTC2974_Isense_2",
            "NE_1>AMC8>ZQ LTC2974_Isense_3",
            "NE_1>AMC8>ZQ LTC2974_Tsense_0",
            "NE_1>AMC8>ZQ LTC2974_Tsense_3",
            "NE_1>AMC8>ZQ LTC2974_Tsense_Int",
            "NE_1>AMC8>ZQ LTC2974_Vin_sns",
            "NE_1>AMC8>ZQ LTC2974_Vout_0",
            "NE_1>AMC8>ZQ LTC2974_Vout_1",
            "NE_1>AMC8>ZQ LTC2974_Vout_2",
            "NE_1>AMC8>ZQ LTC2974_Vout_3",
            "NE_1>AMC9>DCB +12V",
            "NE_1>AMC9>DCB +1V8",
            "NE_1>AMC9>DCB +1V8 KA",
            "NE_1>AMC9>DCB +3.3V",
            "NE_1>AMC9>DCB +3V3",
            "NE_1>AMC9>DCB +3V3 KA",
            "NE_1>AMC9>DCB Hotswap",
            "NE_1>AMC9>DCB Isense",
            "NE_1>AMC9>DCB KA Temp",
            "NE_1>AMC9>DCB KB Temp",
            "NE_1>AMC9>DCB PCB Temp",
            "NE_1>AMC9>DCB Watchdog",
            "NE_1>AMC9>DCB ZQ Temp",
            "NE_1>AMC9>KA LTC2974_Fault_0",
            "NE_1>AMC9>KA LTC2974_Fault_1",
            "NE_1>AMC9>KA LTC2974_Fault_2",
            "NE_1>AMC9>KA LTC2974_Fault_3",
            "NE_1>AMC9>KA LTC2974_Isense_0",
            "NE_1>AMC9>KA LTC2974_Isense_1",
            "NE_1>AMC9>KA LTC2974_Isense_2",
            "NE_1>AMC9>KA LTC2974_Isense_3",
            "NE_1>AMC9>KA LTC2974_Tsense_0",
            "NE_1>AMC9>KA LTC2974_Tsense_2",
            "NE_1>AMC9>KA LTC2974_Tsense_3",
            "NE_1>AMC9>KA LTC2974_Tsense_Int",
            "NE_1>AMC9>KA LTC2974_Vin_sns",
            "NE_1>AMC9>KA LTC2974_Vout_0",
            "NE_1>AMC9>KA LTC2974_Vout_1",
            "NE_1>AMC9>KA LTC2974_Vout_2",
            "NE_1>AMC9>KA LTC2974_Vout_3",
            "NE_1>AMC9>KB LTC2974_Fault_0",
            "NE_1>AMC9>KB LTC2974_Fault_1",
            "NE_1>AMC9>KB LTC2974_Fault_2",
            "NE_1>AMC9>KB LTC2974_Fault_3",
            "NE_1>AMC9>KB LTC2974_Isense_0",
            "NE_1>AMC9>KB LTC2974_Isense_1",
            "NE_1>AMC9>KB LTC2974_Isense_2",
            "NE_1>AMC9>KB LTC2974_Isense_3",
            "NE_1>AMC9>KB LTC2974_Tsense_0",
            "NE_1>AMC9>KB LTC2974_Tsense_2",
            "NE_1>AMC9>KB LTC2974_Tsense_3",
            "NE_1>AMC9>KB LTC2974_Tsense_Int",
            "NE_1>AMC9>KB LTC2974_Vin_sns",
            "NE_1>AMC9>KB LTC2974_Vout_0",
            "NE_1>AMC9>KB LTC2974_Vout_1",
            "NE_1>AMC9>KB LTC2974_Vout_2",
            "NE_1>AMC9>KB LTC2974_Vout_3",
            "NE_1>AMC9>PWR_CTL_PGOOD",
            "NE_1>AMC9>ZQ LTC2974_Fault_0",
            "NE_1>AMC9>ZQ LTC2974_Fault_1",
            "NE_1>AMC9>ZQ LTC2974_Fault_2",
            "NE_1>AMC9>ZQ LTC2974_Fault_3",
            "NE_1>AMC9>ZQ LTC2974_Isense_0",
            "NE_1>AMC9>ZQ LTC2974_Isense_1",
            "NE_1>AMC9>ZQ LTC2974_Isense_2",
            "NE_1>AMC9>ZQ LTC2974_Isense_3",
            "NE_1>AMC9>ZQ LTC2974_Tsense_0",
            "NE_1>AMC9>ZQ LTC2974_Tsense_3",
            "NE_1>AMC9>ZQ LTC2974_Tsense_Int",
            "NE_1>AMC9>ZQ LTC2974_Vin_sns",
            "NE_1>AMC9>ZQ LTC2974_Vout_0",
            "NE_1>AMC9>ZQ LTC2974_Vout_1",
            "NE_1>AMC9>ZQ LTC2974_Vout_2",
            "NE_1>AMC9>ZQ LTC2974_Vout_3",
            "NE_1>BMC>3V3 CB Curr",
            "NE_1>BMC>3V3 CB Volt",
            "NE_1>BMC>AMC1 CB Curr",
            "NE_1>BMC>AMC1 CB Fault",
            "NE_1>BMC>AMC1 CB Volt 12V",
            "NE_1>BMC>AMC10 CB Curr",
            "NE_1>BMC>AMC10 CB Fault",
            "NE_1>BMC>AMC10 CB Volt 12V",
            "NE_1>BMC>AMC11 CB Curr",
            "NE_1>BMC>AMC11 CB Fault",
            "NE_1>BMC>AMC11 CB Volt 12V",
            "NE_1>BMC>AMC12 CB Curr",
            "NE_1>BMC>AMC12 CB Fault",
            "NE_1>BMC>AMC12 CB Volt 12V",
            "NE_1>BMC>AMC2 CB Curr",
            "NE_1>BMC>AMC2 CB Fault",
            "NE_1>BMC>AMC2 CB Volt 12V",
            "NE_1>BMC>AMC3 CB Curr",
            "NE_1>BMC>AMC3 CB Fault",
            "NE_1>BMC>AMC3 CB Volt 12V",
            "NE_1>BMC>AMC4 CB Curr",
            "NE_1>BMC>AMC4 CB Fault",
            "NE_1>BMC>AMC4 CB Volt 12V",
            "NE_1>BMC>AMC5 CB Curr",
            "NE_1>BMC>AMC5 CB Fault",
            "NE_1>BMC>AMC5 CB Volt 12V",
            "NE_1>BMC>AMC6 CB Curr",
            "NE_1>BMC>AMC6 CB Fault",
            "NE_1>BMC>AMC6 CB Volt 12V",
            "NE_1>BMC>AMC7 CB Curr",
            "NE_1>BMC>AMC7 CB Fault",
            "NE_1>BMC>AMC7 CB Volt 12V",
            "NE_1>BMC>AMC8 CB Curr",
            "NE_1>BMC>AMC8 CB Fault",
            "NE_1>BMC>AMC8 CB Volt 12V",
            "NE_1>BMC>AMC9 CB Curr",
            "NE_1>BMC>AMC9 CB Fault",
            "NE_1>BMC>AMC9 CB Volt 12V",
            "NE_1>BMC>FAN CB Curr",
            "NE_1>BMC>FAN CB Volt 12V",
            "NE_1>BMC>FAN1 Tachometer",
            "NE_1>BMC>FAN2 Tachometer",
            "NE_1>BMC>FAN3 Tachometer",
            "NE_1>BMC>FAN4 Tachometer",
            "NE_1>BMC>ICB CB Curr",
            "NE_1>BMC>ICB CB Volt 12V",
            "NE_1>BMC>ICB Local Temp",
            "NE_1>BMC>ICB Remote Temp",
            "NE_1>BMC>ICM adcSENSE_V0P75_DDR_VREF - ADC Channel 3",
            "NE_1>BMC>ICM adcSENSE_V0P75_DDR_VTT - ADC Channel 4",
            "NE_1>BMC>ICM adcSENSE_V12P0 - ADC Channel 14",
            "NE_1>BMC>ICM adcSENSE_V1P2 - ADC Channel 5"
        };
        return green;
    }

    private string[] getYellowStatus()
    {
        string[] yellow = {
            "NE_1>BMC>ICM adcSENSE_V1P5 - ADC Channel 12",
            "NE_1>BMC>ICM adcSENSE_V1P8 - ADC Channel 2",
            "NE_1>BMC>ICM adcSENSE_V2P5 - ADC Channel 13",
            "NE_1>BMC>ICM adcSENSE_V3P3 - ADC Channel 0",
            "NE_1>BMC>ICM adcSENSE_V3P3_STANDBY - ADC Channel 15",
            "NE_1>BMC>PSM DS75 Temp",
            "NE_1>BMC>SSB CB Curr",
            "NE_1>BMC>SSB CB Volt 12V",
            "NE_1>BMC>SSB Temp",
            "NE_1>EPC>_HWM_AUX",
            "NE_1>EPC>_HWM_CPU_Fan",
            "NE_1>EPC>_HWM_CPU_Power",
            "NE_1>EPC>_HWM_CPU_PWR",
            "NE_1>EPC>_HWM_CPU_Temp",
            "NE_1>EPC>_HWM_FC_Fan",
            "NE_1>EPC>_HWM_FC_Temp",
            "NE_1>EPC>_HWM_SB3",
            "NE_1>EPC>_HWM_SB5",
            "NE_1>EPC>_HWM_System_Fan",
            "NE_1>EPC>_HWM_System_Temp",
            "NE_1>EPC>_HWM_Type_Fan",
            "NE_1>EPC>_HWM_Type_Mask",
            "NE_1>EPC>_HWM_Type_Power",
            "NE_1>EPC>_HWM_Type_Temperature",
            "NE_1>EPC>_HWM_Type_Voltage",
            "NE_1>EPC>_HWM_V_128"
        };
        return yellow;
    }

    private string[] getRedStatus()
    {
        string[] red = {
            "NE_1>EPC>_HWM_V12",
            "NE_1>EPC>_HWM_VBATT",
            "NE_1>EPC>_HWM_VCC_DDR",
            "NE_1>EPC>_HWM_VCC1_05",
            "NE_1>EPC>_HWM_VCC1_2",
            "NE_1>EPC>_HWM_VCC1_5",
            "NE_1>EPC>_HWM_VCC1_8",
            "NE_1>EPC>_HWM_VCC2_5",
            "NE_1>EPC>_HWM_VCC34",
            "NE_1>EPC>_HWM_VCC55",
            "NE_1>EPC>_HWM_VCORE",
            "NE_1>EPC>_HWM_VCOREb",
            "NE_1>EPC>_HWM_VIN"
        };
        return red;
    }
}
