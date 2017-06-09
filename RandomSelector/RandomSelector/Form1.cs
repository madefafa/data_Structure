using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace RandomSelector
{
    public partial class Form1 : Form
    {
        private string path = "";
        private string[] info;
        private char[] seleCond = new char[6] { '0', '0', '0', '0', '0', '0',};
        private ArrayList arrL;
        private DataTable ansDt;
        MemberInfo mi;
        private string selectNames = "";
        private int allNums;
        private string[] nameList;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                path = ofd.FileName;
            }
            //路径显示
            textBox1.Text = path;
            //下面要进行信息的显示

            if (path.Split('.').Last() == "zy")
            {
                //检测是否有同名密钥
                if (!File.Exists(path.Split('.').First() + ".keys"))
                {

                    MessageBox.Show("该文件密钥并不存在,请联系管理员!");
                    return;
                }

                else
                { mi.ReadMemberZy(path); }
            }
            //如果使用加密过的数据.zy格式则需要解密

            //解密以后

            else //如果是非.zy文件
            { mi.ReadMemberDat(path); }
            
            //读完之后反馈
            MessageBox.Show("数据导入完毕!");
            button1.Enabled = false;
            info= mi.GetMemberInfo();
             //把info书写到Label标签上
            string infoTxt = String.Format("{0},共有成员{1}名,其中男性{2}名,女性{3}名;\n担任领导职务(主任以上)的有{4}名,该组织共有{5}个课题组;支部委员人数为{6}人;\n该点名系统之前使用过{7}次,最后一次的使用时间为:{8}", info[0], info[3], info[4], Convert.ToInt16(info[3]) - Convert.ToInt16(info[4]), info[5], info[6], info[7], info[1], info[2]);
            label7.Text=  infoTxt;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            mi = new MemberInfo();
            numericUpDown1.Enabled = false;
            
            groupBox1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
        }

        //全体都有互斥事件行为
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked==true) //如果选中,其他checkbox全部取消
            {
                seleCond[0] = '1';
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
            }
            else
            {
                seleCond[0] = '0';
            }

            selectNamesProc(seleCond);
            
            
        }

        //互斥事件
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                seleCond[1] = '1';
                checkBox1.Checked = false;
            }
            else
            {
                seleCond[1] = '0';
            }

            selectNamesProc(seleCond);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                seleCond[2] = '1';
                checkBox1.Checked = false;
            }
            else
            {
                seleCond[2] = '0';
            }

            selectNamesProc(seleCond);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                seleCond[3] = '1';
                checkBox1.Checked = false;
            }
            else
            {
                seleCond[3] = '0';
            }

            selectNamesProc(seleCond);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                seleCond[4] = '1';
                checkBox1.Checked = false;
                checkBox6.Checked = false;
            }
            else
            {
                seleCond[4] = '0';
            }

            selectNamesProc(seleCond);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                seleCond[5] = '1';
                checkBox1.Checked = false;
                checkBox5.Checked = false;
            }
            else
            {
                seleCond[5] = '0';
            }

            selectNamesProc(seleCond);

        }

        //对selectNames进行处理计数
        public void selectNamesProc(char[] seleCond)
        {
            selectNames = mi.CalcSelNum(seleCond);
            if (selectNames == "all")
            {
                allNums = Convert.ToInt16(info[3]);
                nameList = new string[1] { "all" }; 
           }

            else if (selectNames == "none")
            {
                allNums = 0;
                nameList = new string[0];
            }

            else {allNums = selectNames.Split(',').Count();
            nameList = selectNames.Split(','); }

            numericUpDown1.Enabled = true;
            numericUpDown1.Maximum = allNums;
            label8.Text = "之" + allNums + "人";
            button2.Enabled = true;
           

            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }


        //生成名单的函数
        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            { ansDt = mi.DoSelectWithHistory(selectNames, (int)numericUpDown1.Value);
            button3.Enabled = true;
            }
            else
            {
                ansDt = mi.DoSelectWithoutHis(selectNames, (int)numericUpDown1.Value);
                button3.Enabled = false;
            }
            
            //把结果的数据表打印输出
           richTextBox1.Text = "姓名\t选中次数\t参选次数\n";
           for (int i = 0; i < ansDt.Rows.Count; i++)
           {
               richTextBox1.Text += ansDt.Rows[i][0].ToString() + "\t\t" + ansDt.Rows[i][2].ToString() + "\t" + ansDt.Rows[i][4].ToString() + "\n";
           }

           
        }

        private void button3_Click(object sender, EventArgs e)
        {

           mi.SaveHistory(ansDt,path);
           button5.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClassCryptography cce = new ClassCryptography();
            cce.Encrypted(textBox1.Text, MemberInfo.originDt,info);
        }







        //
    }
}
