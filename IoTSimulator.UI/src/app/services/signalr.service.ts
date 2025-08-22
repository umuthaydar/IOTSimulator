import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

export interface SensorDataUpdate {
  deviceId: string;
  roomId: string;
  houseId?: string;
  sensorId: string;
  temperature: number;
  humidity: number;
  timestamp: string;
  deviceName: string;
  roomName?: string;
}

export interface DeviceStatusUpdate {
  deviceId: string;
  roomId: string;
  status: string;
  timestamp: string;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection | null = null;
  private connectionState$ = new BehaviorSubject<signalR.HubConnectionState>(signalR.HubConnectionState.Disconnected);
  private sensorDataUpdate$ = new BehaviorSubject<SensorDataUpdate | null>(null);
  private deviceStatusUpdate$ = new BehaviorSubject<DeviceStatusUpdate | null>(null);
  
  // Observable streams
  public connectionState = this.connectionState$.asObservable();
  public sensorDataUpdates = this.sensorDataUpdate$.asObservable();
  public deviceStatusUpdates = this.deviceStatusUpdate$.asObservable();

  constructor() {}

  public async startConnection(): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/sensorDataHub', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect([0, 2000, 10000, 30000])
      .build();

    // Connection state monitoring
    this.hubConnection.onclose(() => {
      this.connectionState$.next(signalR.HubConnectionState.Disconnected);
      console.log('SignalR connection closed');
    });

    this.hubConnection.onreconnecting(() => {
      this.connectionState$.next(signalR.HubConnectionState.Reconnecting);
      console.log('SignalR reconnecting...');
    });

    this.hubConnection.onreconnected(() => {
      this.connectionState$.next(signalR.HubConnectionState.Connected);
      console.log('SignalR reconnected');
    });

    // Listen for sensor data updates
    this.hubConnection.on('SensorDataUpdate', (data: SensorDataUpdate) => {
      console.log('Received sensor data update:', data);
      this.sensorDataUpdate$.next(data);
    });

    // Listen for device status updates
    this.hubConnection.on('DeviceStatusUpdate', (data: DeviceStatusUpdate) => {
      console.log('Received device status update:', data);
      this.deviceStatusUpdate$.next(data);
    });

    try {
      await this.hubConnection.start();
      this.connectionState$.next(signalR.HubConnectionState.Connected);
      console.log('SignalR connection started successfully');
    } catch (error) {
      console.error('Error starting SignalR connection:', error);
      this.connectionState$.next(signalR.HubConnectionState.Disconnected);
      throw error;
    }
  }

  public async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.hubConnection = null;
      this.connectionState$.next(signalR.HubConnectionState.Disconnected);
      console.log('SignalR connection stopped');
    }
  }

  public async joinRoomGroup(roomId: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.hubConnection.invoke('JoinRoomGroup', roomId);
        console.log(`Joined room group: ${roomId}`);
      } catch (error) {
        console.error('Error joining room group:', error);
      }
    }
  }

  public async leaveRoomGroup(roomId: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.hubConnection.invoke('LeaveRoomGroup', roomId);
        console.log(`Left room group: ${roomId}`);
      } catch (error) {
        console.error('Error leaving room group:', error);
      }
    }
  }

  public async joinHouseGroup(houseId: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.hubConnection.invoke('JoinHouseGroup', houseId);
        console.log(`Joined house group: ${houseId}`);
      } catch (error) {
        console.error('Error joining house group:', error);
      }
    }
  }

  public async leaveHouseGroup(houseId: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.hubConnection.invoke('LeaveHouseGroup', houseId);
        console.log(`Left house group: ${houseId}`);
      } catch (error) {
        console.error('Error leaving house group:', error);
      }
    }
  }

  public getConnectionState(): signalR.HubConnectionState {
    return this.hubConnection?.state ?? signalR.HubConnectionState.Disconnected;
  }

  public isConnected(): boolean {
    return this.hubConnection?.state === signalR.HubConnectionState.Connected;
  }
}
