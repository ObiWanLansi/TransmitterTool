﻿
# RxTxTypes (Version 21)

*The Overview Of The Known RxTxTypes*


## Receiver
Name|Value|Description
-|-|-
IdealSDR|1|Ideal Sdr Receiver (Passes Signal Through)
HackRF|2|HackRF One
B200mini|3|Ettus B200mini
TwinRx|4|Ettus X310 / TwinRx


## Transmitter
Name|Value|Description
-|-|-
QPSK|1|QPSK Signal With 2kHz Bandwidth
SIN|2|This Is A Sine Generator A 500Hz Frequency
FMBroadcast|3|This Is A Fm Broadcast Radio Transmitter (Awgn Noise Signal) With Input 20Khz Signal And 50Khz Bandwidth
GPSJammer|4|10MHz L1 GPS Jammer
Iridium|5|Iridium Satcom Transmitter
LTE|6|LTE Signal
AIS|7|AIS Signal
NFMRadio|8|Narrow Fm Band (Voice With 5Khz Bandwidth)
GSM|9|200Khz GSM Signal With Random Data
SBandRadar|10|SIMRAD's Argus S-Band Radar
