export interface DashboardStatsDto {
  totalDevices: number;
  activeDevices: number;
  inactiveDevices: number;
  totalRooms: number;
  totalHouses: number;
}

export interface House {
  id: string;
  name: string;
  address: string;
  createdAt: Date;
  updatedAt: Date;
  rooms?: Room[];
}

export interface Room {
  id: string;
  name: string;
  houseId: string;
  house?: House;
  devices?: IoTDevice[];
  createdAt: Date;
  updatedAt: Date;
  // Real-time sensor data properties
  currentTemperature?: number;
  currentHumidity?: number;
  lastSensorUpdate?: Date;
  sensorConnectionStatus?: 'connected' | 'disconnected' | 'unknown';
}

export interface IoTDevice {
  id: string;
  name: string;
  roomId: string;
  room?: Room;
  deviceType: DeviceType;
  manufacturer?: string;
  model?: string;
  serialNumber?: string;
  isActive: boolean;
  sensorDataList?: SensorData[];
  createdAt: Date;
  updatedAt: Date;
}

export interface SensorData {
  id: string;
  deviceId: string;
  device?: IoTDevice;
  sensorId?: string;
  sensorName?: string;
  location?: string;
  temperature?: number;
  humidity?: number;
  timestamp: Date;
  createdAt: Date;
  metadata?: string;
}

export enum DeviceType {
  TemperatureSensor = 0,
  HumiditySensor = 1,
  CombinedSensor = 2,
  SmartThermostat = 3,
  AirQualitySensor = 4,
  MotionSensor = 5,
  DoorSensor = 6,
  WindowSensor = 7,
  SmartLight = 8,
  SmartPlug = 9
}

export interface SensorDataAggregate {
  deviceId: string;
  deviceName: string;
  averageTemperature: number;
  averageHumidity: number;
  minTemperature: number;
  maxTemperature: number;
  minHumidity: number;
  maxHumidity: number;
  totalReadings: number;
  lastReading: Date;
}

// API Response interfaces
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

// Create/Update DTOs
export interface CreateHouseDto {
  name: string;
  address: string;
}

export interface UpdateHouseDto {
  name?: string;
  address?: string;
}

export interface CreateRoomDto {
  name: string;
  houseId: string;
}

export interface UpdateRoomDto {
  name?: string;
  houseId?: string;
}

export interface CreateDeviceDto {
  name: string;
  roomId: string;
  deviceType: DeviceType;
  manufacturer?: string;
  model?: string;
  serialNumber?: string;
  isActive?: boolean;
}

export interface UpdateDeviceDto {
  name?: string;
  roomId?: string;
  deviceType?: DeviceType;
  manufacturer?: string;
  model?: string;
  serialNumber?: string;
  isActive?: boolean;
}
