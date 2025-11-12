using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Timers;

namespace CardUpdater
{
    class UsbReader
    {
        // *********************************************************************************************************
        // Function Name: SCardEstablishContext
        // In Parameter : dwScope - Scope of the resource manager context.
        //                pvReserved1 - Reserved for future use and must be NULL
        //                pvReserved2 - Reserved for future use and must be NULL.
        // Out Parameter: phContext - A handle to the established resource manager context
        // Description  : Establishes context to the reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardEstablishContext(uint dwScope,
        IntPtr notUsed1,
        IntPtr notUsed2,
        out IntPtr phContext);


        // *********************************************************************************************************
        // Function Name: SCardReleaseContext
        // In Parameter : phContext - A handle to the established resource manager context              
        // Out Parameter: -------
        // Description  :Releases context from the reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardReleaseContext(IntPtr phContext);


        // *********************************************************************************************************
        // Function Name: SCardConnect
        // In Parameter : hContext - A handle that identifies the resource manager context.
        //                cReaderName  - The name of the reader that contains the target card.
        //                dwShareMode - A flag that indicates whether other applications may form connections to the card.
        //                dwPrefProtocol - A bitmask of acceptable protocols for the connection.  
        // Out Parameter: ActiveProtocol - A flag that indicates the established active protocol.
        //                hCard - A handle that identifies the connection to the smart card in the designated reader. 
        // Description  : Connect to card on reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardConnect(IntPtr hContext,
        string cReaderName,
        uint dwShareMode,
        uint dwPrefProtocol,
        ref IntPtr hCard,
        ref IntPtr ActiveProtocol);


        // *********************************************************************************************************
        // Function Name: SCardDisconnect
        // In Parameter : hCard - Reference value obtained from a previous call to SCardConnect.
        //                Disposition - Action to take on the card in the connected reader on close.  
        // Out(Parameter)
        // Description  : Disconnect card from reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardDisconnect(IntPtr hCard, int Disposition);


        //    *********************************************************************************************************
        // Function Name: SCardListReaders
        // In Parameter : hContext - A handle to the established resource manager context
        //                mszReaders - Multi-string that lists the card readers with in the supplied readers groups
        //                pcchReaders - length of the readerlist buffer in characters
        // Out Parameter: mzGroup - Names of the Reader groups defined to the System
        //                pcchReaders - length of the readerlist buffer in characters
        // Description  : List of all readers connected to system 
        //*********************************************************************************************************
        [DllImport("WinScard.dll", EntryPoint = "SCardListReadersA", CharSet = CharSet.Ansi)]
        public static extern int SCardListReaders(
          IntPtr hContext,
          byte[] mszGroups,
          byte[] mszReaders,
          ref UInt32 pcchReaders
          );


        // *********************************************************************************************************
        // Function Name: SCardState
        // In Parameter : hCard - Reference value obtained from a previous call to SCardConnect.
        // Out Parameter: state - Current state of smart card in  the reader
        //                protocol - Current Protocol
        //                ATR - 32 bytes buffer that receives the ATR string
        //                ATRLen - Supplies the length of ATR buffer
        // Description  : Current state of the smart card in the reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardState(IntPtr hCard, ref IntPtr state, ref IntPtr protocol, ref Byte[] ATR, ref int ATRLen);


        // *********************************************************************************************************
        // Function Name: SCardTransmit
        // In Parameter : hCard - A reference value returned from the SCardConnect function.
        //                pioSendRequest - A pointer to the protocol header structure for the instruction.
        //                SendBuff- A pointer to the actual data to be written to the card.
        //                SendBuffLen - The length, in bytes, of the pbSendBuffer parameter. 
        //                pioRecvRequest - Pointer to the protocol header structure for the instruction ,Pointer to the protocol header structure for the instruction, 
        //                followed by a buffer in which to receive any returned protocol control information (PCI) specific to the protocol in use.
        //                RecvBuffLen - Supplies the length, in bytes, of the pbRecvBuffer parameter and receives the actual number of bytes received from the smart card.
        // Out Parameter: pioRecvRequest - Pointer to the protocol header structure for the instruction ,Pointer to the protocol header structure for the instruction, 
        //                followed by a buffer in which to receive any returned protocol control information (PCI) specific to the protocol in use.
        //                RecvBuff - Pointer to any data returned from the card.
        //                RecvBuffLen - Supplies the length, in bytes, of the pbRecvBuffer parameter and receives the actual number of bytes received from the smart card.
        // Description  : Transmit APDU to card 
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardTransmit(IntPtr hCard, ref HiDWinscard.SCARD_IO_REQUEST pioSendRequest,
                                                           Byte[] SendBuff,
                                                           int SendBuffLen,
                                                           ref HiDWinscard.SCARD_IO_REQUEST pioRecvRequest,
                                                           Byte[] RecvBuff, ref int RecvBuffLen);

        // *********************************************************************************************************
        // Function Name: SCardGetStatusChange
        // In Parameter : hContext - A handle that identifies the resource manager context.
        //                value_TimeOut - The maximum amount of time, in milliseconds, to wait for an action.
        //                ReaderState -  An array of SCARD_READERSTATE structures that specify the readers to watch, and that receives the result.
        //                ReaderCount -  The number of elements in the rgReaderStates array.
        // Out Parameter: ReaderState - An array of SCARD_READERSTATE structures that specify the readers to watch, and that receives the result.
        // Description  : The current availability of the cards in a specific set of readers changes.
        //*********************************************************************************************************
        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        public static extern int SCardGetStatusChange(IntPtr hContext,
        int value_Timeout,
        ref HiDWinscard.SCARD_READERSTATE ReaderState,
        uint ReaderCount);


        static  IntPtr  hContext;               //Context Handle value
        static  uint    dwscope;                //Scope of the resource manager context
        static  String  ReaderList;             //List Of Reader
        static  String  ReaderName;             //Global Reader Variable
        static  IntPtr  hCard;                  //Card handle
        static  IntPtr  Protocol;               //Protocol used currently
        static  HiDWinscard.SCARD_READERSTATE ReaderState;              //Object of SCARD_READERSTATE

        public delegate void DelegateTimer();                   //delegate of the Timer
        static System.Timers.Timer Timer;                      //Object of the Timer

        static byte[]   CardUID = new byte[10];
        static int      CardUIDLen;
        static bool     CardConnected = false;


        public delegate void MyEventDelegate(bool CardPresent, byte[] UID, int Length);       
        public static event MyEventDelegate EventCardPresent;


        private static void UpdateCardPresent(bool CardPresent, byte[] UID, int Length)
        {
            if (EventCardPresent != null)
            {
                EventCardPresent(CardPresent, UID, Length);
            }
        }

        public static bool InitReader(ref string[] ReaderID, ref int Number)
        {
            int retval;
            uint pcchReaders = 0;
            int nullindex = -1;
            char nullchar = (char)0;
            dwscope = 2;

            // Creating a timer with a ten second interval.
            Timer = new System.Timers.Timer(250);
            // Hook up the Elapsed event for the timer.
            Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);            

            // Establish context.
            retval = SCardEstablishContext(dwscope, IntPtr.Zero, IntPtr.Zero, out hContext);
            retval = SCardListReaders(hContext, null, null, ref pcchReaders);
            byte[] mszReaders = new byte[pcchReaders];

            // Fill readers buffer with second call.
            retval = SCardListReaders(hContext, null, mszReaders, ref pcchReaders);

            // Populate List with readers.
            string currbuff = Encoding.ASCII.GetString(mszReaders);
            ReaderList = currbuff;
            int len = (int)pcchReaders;

            if (len > 0)
            {
                Number = 0;
                while (currbuff[0] != nullchar)
                {
                    nullindex = currbuff.IndexOf(nullchar);   // Get null end character.
                    string reader = currbuff.Substring(0, nullindex);
                    ReaderID[Number++] = reader; 
                    len = len - (reader.Length + 1);
                    currbuff = currbuff.Substring(nullindex + 1, len);
                }
                return true;
            }
            return false;
        }



        public static bool CardInField()
        {
            int RetVal;

            ReaderState.RdrName = ReaderName;     
            RetVal = SCardGetStatusChange(hContext, 0, ref ReaderState, 1);

            if (((ReaderState.RdrEventState >> 5) & 1) == 1)    //Check the 5th "card_present" bit. 
                return true;
            else
                return false;
        }

        public static bool ConnectCard()
        {
            int RetVal;

            RetVal = SCardConnect(hContext, ReaderName, HiDWinscard.SCARD_SHARE_SHARED, HiDWinscard.SCARD_PROTOCOL_T1, ref hCard, ref Protocol);       //Command to connect the card ,protocol T=1
            if (RetVal == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            int RetVal;

            //if(CardInField() == true)
            //{
            //    ConnectCard();
            //}

            ReaderState.RdrName = ReaderName;
            ReaderState.RdrCurrState = HiDWinscard.SCARD_STATE_UNAWARE;
            ReaderState.RdrEventState = 0;
            ReaderState.UserData = "Mifare Card";

            RetVal = SCardGetStatusChange(hContext, 0, ref ReaderState, 1);

            if ((ReaderState.ATRLength == 0) || (RetVal != 0))
            {
                UpdateCardPresent(false, CardUID, CardUIDLen);
                CardConnected = false;
            }
            else
            {
                if (CardConnected == false)
                {
                    RetVal = SCardConnect(hContext, ReaderName, HiDWinscard.SCARD_SHARE_SHARED, HiDWinscard.SCARD_PROTOCOL_T1, ref hCard, ref Protocol);       //Command to connect the card ,protocol T=1
                    if (RetVal == 0)
                    {
                        GetUID(ref CardUID, ref CardUIDLen);
                        UpdateCardPresent(true, CardUID, CardUIDLen);
                        CardConnected = true;
                    }
                    else
                    {
                        CardConnected = true;
                    }
                }
            }
        }

        public static void SelectReader(string Reader)
        {
            ReaderName = Reader;
            SCardReleaseContext(hContext);
        }

        public static bool Connect()
        {
            int RetVal;

            dwscope = 2;
            if (ReaderName != "" && ReaderName != null)
            {
                RetVal = SCardEstablishContext(dwscope, IntPtr.Zero, IntPtr.Zero, out hContext);
                if (RetVal == 0)
                {
                    Timer.Enabled = true;                    
                }
            }
            return false;  
        }

        public static bool InitCard(ref byte[] UID, ref int UidLen)
        {
            int Retval;
                        
            Retval = SCardConnect(hContext, ReaderName, HiDWinscard.SCARD_SHARE_SHARED, HiDWinscard.SCARD_PROTOCOL_T1, ref hCard, ref Protocol);       //Command to connect the card ,protocol T=1

            ReaderState.RdrName = ReaderName;
            ReaderState.RdrCurrState = HiDWinscard.SCARD_STATE_UNAWARE;
            ReaderState.RdrEventState = 0;
            ReaderState.UserData = "Mifare Card";

            if (Retval == 0)
            {
                Retval = SCardGetStatusChange(hContext, 0, ref ReaderState, 1);

                GetUID(ref UID, ref UidLen);                
            }

            return true;
        }

        public static bool TranceiveData(byte Cmd, byte[] TxData, int Offset, byte TxLen, ref byte[] RxData, ref int RxLen, ref byte ResultCode)
        {
            int RetVal;
            byte[] TxBuffer = new byte[256];
            byte[] RxBuffer = new byte[256];
            int TxBufLen, RxBufLen; 

            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;

            if (TxLen > 0)
            {
                TxBuffer[0] = 0x90;                
                TxBuffer[1] = Cmd;
                TxBuffer[2] = 0x00;
                TxBuffer[3] = 0x00;
                TxBuffer[4] = TxLen;
                Array.Copy(TxData, Offset, TxBuffer, 5, TxLen);
                TxBuffer[TxLen + 5] = 0x00;

                TxBufLen = TxLen + 6;
                RxBufLen = 255;

                RetVal = SCardTransmit(hCard, ref sioreq, TxBuffer, TxBufLen, ref rioreq, RxBuffer, ref RxBufLen);
            }
            else //Command only
            {
                TxBuffer[0] = 0x90;
                TxBuffer[1] = Cmd;
                TxBuffer[2] = 0x00;
                TxBuffer[3] = 0x00;
                TxBuffer[4] = 0x00;

                RxBufLen = 255;
                RetVal = SCardTransmit(hCard, ref sioreq, TxBuffer, 5, ref rioreq, RxBuffer, ref RxBufLen);
            }
            
            if (RetVal == 0 && RxBuffer[RxBufLen-2] == 0x91)
            {
                Array.Copy(RxBuffer, RxData, RxBufLen);
                RxLen = RxBufLen - 2;
                ResultCode = RxBuffer[RxBufLen - 1]; 
                return true;
            }

            return false;
        }

        public static bool TransmitApdu(byte[] apduCommand, out byte[] responseData, out byte sw1, out byte sw2)
        {
            int RetVal;
            byte[] RxBuffer = new byte[256]; // Ontvangstbuffer
            int RxBufLen = 255; // Maximale lengte voor antwoord

            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;

            // Verstuur het APDU-commando
            RetVal = SCardTransmit(hCard, ref sioreq, apduCommand, apduCommand.Length, ref rioreq, RxBuffer, ref RxBufLen);

            // Controleer of de transmissie succesvol was
            if (RetVal == 0 && RxBufLen >= 2) // Minimaal SW1 en SW2 aanwezig
            {
                // SW1 en SW2 zijn de laatste twee bytes van de response
                sw1 = RxBuffer[RxBufLen - 2];
                sw2 = RxBuffer[RxBufLen - 1];

                // ResponseData zonder SW1/SW2
                responseData = new byte[RxBufLen - 2];
                Array.Copy(RxBuffer, responseData, RxBufLen - 2);
                return true;
            }

            // Fout: geen geldige respons
            responseData = new byte[0]; // Vervangt Array.Empty<byte>()
            sw1 = 0;
            sw2 = 0;
            return false;
        }

        //********************************************************
        //Function Name:ATR_UID
        //Description:Gives ATR and UID of the card 
        //********************************************************
        public static void GetUID(ref byte[] UID, ref int UidLen)
        {
            int Retval;
            Byte[] sendBuffer = new Byte[255];                      //Send Buffer in SCardTransmit
            Byte[] receiveBuffer = new Byte[255];                   //Receive Buffer in SCardTransmit
            int sendbufferlen, receivebufferlen;                    //Send and Receive Buffer length in SCardTransmit

            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;

            int Cnt;

            sendBuffer[0] = 0xFF;
            sendBuffer[1] = 0xCA;
            sendBuffer[2] = 0x00;
            sendBuffer[3] = 0x00;
            sendBuffer[4] = 0x00;

            sendbufferlen = 0x5;

            receivebufferlen = 255;

            Retval = SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, ref rioreq, receiveBuffer, ref receivebufferlen);

            if (Retval == 0)
            {
                if ((receiveBuffer[receivebufferlen - 2] == 0x90) && (receiveBuffer[receivebufferlen - 1] == 0))
                {
                    UidLen = receivebufferlen - 2;
                    for (Cnt = 0; Cnt < UidLen; Cnt++)
                    {
                        UID[Cnt] = receiveBuffer[Cnt];
                    }
                }
            }
        }

        //********************************************************
        //Function Name:LoadKey
        //Description:Loads a Mifare Classic key
        //********************************************************
        public static void LoadKey(ref byte KeyNr, ref byte[] KEY)
        {
            int Retval;
            Byte[] sendBuffer = new Byte[255];                      //Send Buffer in SCardTransmit
            Byte[] receiveBuffer = new Byte[255];                   //Receive Buffer in SCardTransmit
            int sendbufferlen, receivebufferlen;                    //Send and Receive Buffer length in SCardTransmit

            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;

            int Cnt;
            byte len;

            len = 0x6;

            sendBuffer[0] = 0xFF;
            sendBuffer[1] = 0x82;
            sendBuffer[2] = 0x20;
            sendBuffer[3] = KeyNr;
            sendBuffer[4] = len;
            for (int k = 0; k <= 5; k++)
                sendBuffer[k + 5] = KEY[k];

            sendbufferlen = 0xB;
            receivebufferlen = 255;

            Retval = SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, ref rioreq, receiveBuffer, ref receivebufferlen);
            if (Retval == 0)
            {
                if ((receiveBuffer[receivebufferlen - 2] == 0x90) && (receiveBuffer[receivebufferlen - 1] == 0))
                {
                    //LOAD KEY OK
                }
            }
        }

        //********************************************************
        //Function Name:Authenticate
        //Description:Authenticate to a Mifare Classic block
        //********************************************************
        public static bool Authenticate(ref byte KeyNr, ref char KeyType, ref byte Block)
        {
            int Retval;
            Byte[] sendBuffer = new Byte[255];                      //Send Buffer in SCardTransmit
            Byte[] receiveBuffer = new Byte[255];                   //Receive Buffer in SCardTransmit
            int sendbufferlen, receivebufferlen;                    //Send and Receive Buffer length in SCardTransmit

            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;

            byte len = 0x5;

            sendBuffer[0] = 0xFF;
            sendBuffer[1] = 0x86;
            sendBuffer[2] = 0x00;
            sendBuffer[3] = 0x00;
            sendBuffer[4] = len;
            sendBuffer[5] = 0x1;        //Version
            sendBuffer[6] = 0x0;        //Address MSB
            sendBuffer[7] = Block;
            if (KeyType == 'A')
                 sendBuffer[8] = 0x60;  //Key Type A
            else sendBuffer[8] = 0x61;  //Key Type B
            sendBuffer[9] = KeyNr;      //Key Number

            sendbufferlen = 0xA;
            receivebufferlen = 255;

            Retval = SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, ref rioreq, receiveBuffer, ref receivebufferlen);
            if (Retval == 0)
            {
                if ((receiveBuffer[receivebufferlen - 2] == 0x90) && (receiveBuffer[receivebufferlen - 1] == 0))
                {
                    //General Authenticate Successful
                    return true;
                }
            }
            return false;
        }

        //********************************************************
        //Function Name:ClassicWrite
        //Description:Write DATA to a Mifare Classic block
        //********************************************************
        public static bool ClassicWrite(ref byte Block, ref byte[] DATA)
        {
            int Retval;
            Byte[] sendBuffer = new Byte[255];                      //Send Buffer in SCardTransmit
            Byte[] receiveBuffer = new Byte[255];                   //Receive Buffer in SCardTransmit
            int sendbufferlen, receivebufferlen;                    //Send and Receive Buffer length in SCardTransmit

            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;

            byte len = 0x10;

            sendBuffer[0] = 0xFF;
            sendBuffer[1] = 0xD6;
            sendBuffer[2] = 0x00;
            sendBuffer[3] = Block;
            sendBuffer[4] = len;
            for (int k1 = 0; k1 <= 15; k1++)
                sendBuffer[k1 + 5] = DATA[k1];

            sendbufferlen = 0x15;
            receivebufferlen = 0x12;

            Retval = SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, ref rioreq, receiveBuffer, ref receivebufferlen);
            if (Retval == 0)
            {
                if ((receiveBuffer[receivebufferlen - 2] == 0x90) && (receiveBuffer[receivebufferlen - 1] == 0))
                {
                    //Write Successful
                    return true;
                }
            }
            return false;
        }


        //********************************************************
        //Function Name:ClassicRead
        //Description:Write DATA to a Mifare Classic block
        //********************************************************
        public static bool ClassicRead(byte Block, ref byte[] DATA)
        {
            int Retval;
            Byte[] sendBuffer = new Byte[255];                      //Send Buffer in SCardTransmit
            Byte[] receiveBuffer = new Byte[255];                   //Receive Buffer in SCardTransmit
            int sendbufferlen, receivebufferlen;                    //Send and Receive Buffer length in SCardTransmit

            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;

            for (int k1 = 0; k1 <= 15; k1++) DATA[k1] = 0;
            byte len = 0x10;
            sendBuffer[0] = 0xFF;
            sendBuffer[1] = 0xB0;
            sendBuffer[2] = 0x00;
            sendBuffer[3] = Block;
            sendBuffer[4] = len;
            sendbufferlen = 0x05;
            receivebufferlen = 0x12;
            Retval = SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, ref rioreq, receiveBuffer, ref receivebufferlen);
            if (Retval == 0)
            {
                if ((receiveBuffer[receivebufferlen - 2] == 0x90) && (receiveBuffer[receivebufferlen - 1] == 0))
                {
                    for (int k1 = 0; k1 <= 15; k1++)
                        DATA[k1] = receiveBuffer[k1];
                    return true;
                }
            }
            return false;
        }

    }

    //Class for Constants
    public class HiDWinscard
    {
        // Context Scope

        public const int SCARD_STATE_UNAWARE = 0x0;

        //The application is unaware about the curent state, This value results in an immediate return
        //from state transition monitoring services. This is represented by all bits set to zero

        public const int SCARD_SHARE_SHARED = 2;

        // Application will share this card with other 
        // applications.

        //   Disposition

        public const int SCARD_UNPOWER_CARD = 2; // Power down the card on close

        //   PROTOCOL

        public const int SCARD_PROTOCOL_T0 = 0x1;                  // T=0 is the active protocol.
        public const int SCARD_PROTOCOL_T1 = 0x2;                  // T=1 is the active protocol.
        public const int SCARD_PROTOCOL_UNDEFINED = 0x0;

        //IO Request Control
        public struct SCARD_IO_REQUEST
        {
            public int dwProtocol;
            public int cbPciLength;
        }


        //Reader State

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SCARD_READERSTATE
        {
            public string RdrName;
            public string UserData;
            public uint RdrCurrState;
            public uint RdrEventState;
            public uint ATRLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24, ArraySubType = UnmanagedType.U1)]
            public byte[] ATRValue;
        }
        //Card Type
        public const int card_Type_Mifare_1K = 1;
        public const int card_Type_Mifare_4K = 2;

    }
}
