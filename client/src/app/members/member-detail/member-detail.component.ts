import { Variable } from '@angular/compiler/src/render3/r3_ast';
import { ViewChild } from '@angular/core';
import { OnDestroy } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryModule, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from '../../_models/member';
import { Message } from '../../_models/message';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  galleryOptions: NgxGalleryOptions[] | undefined;
  galleryImages: NgxGalleryImage[] | undefined;
  member: Member | undefined;
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;
  constructor(public presence: PresenceService, private route: ActivatedRoute, private messageService: MessageService, private accountService: AccountService
  , private router: Router  ) {
    accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
    router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    });
    this.route.queryParams.subscribe(res => {
      res.tab ? this.selectTab(res.tab) : this.selectTab(0);
    })
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
    this.galleryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    for (const photo of this.member?.photos ?? []) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      })
    }
    return imageUrls;
  }


  selectTab(tabId: number) {
      this.memberTabs?.tabs[tabId]?.active ?? true;
  }

      loadMessages() {
        this.messageService.getMessageThread(this.member?.userName).subscribe(messages => {
          this.messages = messages;
        })
      }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0 && this.user && this.member?.userName) {
      this.messageService.createHubConnectoin(this.user, this.member?.userName);
    }
    else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
