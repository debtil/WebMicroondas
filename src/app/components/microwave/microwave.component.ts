import { Component } from '@angular/core';
import { MicrowaveService, HeatingRequest } from './../../services/microwave.service';

@Component({
  selector: 'app-microwave',
  templateUrl: './microwave.component.html',
  styleUrls: ['./microwave.component.scss']
})
export class MicrowaveComponent {
  timeSeconds: string = '';
  power: string = '';
  activeField: 'time' | 'power' | null = null;

  constructor(private microwaveService: MicrowaveService) {}

  setActiveField(field: 'time' | 'power') {
    this.activeField = field;
  }

  onInput(value: string): void {
    if (this.activeField === 'time') {
      this.timeSeconds += value;
    } else if (this.activeField === 'power') {
      this.power += value;
    }
  }

  onClear(): void {
    if (this.activeField === 'time') {
      this.timeSeconds = '';
    } else if (this.activeField === 'power') {
      this.power = '';
    }
  }

  onStart(): void {
    const request: HeatingRequest = {
      timeSeconds: Number(this.timeSeconds) || 0,
      power: Number(this.power)
    };
    this.microwaveService.startHeating(request).subscribe({
      next: res => console.log(res),
      error: err => console.error(err)
    });
  }

  onQuickStart(): void {
    const request: HeatingRequest = {
      timeSeconds: 30,
      power: 10
    };
    this.microwaveService.startHeating(request).subscribe({
      next: res => console.log(res),
      error: err => console.error(err)
    });
  }

  onPauseOrCancel(): void {
    this.microwaveService.pauseOrCancel().subscribe({
      next: res => console.log(res),
      error: err => console.error(err)
    });
  }

  onResume(): void {
    this.microwaveService.resumeHeating().subscribe({
      next: res => console.log(res),
      error: err => console.error(err)
    });
  }
}
