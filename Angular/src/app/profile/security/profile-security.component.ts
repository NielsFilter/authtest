import { Component, Injector, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AbstractControlOptions, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { finalize, first } from 'rxjs/operators';

import { AlertService } from '@app/_services';
import { MustMatch } from '@app/_helpers';
import { ProfileClient, ProfileSecurityUpdateRequest } from 'src/shared/service-clients/service-clients';
import { AppComponentBase } from 'src/shared/common/app-component-base';

@Component({
    templateUrl: 'profile-security.component.html',
    styleUrl: 'profile-security.component.scss'
})
export class ProfileSecurityComponent extends AppComponentBase implements OnInit {
    form!: FormGroup;
    submitting = false;
    submitted = false;

    constructor(injector: Injector,
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private profileClient: ProfileClient,
        private alertService: AlertService
    ) {
        super(injector);
     }

    ngOnInit() {
        this.form = this.formBuilder.group({
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
        let request : ProfileSecurityUpdateRequest = {
            accountId: this.account!.id!,
            ...this.form.value
        };

        this.profileClient.profileUpdateSecurity(request)
            .pipe(first())
            .pipe(finalize(() => this.submitting = false))
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
}
