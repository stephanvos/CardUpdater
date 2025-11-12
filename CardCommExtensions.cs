using System;

namespace CardUpdater
{
    // Extension methods for CardComm to add AID reading functionality
    public static class CardCommExtensions
    {
        // Overload method to get Application IDs with output parameters
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
    }
}
