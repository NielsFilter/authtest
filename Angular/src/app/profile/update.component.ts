import { Component, Injector, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { AlertService } from '@app/_services';
import { MustMatch } from '@app/_helpers';
import { AccountDto, AccountsClient } from 'src/shared/service-clients/service-clients';
import { AppComponentBase } from 'src/shared/common/app-component-base';

@Component({ templateUrl: 'update.component.html' })
export class UpdateComponent extends AppComponentBase implements OnInit {
    account: AccountDto = this.accountInfo!;
    form!: FormGroup;
    submitting = false;
    submitted = false;
    deleting = false;

    constructor(injector: Injector,
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private accountClient: AccountsClient,
        private alertService: AlertService
    ) {
        super(injector);
     }

    ngOnInit() {
        this.form = this.formBuilder.group({
            title: [this.account.title, Validators.required],
            firstName: [this.account.firstName, Validators.required],
            lastName: [this.account.lastName, Validators.required],
            email: [this.account.email, [Validators.required, Validators.email]],
            password: ['', [Validators.minLength(6)]],
            confirmPassword: ['']
        }, {
            validator: MustMatch('password', 'confirmPassword')
        });
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
        this.accountClient.accountsUpdate(this.account.id!, this.form.value)
            .pipe(first())
            .subscribe({
                next: () => {
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
            this.accountClient.accountsDelete(this.account.id!)
                .pipe(first())
                .subscribe(() => {
                    this.alertService.success('Account deleted successfully');
                });
        }
    }
}