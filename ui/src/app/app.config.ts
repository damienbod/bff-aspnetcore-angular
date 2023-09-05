import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, CSP_NONCE } from '@angular/core';
import { secureApiInterceptor } from './secure-api.interceptor';

import {
  provideRouter,
  withEnabledBlockingInitialNavigation,
} from '@angular/router';
import { appRoutes } from './app.routes';

const nonce = (
  document.querySelector('meta[name="CSP_NONCE"]') as HTMLMetaElement
)?.content;

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(appRoutes, withEnabledBlockingInitialNavigation()),
    provideHttpClient(withInterceptors([secureApiInterceptor])),
    {
      provide: CSP_NONCE,
      useValue: nonce,
    },
  ],
};
