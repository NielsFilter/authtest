import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from 'src/shared/common/app-component-base';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.less'
})
export class SettingsComponent extends AppComponentBase implements OnInit {
  form!: FormGroup;
  submitting = false;
  submitted = false;
  deleting = false;

  constructor(injector: Injector,
      private formBuilder: FormBuilder
  ) {
      super(injector);
   }

  ngOnInit() {
      this.form = this.formBuilder.group({
          daskMode: [this.account!.title]
      });
  }

  onSubmit() {
    // this.submitted = true;

    // // reset alerts on submit
    // //todo: this.alertService.clear();

    // // stop here if form is invalid
    // if (this.form.invalid) {
    //     return;
    // }

    // this.submitting = true;
    // let request : ProfileUpdateRequest = { ...this.form.value};
    // this.profileClient.profileUpdate(this.account!.id!, request)
    //     .pipe(first())
    //     .subscribe({
    //         next: (res) => {
    //             this.authService.updateAccountInfo(res);
    //             this.alertService.success('Update successful');
    //             this.router.navigate(['../'], { relativeTo: this.route });
    //         },
    //         error: error => {
    //             this.alertService.error(error);
    //             this.submitting = false;
    //         }
    //     });
}
}
