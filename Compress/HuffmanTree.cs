using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script;
using System.Web.Script.Serialization;

namespace Compress
{
    class HuffmanTree
    {
        public class HuffmanNote
        {
            public int Weight { get; set; }
            public char Word { get; set; }
            public HuffmanNote LeftNote { get; set; }
            public HuffmanNote RightNote { get; set; }
            public char HuffmanCode { get; set; }
        }

        public string Compression(string content)
        {
            HuffmanNote huffmanNote = CreateHuffmanTree(CreateWordWeightDictionary(content));

            Dictionary<char, string> encodeDictionary = new Dictionary<char, string>();
            CreateWordCodeDictionay(huffmanNote, "", encodeDictionary);

            StringBuilder sb = new StringBuilder(content.Length);
            foreach (var item in content)
            {
                sb.Append(encodeDictionary[item]);
            }

            string huffmanCode = sb.ToString();

            HuffmanTreeInfo huffmanTreeInfo = new HuffmanTreeInfo();
            huffmanTreeInfo.ZipCodeRemainder = huffmanCode.Substring(huffmanCode.Length - huffmanCode.Length % 8);
            huffmanTreeInfo.ZipCode = HuffmanCodeToByte(huffmanCode.Substring(0, huffmanCode.Length - huffmanCode.Length % 8));
            huffmanTreeInfo.UnZipDictionary = CreateUnZipDictionary(encodeDictionary);

            return ObjectToJson(huffmanTreeInfo);
        }

        private string ObjectToJson(object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        private T JsonToObject<T>(string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }

        private List<HuffmanNote> CreateWordWeightDictionary(string content)
        {
            Dictionary<char, int> wordWeightDictionary = new Dictionary<char, int>();

            foreach (var item in content)
            {
                if (wordWeightDictionary.ContainsKey(item))
                {
                    wordWeightDictionary[item] += 1;
                }
                else
                {
                    wordWeightDictionary[item] = 1;
                }
            }

            List<HuffmanNote> huffmanNoteList = new List<HuffmanNote>();
            foreach (var item in wordWeightDictionary)
            {
                huffmanNoteList.Add(new HuffmanNote()
                {
                    Weight = item.Value,
                    Word = item.Key
                });
            }
            return huffmanNoteList;
        }

        public string Unzip(string content)
        {
            HuffmanTreeInfo huffmanTreeInfo = JsonToObject<HuffmanTreeInfo>(content);

            StringBuilder sb = new StringBuilder(huffmanTreeInfo.ZipCode.Length);
            for (int i = 0; i < huffmanTreeInfo.ZipCode.Length; i++)
            {
                sb.Append(Convert.ToString((int)huffmanTreeInfo.ZipCode[i], 2).PadLeft(8, '0'));
            }
            string huffmanCodes = sb.ToString() + huffmanTreeInfo.ZipCodeRemainder;

            StringBuilder decode = new StringBuilder(huffmanCodes.Length);
            string temp = string.Empty;
            for (int i = 0; i < huffmanCodes.Length; i++)
            {
                temp += huffmanCodes[i].ToString();
                if (huffmanTreeInfo.UnZipDictionary.ContainsKey(temp))
                {
                    decode.Append(huffmanTreeInfo.UnZipDictionary[temp]);
                    temp = string.Empty;
                }
            }
            return decode.ToString();
        }

        private string HuffmanCodeToByte(string huffmanCodes)
        {
            StringBuilder sb = new StringBuilder(huffmanCodes.Length);
            for (int i = 0; i * 8 < huffmanCodes.Length; i++)
            {
                sb.Append((char)Convert.ToInt32(huffmanCodes.Substring(i * 8, 8), 2));
            }
            return sb.ToString();
        }

        private Dictionary<string, char> CreateUnZipDictionary(Dictionary<char, string> encodeDictionary)
        {
            Dictionary<string, char> unZipDictionary = new Dictionary<string, char>();
            foreach (var item in encodeDictionary)
            {
                unZipDictionary.Add(item.Value, item.Key);
            }
            return unZipDictionary;
        }

        private HuffmanNote CreateHuffmanTree(List<HuffmanNote> huffmanNoteList)
        {
            if (huffmanNoteList.Count == 1)
            {
                return huffmanNoteList[0];
            }

            var huffmanNoteListBySort = huffmanNoteList.OrderBy(o => o.Weight).ToList();

            var minWeight2Note = huffmanNoteListBySort.Take(2).ToList();
            huffmanNoteListBySort.RemoveAt(0);
            huffmanNoteListBySort.RemoveAt(0);

            var leftNote = minWeight2Note[0];
            leftNote.HuffmanCode = '0';

            var rightNote = minWeight2Note[1];
            rightNote.HuffmanCode = '1';

            var newNote = new HuffmanNote()
            {
                LeftNote = leftNote,
                RightNote = rightNote,
                Weight = leftNote.Weight + rightNote.Weight
            };

            huffmanNoteListBySort.Add(newNote);

            return CreateHuffmanTree(huffmanNoteListBySort);
        }

        private void CreateWordCodeDictionay(HuffmanNote huffmanNote, string HuffmanCode, Dictionary<char, string> encodeDictionary)
        {
            if (huffmanNote.LeftNote == null)
            {
                encodeDictionary[huffmanNote.Word] = HuffmanCode.Substring(1) + huffmanNote.HuffmanCode;//HuffmanCode.Substring(1) 为什么有这句呢  因为char默认值是\0  所以这里要把第一个字符\0去掉
                return;
            }
            HuffmanCode += huffmanNote.HuffmanCode;

            CreateWordCodeDictionay(huffmanNote.LeftNote, HuffmanCode, encodeDictionary);
            CreateWordCodeDictionay(huffmanNote.RightNote, HuffmanCode, encodeDictionary);
        }
    }
}
