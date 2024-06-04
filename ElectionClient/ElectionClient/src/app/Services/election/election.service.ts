import { Injectable } from '@angular/core';
import { HttpClientServiceService } from '../httpClient/http-client.service';
import { Observable, catchError, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { GenerateVoterRequestMessage } from '../../Entities/generate-voter-request-message';

@Injectable({
  providedIn: 'root'
})
export class ElectionService {

  constructor(private httpClientService: HttpClientServiceService) { }

  getElectionDatas(request : GenerateVoterRequestMessage): Observable<any> {
    let requestMessage = new GenerateVoterRequestMessage ;
    requestMessage =  request;
    return this.httpClientService.post({
      controller:"Election",
      action: "GenerateCityDatas",
    },requestMessage).pipe(
      catchError((errorResponse: HttpErrorResponse) => {
    let errorMessage = 'An unexpected error occurred.';
    if (errorResponse.error) {
      errorMessage = errorResponse.error.error || errorResponse.message;
    }
    return throwError(() => new Error(errorMessage));
  })
    );;
  }
}
