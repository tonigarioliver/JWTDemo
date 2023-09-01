import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Login } from '../Models/login';  
import { Register } from '../Models/register';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { JwtAuth } from '../Models/jwtAuth';


@Injectable({
  providedIn: 'root'
})
export class AuthentiationService {
  registerUrl: string = "api/AuthManagment/Register"
  loginUrl: string = "api/AuthManagment/Login"
  weatherrUrl: string = "WeatherForecast"
  constructor(private http:HttpClient) { }

  public register(user:Register): Observable<JwtAuth>
  {
    return this.http.post<JwtAuth>(`${environment.apiUrl}/${this.registerUrl}`,user);
  }

  public login(user:Login): Observable<JwtAuth>
  {
    return this.http.post<JwtAuth>(`${environment.apiUrl}/${this.loginUrl}`,user);
  }
  public getWeather(): Observable<any>
  {
    return this.http.get<any>(`${environment.apiUrl}/${this.weatherrUrl}`);
  }
}
