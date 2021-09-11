import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup | undefined;
  maxDate: Date | undefined;
  validationErrors: string[] | undefined;


  constructor(private service: AccountService, private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm()
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      userName: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    })
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm?.controls.confirmPassword.updateValueAndValidity()
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      if (control?.parent?.controls) {
        return control?.value === (control?.parent?.controls as { [key: string]: AbstractControl })[matchTo]?.value ? null : { isMatching: true };
      }
      return null;
    }
  }
  registerUser() {
    this.service.register(this.registerForm?.value).subscribe(response => {
      this.router.navigateByUrl('/members');
      this.cancel();
    }, error => {
      this.validationErrors = error;
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
