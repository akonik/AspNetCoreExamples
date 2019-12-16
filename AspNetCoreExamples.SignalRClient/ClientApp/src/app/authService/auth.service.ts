import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { environment } from 'src/environments/environment';
import { apiController } from '../controllers/apiController';

export class User {
    token: string;
    expiresIn: Number;

}

export type UserLogout = () => void;
export type UserSetCredentials = () => void;
export type UserSetToken = () => void;

@Injectable({
    providedIn: 'root'
})
export class authService {

    private _user: User;

    onUserLogined: UserSetCredentials = null;

    constructor(private api: apiController, private router: Router, private route: ActivatedRoute) {
        this._user = (JSON.parse(localStorage.getItem(environment.authStrorageKey)));

    }


    isAuthenticated(): boolean {
        if (this._user === null)
            return false;

        if (this._user.expiresIn > Date.now().valueOf())
            return true;

        this.Logout();
    }

    public get CurrentUser(): User {
        return this._user;
    }

    async Login(login: string, password: string) {
        try {
            var token = await this.api.Login(login, password);
            let expiresIn = new Date();
            expiresIn.setSeconds(expiresIn.getSeconds() + 60 * 60);

            this._user = new User();
            this._user.token = token.accessToken;
            this._user.expiresIn = expiresIn.valueOf();

            localStorage.setItem(environment.authStrorageKey, JSON.stringify(this._user));

            if (this.onUserLogined != null)
                this.onUserLogined();

            this.router.navigate(["/"]);
        }
        catch (error) {
            console.log(error);
            return false;
        }

        return true;
    }

    async UpdateCredentials() {
        let token = this.CurrentUser.token;
        var details = await this.api.getUserDetails(token);

        this._user = this._user;
        sessionStorage.setItem(environment.authStrorageKey, JSON.stringify(this._user));

        if (this.onUserLogined != null)
            this.onUserLogined();
    }

    Logout() {
        sessionStorage.removeItem(environment.authStrorageKey);
        this._user = null;
        this.router.navigate(['/'])
    }

    async CreateAccount(email, password, confirmNewPassword) {
        try {
            var token = await this.api.createAccount(email, password, confirmNewPassword);
            let expiresIn = new Date();
            expiresIn.setSeconds(expiresIn.getSeconds() + 60 * 60);

            this._user = new User();
            this._user.token = token.accessToken;
            this._user.expiresIn = expiresIn.valueOf();

            localStorage.setItem(environment.authStrorageKey, JSON.stringify(this._user));

            if (this.onUserLogined != null)
                this.onUserLogined();

            this.router.navigate(["/"]);

        }
        catch (error) {
            if (error.status === 400) {
                return {
                    isSuccess: false,
                    error: error.error.message
                }
            }
            return {
                isSuccess: false
            }
        }
    }

    async ChangePassword(
        login: string,
        oldPassword: string,
        newPassword: string,
        confirmNewPassword: string) {
        try {
            await this.api.changePassword(login, oldPassword, newPassword, confirmNewPassword);
            return {
                isSuccess: true
            }
        }
        catch (error) {
            if (error.status === 400) {
                return {
                    isSuccess: false,
                    error: error.error.message
                }
            }
            return {
                isSuccess: false
            }
        }
    }
}