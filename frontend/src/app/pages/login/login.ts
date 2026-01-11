import { ChangeDetectorRef, Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from './../../services/auth';
import { isDevMode, enableProdMode } from '@angular/core';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginForm!: FormGroup;
  formError: string = '';
  emailError: string = '';
  passwordError: string = '';

  constructor(private fb: FormBuilder, private authService: AuthService, private cdRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    const emailValidator = isDevMode() ? Validators.minLength(2) : Validators.email;

    this.loginForm = this.fb.group({
      email: ['', [Validators.required, emailValidator]],
      password: ['', [Validators.required, Validators.minLength(3)]]
    });

    window.setTimeout(() => this.authService.logout());
  }

  onSubmit(): void {
    if (!this.validateForm())
      return;

    this.authService.login(this.loginForm.value.email, this.loginForm.value.password).subscribe({
      next: (response) => {
        this.authService.setToken(response.token);
        if (response.user)
          this.authService.setUser(response.user);
        this.cdRef.detectChanges();
      },
      error: (err) => {
        if (err.status == 401) //Unauthorized
          this.formError = 'Login failed, check your email and password [401]';
        else if (err.status == 500) //Internal Server Error
          this.formError = 'An error occurred on the server, please try again later [500]';
        else if (err.status == 0) //Server unavailable
          this.formError = 'Unable to reach the server, check your connection [000]';
        else
          this.formError = err.message || err.error.message || 'Login failed. Please try again.';
        this.cdRef.detectChanges();
      }
    });
  }

  private validateForm(): boolean {
    this.formError = '';
    this.emailError = '';
    this.passwordError = '';

    if (this.loginForm.valid)
      return true;

    //if (this.loginForm.get('email')?.touched)
    if (this.loginForm.get('email')?.hasError('required'))
      this.emailError = 'Email is required';
    else if (this.loginForm.get('email')?.invalid)
      this.emailError = 'Please enter a valid email';
    //if (this.loginForm.get('password')?.touched)
    if (this.loginForm.get('password')?.hasError('required'))
      this.passwordError = 'Password is required';
    else if (this.loginForm.get('password')?.value.length < 3)
      this.passwordError = 'Please enter a valid password';

    return false;
  }

  findInvalidControlsRecursive(form: FormGroup): { control: AbstractControl, name: string }[] {
    const invalidControls: { control: AbstractControl, name: string }[] = [];
    const controls = form.controls;
    for (const name in controls) {
      if (controls[name].invalid) {
        invalidControls.push({ control: controls[name], name: name });
      }
      if (controls[name] instanceof FormGroup) {
        invalidControls.push(...this.findInvalidControlsRecursive(controls[name] as FormGroup));
      }
    }
    return invalidControls;
  }


}

