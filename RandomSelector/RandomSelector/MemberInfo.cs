using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandomSelector
{
  public class MemberInfo
    {

        private ArrayList MemberArray;
        private DataTable Result;
        private string[] statsInfo;
        public static  DataTable dt=new DataTable();
        public DataTable usedDt;
        public static DataTable originDt;

        //下面是一个用来读取txt格式化数据的函数,通过读取获取人员的数据结构,该数据结构采用ArrayList泛型来存储
        public  void ReadMemberDat(string path)
        {
            MemberArray = new ArrayList();
            
            //逐行读取人员信息
            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    MemberArray.Add(str);
                }
                
            }

           // return MemberArray;
        }


        

      /// <summary>
      /// //与上面类似还需要一个ReadMemberZy的函数共调用,写到MemberArray上
      /// </summary>
      /// <param name="path">这个path指向.zy文件路径,在此调用解密函数读取</param>
        public void ReadMemberZy(string datapath)
        {
            string keypath = datapath.Split('.').First() + ".keys"; //得到密钥路径
            ClassCryptography cc = new ClassCryptography();
            MemberArray = cc.UnEncrypted(datapath, keypath);
            //调用
        
        }

        //继而统计数据表信息,并返回到相应的Label上
        public string[] GetMemberInfo()
        {
            statsInfo = new string[8];
            statsInfo[0] = MemberArray[0].ToString().TrimEnd();//单位名
            statsInfo[1] = MemberArray[1].ToString().TrimEnd();//已使用次数
            string time = MemberArray[2].ToString().TrimEnd();
            if (time == "datetime")
            {
                time = DateTime.Now.ToShortDateString();
            }
            statsInfo[2] = time; //时间信息
            //获取全人数
            statsInfo[3] = (MemberArray.Count - 4).ToString(); //全人数
            //创建人员信息表
           
                //创建列
            dt.Columns.Add("Name", typeof(String));
            dt.Columns.Add("Title", typeof(String));
            dt.Columns.Add("CountHist", typeof(Int16));
            dt.Columns.Add("Gender", typeof(String));
            dt.Columns.Add("AllTimes", typeof(Int16));
                //实例化列
            for (int i = 4; i < MemberArray.Count;i++ )
            {
                string[] cacheStr = cacheStr = MemberArray[i].ToString().Split('\t');

                dt.Rows.Add(cacheStr);
            }
            //至此人员信息表创建完毕
                //统计男人数目
            DataRow[] drs = dt.Select("Gender='1'");
            statsInfo[4] = drs.Length.ToString();  //男人数
            DataRow[] drs1 = dt.Select("Title LIKE '%A%'");
            statsInfo[5] = drs1.Length.ToString(); //领导职务数
            DataRow[] drs2 = dt.Select("Title LIKE '%B%'");
            statsInfo[6] = drs2.Length.ToString(); //课题组长数
            DataRow[] drs3 = dt.Select("Title LIKE '%C%'");
            statsInfo[7] = drs3.Length.ToString(); //支部委员
            originDt = dt.Clone();
            for (int dtRows = 0; dtRows < dt.Rows.Count; dtRows++)
            {
                originDt.Rows.Add(dt.Rows[dtRows].ItemArray);
            
            }
                return statsInfo;
        }
  

       

        //传入一个checkedInfo字符数组,计算出待候选人索引
        public string CalcSelNum(char[] checkedInfo)
        {
            string returnStr = ""; //返回值str
            string infoStr = ""; //连串Str
            //DataRow[] drs = dt.Select();
            DataTable subDt = new DataTable();
            subDt = dt;
            foreach (char ch in checkedInfo)
            {
                infoStr += ch;
            }

            if (infoStr == "000000")
            {
                returnStr = "none";
            }

            if (checkedInfo[0] == '1')
            {
                returnStr = "all";
            }
            else
            {
                if (checkedInfo[1] == '1') //如果排除领导,对subdt进行删除
                {
                    DataRow[] drs = dt.Select("Title LIKE '%A%'");
                    foreach (DataRow dr in drs)
                    {
                        subDt.Rows.Remove(dr);
                    }
                }

                if (checkedInfo[2] == '1') //如果排除课题组长,对subdt进行删除
                {
                    DataRow[] drs = dt.Select("Title LIKE '%B%'");
                    foreach (DataRow dr in drs)
                    {
                        subDt.Rows.Remove(dr);
                    }
                }

                if (checkedInfo[3] == '1') //如果排除支部委员,对subdt进行删除
                {
                    DataRow[] drs = dt.Select("Title LIKE '%C%'");
                    foreach (DataRow dr in drs)
                    {
                        subDt.Rows.Remove(dr);
                    }
                }

                if (checkedInfo[4] == '1') //如果选择男性,对subdt进行删除,男女必选其一
                {
                    DataRow[] drs = dt.Select("Gender = '0'");
                    foreach (DataRow dr in drs)
                    {
                        subDt.Rows.Remove(dr);
                    }
                }

                if (checkedInfo[5] == '1') //如果选择女性,对subdt进行删除,男女必选其一
                {
                    DataRow[] drs = dt.Select("Gender = '1'");
                    foreach (DataRow dr in drs)
                    {
                        subDt.Rows.Remove(dr);
                    }
                }
                
                //删到最后把剩余的名字输出
                for (int i = 0; i < subDt.Rows.Count; i++)
                {
                    returnStr += subDt.Rows[i]["Name"].ToString() + ",";
                }
            }
            return returnStr;
        
        }


       //随机选人的函数
      /// <summary>
      /// 
      /// </summary>
      /// <param name="inputStr">输入的待选人员名单</param>
      /// <param name="method">历史依赖还是历史无关</param>
      /// n是需要的人数
      /// <returns></returns>
        public DataTable DoSelectWithHistory(string inputStr,int n)
        {
            
            Result = new  DataTable();
            Result.Columns.Add();
            Result.Columns.Add();
            Result.Columns.Add();
            Result.Columns.Add();
            Result.Columns.Add();
            Result.Columns.Add();
            double[] res=new double[]{-1};
            if (inputStr == "all")//对全表进行随机抽
            {
                //给dt 候选次数全部+1 
                //for (int dtrows = 0; dtrows < dt.Rows.Count; dtrows++)
                //{
                //    dt.Rows[dtrows][4] = Convert.ToInt16(dt.Rows[dtrows][4]) + 1;
                //}
                 res = RandomCalc(dt);
                 usedDt = dt.Copy();
                
            }
            else if (inputStr == "none")
            {
                MessageBox.Show("您的筛选记录为零,请重新选择条件!");
                
                
            }
            else //如果输入的是字符串则需要进行
            {
                string[] names = inputStr.Split(',');
                
                DataTable dtCache = new DataTable();
                dtCache = dt.Clone();
                foreach (string str in names)
                {
                    if(str!="")
                    {string condi = String.Format("Name = '{0}'", str);
                    dtCache.Rows.Add(dt.Select(condi)[0].ItemArray); }
                    
                    
                }

                usedDt = dtCache;
                //根据usedDt给候选次数全部+1 


                res = RandomCalc(dtCache);
            }

            //对生成的res结果进行排序和截取
            if(res[0]!=-1) //结果不为空时
            {
                usedDt.Columns.Add("Res", typeof(Double));
                int i=0;
                foreach (double db in res)
                {
                    usedDt.Rows[i][5] = db;
                    i++;
                }

                DataView dataView = usedDt.DefaultView;
                dataView.Sort = "Res asc";  //升序排列
                usedDt = dataView.ToTable();

                //选择前n个输出,再给选中次数上+1

                for (int j = 0; j < n; j++)
                {
                   Result.Rows.Add(usedDt.Rows[j].ItemArray);
                    
                }
               
                
            }
            usedDt.Clear();
            return Result;
        }


        public double[] WithOutHisRand(DataTable dt)
        { 
            //对dt随机排序
            double[] res = new double[dt.Rows.Count];
            int sand = DateTime.Now.Millisecond;
            Random rand = new Random(sand); //用毫秒做随机种子
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = rand.NextDouble() * 100;
  
            }

                return res;
        
        }
        //
        public DataTable DoSelectWithoutHis(string inputStr, int n)
        {
            Result = new DataTable();
            Result.Columns.Add();
            Result.Columns.Add();
            Result.Columns.Add();
            Result.Columns.Add();
            Result.Columns.Add();
            Result.Columns.Add();
            double[] res = new double[] { -1 };
            if (inputStr == "all")//对全表进行随机抽
            {
                //给dt 候选次数全部+1 
                //for (int dtrows = 0; dtrows < dt.Rows.Count; dtrows++)
                //{
                //    dt.Rows[dtrows][4] = Convert.ToInt16(dt.Rows[dtrows][4]) + 1;
                //}
                res = WithOutHisRand(dt);
                usedDt = dt.Copy();

            }
            else if (inputStr == "none")
            {
                MessageBox.Show("您的筛选记录为零,请重新选择条件!");


            }
            else //如果输入的是字符串则需要进行
            {
                string[] names = inputStr.Split(',');

                DataTable dtCache = new DataTable();
                dtCache = dt.Clone();
                foreach (string str in names)
                {
                    if (str != "")
                    {
                        string condi = String.Format("Name = '{0}'", str);
                        dtCache.Rows.Add(dt.Select(condi)[0].ItemArray);
                    }


                }

                usedDt = dtCache;



                res = WithOutHisRand(dtCache);
            }

            //对生成的res结果进行排序和截取
            if (res[0] != -1) //结果不为空时
            {
                usedDt.Columns.Add("Res", typeof(Double));
                int i = 0;
                foreach (double db in res)
                {
                    usedDt.Rows[i][5] = db;
                    i++;
                }

                DataView dataView = usedDt.DefaultView;
                dataView.Sort = "Res asc";  //升序排列
                usedDt = dataView.ToTable();

                //选择前n个输出,再给选中次数上+1

                for (int j = 0; j < n; j++)
                {
                    Result.Rows.Add(usedDt.Rows[j].ItemArray);

                }


            }
            usedDt.Clear();
            return Result;
        }
        public double[] RandomCalc(DataTable dt)
        {
            //DateTime now = new DateTime();
            int sand = DateTime.Now.Millisecond;
            Random rand = new Random(sand); //用毫秒做随机种子
            int  allSelect, allNum;//allratio概率之和;allSelect全部选中数之和;allNum全部参选数之和;
            double[] res = new double[dt.Rows.Count];
            double[,] stats = new double[dt.Rows.Count, 4];
           
            allSelect = Convert.ToInt16(dt.Compute("Sum(CountHist)", null));
            allNum =Convert.ToInt16(dt.Compute("Sum(AllTimes)", null));
            if (allNum == 0 && allSelect==0) //如果根本没有选择记录
            {
                
                for (int i = 0; i < res.Length; i++)
                {
                    res[i] = 80 + rand.Next(20);
                }
            }
            else if (allSelect == 0 && allNum > 0) //如果这一批中被选数为零,而参与选择的数不为零
            {

                for (int j= 0; j < res.Length; j++)
                {
                    res[j] = 40 + 30 + rand.Next(20) + 10 * (Convert.ToInt16(dt.Rows[j][4]) / allNum);
                }

            }
            else //两者都不等于零
            {
                for (int k = 0; k < res.Length; k++)
                {
                    res[k] = 40 * (Convert.ToInt16(dt.Rows[k][2]) / allSelect) + 30 * (Convert.ToInt16(dt.Rows[k][2]) / (Convert.ToInt16(dt.Rows[k][4]) + 1)) + rand.Next(20) + 10 * (Convert.ToInt16(dt.Rows[k][4]) / allNum);
                }
            
            }


            return res;
        
        }


         //把确认的记录反馈过来改写dt,并保存为
        public void SaveHistory(DataTable ansDt,string path)
        {
           //Result 数据是选中人 +1
            //dt为参选人 +1
            //origin 是导入的宣示数据;对其进行累加计算

            //写结果影响
            for (int i = 0; i < ansDt.Rows.Count; i++)
            {
                string condi = String.Format("Name = '{0}'", ansDt.Rows[i][0].ToString());
                int numS;
                numS = Convert.ToInt16(originDt.Select(condi)[0][2]);

                if (path.Split('.').Last()!="txt")
                { numS += 1;
               }

                originDt.Select(condi)[0][2] = numS;
                
            }

            //写全局影响
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string condi = String.Format("Name = '{0}'", dt.Rows[i][0].ToString());
                int  numAll;
                
                numAll = Convert.ToInt16(originDt.Select(condi)[0][4]);

                if (path.Split('.').Last() != "txt")
                {
                   
                    numAll += 1;
                }

               
                originDt.Select(condi)[0][4] = numAll;
            }
            //把dt表写入 文件

            // 如果是密文,则更新保存,如果是明文则提示首先加密
            MessageBox.Show(" 结果已经保存!");
        }

        
       
    }
}
