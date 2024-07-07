import { Component, Injector, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AbstractControlOptions, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { AlertService } from '@app/_services';
import { MustMatch } from '@app/_helpers';
import { AccountDto, AccountsClient, ProfileClient, ProfileUpdateRequest } from 'src/shared/service-clients/service-clients';
import { AppComponentBase } from 'src/shared/common/app-component-base';
import { AuthService } from '@app/_services/auth.service';

@Component({ templateUrl: 'update.component.html' })
export class UpdateComponent extends AppComponentBase implements OnInit {
    form!: FormGroup;
    submitting = false;
    submitted = false;
    deleting = false;

    constructor(injector: Injector,
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
            password: ['', [Validators.minLength(6)]],
            confirmPassword: ['']
        }, {
            validator: MustMatch('password', 'confirmPassword')
        } as AbstractControlOptions);
    }

    // convenience getter for easy access to form fields
    get f() { return this.form.controls; }

    onSubmit() {
        this.submitted = true;

        // reset alerts on submit
        this.alertService.clear();

        // stop here if form is invalid
        if (this.form.invalid) {
            return;
        }

        this.submitting = true;
        let request : ProfileUpdateRequest = { ...this.form.value};
        this.profileClient.profileUpdate(this.account!.id!, request)
            .pipe(first())
            .subscribe({
                next: (res) => {
                    this.authService.updateAccountInfo(res);
                    this.alertService.success('Update successful');
                    this.router.navigate(['../'], { relativeTo: this.route });
                },
                error: error => {
                    this.alertService.error(error);
                    this.submitting = false;
                }
            });
    }

    onDelete() {
        if (confirm('Are you sure?')) {
            this.deleting = true;
            this.profileClient.profileDelete(this.account!.id!)
                .pipe(first())
                .subscribe(() => {
                    this.alertService.success('Account deleted successfully');
                });
        }
    }
}