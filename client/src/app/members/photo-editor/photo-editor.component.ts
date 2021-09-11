import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { Member } from '../../_models/member';
import { Photo } from '../../_models/photo';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member?: Member;
  uploader?: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiBaseUrl;
  user: User | undefined;

  constructor(private accountService: AccountService, private memberService: MembersService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'appusers/add-photo',
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo: Photo = JSON.parse(response);
        this.member?.photos?.push(photo);
        if (photo?.isMain) {
          this.refreshPhotoInfo(photo);
        }
      }
    }
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
      this.refreshPhotoInfo(photo);
    }, error => console.log(error));
  }

  refreshPhotoInfo(photo: Photo) {
      if (this.member && this.user) {
        this.user.photoUrl = photo.url;
        this.accountService.setCurrentUser(this.user);
        this.member.photoUrl = photo.url;
        this.member.photos?.forEach(p => {
          if (p.isMain) p.isMain = false;
          if (p.id == photo.id) p.isMain = true;
        });
      }
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe(
      () => {
        if (this.member)
          this.member.photos = this.member?.photos?.filter(x => x.id !== photoId);
      }
    );
  }

}
