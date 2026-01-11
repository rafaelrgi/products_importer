import { ChangeDetectorRef, Component, Input, isDevMode } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UsersService } from './../../../services/users';
import { AuthService } from './../../../services/auth';
import { FormService } from './../../../services/form';
import { User } from './../../../dtos/user.dto';

@Component({
  selector: 'app-user-form',
  imports: [DatePipe, ReactiveFormsModule],
  templateUrl: './user.form.html',
  styleUrl: './user.form.css',
})
export class UserForm {
  form!: FormGroup;
  public ownProfile: boolean = false;
  public user: User | null = null;
  public isChangingPassword: boolean = false;
  public error: string = '';
  public nameError: string = '';
  public emailError: string = '';
  public passwordOldError: string = '';
  public passwordNewError: string = '';
  @Input() id: number = 0;

  constructor(private fb: FormBuilder, private usersService: UsersService, private authService: AuthService,
    private cdRef: ChangeDetectorRef, private router: Router, private formService: FormService) { }

  ngOnInit(): void {
    this.id = Number(this.id);
    const emailValidator = isDevMode() ? Validators.minLength(2) : Validators.email;
    this.form = this.fb.group({
      id: [this.id],
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, emailValidator]],
      isAdmin: ['', []],
      isDeleted: ['', []],
      password: ['', []],
      passwordNew: ['', []],
      passwordCheck: ['', []],
    });

    //non admins can only access their own profile
    const myId = this.authService.getUser()?.id ?? -1;
    this.ownProfile = myId === this.id;
    if ((!this.authService.isAdmin()) && !this.ownProfile) {
      this.router.navigate([`/users/${myId}`]);
      return;
    }

    this.isChangingPassword = (this.id === 0);

    this.fetch();
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  fetch(): void {
    //this.status = PageStatus.Loading;

    //adding a new user?
    if (this.id === 0) {
      this.user = <User>{ id: 0 };
      return;
    }
    //load user from server
    this.usersService.fetch(this.id).subscribe({
      //success
      next: (response) => {
        this.user = response;
        this.form.patchValue({
          name: this.user?.name,
          email: this.user?.email
        });
        //this.status = (this.users?.length ?? 0) > 0 ? PageStatus.Ready : PageStatus.Empty;
        this.cdRef.detectChanges();
      },
      //error
      error: (error) => {
        //this.status = PageStatus.Error;
        this.error = 'Error calling server: ' + (error.message || error.toString());
        this.cdRef.detectChanges();
      }
    });
  }

  setIsAdmin(): void {
    if (!this.user || !this.isAdmin())
      return;
    this.user!.isAdmin = !this.user?.isAdmin;
  }

  setIsDeleted(): void {
    if (!this.user || !this.isAdmin())
      return;
    this.user!.isDeleted = !this.user?.isDeleted;
  }

  setChangePassword(): void {
    if (!this.user || !this.ownProfile)
      return;
    this.isChangingPassword = !this.isChangingPassword;

    /*
    this.formService.onOffValidation('password', this.form, this.isChangingPassword);
    this.formService.onOffValidation('passwordNew', this.form, this.isChangingPassword);
    this.formService.onOffValidation('passwordCheck', this.form, this.isChangingPassword);
    */
  }


  onSubmit(): void {
    if (!this.validateForm())
      return;

    const user: User = this.form.value as User;
    user.isAdmin = this.user?.isAdmin ?? false;
    user.isDeleted = this.user?.isDeleted ?? false;
    if (!this.isChangingPassword) {
      user.password = user.passwordNew = user.passwordCheck = '';
    }

    //console.log(user);
    this.error = '';
    this.usersService.save(user).subscribe({
      next: (response) => {
        alert('Record saved!');
        this.router.navigate([`/users`]);
      },
      error: (err) => {
        this.error = err.message || err.error.message || 'Could not save the record. Please try again.';
        this.cdRef.detectChanges();
      }
    });

  }

  private validateForm(): boolean {
    //this.formError = '';
    this.nameError = '';
    this.emailError = '';
    this.passwordOldError = '';
    this.passwordNewError = '';

    let passwordsOk = true;
    if (this.isChangingPassword) {
      passwordsOk = false;
      if (this.ownProfile && this.form.get('password')?.value.length < 3)
        this.passwordOldError = 'Please enter the current password';
      else if (this.form.get('passwordNew')?.value.length < 3)
        this.passwordNewError = 'Please enter the new password';
      else if (this.form.get('passwordNew')?.value !== this.form.get('passwordCheck')?.value)
        this.passwordNewError = 'The passwords do not match';
      else passwordsOk = true;
    }

    if (this.form.valid && passwordsOk)
      return true;

    if (this.form.get('name')?.invalid) this.nameError = 'Please enter a valid name';

    if (this.form.get('email')?.invalid) this.emailError = 'Please enter a valid email';

    if (this.form.get('password')?.invalid) this.passwordOldError = 'Please enter your current password';
    if (this.form.get('passwordNew')?.invalid) this.passwordNewError = 'Please enter your passwordNewError ';
    if (this.form.get('passwordCheck')?.invalid) this.passwordNewError = 'Please enter your passwordNewError';

    return false;
  }
}
