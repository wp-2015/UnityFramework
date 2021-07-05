cd gentools
protoc-go --go_out=../../../GoServer/src/PB/ -I ../proto/ ../proto/*.proto
pause