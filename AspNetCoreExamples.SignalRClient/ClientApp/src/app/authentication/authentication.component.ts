import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { authService } from '../authService/auth.service';

@Component({
    selector: 'app-authentication',
    templateUrl: 'authentication.component.html',
    styleUrls: ['authentication.component.css']
})
export class AuthenticationComponent implements OnInit {

    constructor(private formBuilder: FormBuilder, private auth: authService) { }

    private mainForm: FormGroup;
    public submitted = false;

    ngOnInit() {
        this.mainForm = this.formBuilder.group({
            login: ['', Validators.required],
            password: ['', Validators.required]
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
            let result = await this.auth.Login(this.f.login.value, this.f.password.value);
        }
    }

}