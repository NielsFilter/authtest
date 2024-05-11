export class Alert {
    id?: string;
    message?: string;
    type?: AlertType;
    cssClass?: string
    autoCloseDelay?: number;

    constructor(init?: Partial<Alert>) {
        Object.assign(this, init);
    }
}

export enum AlertType {
    Success,
    Error,
    Info,
    Warning
}