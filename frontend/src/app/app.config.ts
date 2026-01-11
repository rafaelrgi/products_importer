import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient, withInterceptorsFromDi, withInterceptors, HTTP_INTERCEPTORS } from '@angular/common/http';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { LoadingInterceptor } from './middlewares/loading.interceptor';
import { routes } from './app.routes';
import { AuthInterceptor } from './middlewares/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptorsFromDi()),
    provideHttpClient(withInterceptors([LoadingInterceptor])),
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ]
};
