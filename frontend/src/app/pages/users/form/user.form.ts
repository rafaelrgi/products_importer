import { ChangeDetectorRef, Component, Input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormsModule } from '@angular/forms';
import { UsersService } from './../../../services/users';
import { AuthService } from './../../../services/auth';
import { FormService } from './../../../services/form';
import { User } from './../../../dtos/user.dto';

@Component({
  selector: 'app-user-form',
  imports: [DatePipe, FormsModule],
  templateUrl: './user.form.html',
  styleUrl: './user.form.css',
})
export class UserForm {
  _ownProfile: boolean = false;
  _user: User = {} as User;
  _isChangingPassword: boolean = false;
  _error: string = '';
  _nameError: string = '';
  _emailError: string = '';
  _passwordOldError: string = '';
  _passwordNewError: string = '';
  @Input() id: number = 0;

  constructor(private fb: FormBuilder, private usersService: UsersService, private authService: AuthService,
    private cdRef: ChangeDetectorRef, private router: Router, private formService: FormService) { }

  ngOnInit(): void {
    this.id = Number(this.id);
    //non admins can only access their own profile
    const myId = Number(this.authService.getUser()?.id ?? -1);
    this._ownProfile = myId === this.id;
    if ((!this.authService.isAdmin()) && !this._ownProfile) {
      this.router.navigate([`/users/${myId}`]);
      return;
    }

    this._isChangingPassword = (this.id === 0);
    this.fetch();
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  fetch(): void {
    //adding a new user?
    if (this.id === 0) {
      this._user = { id: 0 } as User;
      return;
    }
    //load user from server
    this.usersService.fetch(this.id).subscribe({
      //success
      next: (response) => {
        this._user = response;
        this.cdRef.detectChanges();
      },
      //error
      error: (error) => {
        this._error = 'Error calling server: ' + (error.message || error.toString());
        this.cdRef.detectChanges();
      }
    });
  }

  setIsAdmin(): void {
    if (!this._user || !this.isAdmin())
      return;
    this._user!.isAdmin = !this._user?.isAdmin;
  }

  setIsDeleted(): void {
    if (!this._user || !this.isAdmin())
      return;
    this._user!.isDeleted = !this._user?.isDeleted;
  }

  setChangePassword(): void {
    if (!this._user || !this._ownProfile)
      return;
    this._isChangingPassword = !this._isChangingPassword;
  }


  onSubmit(): void {
    if (!this.validateForm())
      return;
    this._user.isAdmin = this._user?.isAdmin ?? false;
    this._user.isDeleted = this._user?.isDeleted ?? false;
    if (!this._isChangingPassword) {
      this._user.password = this._user.passwordNew = this._user.passwordCheck = '';
    }

    //console.log(user);
    this._error = '';
    this.usersService.save(this._user).subscribe({
      next: (response) => {
        alert('Record saved!');
        if (this.authService.isAdmin())
          this.router.navigate(['/users/']);
        else
          this.router.navigate(['/']);
      },
      error: (err) => {
        this._error = err.message || err.error.message || 'Could not save the record. Please try again.';
        this.cdRef.detectChanges();
      }
    });

  }

  private validateForm(): boolean {
    this._error = '';
    this._nameError = '';
    this._emailError = '';
    this._passwordOldError = '';
    this._passwordNewError = '';

    this._user.name = this._user.name.trim();
    if (this._user.name.length < 2)
      this._nameError = 'Please enter a valid name';

    this._user.email = this._user.email.trim();
    if (! /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(this._user.email))
      this._emailError = 'Please enter a valid email';

    if (this._isChangingPassword) {
      if (this._ownProfile && (this._user.password ?? '').length < 3)
        this._passwordOldError = 'Please enter the current password';
      else if ((this._user.passwordNew ?? '').length < 3)
        this._passwordNewError = 'Please enter the new password';
      else if (this._user.passwordNew !== this._user.passwordCheck)
        this._passwordNewError = 'The passwords do not match';
    }

    if (this._error || this._nameError || this._emailError || this._passwordOldError || this._passwordNewError)
      return false;
    return true;
  }
}
