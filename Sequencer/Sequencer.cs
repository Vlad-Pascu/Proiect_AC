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
        static short index;
        static short instrCurenta;
        static short regS, regD;
        static short F, G, TF;
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
                        setDBUS(MIR);
                        exALU(MIR);
                        destRBUS(MIR);
                        exOthers(MIR);
                        salt(MIR);
                        setIndex(MIR);
                        setTF(MIR);
                        if (G != 0)
                        {
                            long microAdresa = ((MIR & 0x00000007F));
                            MAR = microAdresa + index;
                        }
                        else
                            MAR++;
                        short memOperantion = (short)((MIR & 0x0000C0000) >> 18);
                        if (memOperantion != 0)
                            stare = 2;
                        else
                            stare = 0;
                        break;
                    }
                case 2:
                    {
                        stare = 3;
                        break;
                    }
                case 3:
                    {
                        memOp(MIR);
                        stare = 0;
                        break;
                    }
            }
        }

        public static short getClasaInstructiune(short instructiune)
        {
            short clasa = 0;
            short clsInstructiune = (short)(instructiune & 0xE000);
            clsInstructiune = (short)(clsInstructiune >> 13);
            if (clsInstructiune >= 0 && clsInstructiune <= 3)
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

        public static short modAdresareSursa(short instructiune)
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
        public static short modAdresareDestinatie(short instructiune)
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

        public static short opcodeB1(short instructiune)
        {
            short index4 = 0;
            short opB1 = (short)(instructiune & 0xF000);
            opB1 = (short)(opB1 >> 12);
            index4 = (short)(opB1 * 2);
            return index4;
        }

        public static short opcodeB2(short instructiune)
        {
            short index5 = 0;
            short opB2 = (short)(instructiune & 0x1FC0);// b12 ->b6
            opB2 = (short)(opB2 >> 6);
            index5 = (short)(opB2 * 2);
            return index5;
        }

        public static short opcodeB3(short instructiune)
        {
            short index6 = 0;
            short opB3 = (short)(instructiune & 0x1E00);//b12 ->b8
            opB3 = (short)(opB3 >> 8);
            index6 = (short)(opB3 * 2);
            return index6;
        }

        public static short opcodeB4(short instructiune)
        {
            short index7 = 0;
            short opB4 = (short)(instructiune & 0x1FFF);//b12 ->b0
            index7 = (short)(opB4 * 2);
            return index7;
        }

        public static void setRsRd(short instructiune)
        {
            short clsInstr = getClasaInstructiune(instructiune);
            if (clsInstr >= 0 && clsInstr <= 3)
            {
                regS = (short)(instructiune & 0x03C0);
                regS = (short)(regS >> 6);
                regD = (short)(instructiune & 0x000F);
            }
            else if (clsInstr == 4)
            {
                regS = 0;
                regD = (short)(instructiune & 0x000F);
            }
            else if (clsInstr == 6)
            {
                regS = regD = 0;
            }
            else if (clsInstr == 7)
            {
                regS = regD = 0;
            }
        }

        public static void setSBUS(long microinstructiune)
        {
            long Sbus = ((microinstructiune & 0xF00000000) >> 32);
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
                        SBUS = RegFlags.RG[regS];
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

        public static void setDBUS(long microinstructiune)
        {
            long Dbus = ((microinstructiune & 0x0F0000000) >> 28);
            switch (Dbus)
            {
                case 0://none
                    {
                        DBUS = 0;
                        break;
                    }
                case 1://PdFLAG
                    {
                        DBUS = RegFlags.FLAGS;
                        break;
                    }
                case 2://PdRG
                    {
                        DBUS = RegFlags.RG[regD];
                        break;
                    }
                case 3://PdSP
                    {
                        DBUS = RegFlags.SP;
                        break;
                    }
                case 4://PdT
                    {
                        DBUS = RegFlags.T;
                        break;
                    }
                case 5://PdPC
                    {
                        DBUS = RegFlags.PC;
                        break;
                    }
                case 6://PdIVR
                    {
                        DBUS = RegFlags.IVR;
                        break;
                    }
                case 7://PdADR
                    {
                        DBUS = RegFlags.ADR;
                        break;
                    }
                case 8://PdMDR
                    {
                        DBUS = RegFlags.MDR;
                        break;
                    }
                case 9://PdnMDR
                    {
                        DBUS = (short)(~RegFlags.MDR);
                        break;
                    }
                case 10://PdIR[7-0]
                    {
                        short aux = instrCurenta;
                        aux = (short)(aux & 0x00FF);
                        DBUS = (short)aux;
                        break;
                    }
                case 11://Pd0
                    {
                        DBUS = 0;
                        break;
                    }
                case 12://Pd-1
                    {
                        DBUS = -1;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public static void exALU(long microinstructiune)
        {
            long alu = ((microinstructiune & 0x00F000000) >> 24);
            switch (alu)
            {
                case 0://none
                    {
                        break;
                    }
                case 1://SBUS
                    {
                        RBUS = SBUS;
                        break;
                    }
                case 2://DBUS
                    {
                        RBUS = DBUS;
                        break;
                    }
                case 3://ADD
                    {
                        RBUS = (short)(SBUS + DBUS);
                        break;
                    }
                case 4://SUB
                    {
                        RBUS = (short)(SBUS - DBUS);
                        break;
                    }
                case 5://AND
                    {
                        RBUS = (short)(SBUS & DBUS);

                        break;
                    }
                case 6://OR
                    {
                        RBUS = (short)(SBUS | DBUS);
                        break;
                    }
                case 7://XOR
                    {
                        RBUS = (short)(SBUS ^ DBUS);
                        break;
                    }
                case 8://ASL
                    {
                        RBUS = (short)(DBUS << 1);
                        break;
                    }
                case 9://ASR
                    {
                        RBUS = (short)(DBUS >> 1);
                        break;
                    }
                case 10://LSR
                    {
                        RBUS = (short)(DBUS >> 1);
                        break;
                    }
                case 11://ROL
                    {
                        short msb = (short)DBUS;
                        msb = (short)(msb & 0x8000);
                        RBUS = DBUS;
                        RBUS = (short)(RBUS << 1);
                        RBUS = (short)(RBUS | (short)(msb >> 15));
                        break;
                    }
                case 12://ROR
                    {
                        short lsb = (short)DBUS;
                        lsb = (short)(lsb & 0x0001);
                        RBUS = DBUS;
                        RBUS = (short)(RBUS >> 1);
                        RBUS = (short)(RBUS | (short)(lsb << 15));
                        break;
                    }
                case 13://RLC
                    {
                        short msb = (short)DBUS;
                        msb = (short)(msb & 0x8000);
                        short rez = DBUS;
                        rez = (short)(rez << 1);
                        rez = (short)(rez | RegFlags.C);
                        rez = (short)(rez | (msb >> 16));
                        RBUS = rez;
                        break;
                    }
                case 14://RRC
                    {
                        short lsb = (short)DBUS;
                        lsb = (short)(lsb & 0x0001);
                        short rez = DBUS;
                        rez = (short)(rez >> 1);
                        rez = (short)(rez | RegFlags.C);
                        rez = (short)(rez | (lsb << 16));
                        RBUS = rez;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public static void destRBUS(long microinstructiune)
        {
            long Rbus = ((microinstructiune & 0x000F00000) >> 20);
            switch (Rbus)
            {
                case 0://none
                    {
                        break;
                    }
                case 1://PmFLAG
                    {
                        RegFlags.FLAGS = RBUS;
                        break;
                    }
                case 2://PmFLAG[3-0]
                    {
                        RegFlags.FLAGS = (short)(RBUS & 0x000F);
                        break;
                    }
                case 3://PmRG
                    {
                        RegFlags.RG[regD] = RBUS;
                        break;
                    }
                case 4://PmSP
                    {
                        RegFlags.SP = RBUS;
                        break;
                    }
                case 5://PmT
                    {
                        RegFlags.T = RBUS;
                        break;
                    }
                case 6://PmPC
                    {
                        RegFlags.PC = RBUS;
                        break;
                    }
                case 7://PmIVR
                    {
                        RegFlags.IVR = RBUS;
                        break;
                    }
                case 8://PmADR
                    {
                        RegFlags.ADR = RBUS;
                        break;
                    }
                case 9://PmMDR
                    {
                        RegFlags.MDR = RBUS;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public static void exOthers(long microinstructiune)
        {
            long others = ((microinstructiune & 0x00003C000) >> 14);
            switch (others)
            {
                case 0://none
                    {
                        break;
                    }
                case 1://+2SP
                    {
                        RegFlags.SP += 2;
                        break;
                    }
                case 2://-2SP
                    {
                        RegFlags.SP -= 2;
                        break;
                    }
                case 3://+2PC
                    {
                        RegFlags.PC += 2;
                        break;
                    }
                case 4://BE0->1
                    {
                        RegFlags.BE0 = 1;
                        break;
                    }
                case 5://BE1->1
                    {
                        RegFlags.BE1 = 1;
                        break;
                    }
                case 6://PdCONDa
                    {
                        break;
                    }
                case 7://Cin,PdCOND
                    {
                        break;
                    }
                case 8://PdCONDl
                    {
                        break;
                    }
                case 9://BVI->1
                    {
                        RegFlags.BVI = 1;
                        break;
                    }
                case 10://BVI->0
                    {
                        RegFlags.BVI = 0;
                        break;
                    }
                case 11://BPO->0
                    {
                        RegFlags.BPO = 0;
                        break;
                    }
                case 12://INTa,-2SP
                    {
                        //intrerupere
                        RegFlags.SP -= 2;
                        break;
                    }
                case 13:
                    {
                        RegFlags.BE0 = 0;
                        RegFlags.BE1 = 0;
                        RegFlags.BI = 0;
                        break;
                    }
            }
        }

        public static void salt(long microinstructiune)
        {
            long successor = ((microinstructiune & 0x000003800) >> 11);
            switch (successor)
            {
                case 0://STEP
                    {
                        F = 0;
                        break;
                    }
                case 1://JUMP
                    {
                        F = 1;
                        break;
                    }
                case 2://IFACLOW
                    {
                        F = 0;
                        break;
                    }
                case 3://IFCIL
                    {
                        F = 0;
                        break;
                    }
                case 4://IFC
                    {
                        if (RegFlags.C == 1)
                            F = 1;
                        else
                            F = 0;
                        break;
                    }
                case 5://IFZ
                    {
                        if (RegFlags.Z == 1)
                            F = 1;
                        else
                            F = 0;
                        break;
                    }
                case 6://IFS
                    {
                        if (RegFlags.S == 1)
                            F = 1;
                        else
                            F = 0;
                        break;
                    }
                case 7://IFZ
                    {
                        if (RegFlags.Z == 1)
                            F = 1;
                        else
                            F = 0;
                        break;
                    }
            }
            G = (short)(F ^ TF);
        }

        public static void setTF(long microinstructiune)
        {
            TF = (short)((microinstructiune & 0x000000080) >> 7);
        }

        public static void setIndex(long microinstructiune)
        {
            long mascaIndex = ((microinstructiune & 0x000000700) >> 8);
            switch (mascaIndex)
            {
                case 0:
                    {
                        index = 0;
                        break;
                    }
                case 1:
                    {
                        index = getClasaInstructiune(instrCurenta);
                        break;
                    }
                case 2:
                    {
                        index = modAdresareSursa(instrCurenta);
                        break;
                    }
                case 3:
                    {
                        index = modAdresareDestinatie(instrCurenta);
                        break;
                    }
                case 4:
                    {
                        index = opcodeB1(instrCurenta);
                        break;
                    }
                case 5:
                    {
                        index = opcodeB2(instrCurenta);
                        break;
                    }
                case 6:
                    {
                        index = opcodeB3(instrCurenta);
                        break;
                    }
                case 7:
                    {
                        index = opcodeB4(instrCurenta);
                        break;
                    }
            }
        }

        public static void memOp(long microinstructiune)
        {
            long mascaMEM = ((MIR & 0x0000C0000) >> 18);
            switch (mascaMEM)
            {
                case 0://NONE
                    {
                        break;
                    }
                case 1://IFCH
                    {
                        RegFlags.IR = MEM[RegFlags.ADR];
                        instrCurenta = RegFlags.IR;
                        setRsRd(instrCurenta);
                        break;
                    }
                case 2://RD
                    {
                        RegFlags.MDR = MEM[RegFlags.ADR];
                        break;
                    }
                case 3:
                    {
                        MEM[RegFlags.ADR] = RegFlags.MDR;
                        break;
                    }
            }
        }
    }
}
