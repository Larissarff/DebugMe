export interface EventLog {
  id: string;
  userId: string;
  emotionId: string;
  description: string;
  intensity: number;
  eventDate: string;
  createdAt: string;
  updatedAt?: string;
  emotion?: EmotionInfo;
  user?: UserInfo;
}

export interface EmotionInfo {
  id: string;
  name: string;
  description?: string;
}

export interface UserInfo {
  id: string;
  name: string;
  email: string;
}
