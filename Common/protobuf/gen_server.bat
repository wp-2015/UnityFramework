cd gentools

protoc-go --go_out=../../../project-s-server/src/server_core/src/commonmsg/protocol/ -I ../proto/ ../proto/*.proto

protoc-go --go_out=plugins=grpc:../../../project-s-server/src/server_core/src/commonmsg/protocol/ ../proto/*.proto -I ../proto/

pause