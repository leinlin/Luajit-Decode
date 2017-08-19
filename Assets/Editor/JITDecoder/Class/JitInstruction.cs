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
* Filename: JitInstruction
* Created:  2017/8/12 18:57:00
* Author:   HaYaShi ToShiTaKa
* Purpose:  
* ==============================================================================
*/
using System;
using System.Collections.Generic;

namespace LuaJitDecoder {
    public class JitInstruction {

        #region member
        private string m_lineStr = "";

        private int m_line;
        private bool m_isHead = false;
        private bool m_isEnd = false;

        private InstEnum m_action;
        private List<int> m_args = new List<int>();
        private string m_comment;
        #endregion

        #region property
        public string lineStr {
            get {
                return m_lineStr;
            }
        }
        public bool isEnd {
            get {
                return m_isEnd;
            }
        }
        public bool isHead {
            get {
                return m_isHead;
            }
        }
        public int line {
            get {
                return m_line;
            }
        }
        public InstEnum action {
            get {
                return m_action;
            }
        }
        public List<int> args {
            get {
                return m_args;
            }
        }
        public string comment {
            get {
                return m_comment;
            }
        }
        #endregion

        #region ctor
        public JitInstruction(string inst) {
            m_lineStr = inst;
            inst = inst.Replace("=>", "");
            if (string.IsNullOrEmpty(inst)) {
                m_isEnd = true;
                return;
            }

            if (inst.Substring(0, 15) == JitDecoderConst.FUNCTION_HEADER) {
                m_isHead = true;
                return;
            }

            m_line = int.Parse(ReadOneArg(ref inst));
            m_action = (InstEnum)Enum.Parse(typeof(InstEnum), ReadOneArg(ref inst));

            m_args.Clear();
            while (!string.IsNullOrEmpty(inst)) {
                if (inst[0] == ';') {
                    m_comment = inst.Substring(2, inst.Length - 2);
                    break;
                }
                string str = ReadOneArg(ref inst);
                int arg = int.Parse(str);
                m_args.Add(arg);
            }
        }

        private string ReadOneArg(ref string inst) {
            string[] result = null;
            result = inst.Split(new char[] { ' ' }, 2);
            //最后一个参数不存在
            if (result.Length > 1) {
                inst = result[1].TrimStart();
            }
            else {
                inst = string.Empty;
            }
            return result[0];
        }
        #endregion
    }
}
