import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { SignalRService } from './services/signalr.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'IoTSimulator.UI';

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private signalRService: SignalRService
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.initializeSignalR();
    }
  }

  private async initializeSignalR(): Promise<void> {
    try {
      await this.signalRService.startConnection();
      console.log('SignalR connection established successfully');
    } catch (error) {
      console.error('Failed to establish SignalR connection:', error);
    }
  }
}
