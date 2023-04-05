﻿using Core;
using System.Net.Sockets;

namespace Client {
    public partial class ChatRoomForm : Form {
        public ChatRoomForm() {
            InitializeComponent();

            listBoxUsers.Items.Add(Singleton.Instance.Nickname);

            // 이벤트 핸들러에 메소드 추가
            Singleton.Instance.UserEnterResponsed += UserEnterResponsed;  // 유저 입장 메소드 추가
            Singleton.Instance.UserLeaveResponsed += UserLeaveResponsed;  // 유저 퇴장 메소드 추가

            // Form 종료 시, 이벤트 핸들러에 메소드 제거
            FormClosing += async (sender, e) => {
                Singleton.Instance.UserEnterResponsed -= UserEnterResponsed;      // 유저 입장 메소드 제거
                Singleton.Instance.UserLeaveResponsed -= UserLeaveResponsed;      // 유저 퇴장 메소드 제거

                // 유저 퇴장 패킷을 서버에 전송함
                UserLeavePacket packet = new UserLeavePacket(Singleton.Instance.Nickname);          // 패킷 생성
                await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);    // 패킷 직렬화 및 전송

                // Send 스트림 연결 종료(Receive는 가능)
                Singleton.Instance.Socket.Shutdown(SocketShutdown.Send);
            };
        }

        // 유저 입장 이벤트
        private void UserEnterResponsed(object? sender, EventArgs e) {
            UserEnterPacket packet = (UserEnterPacket)sender!;
            listBoxUsers.Items.Add(packet.Nickname);
        }

        // 유저 퇴장 이벤트
        private void UserLeaveResponsed(object? sender, EventArgs e) {
            UserLeavePacket packet = (UserLeavePacket)sender!;
            listBoxUsers.Items.Remove(packet.Nickname);
        }
    }
}
