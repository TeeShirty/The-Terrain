using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


public static class EncryptionDecryption  
{
    public static string EncryptDecrypt(string fileName, int key)
    {
        StringBuilder outStringBuilder = new StringBuilder(fileName.Length);
        for (int i = 0; i < fileName.Length; i++)
        {
            char ch = (char)(fileName[i] ^ key);
            outStringBuilder.Append(ch);
        }
        return outStringBuilder.ToString();
    }
}
