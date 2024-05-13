import { Component, OnInit, OnDestroy, Input, ViewEncapsulation } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Subscription } from 'rxjs';

import { Alert, AlertType } from '@app/_models';
import { AlertService } from '@app/_services';

@Component({
    selector: 'alert',
    templateUrl: 'alert.component.html',
    styleUrls: ['./alert.component.less'],
    encapsulation: ViewEncapsulation.None,
})
export class AlertComponent implements OnInit, OnDestroy {
    @Input() id = 'default-alert';
    @Input() fade = true;

    alerts: Alert[] = [];
    alertSubscription!: Subscription;
    routeSubscription!: Subscription;

    constructor(private router: Router, private alertService: AlertService) { }

    ngOnInit() {
        // subscribe to new alert notifications
        this.alertSubscription = this.alertService.onAlert(this.id)
            .subscribe(alert => {
                if(!alert.message) {
                    return;
                }

                // add alert to array
                this.alerts.push(alert);

                // auto close alert if required
                var delay = alert.autoCloseDelay || 3000;
                setTimeout(() => this.removeAlert(alert), delay);
            });

        // clear alerts on location change
        this.routeSubscription = this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.alertService.clear(this.id);
            }
        });
    }

    ngOnDestroy() {
        // unsubscribe to avoid memory leaks
        this.alertSubscription.unsubscribe();
        this.routeSubscription.unsubscribe();
    }

    removeAlert(alert: Alert) {
    // check if already removed to prevent error on auto close
    if (!this.alerts.includes(alert)) return;

        // remove alert after faded out
        setTimeout(() => {
            this.alerts = this.alerts.filter(x => x !== alert);
        }, 250);
    }

    getAlertStyle(alert: Alert): string{
        if (!alert) return '';

        const alertTypeClass = {
            [AlertType.Success]: 'text-bg-success',
            [AlertType.Error]: 'text-bg-danger',
            [AlertType.Info]: 'text-bg-info',
            [AlertType.Warning]: 'text-bg-warning'
        }

        if (alert.type !== undefined) {
            return alertTypeClass[alert.type];
        }

        return alertTypeClass[AlertType.Info];
    }

    getIconClass(alert: Alert) : string {
        if (!alert) return '';

        const iconClassMapping = {
            [AlertType.Success]: 'fa-circle-check',
            [AlertType.Error]: 'fa-bug',
            [AlertType.Info]: 'fa-circle-info',
            [AlertType.Warning]: 'fa-triangle-exclamation'
        }

        if (alert.type !== undefined) {
            return iconClassMapping[alert?.type];
        }

        return iconClassMapping[AlertType.Info];
    }
}