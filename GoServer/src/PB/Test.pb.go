// Code generated by protoc-gen-go. DO NOT EDIT.
// source: Test.proto

package protocol

import (
	fmt "fmt"
	proto "github.com/golang/protobuf/proto"
	math "math"
)

// Reference imports to suppress errors if they are not otherwise used.
var _ = proto.Marshal
var _ = fmt.Errorf
var _ = math.Inf

// This is a compile-time assertion to ensure that this generated file
// is compatible with the proto package it is being compiled against.
// A compilation error at this line likely means your copy of the
// proto package needs to be updated.
const _ = proto.ProtoPackageIsVersion3 // please upgrade the proto package

type TestStruct struct {
	A                    int32    `protobuf:"varint,1,opt,name=a,proto3" json:"a,omitempty"`
	XXX_NoUnkeyedLiteral struct{} `json:"-"`
	XXX_unrecognized     []byte   `json:"-"`
	XXX_sizecache        int32    `json:"-"`
}

func (m *TestStruct) Reset()         { *m = TestStruct{} }
func (m *TestStruct) String() string { return proto.CompactTextString(m) }
func (*TestStruct) ProtoMessage()    {}
func (*TestStruct) Descriptor() ([]byte, []int) {
	return fileDescriptor_7d72138f86b68d38, []int{0}
}

func (m *TestStruct) XXX_Unmarshal(b []byte) error {
	return xxx_messageInfo_TestStruct.Unmarshal(m, b)
}
func (m *TestStruct) XXX_Marshal(b []byte, deterministic bool) ([]byte, error) {
	return xxx_messageInfo_TestStruct.Marshal(b, m, deterministic)
}
func (m *TestStruct) XXX_Merge(src proto.Message) {
	xxx_messageInfo_TestStruct.Merge(m, src)
}
func (m *TestStruct) XXX_Size() int {
	return xxx_messageInfo_TestStruct.Size(m)
}
func (m *TestStruct) XXX_DiscardUnknown() {
	xxx_messageInfo_TestStruct.DiscardUnknown(m)
}

var xxx_messageInfo_TestStruct proto.InternalMessageInfo

func (m *TestStruct) GetA() int32 {
	if m != nil {
		return m.A
	}
	return 0
}

type C2STestMessage struct {
	ID                   int32    `protobuf:"varint,1,opt,name=ID,proto3" json:"ID,omitempty"`
	XXX_NoUnkeyedLiteral struct{} `json:"-"`
	XXX_unrecognized     []byte   `json:"-"`
	XXX_sizecache        int32    `json:"-"`
}

func (m *C2STestMessage) Reset()         { *m = C2STestMessage{} }
func (m *C2STestMessage) String() string { return proto.CompactTextString(m) }
func (*C2STestMessage) ProtoMessage()    {}
func (*C2STestMessage) Descriptor() ([]byte, []int) {
	return fileDescriptor_7d72138f86b68d38, []int{1}
}

func (m *C2STestMessage) XXX_Unmarshal(b []byte) error {
	return xxx_messageInfo_C2STestMessage.Unmarshal(m, b)
}
func (m *C2STestMessage) XXX_Marshal(b []byte, deterministic bool) ([]byte, error) {
	return xxx_messageInfo_C2STestMessage.Marshal(b, m, deterministic)
}
func (m *C2STestMessage) XXX_Merge(src proto.Message) {
	xxx_messageInfo_C2STestMessage.Merge(m, src)
}
func (m *C2STestMessage) XXX_Size() int {
	return xxx_messageInfo_C2STestMessage.Size(m)
}
func (m *C2STestMessage) XXX_DiscardUnknown() {
	xxx_messageInfo_C2STestMessage.DiscardUnknown(m)
}

var xxx_messageInfo_C2STestMessage proto.InternalMessageInfo

func (m *C2STestMessage) GetID() int32 {
	if m != nil {
		return m.ID
	}
	return 0
}

type S2CTestMessage struct {
	Sz                   string        `protobuf:"bytes,1,opt,name=sz,proto3" json:"sz,omitempty"`
	LongID               int64         `protobuf:"varint,2,opt,name=longID,proto3" json:"longID,omitempty"`
	TestInts             []int32       `protobuf:"varint,3,rep,packed,name=testInts,proto3" json:"testInts,omitempty"`
	TestStructs          []*TestStruct `protobuf:"bytes,4,rep,name=testStructs,proto3" json:"testStructs,omitempty"`
	XXX_NoUnkeyedLiteral struct{}      `json:"-"`
	XXX_unrecognized     []byte        `json:"-"`
	XXX_sizecache        int32         `json:"-"`
}

func (m *S2CTestMessage) Reset()         { *m = S2CTestMessage{} }
func (m *S2CTestMessage) String() string { return proto.CompactTextString(m) }
func (*S2CTestMessage) ProtoMessage()    {}
func (*S2CTestMessage) Descriptor() ([]byte, []int) {
	return fileDescriptor_7d72138f86b68d38, []int{2}
}

func (m *S2CTestMessage) XXX_Unmarshal(b []byte) error {
	return xxx_messageInfo_S2CTestMessage.Unmarshal(m, b)
}
func (m *S2CTestMessage) XXX_Marshal(b []byte, deterministic bool) ([]byte, error) {
	return xxx_messageInfo_S2CTestMessage.Marshal(b, m, deterministic)
}
func (m *S2CTestMessage) XXX_Merge(src proto.Message) {
	xxx_messageInfo_S2CTestMessage.Merge(m, src)
}
func (m *S2CTestMessage) XXX_Size() int {
	return xxx_messageInfo_S2CTestMessage.Size(m)
}
func (m *S2CTestMessage) XXX_DiscardUnknown() {
	xxx_messageInfo_S2CTestMessage.DiscardUnknown(m)
}

var xxx_messageInfo_S2CTestMessage proto.InternalMessageInfo

func (m *S2CTestMessage) GetSz() string {
	if m != nil {
		return m.Sz
	}
	return ""
}

func (m *S2CTestMessage) GetLongID() int64 {
	if m != nil {
		return m.LongID
	}
	return 0
}

func (m *S2CTestMessage) GetTestInts() []int32 {
	if m != nil {
		return m.TestInts
	}
	return nil
}

func (m *S2CTestMessage) GetTestStructs() []*TestStruct {
	if m != nil {
		return m.TestStructs
	}
	return nil
}

func init() {
	proto.RegisterType((*TestStruct)(nil), "protocol.TestStruct")
	proto.RegisterType((*C2STestMessage)(nil), "protocol.C2STestMessage")
	proto.RegisterType((*S2CTestMessage)(nil), "protocol.S2CTestMessage")
}

func init() { proto.RegisterFile("Test.proto", fileDescriptor_7d72138f86b68d38) }

var fileDescriptor_7d72138f86b68d38 = []byte{
	// 183 bytes of a gzipped FileDescriptorProto
	0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xff, 0xe2, 0xe2, 0x0a, 0x49, 0x2d, 0x2e,
	0xd1, 0x2b, 0x28, 0xca, 0x2f, 0xc9, 0x17, 0xe2, 0x00, 0x53, 0xc9, 0xf9, 0x39, 0x4a, 0x52, 0x10,
	0xf1, 0xe0, 0x92, 0xa2, 0xd2, 0xe4, 0x12, 0x21, 0x1e, 0x2e, 0xc6, 0x44, 0x09, 0x46, 0x05, 0x46,
	0x0d, 0xd6, 0x20, 0xc6, 0x44, 0x25, 0x05, 0x2e, 0x3e, 0x67, 0xa3, 0x60, 0x90, 0xb4, 0x6f, 0x6a,
	0x71, 0x71, 0x62, 0x7a, 0xaa, 0x10, 0x1f, 0x17, 0x93, 0xa7, 0x0b, 0x54, 0x01, 0x93, 0xa7, 0x8b,
	0x52, 0x0f, 0x23, 0x17, 0x5f, 0xb0, 0x91, 0x33, 0x9a, 0x92, 0xe2, 0x2a, 0xb0, 0x12, 0xce, 0x20,
	0xa6, 0xe2, 0x2a, 0x21, 0x31, 0x2e, 0xb6, 0x9c, 0xfc, 0xbc, 0x74, 0x4f, 0x17, 0x09, 0x26, 0x05,
	0x46, 0x0d, 0xe6, 0x20, 0x28, 0x4f, 0x48, 0x8a, 0x8b, 0xa3, 0x24, 0xb5, 0xb8, 0xc4, 0x33, 0xaf,
	0xa4, 0x58, 0x82, 0x59, 0x81, 0x59, 0x83, 0x35, 0x08, 0xce, 0x17, 0x32, 0xe3, 0xe2, 0x2e, 0x81,
	0x3b, 0xaa, 0x58, 0x82, 0x45, 0x81, 0x59, 0x83, 0xdb, 0x48, 0x44, 0x0f, 0xe6, 0x68, 0x3d, 0x84,
	0x8b, 0x83, 0x90, 0x15, 0x26, 0xb1, 0x81, 0x55, 0x18, 0x03, 0x02, 0x00, 0x00, 0xff, 0xff, 0x08,
	0x49, 0x6b, 0x74, 0xeb, 0x00, 0x00, 0x00,
}