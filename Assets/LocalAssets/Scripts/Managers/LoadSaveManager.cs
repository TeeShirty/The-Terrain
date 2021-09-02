using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;

public class LoadSaveManager : MonoBehaviour
{
    Aes aes;

    [XmlRoot("GameData")]

    public class GameStateData
    {
        public struct DataTransform
        {
            public float positionX;
            public float positionY;
            public float positionZ;
            public float rotationX;
            public float rotationY;
            public float rotationZ;
            public float scaleX;
            public float scaleY;
            public float scaleZ;
        }

        public class DataEnemy
        {
            //Enemy transform data
            public DataTransform positionRotationScale;

            public int enemyID;

            //public int health;
        }

        public class DataPlayer
        {
            public DataTransform positionRotationScale;

            public float collectedAmmo;

            public bool collectedWeapon;

            public int health;
        }

        public List<DataEnemy> enemies = new List<DataEnemy>();

        public DataPlayer player = new DataPlayer();
    }

    //Game data to save / load
    public GameStateData gameState = new GameStateData(); //gamestate made up of enemy and player


    public void Save(string fileName = "GameData.xml")
    {
        //gameState.enemies.Clear();  
        //Debug.Log("LoadSaveManager Save");
        ////create the xml file
        //XmlSerializer serializer = new XmlSerializer(typeof(GameStateData)); // simialr to require component
        //FileStream stream = new FileStream(fileName, FileMode.Create); //If it exists how is it going to handle the file
        //CryptoStream crypto = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        //serializer.Serialize(stream, gameState);
        //stream.Flush();
        //stream.Dispose();
        //stream.Close();



        XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
        FileStream stream = new FileStream(fileName, FileMode.Open);
        using (Aes aes = Aes.Create())
        {
            byte[] key =
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
            };
            aes.Key = key;

            byte[] iv = aes.IV;
            stream.Write(iv, 0, iv.Length);

            using (CryptoStream crypto = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                using (StreamWriter eWriter = new StreamWriter(crypto))
                {
                    serializer.Serialize(crypto, gameState);
                }
            }
        }
    }


    

    public void Load(string fileName = "GameData.xml")
    {
        //Debug.Log("LoadSaveManager Load");
        //XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
        //FileStream stream = new FileStream(fileName, FileMode.Open);
        //CryptoStream crypto = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        //gameState = serializer.Deserialize(stream) as GameStateData; //making sure it is cast as a game state data
        //stream.Flush();
        //stream.Dispose();
        //stream.Close();



        XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
        FileStream stream = new FileStream(fileName, FileMode.Open);

        using (Aes aes = Aes.Create())
        {
            byte[] iv = new byte[aes.IV.Length];
            int numBytesToRead = aes.IV.Length;
            int numByteRead = 0;
            while (numBytesToRead > 0)
            {
                int n = stream.Read(iv, numByteRead, numBytesToRead);
                if (n == 0)
                {
                    break;
                }
                numByteRead += n;
                numBytesToRead -= n;
            }
            byte[] key =
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
            };

            using (CryptoStream crypto = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read))
            {
                using (StreamWriter dWriter = new StreamWriter(crypto))
                {
                    gameState = serializer.Deserialize(crypto) as GameStateData;
                }
            }
        }
    }
}
