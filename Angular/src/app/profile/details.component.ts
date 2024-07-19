﻿import { Component, Injector } from '@angular/core';

import { AlertService } from '@app/_services';
import { AppComponentBase } from 'src/shared/common/app-component-base';

@Component({ templateUrl: 'details.component.html' })
export class DetailsComponent extends AppComponentBase {
    counter: number = 1;
    imgURL: string = 'https://mdbootstrap.com/img/Photos/Others/placeholder-avatar.jpg';
    imageSrc: string = 'https://mdbootstrap.com/img/Photos/Others/placeholder-avatar.jpg';
    constructor(injector: Injector,
        private alertService: AlertService) { 
            super(injector);
        }

    showSuccess() {
        this.counter++;
        var msg = this.counter %  3 == 0 ? 'This is a short message ' + this.counter : 'This was a long message but there is also a bit more to this story. And here it iss also a bit more to this story. And here it is: ' + this.counter;
        this.alertService.success(msg);
    }
    showWarn() {
        this.counter++;
        var msg = this.counter %  3 == 0 ? 'This is a short message ' + this.counter : 'This was a long message but there is also a bit more to this story. And here it iss also a bit more to this story. And here it is: ' + this.counter;
        this.alertService.warn(msg);
    }
    showInfo() {
        this.counter++;
        var msg = this.counter %  3 == 0 ? 'This is a short message ' + this.counter : 'This was a long message but there is also a bit more to this story. And here it iss also a bit more to this story. And here it is: ' + this.counter;
        this.alertService.info(msg);
    }
    showError() {
        this.counter++;var msg = this.counter %  3 == 0 ? 'This is a short message ' + this.counter : 'This was a long message but there is also a bit more to this story. And here it iss also a bit more to this story. And here it is: ' + this.counter;
        this.alertService.error(msg);
    }

    imgFileSelected(event: any) {
        if (event.target.files && event.target.files[0]) {
          this.imageSrc = URL.createObjectURL(event.target.files[0]);
        }
      }
      preview(files : any) {
        if (files.length === 0)
        {
            console.log('No file selected');
          return;
        }
     
        var mimeType = files[0].type;
        if (mimeType.match(/image\/*/) == null) {
          //todo: this.message = "Only images are supported.";
          console.log('Only images are supported.');
          return;
        }
     
        var reader = new FileReader();
        //this.imagePath = files;
        reader.readAsDataURL(files[0]); 
        reader.onload = this.handleReaderLoaded.bind(this);
      }

      handleReaderLoaded(readerEvt: any) {
        this.imgURL = readerEvt.target.result;
        console.log(this.imgURL);
      }

    // readURL(event: any): void {
    //     if (event.target.files && event.target.files[0]) {
    //         const file = event.target.files[0];
    
    //         const reader = new FileReader();
    //         reader.onload = e => this.imageSrc = reader.result;
    
    //         reader.readAsDataURL(file);
    //     }
    // }

    // onFileSelected(event : any) {

    //     console.log(event);
    //     const file:File = event.target.files[0];

    //     if (file) {

    //         this.profileImgSrc = file.name;

    //         //TODO: 
    //         const formData = new FormData();
    //         formData.append("thumbnail", file);
    //       //  const upload$ = this.http.post("/api/thumbnail-upload", formData);
    //        // upload$.subscribe();
    //     }
    // }

    // profileImageSelected(event: any) {
    //     const fileInput = event.target;
    
    //     if (fileInput.files && fileInput.files[0]) {
    //         const reader = new FileReader();
    
    //         reader.onload = function(e: any) {
    //             this.profileImgSrc = e.target.result;
    //         };
    
    //         reader.readAsDataURL(fileInput.files[0]);
    //     }
    // }
    
}