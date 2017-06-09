using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Collections;
using System.Data;

namespace RandomSelector
{
    class ClassCryptography
    {
        private static ArrayList MemberArr;

        //该方法用于对明文信息进行加密写入
        /// <summary>
        /// 加密一般是把明文转变成 密文和钥匙
        /// </summary>
        /// <param name="txtPath">文档的路径</param>
        /// <param name="txtArr">待加密的Datatable内容</param>
        public  void Encrypted(string txtPath,DataTable dt,string[] infos)
        {
          


            //首先把字符串拆解成路径和文件名
            string[] words = txtPath.Split('\\');
            //最后一个是文件名,前面的串起来作为路径
            string path = "";
            string filename = (words[words.Length - 1].Split('.'))[0];
            for (int i = 0; i < words.Length - 1; i++)
            {
                path += words[i] + "\\";
            }

            //判断更新之前是否有keys ,如果有先删除之
          

            // Create a new file to work with
            FileStream fsOut = File.Create(path+filename+".zy");
            // Create a new crypto provider
            TripleDESCryptoServiceProvider tdes =
             new TripleDESCryptoServiceProvider();
            // Create a cryptostream to encrypt to the filestream
            CryptoStream cs = new CryptoStream(fsOut, tdes.CreateEncryptor(),
             CryptoStreamMode.Write);
            // Create a StreamWriter to format the output
            StreamWriter sw = new StreamWriter(cs);
           // StreamWriter sw = new StreamWriter("C:\\01.txt");
            // And write some data
            
            sw.WriteLine(infos[0]);  //写单位
            if (txtPath.Split('.').Last() != "txt")
            {
                sw.WriteLine(Convert.ToInt16(infos[1]) + 1); //使用次数加一
            }
            else
            {
                sw.WriteLine(infos[1]);
            }
            sw.WriteLine(DateTime.Now.ToShortDateString()); //记录时间日期
            sw.WriteLine("姓名\t职务\t点中数\t性别\t参点数");
            //下面循环写数据dt
            for (int rownum = 0; rownum < dt.Rows.Count; rownum++) //行便利
            {
                for (int col = 0; col < dt.Columns.Count; col++)//列便利
                {
                    if (col < 4)
                    { sw.Write(dt.Rows[rownum][col].ToString() + "\t"); }
                    else
                    {
                        if (rownum < dt.Rows.Count - 1)
                        { sw.WriteLine(dt.Rows[rownum][col].ToString()); }
                        else
                        {
                            sw.Write(dt.Rows[rownum][col].ToString());
                        }  
                    }
                }
            
            }  

                sw.Flush();
                sw.Close();
            // save the key and IV for future use
            FileStream fsKeyOut = File.Create(path+filename+".keys");
            // use a BinaryWriter to write formatted data to the file
            BinaryWriter bw = new BinaryWriter(fsKeyOut);
            // write data to the file
            bw.Write(tdes.Key);
            bw.Write(tdes.IV);
            // flush and close
            bw.Flush();
            bw.Close();
            fsKeyOut.Close();
        }



        /// <summary>
        /// 给定档案和密匙的路径,输出一个数据表
        /// </summary>
        /// <param name="txtPath"></param>
        /// <returns></returns>
        public  ArrayList UnEncrypted(string datapath,string keypath)
        {
            MemberArr = new ArrayList();
           
            TripleDESCryptoServiceProvider tdes =
            new TripleDESCryptoServiceProvider();
            // open the file containing the key and IV
            FileStream fsKeyIn = File.OpenRead(keypath);
            // use a BinaryReader to read formatted data from the file
            BinaryReader br = new BinaryReader(fsKeyIn);
            // read data from the file and close it
            tdes.Key = br.ReadBytes(24);
            tdes.IV = br.ReadBytes(8);
            fsKeyIn.Close();
            // Open the encrypted file
            FileStream fsIn = File.OpenRead(datapath);
            // Create a cryptostream to decrypt from the filestream
            CryptoStream cs = new CryptoStream(fsIn, tdes.CreateDecryptor(),
             CryptoStreamMode.Read);
            // Create a StreamReader to format the input
            StreamReader sr = new StreamReader(cs);
            // And decrypt the data
            
           
               // MemberArr.Add(sr.ReadToEnd());
            //获取长度
            string cache = "";
            while ((cache=sr.ReadLine()) != null)
            {
                MemberArr.Add(cache);
            }

         

              sr.Close();
              //tdes.Dispose();
            return MemberArr;

        }
    }
}
