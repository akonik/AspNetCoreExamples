import { Injectable } from "@angular/core";

import { HttpHeaders, HttpParams, HttpClient } from "@angular/common/http";



import { environment } from "src/environments/environment";

@Injectable()
export class apiController {
    private ApiEndpoint: string;
    private wwwFormHeader = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });


    constructor(private http: HttpClient) {
        this.ApiEndpoint = environment.authApiEndpoint;
    }

    public Login(email: string, password: string): any {

        return this.http.post(this.ApiEndpoint + 'account/login',
            {
                email, password
            }).toPromise();
    }

    public getUserDetails(token: string): any {
        return this.http.get(this.ApiEndpoint + 'account/user-details',
            {
                headers: this.BuildAutorizationToken(token)
            })
            .toPromise();
    }

    public createAccount(
        email: string,
        password: string,
        confirmPassword: string
    ) : any {
        return this.http.post(this.ApiEndpoint + 'account/create',
            {
                email, password, confirmPassword
            }).toPromise();
    }

    public changePassword(
        userName: string,
        password: string,
        newPassword: string,
        confirmNewPassword: string
    ) {
        return this.http.post(this.ApiEndpoint + 'account/change-password',
            {
                login: userName,
                oldPassword: password,
                newPassword: newPassword,
                confirmNewPassword: confirmNewPassword
            }).toPromise();
    }

    private BuildAutorizationToken(token: string): HttpHeaders {
        return new HttpHeaders({ 'Authorization': 'Bearer ' + token });
    }
}