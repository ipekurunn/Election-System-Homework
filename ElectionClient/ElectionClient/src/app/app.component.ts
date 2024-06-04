import { Component, OnInit, ChangeDetectorRef, InjectionToken } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AlertifyService, MessageType, Position } from './Services/alertify/alertify.service';
import { HttpClientServiceService } from './Services/httpClient/http-client.service';
import { ElectionService } from './Services/election/election.service';
import { HttpClientModule } from '@angular/common/http';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GenerateVoterRequestMessage } from './Entities/generate-voter-request-message';
export const BASE_URL = new InjectionToken<string>('baseUrl');
import { NgxChartsModule } from '@swimlane/ngx-charts';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HttpClientModule, FormsModule, CommonModule, ReactiveFormsModule, NgxChartsModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [AlertifyService, ElectionService, HttpClientServiceService, { provide: BASE_URL, useValue: 'http://localhost:5222/api' }]
})
export class AppComponent implements OnInit {
  cityChartsData: any[] = [];
  title = 'ElectionClient';
  form: FormGroup;

  constructor(private alertify: AlertifyService, private electionService: ElectionService, private fb: FormBuilder, private cdr: ChangeDetectorRef) {
    this.form = this.fb.group({
      population: ['', [Validators.required, Validators.min(1)]],
      sampleInterval: ['', [Validators.required, Validators.min(1)]],
      sampleSize: ['', [Validators.required, Validators.min(1)]],
      sampleSizePerStratum: ['', [Validators.required, Validators.min(1)]],
      stratumType: ['', Validators.required]
    });
  }

  ngOnInit(): void {}

  onSearch(): void {
    if (this.form.valid) {
      let request = new GenerateVoterRequestMessage;
      request = this.form.value;
      this.electionService.getElectionDatas(request).subscribe(
        data => {
          this.processElectionData(data.data);
          this.alertify.message('Form submitted successfully!', {
            dismissOthers: true,
            messageType: MessageType.Success,
            position: Position.TopRight
          });
        },
        error => {
          console.error('Error:', error);
          this.alertify.message('An error occurred.', {
            dismissOthers: true,
            messageType: MessageType.Error,
            position: Position.TopRight
          });
        }
      );
    } else {
      this.alertify.message('Please fill in all required fields correctly.', {
        dismissOthers: true,
        messageType: MessageType.Error,
        position: Position.TopRight
      });
    }
  }

  processElectionData(data: any[]) {
    this.cityChartsData = [];  
    data.forEach(city => {
      this.cityChartsData.push(this.createChartData(city.cityName, city.simpleSampleData, 'Simple Sample Data'));
      this.cityChartsData.push(this.createChartData(city.cityName, city.stratifiedSampleData, 'Stratified Sample Data'));
      this.cityChartsData.push(this.createChartData(city.cityName, city.systematicSampleData, 'Systematic Sample Data'));
    });
    this.cdr.detectChanges();  
  }

  createChartData(cityName: string, sampleData: any[], sampleType: string) {
    const voteCounts = sampleData.reduce((acc, curr) => {
      acc[curr.vote] = (acc[curr.vote] || 0) + 1;
      return acc;
    }, {});

    const result = {
      cityName,
      sampleType,
      series: Object.keys(voteCounts).map(key => ({
        name: key,
        value: voteCounts[key]
      }))
    };

    return result;
  }
}
