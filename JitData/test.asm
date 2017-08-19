-- BYTECODE -- all.lua:0-0
0001    KSHORT   0 123
0002    GSET     0   0      ; "a"
0003    GGET     0   1      ; "print"
0004    GGET     1   0      ; "a"
0005    CALL     0   1   2
0006    RET0     0   1

-- BYTECODE -- all.lua:0-0
0001    GGET     0   0      ; "print"
0002    KSTR     1   1      ; "你好"
0003    CALL     0   1   2
0004    FNEW     0   2      ; all.lua:0
0005    GSET     0   3      ; "test"
0006    RET0     0   1

