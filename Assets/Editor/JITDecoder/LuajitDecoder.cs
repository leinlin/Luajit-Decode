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
* Filename: LuajitDecoder
* Created:  2017/8/11 15:02:09
* Author:   HaYaShi ToShiTaKa
* Purpose:  
* ==============================================================================
*/
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;

namespace LuaJitDecoder {

    public class LuaFile {
        private Dictionary<string, LuaFunction> m_fileGlobalFunNames = new Dictionary<string, LuaFunction>();
        public void RegistGlobalFun(string addr, LuaFunction fun) {
            if (!m_fileGlobalFunNames.ContainsKey(addr)) {
                m_fileGlobalFunNames[addr] = fun;
            }
        }
        public LuaFunction FindGlobalFun(string addr) {
            LuaFunction result = null;
            m_fileGlobalFunNames.TryGetValue(addr, out result);
            return result;
        }

        private bool CheckIsHeader(string line) {
            return line.Length >= 4 && line.Substring(0, 15) == JitDecoderConst.FUNCTION_HEADER;
        }

        public LuaFile(string path) {
            List<LuaFunction> funs = new List<LuaFunction>();
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            string line;
            LuaFunction fun = null;

            while ((line = sr.ReadLine()) != null) {
                if (CheckIsHeader(line)) {
                    fun = new LuaFunction(this);
                    funs.Add(fun);
                }
                if (fun == null) throw new Exception("this is not a jit asm file");
                fun.AddChunk(line);
            }
            funs[funs.Count - 1].isMain = true;
            for (int i = 0, imax = funs.Count; i < imax; i++) {
                UnityEngine.Debug.Log(funs[i]);
            }
            sr.Dispose();
            fs.Dispose();
        }
    }


    public static class LuajitDecoder {
        [MenuItem("Lua/JitDecoder", false, 6)]
        static private void StartDecoder() {
            LuaFile f = new LuaFile(JitDecoderConst.JIT_PATH + "test.asm");
        }
    }

}
