import apiClient from './client';
import { AuthResponse } from '../types';

export const authApi = {
  register: async (data: { email: string; firstName: string; lastName: string; password: string; confirmPassword: string }) => {
    const response = await apiClient.post<AuthResponse>('/auth/register', data);
    return response.data;
  },

  login: async (data: { email: string; password: string }) => {
    const response = await apiClient.post<AuthResponse>('/auth/login', data);
    return response.data;
  },

  getProfile: async () => {
    const response = await apiClient.get<AuthResponse['user']>('/auth/profile');
    return response.data;
  },
};
