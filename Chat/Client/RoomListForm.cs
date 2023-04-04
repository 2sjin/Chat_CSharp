﻿using Core;
using System.Net.Sockets;

namespace Client;

public partial class RoomListForm : Form {
    public RoomListForm() {
        InitializeComponent();
        Singleton.Instance.CreateRoomResponsed += CreateRoomResponsed;  // 방 생성 응답 이벤트 추가
        Singleton.Instance.RoomListResponsed += CreateRoomResponsed;    // 방 목록 응답 이벤트 추가

        FormClosing += (sender, e) => {
            Singleton.Instance.CreateRoomResponsed -= CreateRoomResponsed;      // 방 생성 응답 이벤트 제거
            Singleton.Instance.RoomListResponsed -= CreateRoomResponsed;        // 방 목록 응답 이벤트 제거
        };

        // Form이 활성화될 때마다 방 목록 새로고침
        Activated += (sender, e) => {
            btnRefresh_Click(null, EventArgs.Empty);
        };
    }

    // 채팅방 만들기 버튼 클릭
    private async void btnCreate_Click(object sender, EventArgs e) {
        // 텍스트박스 빈칸 확인
        if (string.IsNullOrEmpty(tbRoomName.Text)) {
            MessageBox.Show("방 이름을 입력하세요.", this.Text);
            return;
        }

        // 방 생성 요청 패킷을 서버에 전송함
        CreateRoomRequestPacket packet = new CreateRoomRequestPacket(tbRoomName.Text);      // 패킷 생성
        await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);    // 패킷 직렬화 및 전송
    }

    // 채팅방 입장하기 버튼 클릭
    private async void btnEnter_Click(object sender, EventArgs e) {
        // 목록에서 선택된 채팅방이 없으면 리턴
        if (listBoxRooms.SelectedItem == null)
            return;

        // 채팅방 입장
        ChatRoomForm chatRoom = new ChatRoomForm();
        chatRoom.Text = listBoxRooms.SelectedItem.ToString();
        chatRoom.ShowDialog();
    }

    // 새로고침 버튼 클릭
    private async void btnRefresh_Click(object sender, EventArgs e) {
        // 리스트박스 비우기
        listBoxRooms.Items.Clear();

        // 방 목록 요청 패킷을 서버에 전송함
        RoomListRequestPacket packet = new RoomListRequestPacket();                         // 패킷 생성
        await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);    // 패킷 직렬화 및 전송
    }

    // 방 생성 응답 이벤트
    private void CreateRoomResponsed(object? sender, EventArgs e) {
        CreateRoomResponsePacket packet = (CreateRoomResponsePacket)sender!;

        // 로그인 성공 패킷을 응답받은 경우
        if (packet.ResponseCode == 200) {
            string roomName = tbRoomName.Text;

            // 리스트박스에 방 추가
            listBoxRooms.Items.Add(roomName);
            tbRoomName.Text = null;

            // 채팅방 입장
            ChatRoomForm chatRoom = new ChatRoomForm();
            chatRoom.Text = roomName;
            chatRoom.ShowDialog();
        }

        // 로그인 실패 패킷을 응답받은 경우
        else {
            MessageBox.Show("로그인 실패 (Code " + packet.ResponseCode.ToString() + ")", this.Text);
            Singleton.Instance.Socket.Shutdown(SocketShutdown.Send);    // Send 스트림 연결 종료(Receive는 가능)
        }
    }

    // 방 목록 응답 이벤트
    private void RoomListResponsed(object? sender, EventArgs e) {
        RoomListResponsePacket packet = (RoomListResponsePacket)sender!;

        // 리스트박스 새로고침
        foreach (var item in packet.RoomNames) {
            listBoxRooms.Items.Add(item);
        }

    }
}
