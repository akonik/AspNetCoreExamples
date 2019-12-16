import { Component, OnInit } from '@angular/core';
import { authService } from './authService/auth.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  /**
   *
   */
  constructor(private auth: authService, private router: Router,private route : ActivatedRoute) {


  }
  title = 'app';

  ngOnInit() {

    if (!this.auth.isAuthenticated()) {
      
      if (window.location.href.indexOf('/create-account') != -1)
        return;
      this.router.navigate(["/login"]);
    }
  }
}
