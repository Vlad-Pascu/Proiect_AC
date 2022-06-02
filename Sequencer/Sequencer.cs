using System;

namespace Sequencer
{
    class Sequencer
    {
        static long[] MPM = {0x60184D003,0x00000190A,0x000014804,0x000010804,0x301830000,0x1019C8000,0x301800000,0x6019C0000,0x701980000,0x901634800,0x000000A0E
                    ,0x000000B19,0x000000E38,0x000000E5A,0x201500B14,0x60188C000,0x201880813,0x60188C000,0x283880000,0x901500B14,0x022900C1E,0x000000802,0x022880C1E
                    ,0x05288C000,0x923880C1E,0x022900D25,0x000000802,0x022880C1E,0x05288C000,0x923880D25,0x401900B34,0x483918B34,0x58391CB34,0x58301CF00,0x485920B34
                    ,0x486900B34,0x487920B34,0x0B2920B34,0x092920B34,0xB8391CB34,0xC85918B34,0x088918B34,0x089918B34,0x08A920B34,0x09B920B34,0x08C920B34,0x08D920B34
                    ,0x08E920B34,0x3018C8F00,0x801500000,0x301880000,0x401804B34,0x901300F00,0x000000802,0x0000C0F00,0x0000C0F00,0x000002B54,0x000000F00,0x000002BD4
                    ,0x000000F00,0x000003354,0x000000F00,0x0000033D4,0x000000F00,0x000002354,0x000000F00,0x000002354,0x000000F00,0x000003B54,0x000000F00,0x000003BD4
                    ,0x000000F00,0x000000B54,0x000000000,0x000000B4B,0x000000802,0x60188C850,0x022500851,0x60188C000,0x923500851,0x953500000,0x301808000,0x6019C0000
                    ,0x401600F00,0x000000802,0x60188C859,0x022600F00,0x60188C000,0x923600F00,0x953600F00,0x1A5100F00,0x000000000,0x1A6100F00,0x000000000,0x000000F00
                    ,0x000000000,0x00002C000,0x000000000,0x000024F00,0x000000000,0x000028800,0x000000000,0x301808000,0x6019C0F00,0x301880000,0x901604F00,0x301808000
                    ,0x1019C0F00,0x301880000,0x901104F00,0x301880000,0x901604F00,0x301880000,0x901604000,0x301880000,0x901104F00};
        static short[] MEM = new short[65536];
        static RegFlags regFlags = new RegFlags();
        static short SBUS, DBUS, RBUS;
        static long MAR = 0, MIR = 0;
        static int stare = 0;
        static long mascaDbus = ((MIR & 0x0F0000000) >> 28);
        static long mascaAlu = ((MIR & 0x00F000000) >> 24);
        static long mascaRbus = ((MIR & 0x000F00000) >> 20);
        static long mascaMEM = ((MIR & 0x0000C0000) >> 18);
        static long mascaOthers = ((MIR & 0x00003C000) >> 14);
        static long mascaSuccesor = ((MIR & 0x000003800) >> 11);
        static long mascaIndex = ((MIR & 0x000000700) >> 8);
        static long mascaTF = ((MIR & 0x000000080) >> 7);
        static long microAdresa = ((MIR & 0x00000007F));
        static short instrCurenta;
        static short regS, regD;
        static void Main(string[] args)
        {
            switch (stare)
            {
                case 0:
                    {
                        MIR = MPM[MAR];
                        stare = 1;
                        break;
                    }
                case 1:
                    {
                        setSBUS(MIR);
                        switch (mascaDbus)
                        {
                            case 0:
                                {
                                    break;
                                }
                            case 1:
                                {
                                    DBUS = Convert.ToString(Flags.getFlagC()) + Convert.ToString(Flags.getFlagN()) +
                                        Convert.ToString(Flags.getFlagV()) + Convert.ToString(Flags.getFlagZ()) +
                                        Convert.ToString(Flags.getFlagACLOW()) + Convert.ToString(Flags.getFlagCIL()) +
                                        Convert.ToString(Flags.getFlagBE0()) + Convert.ToString(Flags.getFlagBE1())
                                      + Convert.ToString(Flags.getFlagBI() + Convert.ToString(Flags.getFlagBVI()));
                                    break;
                                }
                            case 2:
                                {
                                    DBUS = registrii.readR();
                                    break;
                                }
                            case 3:
                                {
                                    DBUS = registrii.readSP();
                                    break;
                                }
                            case 4:
                                {
                                    DBUS = registrii.readT();
                                    break;
                                }
                            case 5:
                                {
                                    DBUS = registrii.readPC();
                                    break;
                                }
                            case 6:
                                {
                                    DBUS = registrii.readIVR();
                                    break;
                                }
                            case 7:
                                {
                                    DBUS = registrii.readADR();
                                    break;
                                }
                            case 8:
                                {
                                    DBUS = registrii.readMDR();
                                    break;
                                }
                            case 9:
                                {
                                    DBUS = registrii.notMDR();
                                    break;
                                }
                            case 10:
                                {
                                    DBUS = registrii.readIR();
                                    break;
                                }
                            case 11:
                                {
                                    DBUS = "0";
                                    break;
                                }
                            case 12:
                                {
                                    DBUS = "-1";
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        switch (mascaAlu)
                        {
                            case 0:
                                {
                                    break;
                                }
                            case 1:
                                {
                                    RBUS = SBUS;
                                    break;
                                }
                            case 2:
                                {
                                    RBUS = DBUS;
                                    break;
                                }
                            case 3:
                                {
                                    int SBUS_int = Convert.ToInt16(SBUS, 2);
                                    int DBUS_int = Convert.ToInt16(DBUS, 2);
                                    int RBUS_int = SBUS_int + DBUS_int + Convert.ToInt16(Flags.getFlagC());
                                    RBUS = Convert.ToString(RBUS_int, 2);

                                    if (RBUS.Length > 16)
                                    {
                                        Flags.setFlagC();
                                        Flags.setFlagV();
                                        RBUS = RBUS.Substring(1);
                                    }

                                    if (RBUS_int < 0)
                                    {
                                        Flags.setFlagN();
                                    }

                                    if (RBUS_int == 0)
                                    {
                                        Flags.setFlagZ();
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    int SBUS_int = Convert.ToInt16(SBUS, 2);
                                    int DBUS_int = Convert.ToInt16(DBUS, 2);
                                    int RBUS_int = SBUS_int - DBUS_int;
                                    RBUS = Convert.ToString(RBUS_int, 2);
                                    break;
                                }
                            case 5:
                                {
                                    int SBUS_int = Convert.ToInt16(SBUS, 2);
                                    int DBUS_int = Convert.ToInt16(DBUS, 2);
                                    int RBUS_int = SBUS_int & DBUS_int;
                                    RBUS = Convert.ToString(RBUS_int, 2);
                                    if (RBUS_int < 0)
                                    {
                                        Flags.setFlagN();
                                    }

                                    if (RBUS_int == 0)
                                    {
                                        Flags.setFlagZ();
                                    }

                                    break;
                                }
                            case 6:
                                {
                                    int SBUS_int = Convert.ToInt16(SBUS, 2);
                                    int DBUS_int = Convert.ToInt16(DBUS, 2);
                                    int RBUS_int = SBUS_int | DBUS_int;
                                    RBUS = Convert.ToString(RBUS_int, 2);
                                    if (RBUS_int < 0)
                                    {
                                        Flags.setFlagN();
                                    }
                                    if (RBUS_int == 0)
                                    {
                                        Flags.setFlagZ();
                                    }
                                    break;
                                }
                            case 7:
                                {
                                    int SBUS_int = Convert.ToInt16(SBUS, 2);
                                    int DBUS_int = Convert.ToInt16(DBUS, 2);
                                    int RBUS_int = SBUS_int ^ DBUS_int;
                                    RBUS = Convert.ToString(RBUS_int, 2);
                                    if (RBUS_int < 0)
                                    {
                                        Flags.setFlagN();
                                    }

                                    if (RBUS_int == 0)
                                    {
                                        Flags.setFlagZ();
                                    }
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        switch (mascaRbus)
                        {
                            case 0:
                                {
                                    break;
                                }
                            case 1:
                                {
                                    Flags.setFlagZ();
                                    Flags.setFlagN();
                                    Flags.setFlagC();
                                    Flags.setFlagV();
                                    break;
                                }
                            case 2:
                                {
                                    Flags.setFlagZ();
                                    Flags.setFlagN();
                                    Flags.setFlagC();
                                    Flags.setFlagV();
                                    break;
                                }
                            case 3:
                                {
                                    registrii.writeR(RBUS,index);
                                    break;
                                }
                            case 4:
                                {
                                    registrii.writeSP(RBUS);
                                    break;
                                }
                            case 5:
                                {
                                    registrii.writeT(RBUS);
                                    break;
                                }
                            case 6:
                                {
                                    registrii.writePC(RBUS);
                                    break;
                                }
                            case 7:
                                {
                                    registrii.writeIVR(RBUS);
                                    break;
                                }
                            case 8:
                                {
                                    registrii.writeADR(RBUS);
                                    break;
                                }
                            case 9:
                                {
                                    registrii.writeMDR(RBUS);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        switch (mascaOthers)
                        {
                            case 0:
                                {
                                    break;
                                }
                            case 1:
                                {
                                    registrii.incSP();
                                    break;
                                }
                            case 2:
                                {
                                    registrii.decSP();
                                    break;
                                }
                            case 3:
                                {
                                    registrii.incPC();
                                    break;
                                }
                            case 4:
                                {
                                    Flags.setFlagBE0();
                                    break;
                                }
                            case 5:
                                {
                                    Flags.setFlagBE1();
                                    break;
                                }
                            case 6:
                                {
                                    
                                }
                            case 7:
                                {
                                    Flags.setFlagC();
                                }
                        }
                        stare = 0;
                        break;
                    }
            }
        }

        public short getClasaInstructiune(short instructiune)
        {
            short clasa=0;
            short clsInstructiune = (short)(instructiune & 0xE000);
            clsInstructiune = (short)(clsInstructiune >> 13);
            if(clsInstructiune>=0 && clsInstructiune <= 3)
            {
                clasa = 0;//2 operanzi
            }
            else if (clsInstructiune == 4)
            {
                clasa = 1;//un operand
            }
            else if (clsInstructiune == 6)
            {
                clasa = 2;//salt
            }
            else if (clsInstructiune == 7)
            {
                clasa = 3;//diverse
            }
            return clasa;
        }

        public short modAdresareSursa(short instructiune)
        {
            short index2 = 0;
            short MAS = 0;
            MAS = (short)(instructiune & 0x0C00);
            MAS = (short)(MAS >> 10);
            switch (MAS)
            {
                case 0:
                    {
                        index2 = 0;
                        break;
                    }
                case 1:
                    {
                        index2 = 2;
                        break;
                    }
                case 2:
                    {
                        index2 = 4;
                        break;
                    }
                case 3:
                    {
                        index2 = 6;
                        break;
                    }
            }
            return index2;
        }
        public short modAdresareDestinatie(short instructiune)
        {
            short index3 = 0;
            short MAD = 0;
            MAD = (short)(instructiune & 0x0030);
            MAD = (short)(MAD >> 4);
            switch (MAD)
            {
                case 0:
                    {
                        index3 = 0;
                        break;
                    }
                case 1:
                    {
                        index3 = 2;
                        break;
                    }
                case 2:
                    {
                        index3 = 4;
                        break;
                    }
                case 3:
                    {
                        index3 = 6;
                        break;
                    }
            }
            return index3;
        }

        public short opcodeB1(short instructiune)
        {
            short index4 = 0;
            short opB1 = (short)(instructiune & 0xF000);
            opB1 = (short)(opB1 >> 12);
            index4 = (short)(opB1 * 2);
            return index4;
        }

        public short opcodeB2(short instructiune)
        {
            short index5 = 0;
            short opB2 = (short)(instructiune & 0x1FC0);// b12 ->b6
            opB2 = (short)(opB2 >> 6);
            index5 = (short)(opB2 * 2);
            return index5;
        }

        public short opcodeB3(short instructiune)
        {
            short index6 = 0;
            short opB3 = (short)(instructiune & 0x1E00);//b12 ->b8
            opB3 = (short)(opB3 >> 8);
            index6 = (short)(opB3 * 2);
            return index6;
        }

        public short opcodeB4(short instructiune)
        {
            short index7 = 0;
            short opB4 = (short)(instructiune & 0x1FFF);//b12 ->b0
            index7 = (short)(opB4 * 2);
            return index7;
        }

        public static void setSBUS(long microinstructiune)
        {
            long Sbus = ((MIR & 0xF00000000) >> 32);
            switch (Sbus)
            {
                case 0://none
                    {
                        SBUS = 0;
                        break;
                    }
                case 1://PdFLAG
                    {
                        SBUS = RegFlags.FLAGS;
                        break;
                    }
                case 2://PdRG
                    {
                        SBUS = RegFlags.RG[];
                        break;
                    }
                case 3://PdSP
                    {
                        SBUS = RegFlags.SP;
                        break;
                    }
                case 4://PdT
                    {
                        SBUS = RegFlags.T;
                        break;
                    }
                case 5://PdnT
                    {
                        SBUS = (short)(~RegFlags.T);
                        break;
                    }
                case 6://PdPC
                    {
                        SBUS = RegFlags.PC;
                        break;
                    }
                case 7://PdIVR
                    {
                        SBUS = RegFlags.IVR;
                        break;
                    }
                case 8://PdADR
                    {
                        SBUS = RegFlags.ADR;
                        break;
                    }
                case 9://PdMDR
                    {
                        SBUS = RegFlags.MDR;
                        break;
                    }
                case 10://PdIR[7-0]
                    {
                        short aux = instrCurenta;
                        aux = (short)(aux & 0x00FF);
                        SBUS = (short)aux;
                        break;
                    }
                case 11://Pd0
                    {
                        SBUS = 0;
                        break;
                    }
                case 12://Pd-1
                    {
                        SBUS = -1;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
