using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WindowsFormsTetris
{
    /***
     * Administer data to be saved (ex. High Score)
     ***/
    public class SavedDataManager
    {
        const string FILENAME = "SavedData.xml";

        /* Saved Data Format */
        public struct SAVED_DATA
        {
            public int scoreMax;
            public DateTime date;
        }

        SAVED_DATA m_savedData;

        public void setScoreMax(int score){
            if (score > m_savedData.scoreMax) {
                m_savedData.scoreMax = score;
                m_savedData.date = DateTime.Now;
            }
        }

        public int getScoreMax()
        {
            return m_savedData.scoreMax;
        }

        public void save()
        {
            _save<SAVED_DATA>(m_savedData);
        }

        public void load()
        {
            _load<SAVED_DATA>(out m_savedData);
        }

        private void _save<T>(T obj)
        {
            try {
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StreamWriter writer = new System.IO.StreamWriter(
                    FILENAME, false, new System.Text.UTF8Encoding(false));

                serializer.Serialize(writer, obj);
                writer.Close();
            } catch (Exception e) {
                Debug.WriteLine("SavedDataManager.save e = {0}", e.Message);
            }
        }

        private bool _load<T>(out T obj)
        {
            try { 
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StreamReader reader = new System.IO.StreamReader(
                    FILENAME, new System.Text.UTF8Encoding(false));
                //XMLファイルから読み込み、逆シリアル化する
                obj = (T)serializer.Deserialize(reader);
                //ファイルを閉じる
                reader.Close();
                return true;
            } catch (Exception e) {
                obj = default(T);
                return false;
            }
        }

    }
}
