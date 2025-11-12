using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography;
using System.Reflection;

namespace CardUpdater
{

    class CardComm
    {
        public delegate void MyEventDelegate(char Cmd, byte[] Data, int Length);

        static byte[]   TxData = new byte[512];
        static byte[]   RxData = new byte[512];
        static byte     PcdBlock = 0x03;
        static bool     RxFlag;
        static int      RxLen;
        static byte     ResultCode;
        static byte     CurrentAuthKey;

        static byte[]   InitVector = new byte[16];
        static byte[]   SessionKey = new byte[16];
        static byte[]   SubKeyK1 = new byte[16];
        static byte[]   SubKeyK2 = new byte[16];
        static byte[]   InitVectorTX = new byte[16];
        static byte[]   InitVectorRX = new byte[16];

        public static event MyEventDelegate MyEvent;

        static bool USB_READER = true;

        static Random rnd = new Random();


        private static void MyEventWrapper(char Cmd, byte[] Data, int Length)
        {
            if (MyEvent != null)
            {
                MyEvent(Cmd, Data, Length);
            }
        }

        public static byte InsertBlock()
        {
            if (PcdBlock == 0x02)
                PcdBlock = 0x03;
            else
                PcdBlock = 0x02;

            return PcdBlock;
        }

        //TODO
        public static bool GetApplicationIDs()
        {
            bool RetVal;

            RetVal = UsbReader.TranceiveData(0x6A, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;

        }

        public static bool SelectApplication(byte[] AID)
        {
            bool RetVal;

            TxData[0] = AID[0];
            TxData[1] = AID[1];
            TxData[2] = AID[2];

            RetVal = UsbReader.TranceiveData(0x5A, TxData, 0, 3, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;

        }


        public static bool GetFreeMemory(ref int size)
        {
            bool RetVal;
            
            RetVal = UsbReader.TranceiveData(0x6E, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
            {
                size = RxData[2];
                size <<= 8;
                size += RxData[1];
                size <<= 8;
                size += RxData[0];

                return true;
            }
            else
                return false;

        }

        public static bool StartSessionAES(byte[] Key, byte KeyNumber)
        {
            bool RetVal = false;
            byte[] RndA = new byte[16];
            byte[] RndB = new byte[16];
            byte[] BlockData = new byte[32];
            int Cnt;

            for (Cnt = 0; Cnt < 16; Cnt++)
                InitVector[Cnt] = 0;

            TxData[0] = KeyNumber;
            RetVal = UsbReader.TranceiveData(0xAA, TxData, 0, 1, ref RxData, ref RxLen, ref ResultCode);
               
            if (RetVal == true && ResultCode == 0xAF)
            {
                for (Cnt = 0; Cnt < 16; Cnt++)
                {
                    RndB[Cnt] = RxData[Cnt];
                }

                AesDecrypt(ref RndB, 0, Key, ref InitVector);

                for (Cnt = 0; Cnt < 16; Cnt++)
                {
                    RndA[Cnt] = (byte)rnd.Next(255);
                        // .Next(255); // 0x01;                           //Fill array with random values
                }

                Array.Copy(RndA, 0, BlockData, 0, 16);          //Copy RndA to Blockdata
                Array.Copy(RndB, 1, BlockData, 16, 15);         //Copy RndB to Blockdata
                BlockData[31] = RndB[0];

                AesEncrypt(ref BlockData, 0, Key, ref InitVector);
                AesEncrypt(ref BlockData, 16, Key, ref InitVector);

                RetVal = UsbReader.TranceiveData(0xAF, BlockData, 0, 32, ref RxData, ref RxLen, ref ResultCode);

                if (RetVal == true && ResultCode == 0x00)
                {

                    for (Cnt = 0; Cnt < 16; Cnt++)
                    {
                        BlockData[Cnt] = RxData[Cnt];                           //Fill array with random values
                    }

                    AesDecrypt(ref BlockData, 0, Key, ref InitVector);

                    if (BlockData[15] == RndA[0] &&
                        BlockData[0] == RndA[1] &&
                        BlockData[1] == RndA[2] &&
                        BlockData[2] == RndA[3] &&
                        BlockData[3] == RndA[4] &&
                        BlockData[4] == RndA[5] &&
                        BlockData[5] == RndA[6] &&
                        BlockData[6] == RndA[7] &&
                        BlockData[7] == RndA[8] &&
                        BlockData[8] == RndA[9] &&
                        BlockData[9] == RndA[10] &&
                        BlockData[10] == RndA[11] &&
                        BlockData[11] == RndA[12] &&
                        BlockData[12] == RndA[13] &&
                        BlockData[13] == RndA[14] &&
                        BlockData[14] == RndA[15])
                    {
                        SessionKey[0] = RndA[0];
                        SessionKey[1] = RndA[1];
                        SessionKey[2] = RndA[2];
                        SessionKey[3] = RndA[3];
                        SessionKey[4] = RndB[0];
                        SessionKey[5] = RndB[1];
                        SessionKey[6] = RndB[2];
                        SessionKey[7] = RndB[3];
                        SessionKey[8] = RndA[12];
                        SessionKey[9] = RndA[13];
                        SessionKey[10] = RndA[14];
                        SessionKey[11] = RndA[15];
                        SessionKey[12] = RndB[12];
                        SessionKey[13] = RndB[13];
                        SessionKey[14] = RndB[14];
                        SessionKey[15] = RndB[15];

                        for (Cnt = 0; Cnt < 16; Cnt++)
                            InitVector[Cnt] = 0;

                        DoCalcCMACsubkeys(ref SubKeyK1, ref SubKeyK2, SessionKey);

                        CurrentAuthKey = KeyNumber; 

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool StartSessionNativeTDES(byte[] Key, byte KeyNumber)
        {
            bool RetVal = false;
            byte[] DesBlock = new byte[8];
            byte[] RandRx = new byte[8];
            byte[] RndA = new byte[16];
            byte[] RndB = new byte[16];
            byte[] BlockData = new byte[32];
            int Cnt;

            TxData[0] = KeyNumber;          //Key number
            RetVal = UsbReader.TranceiveData(0x0A, TxData, 0, 1, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0xAF)
            {
                Array.Clear(BlockData, 0, 32);

                Array.Copy(RxData, 0, DesBlock, 0, 8);

                TDESNativeDecryptionCBC(ref DesBlock, 0, 8, Key);

                Array.Copy(DesBlock, RndB, 8);
                Array.Copy(RndB, 1, BlockData, 8, 7); //Src index, dest index, length 
                Array.Copy(RndB, 0, BlockData, 15, 1);

                RndA[0] = 0x11;
                RndA[1] = 0x12;
                RndA[2] = 0x13;
                RndA[3] = 0x14;
                RndA[4] = 0x15;
                RndA[5] = 0x16;
                RndA[6] = 0x17;
                RndA[7] = 0x18;

                Array.Copy(RndA, BlockData, 8);

                TDESNativeEncryptionCBC(ref BlockData, 0, 16, Key);

                /*
                BlockData[7] = 0x18;
                BlockData[8] = 0x76;
                BlockData[9] = 0x3a;
                BlockData[10] = 0x68;
                BlockData[11] = 0x44;
                BlockData[12] = 0xfd;
                BlockData[13] = 0x42;
                BlockData[14] = 0xc8;
                BlockData[15] = 0xa0;
                */

                //TDESNativeEncryptionCBC(ref BlockData, 0, 16, Key);
                //TDESNativeDecryptionCBC(ref BlockData, 0, 16, Key);

                //TDESNativeDecryptionCBC(ref BlockData, 0, 16, Key);

                //TDESNativeEncryptionCBC(ref BlockData, 0, 8, Key);
                /*
                for (Cnt = 0; Cnt < 8; Cnt++)
                {
                    //BlockData[Cnt + 8] = BlockData[Cnt] ^ BlockData[Cnt + 8];
                    BlockData[Cnt + 8] ^= BlockData[Cnt];
                }
                */

                //TDESNativeDecryptionCBC(ref BlockData, 8, 8, Key);

                RetVal = UsbReader.TranceiveData(0xAF, BlockData, 0, 16, ref RxData, ref RxLen, ref ResultCode);

                if (RetVal == true && ResultCode == 0x00)
                {
                    /*******************************************
                        * AUTH PART 2
                    *******************************************/
                    Array.Copy(RxData, 0, DesBlock, 0, 8);
                    TDESNativeDecryptionCBC(ref DesBlock, 0, 8, Key);

                    RandRx[0] = DesBlock[7];
                    RandRx[1] = DesBlock[0];
                    RandRx[2] = DesBlock[1];
                    RandRx[3] = DesBlock[2];
                    RandRx[4] = DesBlock[3];
                    RandRx[5] = DesBlock[4];
                    RandRx[6] = DesBlock[5];
                    RandRx[7] = DesBlock[6];

                    if ((RandRx[0] == RndA[0]) &&
                        (RandRx[1] == RndA[1]) &&
                        (RandRx[2] == RndA[2]) &&
                        (RandRx[3] == RndA[3]) &&
                        (RandRx[4] == RndA[4]) &&
                        (RandRx[5] == RndA[5]) &&
                        (RandRx[6] == RndA[6]) &&
                        (RandRx[7] == RndA[7]))
                    {
                        //Check if single DES. Single DES is true when first eight bytes are equal to eight last bytes.
                        if( Key[0] == Key[8] &&
                            Key[1] == Key[9] &&
                            Key[2] == Key[10] &&
                            Key[3] == Key[11] &&
                            Key[4] == Key[12] &&
                            Key[5] == Key[13] &&
                            Key[6] == Key[14] &&
                            Key[7] == Key[15])
                        {
                            Array.Copy(RndA, 0, SessionKey, 0, 4);
                            Array.Copy(RndB, 0, SessionKey, 4, 4);
                            Array.Copy(RndA, 0, SessionKey, 8, 4);
                            Array.Copy(RndB, 0, SessionKey, 12, 4);
                        }
                        else
                        {
                            Array.Copy(RndA, 0, SessionKey, 0, 4);
                            Array.Copy(RndB, 0, SessionKey, 4, 4);
                            Array.Copy(RndA, 4, SessionKey, 8, 4);
                            Array.Copy(RndB, 4, SessionKey, 12, 4);
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }                       
        }

        //DES/2K3DES --> AES
        public static bool ChangePiccMasterKeyNativeTDES(byte[] NewKey)
        {
            bool RetVal = false;
            byte[] Temp = new byte[32];
            UInt16 CrcA;

            Array.Copy(NewKey, 0, Temp, 0, 16);
            Temp[16] = 00;                          //Key version

            CrcA = CalculateCrc16(Temp, 0, 16);     //Wrong endianess

            Array.Copy(NewKey, 0, Temp, 0, 16);
            Temp[16] = (byte)(CrcA >> 0);                          //KeyVersion
            Temp[17] = (byte)(CrcA >> 8);
            Temp[18] = 0x00;
            Temp[19] = 0x00;
            Temp[20] = 0x00;
            Temp[21] = 0x00;
            Temp[22] = 0x00;
            Temp[23] = 0x00;

            TDESNativeEncryptionCBC(ref Temp, 0, 24, SessionKey);

            TxData[0] = 0x80;   //0x00 16 byte des, 0x40 24 byte DES, 0x80 AES
            Array.Copy(Temp, 0, TxData, 1, 24);

            RetVal = UsbReader.TranceiveData(0xC4, TxData, 0, 25, ref RxData, ref RxLen, ref ResultCode);

            if(RetVal == true && ResultCode == 0x00)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }


        //NOT TESTED NOT TESTED NOT TESTED NOT TESTED NOT TESTED NOT TESTED NOT TESTED
        public static bool ChangePiccMasterKeyStandardTDES()
        {
            bool RetVal = false;
            byte[] Temp = new byte[32];
            byte[] NewKey = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            UInt32 CrcA, CrcB;
            byte[] Prev = new byte[8];
            int Cnt;

            Temp[0] = 0xC4;
            Temp[1] = 0x80;     //Key type
            Array.Copy(NewKey, 0, Temp, 2, 16);
            Temp[18] = 00;      //Key version

            CrcA = CalculateCrc32(Temp, 0, 19);     //Wrong endianess

            Array.Copy(NewKey, 0, Temp, 0, 16);
            Temp[16] = 00;                          //KeyVersion
            Temp[17] = (byte)(CrcA >> 0);
            Temp[18] = (byte)(CrcA >> 8);
            Temp[19] = (byte)(CrcA >> 16);
            Temp[20] = (byte)(CrcA >> 24);
            Temp[21] = 0x00;
            Temp[22] = 0x00;
            Temp[23] = 0x00;

            //SessionKey[0] = 0xda;
            //SessionKey[1] = 0xff;
            //SessionKey[2] = 0x37;
            //SessionKey[3] = 0x8c;
            //SessionKey[4] = 0x02;
            //SessionKey[5] = 0x46;
            //SessionKey[6] = 0x10;
            //SessionKey[7] = 0xfb;
            //SessionKey[8] = 0xda;
            //SessionKey[9] = 0xff;
            //SessionKey[10] = 0x37;
            //SessionKey[11] = 0x8c;
            //SessionKey[12] = 0x02;
            //SessionKey[13] = 0x46;
            //SessionKey[14] = 0x10;
            //SessionKey[15] = 0xfb;

            //TDESEncryptionCBC(ref Temp, 0, SessionKey, ref InitVector);
            //TDESEncryptionCBC(ref Temp, 8, SessionKey, ref InitVector);
            //TDESEncryptionCBC(ref Temp, 16, SessionKey, ref InitVector);

            TxData[0] = 0x80;
            Array.Copy(Temp, 0, TxData, 1, 24);

            RetVal = UsbReader.TranceiveData(0xC4, TxData, 0, 25, ref RxData, ref RxLen, ref ResultCode);

            return true;
        }



        public static bool ChangeKeyAes(byte KeyNrChange, byte KeyVersion, byte[] CurrentKey, byte[] NewKey)
        {
            int Cnt;
            byte[] KeysXor = new byte[16];
            byte[] Temp = new byte[32];
            byte[] TxData = new byte[33];
            byte[] RxData = new byte[16];
            int RxLen=0;
            byte Result=0;
            UInt32 CrcA, CrcB;
            bool RetVal;
            string Error;

            //Two modes of operation. Depending on keynumber used in session
            if (CurrentAuthKey == KeyNrChange)
            {
                Temp[0] = 0xC4;
                Temp[1] = KeyNrChange;
                Array.Copy(NewKey, 0, Temp, 2, 16);
                Temp[18] = KeyVersion;

                CrcA = CalculateCrc32(Temp, 0, 19);     //Wrong endianess

                Array.Copy(NewKey, 0, Temp, 0, 16);
                Temp[16] = KeyVersion;
                Temp[17] = (byte)(CrcA >> 0);
                Temp[18] = (byte)(CrcA >> 8);
                Temp[19] = (byte)(CrcA >> 16);
                Temp[20] = (byte)(CrcA >> 24);
                Temp[21] = 0x00;
                Temp[22] = 0x00;
                Temp[23] = 0x00;
                Temp[24] = 0x00;
                Temp[25] = 0x00;   
                Temp[26] = 0x00;
                Temp[27] = 0x00;
                Temp[28] = 0x00;
                Temp[29] = 0x00;
                Temp[30] = 0x00;
                Temp[31] = 0x00;

                for (Cnt = 0; Cnt < 16; Cnt++)
                    InitVector[Cnt] = 0;

                AesEncrypt(ref Temp, 0, SessionKey, ref InitVector);
                AesEncrypt(ref Temp, 16, SessionKey, ref InitVector);

                TxData[0] = KeyNrChange;
                Array.Copy(Temp, 0, TxData, 1, 32);

                RetVal = UsbReader.TranceiveData(0xC4, TxData, 0, 33, ref RxData, ref RxLen, ref Result);

                if (Result == 0)
                    return true;                
            }
            else
            {
                for (Cnt = 0; Cnt < 16; Cnt++)
                {
                    KeysXor[Cnt] = (byte)(CurrentKey[Cnt] ^ NewKey[Cnt]);
                }

                Temp[0] = 0xC4;     //Command
                Temp[1] = KeyNrChange;     //Key number to be changed
                Array.Copy(KeysXor, 0, Temp, 2, 16);
                Temp[18] = KeyVersion;    //Key version

                CrcA = CalculateCrc32(Temp, 0, 19);     //Wrong endianess
                CrcB = CalculateCrc32(NewKey, 0, 16);   //Wrong endianess

                Array.Copy(KeysXor, 0, Temp, 0, 16);
                Temp[16] = KeyVersion;    //Key version
                Temp[17] = (byte)(CrcA >> 0);
                Temp[18] = (byte)(CrcA >> 8);
                Temp[19] = (byte)(CrcA >> 16);
                Temp[20] = (byte)(CrcA >> 24);
                Temp[21] = (byte)(CrcB >> 0);
                Temp[22] = (byte)(CrcB >> 8);
                Temp[23] = (byte)(CrcB >> 16);
                Temp[24] = (byte)(CrcB >> 24);
                Temp[25] = 0x00;    //padding
                Temp[26] = 0x00;
                Temp[27] = 0x00;
                Temp[28] = 0x00;
                Temp[29] = 0x00;
                Temp[30] = 0x00;
                Temp[31] = 0x00;

                AesEncrypt(ref Temp, 0, SessionKey, ref InitVector);
                AesEncrypt(ref Temp, 16, SessionKey, ref InitVector);

                TxData[0] = KeyNrChange;
                Array.Copy(Temp, 0, TxData, 1, 32);

                RetVal = UsbReader.TranceiveData(0xC4, TxData, 0, 33, ref RxData, ref RxLen, ref Result);

                //CMAC is returned, for now it is not used so IV is invalid and session has to be setup again.

                if (Result == 0)
                    return true;
            }

            return false;
        }

        //From AES to AES
        public static bool ChangePICCMasterKeyAes(byte KeyVersion, byte[] CurrentKey, byte[] NewKey)
        {
            int Cnt;
            byte[] KeysXor = new byte[16];
            byte[] Temp = new byte[32];
            byte[] TxData = new byte[33];
            byte[] RxData = new byte[16];
            int RxLen = 0;
            byte Result = 0;
            UInt32 CrcA;
            bool RetVal;
            byte KeyNr = 0x80;      //0x80 = AES128

            Temp[0] = 0xC4;
            Temp[1] = KeyNr;     
            Array.Copy(NewKey, 0, Temp, 2, 16);
            Temp[18] = KeyVersion;

            CrcA = CalculateCrc32(Temp, 0, 19);     //Wrong endianess

            Array.Copy(NewKey, 0, Temp, 0, 16);
            Temp[16] = KeyVersion;
            Temp[17] = (byte)(CrcA >> 0);
            Temp[18] = (byte)(CrcA >> 8);
            Temp[19] = (byte)(CrcA >> 16);
            Temp[20] = (byte)(CrcA >> 24);
            Temp[21] = 0x00;
            Temp[22] = 0x00;
            Temp[23] = 0x00;
            Temp[24] = 0x00;
            Temp[25] = 0x00;
            Temp[26] = 0x00;
            Temp[27] = 0x00;
            Temp[28] = 0x00;
            Temp[29] = 0x00;
            Temp[30] = 0x00;
            Temp[31] = 0x00;

            for (Cnt = 0; Cnt < 16; Cnt++)
                InitVector[Cnt] = 0;

            AesEncrypt(ref Temp, 0, SessionKey, ref InitVector);
            AesEncrypt(ref Temp, 16, SessionKey, ref InitVector);

            TxData[0] = KeyNr;
            Array.Copy(Temp, 0, TxData, 1, 32);

            RetVal = UsbReader.TranceiveData(0xC4, TxData, 0, 33, ref RxData, ref RxLen, ref Result);

            if (Result == 0)
                return true;
            
            return false;
        }
        public static bool GetUID_TDES(ref byte[] Data)
        {
            bool RetVal;
            byte[] RxMsg = new byte[8192];
            int Cnt, WritePtr, DecryptPtr, NrTDesBlocks;
            UInt32 RxCRC, CalcCRC;

            Array.Clear(RxData, 0, 512);

            RetVal = UsbReader.TranceiveData(0x51, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);


            TDESNativeDecryptionCBC(ref RxData, 0, 16, SessionKey);

            Array.Copy(RxData, 0, Data, 0, 7); // Kopieert 2 bytes vanaf index 1 naar index 2 van destination



            //todo crc check



            return true;
        }

        public static bool ReadDataFile_TDES(int FileNumber, int FileOffset, int FileLength, ref byte[] Data)
        {
            bool RetVal;
            byte[] RxMsg = new byte[8192];
            int Cnt, WritePtr, DecryptPtr, NrTDesBlocks;
            UInt32 RxCRC, CalcCRC;

            TxData[2] = 0xBD;                                   //Command
            TxData[3] = (byte)FileNumber;                       //File number
            TxData[4] = (byte)(FileOffset & 0xFF);              //File offset
            TxData[5] = (byte)((FileOffset >> 8) & 0xFF);
            TxData[6] = (byte)((FileOffset >> 16) & 0xFF);
            TxData[7] = (byte)(FileLength & 0xFF);              //File length
            TxData[8] = (byte)((FileLength >> 8) & 0xFF);
            TxData[9] = (byte)((FileLength >> 16) & 0xFF);

            Array.Clear(RxData, 0, 512);

            RetVal = UsbReader.TranceiveData(0xBD, TxData, 3, 7, ref RxData, ref RxLen, ref ResultCode);

            WritePtr = 0;
            Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
            WritePtr += RxLen;

            while (true)
            {
                if (ResultCode == 0xAF)
                {
                    RetVal = UsbReader.TranceiveData(0xAF, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);
                    Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
                    WritePtr += RxLen;
                }
                else
                {
                    break;
                }

            }

            if (ResultCode == 0x00 && WritePtr % 8 == 0)
            {
                TDESNativeDecryptionCBC(ref RxMsg, 0, WritePtr, SessionKey);
            }

            if (FileLength == 0)
            {
                int n = RxLen;
                while ((RxMsg[n] == 0x00) && (n > 0)) n--;  //skip trailing zeros
                if (RxMsg[n] == 0x80) n--;                  //skip padding character
                FileLength = n - 1;
            }


            RxCRC = RxMsg[FileLength + 1];
            RxCRC <<= 8;
            RxCRC += RxMsg[FileLength];

            RxMsg[FileLength] = ResultCode;

            CalcCRC = CalculateCrc16(RxMsg, 0, FileLength);

            if (CalcCRC == RxCRC)
            {
                Array.Copy(RxMsg, 0, Data, 0, FileLength);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ReadDataFile(int FileNumber, int FileOffset, int FileLength, ref byte[] Data)
        {
            bool RetVal;
            byte[] RxMsg = new byte[8192];
            int Cnt, WritePtr, DecryptPtr, NrAesBlocks;
            UInt32 RxCRC, CalcCRC;

            TxData[2] = 0xBD;                                   //Command
            TxData[3] = (byte)FileNumber;                       //File number
            TxData[4] = (byte)(FileOffset & 0xFF);              //File offset
            TxData[5] = (byte)((FileOffset >> 8) & 0xFF);
            TxData[6] = (byte)((FileOffset >> 16) & 0xFF);
            TxData[7] = (byte)(FileLength & 0xFF);              //File length
            TxData[8] = (byte)((FileLength >> 8) & 0xFF);
            TxData[9] = (byte)((FileLength >> 16) & 0xFF);

            DoCMAC(ref InitVector, TxData, 2, 8, true, SessionKey, SubKeyK1, SubKeyK2);

            Array.Clear(RxData, 0, 512);

            RetVal = UsbReader.TranceiveData(0xBD, TxData, 3, 7, ref RxData, ref RxLen, ref ResultCode);

            WritePtr = 0;
            Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
            WritePtr += RxLen;

            while (true)
            {
                if (ResultCode == 0xAF)
                {
                    RetVal = UsbReader.TranceiveData(0xAF, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);
                    Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
                    WritePtr += RxLen;
                }
                else
                {
                    break;
                }

            }

            if (ResultCode == 0x00 && WritePtr % 16 == 0)
            {
                NrAesBlocks = WritePtr / 16;
                DecryptPtr = 0;
                for (Cnt = 0; Cnt < NrAesBlocks; Cnt++)
                {
                    AesDecrypt(ref RxMsg, DecryptPtr, SessionKey, ref InitVector);
                    DecryptPtr += 16;
                }
            }


/*            if ((FileLength != 0) && (FileLength < RxLen))
            {
                //FileLength = FileLength + 4;                //add CRC length
            }
            else*/
            if (FileLength == 0)
            {
                int n = RxLen;
                while ((RxMsg[n] == 0x00) && (n > 0)) n--;  //skip trailing zeros
                if (RxMsg[n] == 0x80) n--;                  //skip padding character
                FileLength = n - 3;
            }

            if (FileLength < 0) return false;               //fout bij het lezen van de datafile

            RxCRC = RxMsg[FileLength + 3];
            RxCRC <<= 8;
            RxCRC += RxMsg[FileLength + 2];
            RxCRC <<= 8;
            RxCRC += RxMsg[FileLength + 1];
            RxCRC <<= 8;
            RxCRC += RxMsg[FileLength];

            RxMsg[FileLength] = ResultCode;

            CalcCRC = CalculateCrc32(RxMsg, 0, FileLength + 1);

            if (CalcCRC == RxCRC)
            {
                Array.Copy(RxMsg, 0, Data, 0, FileLength);
                return true;
            }
            else
            {
                return false;
            }            
        }

        public static bool GetUID(ref byte[] UID)
        {
            bool RetVal;
            UInt32 RxCRC, CalcCRC;
            int FileLength;

            TxData[0] = 0x51;                                   //Command
            DoCMAC(ref InitVector, TxData, 0, 1, true, SessionKey, SubKeyK1, SubKeyK2);


            Array.Clear(RxData, 0, 512);
            //  CARD_GETUID             = $51;
            RetVal = UsbReader.TranceiveData(0x51, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
            {
                AesDecrypt(ref RxData, 0, SessionKey, ref InitVector);

                int n = RxLen-1;
                while ((RxData[n] == 0x00) && (n > 0)) n--;  //skip trailing zeros
                //if (RxData[n] == 0x80) n--;                  //skip padding character
                FileLength = n - 3;
                if ((FileLength > 4) && (FileLength < 7)) FileLength = 7;
                
                RxCRC = RxData[FileLength + 3];
                RxCRC <<= 8;
                RxCRC += RxData[FileLength + 2];
                RxCRC <<= 8;
                RxCRC += RxData[FileLength + 1];
                RxCRC <<= 8;
                RxCRC += RxData[FileLength];

                RxData[FileLength] = ResultCode;

                CalcCRC = CalculateCrc32(RxData, 0, FileLength + 1);

                if (CalcCRC == RxCRC)
                {
                    Array.Copy(RxData, 0, UID, 0, FileLength);
                    return true;
                }
                else
                {
                    return false;
                }


            }
            else
                return false;

        }

        public static bool SetRandomUID()
        {
            bool RetVal;
            UInt32 TxCRC;

            Array.Clear(TxData, 0, 32);
            TxData[0] = 0x5C;                                   //Command SET_CONFIGURATION = $5C;
            TxData[1] = 0x00;                                   //Option
            TxData[2] = 0x02;                                   //RandomID enable, !!!cannot be reset!!!
            TxCRC = CalculateCrc32(TxData, 0, 3);
            TxData[3] = (byte)TxCRC;
            TxData[4] = (byte)(TxCRC >> 8);
            TxData[5] = (byte)(TxCRC >> 16);
            TxData[6] = (byte)(TxCRC >> 24);

            AesEncrypt(ref TxData, 2, SessionKey, ref InitVector);

            //DoCMAC(ref InitVector, TxData, 2, 6, true, SessionKey, SubKeyK1, SubKeyK2);

            Array.Clear(RxData, 0, 512);
            RetVal = UsbReader.TranceiveData(0x5C, TxData, 1, 17, ref RxData, ref RxLen, ref ResultCode);
            if (RetVal == true && ResultCode == 0x00) return true;
            else return false;
        }

        //  _CmdFormatPICC             = $FC;
        //PICC master keysettings and key are unaffected by format
        public static bool FormatCard()
        {
            bool RetVal;

            RetVal = UsbReader.TranceiveData(0xFC, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;
        }

        public static bool CreateApplication(byte[] AID)
        {
            return CreateApplication(AID, 0x0B, 3);
        }
        public static bool CreateApplication(byte[] AID, byte KeySettings, byte NumberOfKeysOnCard)
        {
            bool RetVal = false;

            TxData[0] = AID[0];                 //AID
            TxData[1] = AID[1];
            TxData[2] = AID[2];
            TxData[3] = KeySettings;            //Key settings  Page 41 from document 134036_MF3ICD81 MIFARE DESFire EV1_3.6
            //TxData[4] = 0x00;                   //DES encryption
            TxData[4] = 0x80;                   //AES encryption
            TxData[4] += NumberOfKeysOnCard;    //0-3 = define # of keys.  

            RetVal = UsbReader.TranceiveData(0xCA, TxData, 0, 5, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;
        }

        public static bool DeleteApplication(byte[] AID)
        {
            bool RetVal = false;

            TxData[0] = AID[0];             //AID
            TxData[1] = AID[1];
            TxData[2] = AID[2];

            RetVal = UsbReader.TranceiveData(0xDA, TxData, 0, 3, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;
        }


        //Define keynumber to the following four access types
        //0 = APP master key
        //1-13 = user key
        //14 = free access
        //15 = never access
        public static bool CreateStandardPlainDataFile(int FileNr, int FileLength, byte ChangeKeyNr,
                                                                                byte ReadWriteKeyNr,
                                                                                byte ReadKeyNr,
                                                                                byte WriteKeyNr)
        {
            bool RetVal;

            TxData[0] = (byte)FileNr;                   //FileNr
            TxData[1] = 0x00;                           //Fully enciphered 0x03 or plain data 0x00

            TxData[2] = ReadWriteKeyNr;
            TxData[2] <<= 4;
            TxData[2] += ChangeKeyNr;

            TxData[3] = ReadKeyNr;
            TxData[3] <<= 4;
            TxData[3] += WriteKeyNr;

            TxData[4] = (byte)FileLength;               //Filelength LSB        32
            TxData[5] = (byte)(FileLength >> 8);        //Filelength 
            TxData[6] = (byte)(FileLength >> 16);       //Filelength MSB

            RetVal = UsbReader.TranceiveData(0xCD, TxData, 0, 7, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;

        }

        //Define keynumber to the following four access types
        //0 = APP master key
        //1-13 = user key
        //14 = free access
        //15 = never access
        public static bool CreateStandardDataFile(int FileNr, int FileLength,   byte ChangeKeyNr, 
                                                                                byte ReadWriteKeyNr,
                                                                                byte ReadKeyNr,
                                                                                byte WriteKeyNr)
        {
            bool RetVal;

            TxData[0] = (byte)FileNr;                   //FileNr
            TxData[1] = 0x03;                           //Fully enciphered 0x03 or plain data 0x00

            TxData[2] = ReadWriteKeyNr;
            TxData[2] <<= 4;
            TxData[2] += ChangeKeyNr;

            TxData[3] = ReadKeyNr;
            TxData[3] <<= 4;
            TxData[3] += WriteKeyNr; 

            TxData[4] = (byte)FileLength;               //Filelength LSB        32
            TxData[5] = (byte)(FileLength >> 8);        //Filelength 
            TxData[6] = (byte)(FileLength >> 16);       //Filelength MSB

            RetVal = UsbReader.TranceiveData(0xCD, TxData, 0, 7, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;
     
        }

        public static bool DeleteFile(int FileNr)
        {
            bool RetVal;

            TxData[0] = (byte)FileNr;                   //FileNr

            RetVal = UsbReader.TranceiveData(0xDF, TxData, 0, 1, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;
        }


        //Define keynumber to the following four access types
        //0 = APP master key
        //1-13 = user key
        //14 = free access
        //15 = never access
        public static bool CreateStandardDataFileISO(int FileNr, int FileLength, byte ChangeKeyNr,
                                                                                byte ReadWriteKeyNr,
                                                                                byte ReadKeyNr,
                                                                                byte WriteKeyNr)
        {
            bool RetVal;

            TxData[0] = (byte)FileNr;                   //FileNr

            TxData[1] = 0;
            TxData[2] = (byte)FileNr;


            TxData[3] = 0x03;                           //Fully enciphered 0x03 or plain data 0x00

            TxData[4] = ReadWriteKeyNr;
            TxData[4] <<= 4;
            TxData[4] += ChangeKeyNr;

            TxData[5] = ReadKeyNr;
            TxData[5] <<= 4;
            TxData[5] += WriteKeyNr;

            TxData[6] = (byte)FileLength;               //Filelength LSB        32
            TxData[7] = (byte)(FileLength >> 8);        //Filelength 
            TxData[8] = (byte)(FileLength >> 16);       //Filelength MSB

            RetVal = UsbReader.TranceiveData(0xCD, TxData, 0, 9, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;

        }

        //Define keynumber to the following four access types
        //0 = APP master key
        //1-13 = user key
        //14 = free access
        //15 = never access
        public static bool CreateBackupDataFile(int FileNr, int FileLength, byte ChangeKeyNr,
                                                                            byte ReadWriteKeyNr,
                                                                            byte ReadKeyNr,
                                                                            byte WriteKeyNr)
        {
            bool RetVal;

            TxData[0] = (byte)FileNr;                   //FileNr
            TxData[1] = 0x03;                           //Fully enciphered 0x03 or plain data 0x00

            TxData[2] = ReadWriteKeyNr;
            TxData[2] <<= 4;
            TxData[2] += ChangeKeyNr;

            TxData[3] = ReadKeyNr;
            TxData[3] <<= 4;
            TxData[3] += WriteKeyNr;

            TxData[4] = (byte)FileLength;               //Filelength LSB        32
            TxData[5] = (byte)(FileLength >> 8);        //Filelength 
            TxData[6] = (byte)(FileLength >> 16);       //Filelength MSB

            RetVal = UsbReader.TranceiveData(0xCB, TxData, 0, 7, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
                return true;
            else
                return false;

        }


        public static bool WriteFile(int FileNr, int Offset, int Length, byte[] Data)
        {
            bool RetVal;
            int Cnt;
            int EncryptPointer;
            int NrAesBlocks, NrPadding;
            byte[]  Msg     = new byte[8192];
            byte[]  TxMsg   = new byte[100];
            int MsgPtr, ReadPtr, Remaining;
            UInt32 TxCRC;

            MsgPtr = 0;

            Msg[MsgPtr++] = 0x3D;
            Msg[MsgPtr++] = (byte)FileNr;                       //FileNr
            Msg[MsgPtr++] = (byte)Offset;                       //Offset LSB
            Msg[MsgPtr++] = (byte)(Offset >> 8);                //Offset
            Msg[MsgPtr++] = (byte)(Offset >> 16);               //Offset MSB
            Msg[MsgPtr++] = (byte)Length;                       //Length LSB        
            Msg[MsgPtr++] = (byte)(Length >> 8); ;              //Length 
            Msg[MsgPtr++] = (byte)(Length >> 16); ;             //Length MSB    

            for (Cnt = 0; Cnt < Length; Cnt++)
                Msg[MsgPtr++] = Data[Cnt];

            TxCRC = CalculateCrc32(Msg, 0, MsgPtr);

            Msg[MsgPtr++] = (byte)TxCRC;
            Msg[MsgPtr++] = (byte)(TxCRC >> 8);
            Msg[MsgPtr++] = (byte)(TxCRC >> 16);
            Msg[MsgPtr++] = (byte)(TxCRC >> 24);

            NrPadding = ((MsgPtr - 8) % 16);    //Calculate remaining bytes
            if (NrPadding > 0)
                NrPadding = 16 - NrPadding;

            for (Cnt = 0; Cnt < NrPadding; Cnt++)
                Msg[MsgPtr++] = 0;

            NrAesBlocks = (MsgPtr - 8) / 16;
            EncryptPointer = 8;

            for (Cnt = 0; Cnt < NrAesBlocks; Cnt++)
            {
                AesEncrypt(ref Msg, EncryptPointer, SessionKey, ref InitVector);
                EncryptPointer += 16;
            }

            ReadPtr = 1;
            if (NrAesBlocks <= 2)
            {
                RetVal = UsbReader.TranceiveData(0x3D, Msg, ReadPtr, (byte)(NrAesBlocks * 16 + 7), ref RxData, ref RxLen, ref ResultCode);
                if(RetVal == true && ResultCode == 0x00)
                {
                    RxData[0] = ResultCode;     //use for temporary storage
                    DoCMAC(ref InitVector, RxData, 0, 1, true, SessionKey, SubKeyK1, SubKeyK2);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                RetVal = UsbReader.TranceiveData(0x3D, Msg, ReadPtr, 39, ref RxData, ref RxLen, ref ResultCode);
                if (RetVal == true && ResultCode == 0xAF)
                {
                    ReadPtr += 39;
                    Remaining = MsgPtr - ReadPtr;

                    while (true)
                    {
                        if (Remaining <= 48)
                        {
                            RetVal = UsbReader.TranceiveData(0xAF, Msg, ReadPtr, (byte)Remaining, ref RxData, ref RxLen, ref ResultCode);
                            if (RetVal == true && ResultCode == 0x00)
                            {
                                RxData[0] = ResultCode;     //use for temporary storage
                                DoCMAC(ref InitVector, RxData, 0, 1, true, SessionKey, SubKeyK1, SubKeyK2);
                                return true;
                            }
                            else
                            {
                                return false;                
                            }
                        }
                        else
                        {
                            RetVal = UsbReader.TranceiveData(0xAF, Msg, ReadPtr, 48, ref RxData, ref RxLen, ref ResultCode);
                            if (RetVal == true && ResultCode == 0xAF)
                            {
                                ReadPtr += 48;
                                Remaining -= 48;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    return false;   
                }
            }          
        }

        public static bool CommitTransaction()
        {
            bool RetVal;
            byte[] Data = new byte[10];   
                          
            RetVal = UsbReader.TranceiveData(0xC7, Data, 0, 0, ref Data, ref RxLen, ref ResultCode);
            
            if (RetVal == true && ResultCode == 0x00)
            {
                RxData[0] = ResultCode;     //use for temporary storage
                DoCMAC(ref InitVector, RxData, 0, 1, true, SessionKey, SubKeyK1, SubKeyK2);
                return true;
            }
            else
            {
                return false;
            }        
        }

        public static bool GetFileSettings(byte FileNr, ref byte FileType, ref byte CommType, ref ushort AccessRights, ref int FileSize)
        {
            bool RetVal;
            byte[] TxData = new byte[1];
            byte[] RxData = new byte[32];

            TxData[0] = FileNr;

            DoCMAC(ref InitVector, TxData, 0, 1, true, SessionKey, SubKeyK1, SubKeyK2);

            RetVal = UsbReader.TranceiveData(0xF5, TxData, 0, 1, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0x00)
            {
                FileType = RxData[0];

                CommType = RxData[1];

                AccessRights = RxData[3];
                AccessRights <<= 8;
                AccessRights += RxData[2];

                FileSize = RxData[6];
                FileSize <<= 8;
                FileSize += RxData[5];
                FileSize <<= 8;
                FileSize += RxData[4];	

                RxData[7] = ResultCode;     //use for temporary storage
                DoCMAC(ref InitVector, RxData, 0, 8, true, SessionKey, SubKeyK1, SubKeyK2);

                return true;
            }
            else
            {
                return false;
            }
        }

/*
        public static bool StartSessionEV2First(byte[] Key, byte KeyNumber)
        {
            bool RetVal = false;
            byte[] RndA = new byte[16];
            byte[] RndB = new byte[16];
            byte[] BlockData = new byte[32];
            int Cnt;

            for (Cnt = 0; Cnt < 16; Cnt++)
            {
                InitVector[Cnt] = 0;
                InitVectorTX[Cnt] = 0;
                InitVectorRX[Cnt] = 0;
            }

            TxData[0] = KeyNumber;
            TxData[1] = 0;  //LenCap
            RetVal = UsbReader.TranceiveData(0x71, TxData, 0, 2, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0xAF)
            {
                //for (Cnt = 0; Cnt < 16; Cnt++)
                //{
                //    RndB[Cnt] = RxData[Cnt];
                //}
                Array.Copy(RxData, 0, RndB, 0, 16);       
                AesDecrypt(ref RndB, 0, Key, ref InitVector);

                for (Cnt = 0; Cnt < 16; Cnt++)
                {
                    RndA[Cnt] = 0x01; // (byte)rnd.Next(255);   //Fill array with random values
                }

                Array.Copy(RndA, 0, BlockData, 0, 16);          //Copy RndA to Blockdata
                Array.Copy(RndB, 1, BlockData, 16, 15);         //Copy RndB to Blockdata
                BlockData[31] = RndB[0];

                AesEncrypt(ref BlockData, 0, Key, ref InitVector);
                AesEncrypt(ref BlockData, 16, Key, ref InitVector);

                RetVal = UsbReader.TranceiveData(0xAF, BlockData, 0, 32, ref RxData, ref RxLen, ref ResultCode);

                if (RetVal == true && ResultCode == 0x00)
                {

                    //for (Cnt = 0; Cnt < 32; Cnt++)
                    //{
                    //    BlockData[Cnt] = RxData[Cnt];                           //Fill array with random values
                    //}
                    Array.Copy(RxData, BlockData, 32);


                    AesEncrypt(ref BlockData, 0, Key, ref InitVector);
                    AesEncrypt(ref BlockData, 16, Key, ref InitVector);






                    if (BlockData[15] == RndA[0] &&
                        BlockData[0] == RndA[1] &&
                        BlockData[1] == RndA[2] &&
                        BlockData[2] == RndA[3] &&
                        BlockData[3] == RndA[4] &&
                        BlockData[4] == RndA[5] &&
                        BlockData[5] == RndA[6] &&
                        BlockData[6] == RndA[7] &&
                        BlockData[7] == RndA[8] &&
                        BlockData[8] == RndA[9] &&
                        BlockData[9] == RndA[10] &&
                        BlockData[10] == RndA[11] &&
                        BlockData[11] == RndA[12] &&
                        BlockData[12] == RndA[13] &&
                        BlockData[13] == RndA[14] &&
                        BlockData[14] == RndA[15])
                    {
                        SessionKey[0] = RndA[0];
                        SessionKey[1] = RndA[1];
                        SessionKey[2] = RndA[2];
                        SessionKey[3] = RndA[3];
                        SessionKey[4] = RndB[0];
                        SessionKey[5] = RndB[1];
                        SessionKey[6] = RndB[2];
                        SessionKey[7] = RndB[3];
                        SessionKey[8] = RndA[12];
                        SessionKey[9] = RndA[13];
                        SessionKey[10] = RndA[14];
                        SessionKey[11] = RndA[15];
                        SessionKey[12] = RndB[12];
                        SessionKey[13] = RndB[13];
                        SessionKey[14] = RndB[14];
                        SessionKey[15] = RndB[15];

                        for (Cnt = 0; Cnt < 16; Cnt++)
                            InitVector[Cnt] = 0;

                        DoCalcCMACsubkeys(ref SubKeyK1, ref SubKeyK2, SessionKey);

                        CurrentAuthKey = KeyNumber;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        */

        public static void AesDecrypt(ref byte[] DataBlock, int Offset, byte[] Key, ref byte[] IV)
        {
            // Declare the string used to hold the decrypted text. 
            int Cnt;
            byte[] TempBlock = new byte[16];

            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Mode = CipherMode.ECB;

                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 128;
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Key = Key;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                decryptor.TransformBlock(DataBlock, Offset, 16, TempBlock, 0);
            }

            for (Cnt = 0; Cnt < 16; Cnt++)
            {
                TempBlock[Cnt] ^= IV[Cnt];
            }

            Array.Copy(DataBlock, Offset, IV, 0, 16);
            Array.Copy(TempBlock, 0, DataBlock, Offset, 16);
        }


        public static void AesEncrypt(ref byte[] DataBlock, int Offset, byte[] Key, ref byte[] IV)
        {
            int Cnt;
            byte[] TempBlock = new byte[16];

            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Mode = CipherMode.ECB;

                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 128;
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Key = Key;

                for (Cnt = 0; Cnt < 16; Cnt++)
                {
                    TempBlock[Cnt] = (byte)(DataBlock[Cnt + Offset] ^ IV[Cnt]);
                }

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                encryptor.TransformBlock(TempBlock, 0, 16, DataBlock, Offset);

                Array.Copy(DataBlock, Offset, IV, 0, 16);

            }
        }

        public static void DoCalcCMACsubkeys(ref byte[] cmacK1, ref byte[] cmacK2, byte[] KeyEncrypt)
        {
            byte[] Block = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] TempIV = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            int j;
            bool b, r, s;

            AesEncrypt(ref Block, 0, KeyEncrypt, ref TempIV);

            b = (Block[0] & 0x80) == 0x80;

            r = false;
            for (j = 15; j >= 0; j--)
            {
                s = (Block[j] & 0x80) == 0x80;      //msbit bewaren
                Block[j] *= 2;
                if (r) Block[j] |= 0x01;
                r = s;
            }

            Array.Copy(Block, cmacK1, 16);

            if (b)
            {
                for (j = 0; j <= 14; j++) cmacK1[j] ^= 0x00;	//xor 0x00
                cmacK1[15] ^= 0x87;							//xor 0x87
            }

            Array.Copy(cmacK1, Block, 16);

            b = (Block[0] & 0x80) == 0x80;
            r = false;
            for (j = 15; j >= 0; j--)
            {
                s = (Block[j] & 0x80) == 0x80;      //msbit bewaren
                Block[j] *= 2;
                if (r) Block[j] |= 0x01;					//or 0x01
                r = s;
            }

            Array.Copy(Block, cmacK2, 16);
            if (b)
            {
                for (j = 0; j <= 14; j++) cmacK2[j] ^= 0x00;	//xor 0x00
                cmacK2[15] ^= 0x87;							//xor 0x87
            }
        }


        public static void DoCMAC(ref byte[] InitVector,
                        byte[] InData,
                        int Start,
                        int Len,
                        bool LastBlock,
                        byte[] Key,
                        byte[] cmacK1,
                        byte[] cmacK2)
        {
            byte[] Block = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int j;

            if (Len > 16)
                Len = 16;						//lengte maximaal 16 bytes

            if (Len > 0)
                Array.Copy(InData, Start, Block, 0, Len);

            if (Len < 16)
            {
                Block[Len] = 0x80;
                if (LastBlock) for (j = 0; j < 16; j++) Block[j] ^= cmacK2[j];
            }
            else
            {
                if (LastBlock) for (j = 0; j < 16; j++) Block[j] ^= cmacK1[j];
            }

            //IV: gebruik voorgaande waarde
            AesEncrypt(ref Block, 0, Key, ref InitVector);

            Array.Copy(Block, InitVector, 16);
        }


        /************************************************************************
        There is also a Standard TDES crypto version. Refer to Mifare Desfire EV1 - Features and Hint document
        This version differs in IV handling and encrypt/decrypt useage.

        Native crypto always uses the decrypt functions to perform encrypt and decrypt.
        Code block chaining with IV only used on single crypt operation.  
        ************************************************************************/
        public static void TDESNativeEncryptionCBC(ref byte[] DataBlock, int Offset, int Length, byte[] Key)
        {
            byte[] tmpIv = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] IV = new byte[8];
            byte[] Result = new byte[8];
            int Cnt;
            int DataPtr;

            if (Length % 8 == 0)
            {
                DataPtr = Offset;
                for (Cnt = 0; Cnt < 8; Cnt++)
                    IV[Cnt] = 0x00;

                while (Length > 0)
                {
                    TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                    des.KeySize = 128;
                    des.Padding = PaddingMode.None;
                    ICryptoTransform ct = des.CreateWeakDecryptor(Key, tmpIv);

                    for (Cnt = 0; Cnt < 8; Cnt++)
                    {
                        DataBlock[Offset + Cnt] = (byte)(DataBlock[Offset + Cnt] ^ IV[Cnt]);
                    }

                    Result = ct.TransformFinalBlock(DataBlock, Offset, 8);
                    Array.Copy(Result, 0, IV, 0, 8);
                    Array.Copy(Result, 0, DataBlock, Offset, 8);

                    Offset += 8;
                    Length -= 8;
                }
            }
        }

        /************************************************************************
        There is also a Standard TDES crypto version. Refer to Mifare Desfire EV1 - Features and Hint document
        This version differs in IV handling and encrypt/decrypt useage.

        Native crypto always uses the decrypt functions to perform encrypt and decrypt.
        Code block chaining with IV only used on single crypt operation.  
        ************************************************************************/
        public static void TDESNativeDecryptionCBC(ref byte[] DataBlock, int Offset, int Length, byte[] Key)
        {
            byte[] IV = new byte[8];
            byte[] Result = new byte[8];
            int Cnt;
            int DataPtr;

            Array.Clear(IV, 0, 8);
            if (Length % 8 == 0)
            {
                DataPtr = Offset;

                while (Length > 0)
                {
                    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                    tdes.KeySize = 128;
                    tdes.Padding = PaddingMode.None;
                    tdes.Mode = CipherMode.ECB;
                    try
                    {
                        ICryptoTransform Decryptor = tdes.CreateWeakDecryptor(Key, IV);
                        Result = Decryptor.TransformFinalBlock(DataBlock, Offset, 8);
                    }
                    finally
                    {
                        tdes.Clear();
                    }

                    for (Cnt = 0; Cnt < 8; Cnt++)
                    {
                        Result[Cnt] = (byte)(Result[Cnt] ^ IV[Cnt]);
                    }

                    Array.Copy(DataBlock, Offset, IV, 0, 8);
                    Array.Copy(Result, 0, DataBlock, Offset, 8);

                    Offset += 8;
                    Length -= 8;
                }
            }
        }





        //public static void TDESEncryption(ref byte[] Data, int Offset, byte[] Key)
        //{
        //    byte[] tmpIv = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //    byte[] Result = new byte[8];

        //    TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
        //    des.KeySize = 128;
        //    des.Padding = PaddingMode.None;
        //    ICryptoTransform ct = des.CreateWeakEncryptor(Key, tmpIv);

        //    Result = ct.TransformFinalBlock(Data, Offset, 8);
        //    Array.Copy(Result, 0, Data, Offset, 8);
        //}

        //public static void TDESEncryptionCBC(ref byte[] Data, int Offset, byte[] Prev, byte[] Key)
        //{
        //    byte[]  tmpIv = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //    byte[]  Result = new byte[8];
        //    int Cnt;  

        //    for (Cnt = 0; Cnt < 8; Cnt++)
        //        Data[Cnt] = (byte)(Data[Cnt] ^ Prev[Cnt]);

        //    TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
        //    des.KeySize = 128;
        //    des.Padding = PaddingMode.None;
        //    ICryptoTransform ct = des.CreateWeakEncryptor(Key, tmpIv);

        //    Result = ct.TransformFinalBlock(Data, Offset, 8);
        //    Array.Copy(Result, 0, Data, Offset, 8);
        //}



        //public static void TDESEncryptionCBC(ref byte[] DataBlock, int Offset, byte[] Key, ref byte[] IV)
        //{
        //    byte[] tmpIv = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //    byte[] Result = new byte[8];
        //    int Cnt;

        //    TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
        //    des.KeySize = 128;
        //    des.Padding = PaddingMode.None;
        //    using (ICryptoTransform ct = des.CreateWeakDecryptor(Key, tmpIv))
        //    {
        //        for (Cnt = 0; Cnt < 8; Cnt++)
        //            DataBlock[Offset + Cnt] = (byte)(DataBlock[Offset + Cnt] ^ IV[Cnt]);

        //        Result = ct.TransformFinalBlock(DataBlock, Offset, 8);
        //        Array.Copy(Result, 0, IV, 0, 8);
        //        Array.Copy(Result, 0, DataBlock, Offset, 8);
        //    }
        //}

        //public static void TDESDecryption(ref byte[] Data, int Offset, byte[] Key)
        //{
        //    byte[] tmpIv = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //    byte[] Result = new byte[8];

        //    TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
        //    des.KeySize = 128;
        //    des.Padding = PaddingMode.None;
        //    ICryptoTransform ct = des.CreateWeakDecryptor(Key, tmpIv);
        //    Result =  ct.TransformFinalBlock(Data, Offset, 8);
        //    Array.Copy(Result, 0, Data, Offset, 8);
        //}

        //public static void TDESDecryptionCBC(ref byte[] DataBlock, int Offset, byte[] Key, ref byte[] IV)
        //{
        //    byte[] tmpIv = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //    byte[] Result = new byte[8];
        //    int Cnt;

        //    TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
        //    des.KeySize = 128;
        //    des.Padding = PaddingMode.None;
        //    ICryptoTransform ct = des.CreateWeakDecryptor(Key, tmpIv);

        //    Result = ct.TransformFinalBlock(DataBlock, Offset, 8);

        //    for (Cnt = 0; Cnt < 8; Cnt++)
        //        Result[Cnt] = (byte)(Result[Cnt] ^ IV[Cnt]);

        //    Array.Copy(DataBlock, Offset, IV, 0, 8);
        //    Array.Copy(Result, 0, DataBlock, Offset, 8);
        //}

        public static UInt32 CalculateCrc32(byte[] Data, int Offset, int Length)
        {
            UInt32 CrcResult;
            int ByteCnt;

            UInt32[] Crc32Table = {
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA,     
            0x076DC419, 0x706AF48F, 0xE963A535, 0x9E6495A3,     
            0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988,     
            0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,     
            0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE,
            0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
            0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC,
            0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5,
            0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172,
            0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
            0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940,
            0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
            0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116,
            0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F,
            0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
            0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D,
            0x76DC4190, 0x01DB7106, 0x98D220BC, 0xEFD5102A,
            0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
            0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818,
            0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
            0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E,
            0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457,
            0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C,
            0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
            0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2,
            0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB,
            0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0,
            0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9,
            0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086,
            0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
            0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4,
            0x59B33D17, 0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD,
            0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A,
            0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
            0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8,
            0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
            0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE,
            0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7,
            0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC,
            0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
            0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252,
            0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
            0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60,
            0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79,
            0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
            0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F,
            0xC5BA3BBE, 0xB2BD0B28, 0x2BB45A92, 0x5CB36A04,
            0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
            0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A,
            0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
            0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38,
            0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21,
            0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E,
            0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
            0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C,
            0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45,
            0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2,
            0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB,
            0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0,
            0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
            0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6,
            0xBAD03605, 0xCDD70693, 0x54DE5729, 0x23D967BF,
            0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94,
            0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D};

            CrcResult = 0xFFFFFFFF;

            for (ByteCnt=0; ByteCnt<Length; ByteCnt++)
            {
                CrcResult = ((CrcResult >> 8) ^ CrcTable(Data[Offset + ByteCnt] ^ (CrcResult & 0x000000FF)));
            }

            return CrcResult;
        }

        public static UInt32 CrcTable(UInt32 Index)
        {
            UInt32 Result;
            int Cnt;
            UInt32 Poly = 0xEDB88320; /* polynomial exclusive-or pattern */

            Result = Index;
            for (Cnt = 0; Cnt < 8; Cnt++)
            {
                if ((Result & 1) == 1)
                {
                    Result = Poly ^ (Result >> 1); 
                }
                else
                {
                    Result = Result >> 1; 
                }
            }
            return Result;
        }


        public static UInt16 CalculateCrc16(byte[] Data, int Offset, int Length)
        {
            UInt16 Result;
            byte FcsLsb;
            byte FcsMsb;
            byte Tmp;

            byte[] _Crc16TableL = {
	        0x00, 0x89, 0x12, 0x9b, 0x24, 0xad, 0x36, 0xbf,
	        0x48, 0xc1, 0x5a, 0xd3, 0x6c, 0xe5, 0x7e, 0xf7,
	        0x81, 0x08, 0x93, 0x1a, 0xa5, 0x2c, 0xb7, 0x3e,
	        0xc9, 0x40, 0xdb, 0x52, 0xed, 0x64, 0xff, 0x76,
	        0x02, 0x8b, 0x10, 0x99, 0x26, 0xaf, 0x34, 0xbd,
	        0x4a, 0xc3, 0x58, 0xd1, 0x6e, 0xe7, 0x7c, 0xf5,
	        0x83, 0x0a, 0x91, 0x18, 0xa7, 0x2e, 0xb5, 0x3c,
	        0xcb, 0x42, 0xd9, 0x50, 0xef, 0x66, 0xfd, 0x74,
	        0x04, 0x8d, 0x16, 0x9f, 0x20, 0xa9, 0x32, 0xbb,
	        0x4c, 0xc5, 0x5e, 0xd7, 0x68, 0xe1, 0x7a, 0xf3,
	        0x85, 0x0c, 0x97, 0x1e, 0xa1, 0x28, 0xb3, 0x3a,
	        0xcd, 0x44, 0xdf, 0x56, 0xe9, 0x60, 0xfb, 0x72,
	        0x06, 0x8f, 0x14, 0x9d, 0x22, 0xab, 0x30, 0xb9,
	        0x4e, 0xc7, 0x5c, 0xd5, 0x6a, 0xe3, 0x78, 0xf1,
	        0x87, 0x0e, 0x95, 0x1c, 0xa3, 0x2a, 0xb1, 0x38,
	        0xcf, 0x46, 0xdd, 0x54, 0xeb, 0x62, 0xf9, 0x70,
	        0x08, 0x81, 0x1a, 0x93, 0x2c, 0xa5, 0x3e, 0xb7,
	        0x40, 0xc9, 0x52, 0xdb, 0x64, 0xed, 0x76, 0xff,
	        0x89, 0x00, 0x9b, 0x12, 0xad, 0x24, 0xbf, 0x36,
	        0xc1, 0x48, 0xd3, 0x5a, 0xe5, 0x6c, 0xf7, 0x7e,
	        0x0a, 0x83, 0x18, 0x91, 0x2e, 0xa7, 0x3c, 0xb5,
	        0x42, 0xcb, 0x50, 0xd9, 0x66, 0xef, 0x74, 0xfd,
	        0x8b, 0x02, 0x99, 0x10, 0xaf, 0x26, 0xbd, 0x34,
	        0xc3, 0x4a, 0xd1, 0x58, 0xe7, 0x6e, 0xf5, 0x7c,
	        0x0c, 0x85, 0x1e, 0x97, 0x28, 0xa1, 0x3a, 0xb3,
	        0x44, 0xcd, 0x56, 0xdf, 0x60, 0xe9, 0x72, 0xfb,
	        0x8d, 0x04, 0x9f, 0x16, 0xa9, 0x20, 0xbb, 0x32,
	        0xc5, 0x4c, 0xd7, 0x5e, 0xe1, 0x68, 0xf3, 0x7a,
	        0x0e, 0x87, 0x1c, 0x95, 0x2a, 0xa3, 0x38, 0xb1,
	        0x46, 0xcf, 0x54, 0xdd, 0x62, 0xeb, 0x70, 0xf9,
	        0x8f, 0x06, 0x9d, 0x14, 0xab, 0x22, 0xb9, 0x30,
	        0xc7, 0x4e, 0xd5, 0x5c, 0xe3, 0x6a, 0xf1, 0x78};

            byte[] _Crc16TableM = {
	        0x00, 0x11, 0x23, 0x32, 0x46, 0x57, 0x65, 0x74,
	        0x8c, 0x9d, 0xaf, 0xbe, 0xca, 0xdb, 0xe9, 0xf8,
	        0x10, 0x01, 0x33, 0x22, 0x56, 0x47, 0x75, 0x64,
	        0x9c, 0x8d, 0xbf, 0xae, 0xda, 0xcb, 0xf9, 0xe8,
	        0x21, 0x30, 0x02, 0x13, 0x67, 0x76, 0x44, 0x55,
	        0xad, 0xbc, 0x8e, 0x9f, 0xeb, 0xfa, 0xc8, 0xd9,
	        0x31, 0x20, 0x12, 0x03, 0x77, 0x66, 0x54, 0x45,
	        0xbd, 0xac, 0x9e, 0x8f, 0xfb, 0xea, 0xd8, 0xc9,
	        0x42, 0x53, 0x61, 0x70, 0x04, 0x15, 0x27, 0x36,
	        0xce, 0xdf, 0xed, 0xfc, 0x88, 0x99, 0xab, 0xba,
	        0x52, 0x43, 0x71, 0x60, 0x14, 0x05, 0x37, 0x26,
	        0xde, 0xcf, 0xfd, 0xec, 0x98, 0x89, 0xbb, 0xaa,
	        0x63, 0x72, 0x40, 0x51, 0x25, 0x34, 0x06, 0x17,
	        0xef, 0xfe, 0xcc, 0xdd, 0xa9, 0xb8, 0x8a, 0x9b,
	        0x73, 0x62, 0x50, 0x41, 0x35, 0x24, 0x16, 0x07,
	        0xff, 0xee, 0xdc, 0xcd, 0xb9, 0xa8, 0x9a, 0x8b,
	        0x84, 0x95, 0xa7, 0xb6, 0xc2, 0xd3, 0xe1, 0xf0,
	        0x08, 0x19, 0x2b, 0x3a, 0x4e, 0x5f, 0x6d, 0x7c,
	        0x94, 0x85, 0xb7, 0xa6, 0xd2, 0xc3, 0xf1, 0xe0,
	        0x18, 0x09, 0x3b, 0x2a, 0x5e, 0x4f, 0x7d, 0x6c,
	        0xa5, 0xb4, 0x86, 0x97, 0xe3, 0xf2, 0xc0, 0xd1,
	        0x29, 0x38, 0x0a, 0x1b, 0x6f, 0x7e, 0x4c, 0x5d,
	        0xb5, 0xa4, 0x96, 0x87, 0xf3, 0xe2, 0xd0, 0xc1,
	        0x39, 0x28, 0x1a, 0x0b, 0x7f, 0x6e, 0x5c, 0x4d,
	        0xc6, 0xd7, 0xe5, 0xf4, 0x80, 0x91, 0xa3, 0xb2,
	        0x4a, 0x5b, 0x69, 0x78, 0x0c, 0x1d, 0x2f, 0x3e,
	        0xd6, 0xc7, 0xf5, 0xe4, 0x90, 0x81, 0xb3, 0xa2,
	        0x5a, 0x4b, 0x79, 0x68, 0x1c, 0x0d, 0x3f, 0x2e,
	        0xe7, 0xf6, 0xc4, 0xd5, 0xa1, 0xb0, 0x82, 0x93,
	        0x6b, 0x7a, 0x48, 0x59, 0x2d, 0x3c, 0x0e, 0x1f,
	        0xf7, 0xe6, 0xd4, 0xc5, 0xb1, 0xa0, 0x92, 0x83,
            0x7b, 0x6a, 0x58, 0x49, 0x3d, 0x2c, 0x1e, 0x0f};

            Result = 0x6363;

            for (int Cnt = 0; Cnt < Length; Cnt++)
            {
                FcsLsb = (byte)(Result % 256);
                FcsMsb = (byte)(Result / 256);

                Tmp =   (byte)(Data[Offset+Cnt] ^ FcsLsb);
                FcsLsb = (byte)(_Crc16TableL[Tmp] ^ FcsMsb);
                FcsMsb = (byte)(_Crc16TableM[Tmp]);

                Result = (UInt16)(FcsLsb + (256 * FcsMsb));
            }

            return Result;
        }


        public static string ErrorCode(int Value)
        {
            string[] ErrorList = new string[0xFF];

            ErrorList[0x00] = "OPERATION OK";
            ErrorList[0x0C] = "NO CHANGES";
            ErrorList[0x0E] = "OUT OF EEPROM";
            ErrorList[0x1C] = "ILLEGAL COMMAND CODE";
            ErrorList[0x1E] = "INTEGRITY ERROR";
            ErrorList[0x40] = "NO SUCK KEY";
            ErrorList[0x7E] = "LENGTH ERROR";
            ErrorList[0x9D] = "PERMISSION DENIED";
            ErrorList[0x9E] = "PARAMETER ERROR";
            ErrorList[0xA0] = "APPLICATION NOT FOUND";
            ErrorList[0xA1] = "APPLICATION INTEGRITY ERROR";
            ErrorList[0xAE] = "AUTHENTICATION ERROR";
            ErrorList[0xAF] = "ADDITIONAL FRAME";
            ErrorList[0xBE] = "BOUNDARY ERROR";
            ErrorList[0xC1] = "PICC INTEGRITY ERROR";
            ErrorList[0xCA] = "COMMAND ABORTED";
            ErrorList[0xCD] = "PICC DISABLED ERROR";
            ErrorList[0xCE] = "COUNT ERROR";
            ErrorList[0xDE] = "DUPLICATE ERROR";
            ErrorList[0xEE] = "EEPROM ERROR";
            ErrorList[0xF0] = "FILE NOT FOUND";
            ErrorList[0xF1] = "FILE INTEGRITY ERROR";

            return ErrorList[Value];
        }

        public static void ResetBlock()
        {
            PcdBlock = 0x03;
        }

        public static void ResetIV()
        {
            for (int Cnt = 0; Cnt < 16; Cnt++)
                InitVector[Cnt] = 0;
        }


        //CLASSIC functies:
        public static void LoadKey(byte KeyNr, byte[] KEY)
        {
            UsbReader.LoadKey(ref KeyNr, ref KEY);
        }
        public static bool Authenticate(byte KeyNr, char KeyType, byte Block)
        {
            return UsbReader.Authenticate(ref KeyNr, ref KeyType, ref Block);
        }
        public static bool WriteBlock(byte Block, byte[] DATA)
        {
            return UsbReader.ClassicWrite(ref Block, ref DATA);
        }
        public static bool ReadBlock(byte Block, ref byte[] DATA)
        {
            return UsbReader.ClassicRead(Block, ref DATA);
        }



        public static bool StartSessionEV2(byte[] Key, byte KeyNumber, ref byte[] SesAuthMACKey, ref byte[] SesAuthENCKey, ref byte[] TI, ref int CmdCntr)
        {
            bool RetVal = false;
            byte[] RndA = new byte[16];
            byte[] RndB = new byte[16];
            byte[] BlockData = new byte[50];
            byte[] Mix = new byte[50];
            int Cnt;

            byte[] PDCap2 = new byte[6];
            byte[] PCDCap2 = new byte[6];
            byte j;

            Array.Clear(InitVectorTX, 0, 16);
            Array.Clear(InitVectorRX, 0, 16);
            Array.Clear(InitVector, 0, 16);


            TxData[0] = KeyNumber;
            TxData[1] = 0;          //LenCap
            RetVal = UsbReader.TranceiveData(0x71, TxData, 0, 2, ref RxData, ref RxLen, ref ResultCode);

            if (RetVal == true && ResultCode == 0xAF)
            {
                for (Cnt = 0; Cnt < 16; Cnt++)
                {
                    RndB[Cnt] = RxData[Cnt];
                }

                AesDecrypt(ref RndB, 0, Key, ref InitVectorRX);

                for (Cnt = 0; Cnt < 16; Cnt++)
                {
                    RndA[Cnt] = (byte)rnd.Next(255);
                }

                Array.Copy(RndA, 0, BlockData, 0, 16);          //Copy RndA to Blockdata
                Array.Copy(RndB, 1, BlockData, 16, 15);         //Copy RndB to Blockdata
                BlockData[31] = RndB[0];

                AesEncrypt(ref BlockData, 0, Key, ref InitVectorTX);
                AesEncrypt(ref BlockData, 16, Key, ref InitVectorTX);

                RetVal = UsbReader.TranceiveData(0xAF, BlockData, 0, 32, ref RxData, ref RxLen, ref ResultCode);

                if (RetVal == true && ResultCode == 0x00)
                {
                    Array.Copy(RxData, BlockData, 32);          //Fill array with random values

                    AesDecrypt(ref BlockData, 0, Key, ref InitVector);
                    AesDecrypt(ref BlockData, 16, Key, ref InitVector);

                    if (BlockData[19] == RndA[0] &&
                        BlockData[4] == RndA[1] &&
                        BlockData[5] == RndA[2] &&
                        BlockData[6] == RndA[3] &&
                        BlockData[7] == RndA[4] &&
                        BlockData[8] == RndA[5] &&
                        BlockData[9] == RndA[6] &&
                        BlockData[10] == RndA[7] &&
                        BlockData[11] == RndA[8] &&
                        BlockData[12] == RndA[9] &&
                        BlockData[13] == RndA[10] &&
                        BlockData[14] == RndA[11] &&
                        BlockData[15] == RndA[12] &&
                        BlockData[16] == RndA[13] &&
                        BlockData[17] == RndA[14] &&
                        BlockData[18] == RndA[15])
                    {
                        Array.Copy(BlockData, 0, TI, 0, 4);
                        //BlockData[4..19] = RndA'
                        Array.Copy(BlockData, 20, PDCap2, 0, 6);
                        Array.Copy(BlockData, 26, PCDCap2, 0, 6);


                        /*
                        //AN363011 §4.3.3.1 p49
                        for (Cnt = 0; Cnt < 16; Cnt++) Key[Cnt] = 0x00;

                        RndA[0] = 0x87;
                        RndA[1] = 0x6D;
                        RndA[2] = 0x85;
                        RndA[3] = 0xB7;
                        RndA[4] = 0xFC;
                        RndA[5] = 0x71;
                        RndA[6] = 0x70;
                        RndA[7] = 0x73;
                        RndA[8] = 0xAF;
                        RndA[9] = 0xBF;
                        RndA[10] = 0x56;
                        RndA[11] = 0x48;
                        RndA[12] = 0x34;
                        RndA[13] = 0xF9;
                        RndA[14] = 0x8F;
                        RndA[15] = 0x1E;

                        RndB[0] = 0x6C;
                        RndB[1] = 0xCC;
                        RndB[2] = 0x83;
                        RndB[3] = 0xFC;
                        RndB[4] = 0xBF;
                        RndB[5] = 0x58;
                        RndB[6] = 0x2B;
                        RndB[7] = 0xA7;
                        RndB[8] = 0x7D;
                        RndB[9] = 0x10;
                        RndB[10] = 0xB2;
                        RndB[11] = 0x80;
                        RndB[12] = 0x25;
                        RndB[13] = 0xF2;
                        RndB[14] = 0x24;
                        RndB[15] = 0xE6;
                        */


                        //sessie key berekenen
                        Mix[0] = 0x5A;
                        Mix[1] = 0xA5;
                        Mix[2] = 0x00;
                        Mix[3] = 0x01;
                        Mix[4] = 0x00;
                        Mix[5] = 0x80;
                        Array.Copy(RndA, 0, Mix, 6, 2);
                        for (Cnt = 0; Cnt < 6; Cnt++) Mix[8 + Cnt] = (byte)(RndA[2 + Cnt] ^ RndB[Cnt]);
                        Array.Copy(RndB, 6, Mix, 14, 10);
                        Array.Copy(RndA, 8, Mix, 24, 8);

             
                        DoCalcCMACsubkeys(ref SubKeyK1, ref SubKeyK2, Key);

                        Array.Clear(InitVector, 0, 16);
                        DoCMAC(ref InitVector, Mix, 0, 16, false, Key, SubKeyK1, SubKeyK2);
                        DoCMAC(ref InitVector, Mix, 16, 16, true, Key, SubKeyK1, SubKeyK2);
                        Array.Copy(InitVector, 0, SesAuthMACKey, 0, 16);

                        //KeyID.SesAuthMACKey = 185D0E3CEA4C0C32DFAD84B3414A5054
                        

                        Mix[0] = 0xA5;
                        Mix[1] = 0x5A;

                        Array.Clear(InitVector, 0, 16);
                        DoCMAC(ref InitVector, Mix, 0, 16, false, Key, SubKeyK1, SubKeyK2);
                        DoCMAC(ref InitVector, Mix, 16, 16, true, Key, SubKeyK1, SubKeyK2);
                        Array.Copy(InitVector, 0, SesAuthENCKey, 0, 16);

                        //KeyID.SesAuthENCKey = 030A8C76BBC954513A52B2AAD161D15B


                        // --- EV2: ---
                        //IV = 0
                        //counter = 0

                        CmdCntr = 0;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /*
        public static bool ReadDataFileEV2_CMAC(int FileNumber, int FileOffset, int FileLength, ref byte[] Data, byte[] SesAuthMACKey, byte[] SesAuthENCKey, byte[] TI, int CmdCntr)
        {
            bool RetVal;
            byte[] RxMsg = new byte[8192];
            int Cnt, WritePtr, DecryptPtr, NrAesBlocks;
            UInt32 RxCRC, CalcCRC;
            //int CmdCntr = 0;

            byte[] BlockData = new byte[50];
            byte[] CMACdata = new byte[8];
            byte[] CMACcalc = new byte[8];

            / *
            //--AN363011 §4.3.6.2 p53
            //BD 0200 B04D6C11 02 000000 150000 | 98 87 18 42 F0 1D AB 5F

            //CMAC: BD 0200 B04D6C11 02 000000 150000
            //TX:   BD 02 000000 150000 98871842F01DAB5F
            CmdCntr = 0x0002;
            TI[0] = 0xB0;
            TI[1] = 0x4D;
            TI[2] = 0x6C;
            TI[3] = 0x11;
            FileNumber = 2;
            FileOffset = 0;
            FileLength = 0x15;

            //KeyID.SesAuthMACKey = 185D0E3CEA4C0C32DFAD84B3414A5054
            SesAuthMACKey[0] = 0x18;
            SesAuthMACKey[1] = 0x5D;
            SesAuthMACKey[2] = 0x0E;
            SesAuthMACKey[3] = 0x3C;
            SesAuthMACKey[4] = 0xEA;
            SesAuthMACKey[5] = 0x4C;
            SesAuthMACKey[6] = 0x0C;
            SesAuthMACKey[7] = 0x32;
            SesAuthMACKey[8] = 0xDF;
            SesAuthMACKey[9] = 0xAD;
            SesAuthMACKey[10] = 0x84;
            SesAuthMACKey[11] = 0xB3;
            SesAuthMACKey[12] = 0x41;
            SesAuthMACKey[13] = 0x4A;
            SesAuthMACKey[14] = 0x50;
            SesAuthMACKey[15] = 0x54;
            * /


            for (Cnt = 0; Cnt < 16; Cnt++) { InitVector[Cnt] = 0; }


            BlockData[0] = 0xBD;
            BlockData[1] = (byte)(CmdCntr & 0xFF);                 //current command counter (LSB first)
            BlockData[2] = (byte)((CmdCntr >> 8) & 0xFF);
            BlockData[3] = TI[0];              //TI
            BlockData[4] = TI[1];
            BlockData[5] = TI[2];
            BlockData[6] = TI[3];
            BlockData[7] = (byte)FileNumber;                       //File number
            BlockData[8] = (byte)(FileOffset & 0xFF);              //File offset
            BlockData[9] = (byte)((FileOffset >> 8) & 0xFF);
            BlockData[10] = (byte)((FileOffset >> 16) & 0xFF);
            BlockData[11] = (byte)(FileLength & 0xFF);             //File length
            BlockData[12] = (byte)((FileLength >> 8) & 0xFF);
            BlockData[13] = (byte)((FileLength >> 16) & 0xFF);

            DoCalcCMACsubkeys(ref SubKeyK1, ref SubKeyK2, SesAuthMACKey);
            DoCMAC(ref InitVector, BlockData, 0, 14, true, SesAuthMACKey, SubKeyK1, SubKeyK2);


            TxData[ 0] = 0xBD;                                   //Command
            TxData[ 1] = (byte)FileNumber;                       //File number
            TxData[ 2] = (byte)(FileOffset & 0xFF);              //File offset
            TxData[ 3] = (byte)((FileOffset >> 8) & 0xFF);
            TxData[ 4] = (byte)((FileOffset >> 16) & 0xFF);
            TxData[ 5] = (byte)(FileLength & 0xFF);              //File length
            TxData[ 6] = (byte)((FileLength >> 8) & 0xFF);
            TxData[ 7] = (byte)((FileLength >> 16) & 0xFF);

            TxData[ 8] = InitVector[1];
            TxData[ 9] = InitVector[3];
            TxData[10] = InitVector[5];
            TxData[11] = InitVector[7];
            TxData[12] = InitVector[9];
            TxData[13] = InitVector[11];
            TxData[14] = InitVector[13];
            TxData[15] = InitVector[15];


            CmdCntr++;


            Array.Clear(RxData, 0, 512);


            RetVal = UsbReader.TranceiveData(0xBD, TxData, 1, 15, ref RxData, ref RxLen, ref ResultCode);

            //hier op antwoord wachten....

            WritePtr = 0;
            Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
            WritePtr += RxLen;

            while (true)
            {
                if (ResultCode == 0xAF)
                {
                    RetVal = UsbReader.TranceiveData(0xAF, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);
                    Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
                    WritePtr += RxLen;
                }
                else
                {
                    break;
                }

            }


            if (ResultCode == 0x00)     //geldige data ontvangen?
            {
                RxLen = WritePtr - 8;       //bericht + CMAC[8]

                Array.Copy(RxMsg, RxLen, CMACdata, 0, 8);

                //IV response bepalen
                for (Cnt = 0; Cnt < 16; Cnt++) { BlockData[Cnt] = 0; }
                BlockData[0] = 0x5A;
                BlockData[1] = 0xA5;
                BlockData[2] = TI[0];              //TI
                BlockData[3] = TI[1];
                BlockData[4] = TI[2];
                BlockData[5] = TI[3];
                BlockData[6] = (byte)(CmdCntr & 0xFF);                 //current command counter (LSB first)
                BlockData[7] = (byte)((CmdCntr >> 8) & 0xFF);

                for (Cnt = 0; Cnt < 16; Cnt++) { InitVector[Cnt] = 0; }
                AesEncrypt(ref BlockData, 0, SesAuthENCKey, ref InitVector);

                Array.Copy(BlockData, InitVector, 16);

                Array.Copy(RxMsg, BlockData, 16);

                AesDecrypt(ref BlockData, 0, SesAuthENCKey, ref InitVector);

                if (FileLength == 0)
                {
                    int n = RxLen;
                    while ((BlockData[n] == 0x00) && (n > 0)) n--;  //skip trailing zeros
                    if (BlockData[n] == 0x80) n--;                  //skip padding character
                    FileLength = n+1; // n - 3;
                }
                Array.Copy(BlockData, Data, FileLength);


                //CMAC ontvangen data berekenen
                BlockData[0] = 0x00;
                BlockData[1] = (byte)(CmdCntr & 0xFF);                 //current command counter (LSB first)
                BlockData[2] = (byte)((CmdCntr >> 8) & 0xFF);
                BlockData[3] = TI[0];              //TI
                BlockData[4] = TI[1];
                BlockData[5] = TI[2];
                BlockData[6] = TI[3];
                Array.Copy(RxMsg, 0, BlockData, 7, RxLen);

                for (Cnt = 0; Cnt < 16; Cnt++) { InitVector[Cnt] = 0; }
                DoCalcCMACsubkeys(ref SubKeyK1, ref SubKeyK2, SesAuthMACKey);
                DoCMAC(ref InitVector, BlockData, 0, 16, false, SesAuthMACKey, SubKeyK1, SubKeyK2);
                DoCMAC(ref InitVector, BlockData, 16, RxLen+7-16, true, SesAuthMACKey, SubKeyK1, SubKeyK2);

                CMACcalc[0] = InitVector[1];
                CMACcalc[1] = InitVector[3];
                CMACcalc[2] = InitVector[5];
                CMACcalc[3] = InitVector[7];
                CMACcalc[4] = InitVector[9];
                CMACcalc[5] = InitVector[11];
                CMACcalc[6] = InitVector[13];
                CMACcalc[7] = InitVector[15];

                if ((CMACcalc[0] == CMACdata[0]) &&
                    (CMACcalc[1] == CMACdata[1]) &&
                    (CMACcalc[2] == CMACdata[2]) &&
                    (CMACcalc[3] == CMACdata[3]) &&
                    (CMACcalc[4] == CMACdata[4]) &&
                    (CMACcalc[5] == CMACdata[5]) &&
                    (CMACcalc[6] == CMACdata[6]) &&
                    (CMACcalc[7] == CMACdata[7]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else  //geen geldige data ontvangen
            {
                return false;
            }
        }
        */

        public static bool ReadDataFileEV2_FULL(int FileNumber, int FileOffset, int FileLength, ref byte[] Data, byte[] SesAuthMACKey, byte[] SesAuthENCKey, byte[] TI, ref int CmdCntr)
        {
            bool RetVal;
            byte[] RxMsg = new byte[8192];
            int Cnt, WritePtr, DecryptPtr, NrAesBlocks;
            UInt32 RxCRC, CalcCRC;
            //int CmdCntr = 0;

            byte[] BlockData = new byte[50];
            byte[] CMACdata = new byte[8];
            byte[] CMACcalc = new byte[8];

            /*
            //--AN363011 §4.3.6.2 p53
            //BD 0200 B04D6C11 02 000000 150000 | 98 87 18 42 F0 1D AB 5F

            //CMAC: BD 0200 B04D6C11 02 000000 150000
            //TX:   BD 02 000000 150000 98871842F01DAB5F
            CmdCntr = 0x0002;
            TI[0] = 0xB0;
            TI[1] = 0x4D;
            TI[2] = 0x6C;
            TI[3] = 0x11;
            FileNumber = 2;
            FileOffset = 0;
            FileLength = 0x15;

            //KeyID.SesAuthMACKey = 185D0E3CEA4C0C32DFAD84B3414A5054
            SesAuthMACKey[0] = 0x18;
            SesAuthMACKey[1] = 0x5D;
            SesAuthMACKey[2] = 0x0E;
            SesAuthMACKey[3] = 0x3C;
            SesAuthMACKey[4] = 0xEA;
            SesAuthMACKey[5] = 0x4C;
            SesAuthMACKey[6] = 0x0C;
            SesAuthMACKey[7] = 0x32;
            SesAuthMACKey[8] = 0xDF;
            SesAuthMACKey[9] = 0xAD;
            SesAuthMACKey[10] = 0x84;
            SesAuthMACKey[11] = 0xB3;
            SesAuthMACKey[12] = 0x41;
            SesAuthMACKey[13] = 0x4A;
            SesAuthMACKey[14] = 0x50;
            SesAuthMACKey[15] = 0x54;
            */


            Array.Clear(InitVector, 0, 16);

            BlockData[0] = 0xBD;
            BlockData[1] = (byte)(CmdCntr & 0xFF);                 //current command counter (LSB first)
            BlockData[2] = (byte)((CmdCntr >> 8) & 0xFF);
            BlockData[3] = TI[0];              //TI
            BlockData[4] = TI[1];
            BlockData[5] = TI[2];
            BlockData[6] = TI[3];
            BlockData[7] = (byte)FileNumber;                       //File number
            BlockData[8] = (byte)(FileOffset & 0xFF);              //File offset
            BlockData[9] = (byte)((FileOffset >> 8) & 0xFF);
            BlockData[10] = (byte)((FileOffset >> 16) & 0xFF);
            BlockData[11] = (byte)(FileLength & 0xFF);             //File length
            BlockData[12] = (byte)((FileLength >> 8) & 0xFF);
            BlockData[13] = (byte)((FileLength >> 16) & 0xFF);

            DoCalcCMACsubkeys(ref SubKeyK1, ref SubKeyK2, SesAuthMACKey);
            DoCMAC(ref InitVector, BlockData, 0, 14, true, SesAuthMACKey, SubKeyK1, SubKeyK2);


            TxData[0] = 0xBD;                                   //Command
            TxData[1] = (byte)FileNumber;                       //File number
            TxData[2] = (byte)(FileOffset & 0xFF);              //File offset
            TxData[3] = (byte)((FileOffset >> 8) & 0xFF);
            TxData[4] = (byte)((FileOffset >> 16) & 0xFF);
            TxData[5] = (byte)(FileLength & 0xFF);              //File length
            TxData[6] = (byte)((FileLength >> 8) & 0xFF);
            TxData[7] = (byte)((FileLength >> 16) & 0xFF);

            TxData[8] = InitVector[1];
            TxData[9] = InitVector[3];
            TxData[10] = InitVector[5];
            TxData[11] = InitVector[7];
            TxData[12] = InitVector[9];
            TxData[13] = InitVector[11];
            TxData[14] = InitVector[13];
            TxData[15] = InitVector[15];


            CmdCntr++;


            Array.Clear(RxData, 0, 512);


            RetVal = UsbReader.TranceiveData(0xBD, TxData, 1, 15, ref RxData, ref RxLen, ref ResultCode);

            //hier op antwoord wachten....

            WritePtr = 0;
            Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
            WritePtr += RxLen;

            while (true)
            {
                if (ResultCode == 0xAF)
                {
                    RetVal = UsbReader.TranceiveData(0xAF, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);
                    Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
                    WritePtr += RxLen;
                }
                else
                {
                    break;
                }

            }


            if (ResultCode == 0x00)     //geldige data ontvangen?
            {
                RxLen = WritePtr - 8;       //bericht + CMAC[8]

                Array.Copy(RxMsg, RxLen, CMACdata, 0, 8);

                //IV response bepalen
                for (Cnt = 0; Cnt < 16; Cnt++) { BlockData[Cnt] = 0; }
                BlockData[0] = 0x5A;
                BlockData[1] = 0xA5;
                BlockData[2] = TI[0];              //TI
                BlockData[3] = TI[1];
                BlockData[4] = TI[2];
                BlockData[5] = TI[3];
                BlockData[6] = (byte)(CmdCntr & 0xFF);                 //current command counter (LSB first)
                BlockData[7] = (byte)((CmdCntr >> 8) & 0xFF);

                for (Cnt = 0; Cnt < 16; Cnt++) { InitVector[Cnt] = 0; }
                AesEncrypt(ref BlockData, 0, SesAuthENCKey, ref InitVector);

                Array.Copy(BlockData, InitVector, 16);

                Array.Copy(RxMsg, BlockData, 16);

                AesDecrypt(ref BlockData, 0, SesAuthENCKey, ref InitVector);

                if (FileLength == 0)
                {
                    int n = RxLen;
                    while ((BlockData[n] == 0x00) && (n > 0)) n--;  //skip trailing zeros
                    if (BlockData[n] == 0x80) n--;                  //skip padding character
                    FileLength = n + 1; // n - 3;
                }
                Array.Copy(BlockData, Data, FileLength);


                //CMAC ontvangen data berekenen
                BlockData[0] = 0x00;
                BlockData[1] = (byte)(CmdCntr & 0xFF);                 //current command counter (LSB first)
                BlockData[2] = (byte)((CmdCntr >> 8) & 0xFF);
                BlockData[3] = TI[0];              //TI
                BlockData[4] = TI[1];
                BlockData[5] = TI[2];
                BlockData[6] = TI[3];
                Array.Copy(RxMsg, 0, BlockData, 7, RxLen);

                Array.Clear(InitVector, 0, 16);
                DoCalcCMACsubkeys(ref SubKeyK1, ref SubKeyK2, SesAuthMACKey);
                DoCMAC(ref InitVector, BlockData, 0, 16, false, SesAuthMACKey, SubKeyK1, SubKeyK2);
                DoCMAC(ref InitVector, BlockData, 16, RxLen + 7 - 16, true, SesAuthMACKey, SubKeyK1, SubKeyK2);

                CMACcalc[0] = InitVector[1];
                CMACcalc[1] = InitVector[3];
                CMACcalc[2] = InitVector[5];
                CMACcalc[3] = InitVector[7];
                CMACcalc[4] = InitVector[9];
                CMACcalc[5] = InitVector[11];
                CMACcalc[6] = InitVector[13];
                CMACcalc[7] = InitVector[15];

                /*
                if ((CMACcalc[0] == CMACdata[0]) &&
                    (CMACcalc[1] == CMACdata[1]) &&
                    (CMACcalc[2] == CMACdata[2]) &&
                    (CMACcalc[3] == CMACdata[3]) &&
                    (CMACcalc[4] == CMACdata[4]) &&
                    (CMACcalc[5] == CMACdata[5]) &&
                    (CMACcalc[6] == CMACdata[6]) &&
                    (CMACcalc[7] == CMACdata[7]))
                    */
                if (CMACcalc.SequenceEqual(CMACdata))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else  //geen geldige data ontvangen
            {
                return false;
            }

        }

        public static bool EV2_ProximityCheck(byte[] VCProximityKey)
        {
            bool RetVal;
            byte[] RxMsg = new byte[8192];
            int Cnt, WritePtr, DecryptPtr, NrAesBlocks;
            UInt32 RxCRC, CalcCRC;
            //int CmdCntr = 0;
            int j;

            bool result = false;
            byte[] BlockData = new byte[250];
            byte[] CMACtx = new byte[8];
            byte[] CMACdata = new byte[8];
            byte[] CMACcalc = new byte[8];
            int len;

            byte[] pRndC = new byte[8];
            int RndLen;

            Array.Clear(RxData, 0, 512);

            BlockData[0] = 0xFD;    //command
            len = 1;

            RetVal = UsbReader.TranceiveData(0xF0, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);
            if (ResultCode == 0x90)
            {
                Array.Copy(RxData, 0, BlockData, len, RxLen);
                len += RxLen;

                RndLen = 0;
                //for (Cnt = 0; Cnt < 4; Cnt++)
                do
                {
                    j = (byte)rnd.Next(7);
                    j++;
                    if ((RndLen + j) > 8) j = 8 - RndLen;

                    TxData[0] = (byte)j;
                    for (Cnt = 0; Cnt < j; Cnt++)
                    {
                        pRndC[Cnt] = (byte)rnd.Next(255);
                        TxData[Cnt + 1] = pRndC[Cnt];
                    }

                    Array.Clear(RxData, 0, 512);
                    RetVal = UsbReader.TranceiveData(0xF2, TxData, 0, (byte)(j+1), ref RxData, ref RxLen, ref ResultCode);

                    if (ResultCode == 0x90)
                    {
                        Array.Copy(RxData, 0, BlockData, len, RxLen);
                        len += RxLen;

                        Array.Copy(pRndC, 0, BlockData, len, j);
                        len += j;

                    }

                    RndLen += j;
                } while (RndLen < 8);


                Array.Clear(InitVector, 0, 16);

                DoCalcCMACsubkeys(ref SubKeyK1, ref SubKeyK2, VCProximityKey);

                BlockData[0] = 0xFD;    //command
                DoCMAC(ref InitVector, BlockData, 0, 16, false, VCProximityKey, SubKeyK1, SubKeyK2);
                DoCMAC(ref InitVector, BlockData, 16, 5, true, VCProximityKey, SubKeyK1, SubKeyK2);

                CMACtx[0] = InitVector[1];
                CMACtx[1] = InitVector[3];
                CMACtx[2] = InitVector[5];
                CMACtx[3] = InitVector[7];
                CMACtx[4] = InitVector[9];
                CMACtx[5] = InitVector[11];
                CMACtx[6] = InitVector[13];
                CMACtx[7] = InitVector[15];

                Array.Copy(CMACtx, TxData, 8);

                Array.Clear(RxData, 0, 512);
                RetVal = UsbReader.TranceiveData(0xFD, TxData, 0, 8, ref RxData, ref RxLen, ref ResultCode);

                if (ResultCode == 0x90)
                {
                    BlockData[0] = 0x90;    //command
                    DoCMAC(ref InitVector, BlockData, 0, 16, false, VCProximityKey, SubKeyK1, SubKeyK2);
                    DoCMAC(ref InitVector, BlockData, 16, 5, true, VCProximityKey, SubKeyK1, SubKeyK2);

                    CMACcalc[0] = InitVector[1];
                    CMACcalc[1] = InitVector[3];
                    CMACcalc[2] = InitVector[5];
                    CMACcalc[3] = InitVector[7];
                    CMACcalc[4] = InitVector[9];
                    CMACcalc[5] = InitVector[11];
                    CMACcalc[6] = InitVector[13];
                    CMACcalc[7] = InitVector[15];

                    Array.Copy(RxData, CMACdata, 8);

                    if (CMACcalc.SequenceEqual(CMACdata))
                    {
                        RetVal = true;
                    }

                }
                result = true;
            }

            return result;
        }


    }
}
