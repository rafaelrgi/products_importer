import { Injectable } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class FormService {

  constructor() { }

  /*
  checkRequired(ctrl: string, frm: FormGroup, setMessage: () => void): boolean {
    if (frm.get(ctrl)?.hasError('required')) {
      setMessage();
      return false;
    }
    return true;
  }

  checkInvalid(ctrl: string, frm: FormGroup, setMessage: () => string): boolean {
    if (frm.get(ctrl)?.invalid) {
      setMessage();
      return false;
    }
    return true;
  }
  onOffValidation(ctrl: string, frm: FormGroup, on: boolean) {
    const control = frm.get('password');
    if (on)
    control?.setValidators([Validators.required]);
    else
      control?.clearValidators();
    control?.updateValueAndValidity();
  }
  */

}
