import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { ConfirmService } from '../_services/confirm.service';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  pagination?: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  constructor(private messageService: MessageService, private confirmService: ConfirmService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  deleteMessage(id: number) {
    this.confirmService.confirm('Confirm delete message', "This can't be undone")
      .subscribe(result => {
        if (result) {
            this.messageService.deleteMessage(id).subscribe(res => {
            this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
            })
         }
      })
  }

  loadMessages() {
    this.loading = true;
    this.messageService.gentMessages(this.pageNumber, this.pageSize, this.container).subscribe(res => {
        this.messages = res?.result as Message[];
      this.pagination = res.pagination;
      this.loading = false;
    })
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadMessages();
  }
}
