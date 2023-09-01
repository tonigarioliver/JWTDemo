import { Component } from '@angular/core';
import { Login } from './Models/login';
import { Register } from './Models/register';
import { JwtAuth } from './Models/jwtAuth';
import { AuthentiationService } from './Services/authentiation.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Drivers.App';
  loginDto = new Login();
  registerDto = new Register();
  jwtDto = new JwtAuth();

  constructor(private authService: AuthentiationService){}

  register(registerDto: Register)
  {
    this.authService.register(registerDto).subscribe();
  }
  login(loginDto: Login)
  {
    this.authService.login(loginDto).subscribe((jwtDto)=>{
      localStorage.setItem('jwtToken',jwtDto.token);
    });
  }

  Weather()
  {
    this.authService.getWeather().subscribe((weatherdata:any)=>{
      console.log(weatherdata);
    });
  }
}
