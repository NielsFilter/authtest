import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { filter } from 'rxjs/operators';

import { Alert, AlertOptions, AlertType } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class AlertService {
    private subject = new Subject<Alert>();
    private defaultId = 'default-alert';

    // enable subscribing to alerts observable
    onAlert(id = this.defaultId): Observable<Alert> {
        return this.subject.asObservable().pipe(filter(x => x && x.id === id));
    }

    // convenience methods
    //todo:
    // success(title: string, message: string, options?: AlertOptions) {
    //     this.alert(new Alert({ ...options, type: AlertType.Success, message, title: title }));
    // }

    // error(title: string, message: string, options?: AlertOptions) {
    //     this.alert(new Alert({ ...options, type: AlertType.Error, message, title: title }));
    // }

    // info(title: string, message: string, options?: AlertOptions) {
    //     this.alert(new Alert({ ...options, type: AlertType.Info, message, title: title }));
    // }

    // warn(title: string, message: string, options?: AlertOptions) {
    //     this.alert(new Alert({ ...options, type: AlertType.Warning, message, title: title }));
    // }

    success(title: string, options?: AlertOptions) {
        this.alert(new Alert({ ...options, type: AlertType.Success, message: '', title: title }));
    }

    error(title: string,  options?: AlertOptions) {
        this.alert(new Alert({ ...options, type: AlertType.Error, message: '', title: title }));
    }

    info(title: string, options?: AlertOptions) {
        this.alert(new Alert({ ...options, type: AlertType.Info, message: '', title: title }));
    }

    warn(title: string, options?: AlertOptions) {
        this.alert(new Alert({ ...options, type: AlertType.Warning, message: '', title: title }));
    }

    // core alert method
    alert(alert: Alert) {
        alert.id = alert.id || this.defaultId;
        alert.autoClose = (alert.autoClose === undefined ? true : alert.autoClose);
        this.subject.next(alert);
    }

    // clear alerts
    clear(id = this.defaultId) {
        this.subject.next(new Alert({ id }));
    }
}