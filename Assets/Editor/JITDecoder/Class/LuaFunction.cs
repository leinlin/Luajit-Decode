/*
               #########                       
              ############                     
              #############                    
             ##  ###########                   
            ###  ###### #####                  
            ### #######   ####                 
           ###  ########## ####                
          ####  ########### ####               
         ####   ###########  #####             
        #####   ### ########   #####           
       #####   ###   ########   ######         
      ######   ###  ###########   ######       
     ######   #### ##############  ######      
    #######  #####################  ######     
    #######  ######################  ######    
   #######  ###### #################  ######   
   #######  ###### ###### #########   ######   
   #######    ##  ######   ######     ######   
   #######        ######    #####     #####    
    ######        #####     #####     ####     
     #####        ####      #####     ###      
      #####       ###        ###      #        
        ###       ###        ###               
         ##       ###        ###               
__________#_______####_______####______________

                我们的未来没有BUG              
* ==============================================================================
* Filename: LuaFunction
* Created:  2017/8/14 13:58:01
* Author:   HaYaShi ToShiTaKa
* Purpose:  
* ==============================================================================
*/
using System;
using System.Text;
using System.Collections.Generic;

namespace LuaJitDecoder {
    public class LuaFunction {
        public class InstLine {
            public string lineStr { get; set; }
            private bool m_isNeed = true;
            public bool isNeed {
                get {
                    return m_isNeed;
                }
            }
            public InstLine(string lineStr) {
                this.lineStr = lineStr;
            }
            public void MarkNotNeed() {
                m_isNeed = false;
            }
        }

        private Dictionary<int, string> m_varsName = new Dictionary<int, string>();
        private Dictionary<int, JitInstruction> m_varsInsts = new Dictionary<int, JitInstruction>();
        private List<InstLine> m_lineStrList = new List<InstLine>();

        #region member
        private List<LuaFunction> m_childFuns = new List<LuaFunction>();
        private LuaFile m_file;
        private List<JitInstruction> m_insts = new List<JitInstruction>();
        private string m_name = "";
        private bool m_isMain = false;
        private int m_inputNum = -1;
        public bool isMain {
            get { return m_isMain; }
            set { m_isMain = value; }
        }
        #endregion

        #region public
        public LuaFunction(LuaFile file) {
            m_file = file;
        }
        public void SetName(string name) {
            m_name = name;
        }
        public void AddChunk(string line) {
            int len = m_insts.Count - 1;
            JitInstruction ji = new JitInstruction(line);

            if (ji.isHead && !m_isMain) {
                string addr = ji.lineStr.Replace(JitDecoderConst.FUNCTION_HEADER, "").Split('-')[0];
                m_file.RegistGlobalFun(addr, this);
            }

            if (len >= 0 && m_insts[len].action == InstEnum.FNEW && ji.action == InstEnum.GSET) {
                LuaFunction childFun = m_file.FindGlobalFun(m_insts[len].comment);
                if (childFun == null) throw new Exception("can't be null");
                childFun.SetName(ji.comment.Replace("\"", ""));
            }
            m_insts.Add(ji);
        }
        private void Clear() {
            m_varsName.Clear();
            m_varsInsts.Clear();
            m_childFuns.Clear();
            m_lineStrList.Clear();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            Clear();

            for (int i = 0, imax = m_insts.Count; i < imax; i++) {
                JitInstruction ji = m_insts[i];
                InstLine il;
                if (ji.isHead) {
                    string head = string.Empty;
                    if (string.IsNullOrEmpty(m_name)) {
                        m_name = ji.lineStr.Replace(JitDecoderConst.FUNCTION_HEADER, "");
                        head = "local function " + m_name + "({0})";
                    }
                    else {
                        head = "function " + m_name + "({0})";
                    }
                    il = new InstLine(head);
                    if (m_isMain) il.MarkNotNeed();
                    m_lineStrList.Add(il);
                }
                else if (ji.isEnd) {
                    il = new InstLine("end");
                    if (m_isMain) il.MarkNotNeed();
                    m_lineStrList.Add(il);
                }
                else {
                    m_lineStrList.Add(TranslateInst(i));
                }
            }


            if (m_lineStrList.Count <= 0) throw new Exception("无效的函数 bytecode");
            string input = string.Empty;
            for (int i = 0; i < m_inputNum; i++) {
                if (i != m_inputNum - 1) {
                    input += string.Format("input{0},", i);
                }
                else {
                    input += string.Format("input{0}", i);
                }
            }
            m_lineStrList[0].lineStr = string.Format(m_lineStrList[0].lineStr, input);


            for (int i = 0, imax = m_lineStrList.Count;i< imax; i++) {
                if (m_lineStrList[i].isNeed) {
                    sb.AppendLine(m_lineStrList[i].lineStr);
                }
            }
            return sb.ToString();
        }
        #endregion

        private bool CheckIsAssignAction(JitInstruction ji) {
            return ji.action.ToString().Substring(0, 1) == "K" ||
                    ji.action.ToString().Contains("GET");
        }

        public InstLine TranslateInst(int line) {
            JitInstruction ji = m_insts[line];

            string result = ji.lineStr;

            // 赋值语句
            if (m_inputNum == -1 && CheckIsAssignAction(ji)) {
                if (ji.args.Count <= 0) throw new Exception("赋值语句参数个数必须大于 0");
                m_inputNum = ji.args[0];
                for (int i = 0; i < m_inputNum; i++) {
                    m_varsName.Add(i, string.Format("input{0}", i));
                    m_varsInsts.Add(i, null);
                }
            }

            switch (ji.action) {
                case InstEnum.GGET:
                    result = DoGGET(line);
                    break;
                case InstEnum.KSTR:
                    result = DoKSTR(line);
                    break;
                case InstEnum.CALL:
                    result = DoCall(line);
                    break;
            }

            return new InstLine(result);
        }

        private bool GetLocalArgName(int index, JitInstruction ji, out string result) {
            bool ret = false;
            result = string.Empty;

            if (!m_varsName.ContainsKey(index)) {
                if (ji.action == InstEnum.KSTR) {
                    result = string.Format("str{0}", index);
                }
                else {
                    result = string.Format("var{0}", index);
                }
                m_varsName.Add(index, result);
                m_varsInsts.Add(index, ji);
                ret = true;
            }
            else {
                result = m_varsName[index];
            }
            return ret;
        }
        private string DoGGET(int line) {
            string result = "";
            JitInstruction ji = m_insts[line];
            string varStr;
            bool isFirst = GetLocalArgName(ji.args[0], ji, out varStr);
            if (isFirst) {
                varStr = "local " + varStr;
            }
            result = string.Format("{0} = {1}", varStr, ji.comment.Replace("\"", ""));
            return result;
        }
        private string DoKSTR(int line) {
            string result = "";
            JitInstruction ji = m_insts[line];
            string varStr;
            bool isFirst = GetLocalArgName(ji.args[0], ji, out varStr);
            if (isFirst) {
                varStr = "local " + varStr;
            }
            result = string.Format("{0} = {1}", varStr, ji.comment);
            return result;
        }
        private string DoCall(int line) {
            string result = "";
            JitInstruction ji = m_insts[line];
            JitInstruction funJi = m_varsInsts[ji.args[0]];

            m_lineStrList[funJi.line].MarkNotNeed();
            string funName = funJi.comment.Replace("\"", "");
            string args = "";
            string varStr = "";
            int delta = ji.line - funJi.line;

            for (int i = funJi.line + 1; i < ji.line; i++) {
                JitInstruction tmpJi = m_insts[i];
                if (tmpJi.action.ToString().Substring(0, 1) == "K") {
                    varStr = tmpJi.comment;
                    m_lineStrList[i].MarkNotNeed();
                }
                else if (tmpJi.action.ToString() == "GGET") {
                    varStr = tmpJi.comment.Replace("\"", "");
                    m_lineStrList[i].MarkNotNeed();
                }
                else {
                    GetLocalArgName(tmpJi.args[0], tmpJi, out varStr);
                }

                args += varStr;
                if (i != ji.line - 1) {
                    args += ",";
                }
            }
            result = string.Format("{0}({1})", funName, args);

            return result;
        }
    }
}