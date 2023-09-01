import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}


export function getDataUrl() {
    if (environment.production) {
        return ''
    } else if (environment.staging) {
        return ''
    } else {
        return 'http://localhost:5174'
    }
}

export function getMode() {
    if (environment.production) {
        return 'Production'
    } else if (environment.staging) {
        return 'Staging'
    } else {
        return 'Development'
    }
}

const providers = [
    { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
    { provide: 'DATA_URL', useFactory: getDataUrl, deps: [] },
    { provide: 'MODE', useFactory: getMode, deps:[] }
];


if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic(providers).bootstrapModule(AppModule)
  .catch(err => console.log(err));
