import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {first} from 'rxjs/operators';

import { AlertService } from '@app/_services';
import { AccountsClient, AuthenticateRequest } from 'src/shared/service-clients/service-clients';
import { AuthService } from '@app/_services/auth.service';

@Component({ templateUrl: 'login.component.html' })
export class LoginComponent implements OnInit {
    form!: FormGroup;
    submitting = false;
    submitted = false;

    constructor(
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private accountClient: AccountsClient,
        private alertService: AlertService,
        private authService: AuthService
    ) { }

    ngOnInit() {
        this.authService.clearData();
        this.form = this.formBuilder.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', Validators.required]
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
        // create auth body for login
        const authBody = new AuthenticateRequest();
        authBody.email = this.f.email.value;
        authBody.password = this.f.password.value;

        this.authService.login(authBody)
            .pipe(first())
            .subscribe({
                next: authResp => {                    
                    // get return url from query parameters or default to home page
                    const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
                    this.router.navigateByUrl(returnUrl);
                },
                error: error => {
                    this.alertService.error(error);
                    this.submitting = false;
                }
            });
    }
}
