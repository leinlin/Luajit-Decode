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
* Filename: LuaJitDecoder
* Created:  2017/8/14 13:56:11
* Author:   HaYaShi ToShiTaKa
* Purpose:  
* ==============================================================================
*/
using UnityEngine;

namespace LuaJitDecoder {
    public enum InstEnum {
        /* Comparison ops. ORDER OPR. */
        ISLT,
        ISGE,
        ISLE,
        ISGT,
        ISEQV,
        ISNEV,
        ISEQS,
        ISNES,
        ISEQN,
        ISNEN,
        ISEQP,
        ISNEP,

        /* Unary test and copy ops. */
        ISTC,
        ISFC,
        IST,
        ISF,
        ISTYPE,
        ISNUM,

        /* Unary ops. */
        MOV,
        NOT,
        UNM,
        LEN,

        /* Binary ops. ORDER OPR. VV last, POW must be next. */
        ADDVN,
        SUBVN,
        MULVN,
        DIVVN,
        MODVN,
        ADDNV,
        SUBNV,
        MULNV,
        DIVNV,
        MODNV,

        ADDVV,
        SUBVV,
        MULVV,
        DIVVV,
        MODVV,
        POW,
        CAT,

        /* Constant ops. */
        KSTR,
        KCDATA,
        KSHORT,
        KNUM,
        KPRI,
        KNIL,

        /* Upvalue and function ops. */
        UGET,
        USETV,
        USETS,
        USETN,
        USETP,
        UCLO,
        FNEW,

        /* Table ops. */
        TNEW,
        TDUP,
        GGET,
        GSET,
        TGETV,
        TGETS,
        TGETB,
        TGETR,
        TSETV,
        TSETS,
        TSETB,
        TSETM,
        TSETR,

        /* Calls and vararg handling. T = tail call. */
        CALLM,
        CALL,
        CALLMT,
        CALLT,
        ITERC,
        ITERN,
        VARG,
        ISNEXT,

        /* Returns. */
        RETM,
        RET,
        RET0,
        RET1,

        /* Loops and branches. I/J = interp/JIT, I/C/L = init/call/loop. */
        FORI,
        JFORI,
        FORL,
        IFORL,
        JFORL,
        ITERL,
        IITERL,
        JITERL,
        LOOP,
        ILOOP,
        JLOOP,
        JMP,

        /* Function headers. I/J = interp/JIT, F/V/C = fixarg/vararg/C func. */
        FUNCF,
        IFUNCF,
        JFUNCF,
        FUNCV,
        IFUNCV,
        JFUNCV,
        FUNCC,
        FUNCCW,
    }

    public class JitDecoderConst {
        public static readonly string JIT_PATH = Application.dataPath.Replace("Assets", "") + "JitData/";
        public static readonly string FUNCTION_HEADER = "-- BYTECODE -- ";
    }
}
