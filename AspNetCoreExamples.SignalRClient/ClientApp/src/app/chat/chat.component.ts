import { Component, OnInit } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { authService } from '../authService/auth.service';

class Message {
    private date: Date;
    private user;
    private message;
    constructor(user, message, date) {
        this.user = user;
        this.message = message;
        this.date = new Date(date);
        
    }

    get text() {
        return `${this.dateText} ${this.user.name} : ${this.message}`;
    }

    private get dateText() {
        let day = this.date.getDate().toString().padStart(2, "0");
        let month = (this.date.getMonth() + 1).toString().padStart(2, "0");

        let h = this.date.getHours().toString().padStart(2, "0");
        let m = this.date.getMinutes().toString().padStart(2, "0");
        let s = this.date.getSeconds().toString().padStart(2, "0");

        return `${day}/${month} ${h}:${m}:${s}`;
    }
}

@Component({
    selector: 'app-chat',
    templateUrl: 'chat.component.html'
})
export class ChatComponent implements OnInit {

    message = '';
    messages: Array<Message>;
    private hubConnection: signalR.HubConnection

    constructor(private auth: authService) {
        this.messages = new Array<Message>();

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:16140/chat", { accessTokenFactory: () => this.auth.CurrentUser.token })
            .build();


        this.hubConnection.start().then();


        this.hubConnection.on("OnUserJoined", (data) => { console.log(data) });
        this.hubConnection.on("OnMessageReceived", (data) => {
            let msg = new Message(data.user, data.message, data.date);
            this.messages.push(msg);
            console.log(msg);
        });

    }

    async ngOnInit() {




    }

    async sendMessage(event) {
        event.preventDefault();
        await this.hubConnection.send("say", this.message);
        this.message = '';
    }

    get messagesList() {
        return this.messages;
    }
}