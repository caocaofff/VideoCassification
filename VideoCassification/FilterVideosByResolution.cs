using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VideoCassification
{
    public partial class FilterVideosByResolution : Form
    {
        public FilterVideosByResolution()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string DirPath = @"X:\tmp";
            string[] files = Directory.GetFiles(DirPath , "*.mp4");
            foreach (string file in files)
            {
                label1.Text = file + " --> " ;
                MovMove (file);
            }
            
        }
        

        public static void MovMove(string movfilepath)
        {
            int? movW, movH;
            GetMovWidthAndHeight(movfilepath, out movW, out movH);
            if (movW != null && movH != null)
            {
                string destDir = @"Y:\" + movW.ToString() + "x" + movH.ToString();
                string destFile = destDir + @"\" + Path.GetFileName(movfilepath);
                if (!System.IO.Directory.Exists(destDir))
                {
                    System.IO.Directory.CreateDirectory(destDir);
                }
                File.Move(movfilepath, destFile);
            }
        }



        /// <summary>

        /// 执行一条command命令

        /// </summary>

        /// <param name="command">需要执行的Command</param>

        /// <param name="output">输出</param>

        /// <param name="error">错误</param>

        public static void ExecuteCommand(string command, out string output, out string error)

        {

            try

            {

                //创建一个进程

                Process pc = new Process();

                pc.StartInfo.FileName = command;

                pc.StartInfo.UseShellExecute = false;

                pc.StartInfo.RedirectStandardOutput = true;

                pc.StartInfo.RedirectStandardError = true;

                pc.StartInfo.CreateNoWindow = true;
                //启动进程

                pc.Start();
                //准备读出输出流和错误流

                string outputData = string.Empty;

                string errorData = string.Empty;

                pc.BeginOutputReadLine();

                pc.BeginErrorReadLine();



                pc.OutputDataReceived += (ss, ee) =>

                {

                    outputData += ee.Data;

                };
                pc.ErrorDataReceived += (ss, ee) =>

                {

                    errorData += ee.Data;

                };



                //等待退出

                pc.WaitForExit();
                //关闭进程

                pc.Close();
                //返回流结果

                output = outputData;

                error = errorData;

            }

            catch (Exception)

            {

                output = null;

                error = null;

            }

        }


        /// <summary>

        /// 获取视频的帧宽度和帧高度

        /// </summary>

        /// <param name="videoFilePath">mov文件的路径</param>

        /// <returns>null表示获取宽度或高度失败</returns>

        public static void GetMovWidthAndHeight(string videoFilePath, out int? width, out int? height)

        {

            try

            {

                //判断文件是否存在

                if (!File.Exists(videoFilePath))

                {

                    width = null;

                    height = null;

                }
                //执行命令获取该文件的一些信息 

                string ffmpegPath = new FileInfo(Process.GetCurrentProcess().MainModule.FileName).DirectoryName + @"\ffmpeg.exe";
                string output;

                string error;

                ExecuteCommand("\"" + ffmpegPath + "\"" + " -i " + "\"" + videoFilePath + "\"", out output, out error);

                if (string.IsNullOrEmpty(error))

                {

                    width = null;

                    height = null;

                }
                //通过正则表达式获取信息里面的宽度信息

                Regex regex = new Regex("(\\d{2,4})x(\\d{2,4})", RegexOptions.Compiled);

                Match m = regex.Match(error);

                if (m.Success)

                {

                    width = int.Parse(m.Groups[1].Value);

                    height = int.Parse(m.Groups[2].Value);

                }

                else

                {

                    width = null;

                    height = null;

                }

            }

            catch (Exception)

            {

                width = null;

                height = null;

            }

        }

    }
}
