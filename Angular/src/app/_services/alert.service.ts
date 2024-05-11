import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { filter } from 'rxjs/operators';

import { Alert, AlertType } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class AlertService {
    private subject = new Subject<Alert>();
    private defaultId = 'default-alert';

    // enable subscribing to alerts observable
    onAlert(id = this.defaultId): Observable<Alert> {
        return this.subject.asObservable().pipe(filter(x => x && x.id === id));
    }

    success(message: string, autoCloseDelay?: number) {
        this.alert(new Alert({type: AlertType.Success, message, autoCloseDelay }));
    }

    error(message: string, autoCloseDelay?: number) {
        this.alert(new Alert({type: AlertType.Error, message, autoCloseDelay }));
    }

    info(message: string, autoCloseDelay?: number) {
        this.alert(new Alert({type: AlertType.Info, message, autoCloseDelay }));
    }

    warn(message: string, autoCloseDelay?: number) {
        this.alert(new Alert({type: AlertType.Warning, message, autoCloseDelay }));
    }

    // core alert method
    alert(alert: Alert) {
        alert.id = alert.id || this.defaultId;
        this.subject.next(alert);
    }

    // clear alerts
    clear(id = this.defaultId) {
        this.subject.next(new Alert({ id }));
    }
}