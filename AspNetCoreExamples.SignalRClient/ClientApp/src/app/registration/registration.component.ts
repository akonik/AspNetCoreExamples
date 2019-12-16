import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { authService } from '../authService/auth.service';

@Component({
    selector: 'app-registration',
    templateUrl: 'registration.component.html',
    styleUrls: ['registration.component.css']
})
export class RegistrationComponent implements OnInit {

    constructor(private formBuilder: FormBuilder, private auth: authService) { }

    private mainForm: FormGroup;
    public submitted = false;

    ngOnInit() {
        this.mainForm = this.formBuilder.group({
            login: ['', Validators.required],
            password: ['', Validators.required],
            passwordConfirm: ['', Validators.required]
        });
    }


    get form() {
        return this.mainForm;
    }

    get f() {
        return this.form.controls;
    }

    async onSubmit() {
        this.submitted = true;

        if (this.form.valid) {
            let result = await this.auth.CreateAccount(this.f.login.value, this.f.password.value,this.f.passwordConfirm.value);
        }
    }

}