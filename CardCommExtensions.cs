using System;

namespace CardUpdater
{
    // Extension methods for CardComm to add AID and File reading functionality
    public static class CardCommExtensions
    {
        // Get Application IDs
        public static bool GetApplicationIDs(ref byte[] aids, ref int aidCount)
        {
    bool RetVal;
   byte[] RxData = new byte[512];
  byte[] TxData = new byte[512];
            int RxLen = 0;
      byte ResultCode = 0;

            RetVal = UsbReader.TranceiveData(0x6A, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);

      if (RetVal == true && ResultCode == 0x00)
            {
              // Each AID is 3 bytes, calculate number of AIDs
 aidCount = RxLen / 3;
   
     // Copy AIDs to output array
         if (aidCount > 0 && RxLen <= aids.Length)
       {
               Array.Copy(RxData, aids, RxLen);
             }
  
              return true;
     }
       else
        {
     aidCount = 0;
       return false;
  }
     }

        // Get File IDs from currently selected application
    public static bool GetFileIDs(ref byte[] fileIds, ref int fileCount)
        {
        bool RetVal;
            byte[] RxData = new byte[512];
  byte[] TxData = new byte[512];
         int RxLen = 0;
   byte ResultCode = 0;

        // DESFire command 0x6F = GetFileIDs
          RetVal = UsbReader.TranceiveData(0x6F, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);

   if (RetVal == true && ResultCode == 0x00)
  {
          // Each File ID is 1 byte
    fileCount = RxLen;
   
   // Copy File IDs to output array
          if (fileCount > 0 && RxLen <= fileIds.Length)
       {
           Array.Copy(RxData, fileIds, RxLen);
         }
  
         return true;
 }
      else
       {
                fileCount = 0;
       return false;
          }
        }
      
   // Read data from a plain/unencrypted file (no authentication needed)
    public static bool ReadPlainDataFile(int fileNumber, int offset, int length, ref byte[] data, ref int actualLength)
        {
bool RetVal;
         byte[] RxData = new byte[8192];
   byte[] TxData = new byte[512];
     int RxLen = 0;
            byte ResultCode = 0;
            int WritePtr = 0;

       // Build Read Data command (0xBD)
        TxData[0] = (byte)fileNumber;
     TxData[1] = (byte)(offset & 0xFF);
            TxData[2] = (byte)((offset >> 8) & 0xFF);
            TxData[3] = (byte)((offset >> 16) & 0xFF);
      TxData[4] = (byte)(length & 0xFF);
 TxData[5] = (byte)((length >> 8) & 0xFF);
          TxData[6] = (byte)((length >> 16) & 0xFF);

  RetVal = UsbReader.TranceiveData(0xBD, TxData, 0, 7, ref RxData, ref RxLen, ref ResultCode);

            // Copy first response
       byte[] RxMsg = new byte[8192];
            Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
   WritePtr += RxLen;

 // Handle additional frames
         while (ResultCode == 0xAF)
  {
       RetVal = UsbReader.TranceiveData(0xAF, TxData, 0, 0, ref RxData, ref RxLen, ref ResultCode);
         if (RetVal)
    {
             Array.Copy(RxData, 0, RxMsg, WritePtr, RxLen);
  WritePtr += RxLen;
 }
    else
     {
                break;
   }
            }

       if (ResultCode == 0x00)
            {
            actualLength = WritePtr;
     if (actualLength > 0 && actualLength <= data.Length)
        {
       Array.Copy(RxMsg, data, actualLength);
      return true;
     }
            }

         actualLength = 0;
return false;
        }
    }
}
