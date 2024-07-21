import { Component, Injector, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { finalize, first } from 'rxjs/operators';

import { AlertService } from '@app/_services';
import {
  ProfileClient,
  ProfilePersonalUpdateRequest,
  ProfileUpdateRequest,
} from 'src/shared/service-clients/service-clients';
import { AppComponentBase } from 'src/shared/common/app-component-base';
import { AuthService } from '@app/_services/auth.service';

@Component({
  templateUrl: 'profile-personal.component.html',
  styleUrl: 'profile-personal.component.scss',
})
export class ProfilePersonalComponent
  extends AppComponentBase
  implements OnInit
{
  imageSrc: string = 'https://mdbootstrap.com/img/Photos/Others/placeholder-avatar.jpg';
  form!: FormGroup;
  submitting = false;
  submitted = false;

  constructor(
    injector: Injector,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private profileClient: ProfileClient,
    private alertService: AlertService,
    private authService: AuthService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      title: [this.account!.title, Validators.required],
      firstName: [this.account!.firstName, Validators.required],
      lastName: [this.account!.lastName, Validators.required],
      email: [this.account!.email, [Validators.required, Validators.email]],
    });
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.form.controls;
  }

  onSubmit() {
    this.submitted = true;

    // reset alerts on submit
    this.alertService.clear();

    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }

    this.submitting = true;
    let request: ProfilePersonalUpdateRequest = {
      accountId: this.account!.id!,
      ...this.form.value,
      image: this.imageSrc
    };
    this.profileClient
      .profileUpdatePersonal(request)
      .pipe(first())
      .pipe(finalize(() => (this.submitting = false)))
      .subscribe({
        next: (res) => {
          this.authService.updateAccountInfo(res);
          this.alertService.success('Update successful');
          this.router.navigate(['../'], { relativeTo: this.route });
        },
        error: (error) => {
          this.alertService.error(error);
          this.submitting = false;
        },
      });
  }

  preview(files: any) {
    if (files.length === 0) {
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
    this.imageSrc = readerEvt.target.result;
  }
}
