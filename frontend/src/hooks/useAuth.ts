import { useState } from 'react';
import { useAuthStore } from '../stores/authStore';
import toast from 'react-hot-toast';

export function useAuth() {
  const { login, register, logout, user, isAuthenticated } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);

  const handleLogin = async (email: string, password: string) => {
    try {
      setIsLoading(true);
      await login(email, password);
      toast.success('Başarıyla giriş yaptınız!');
      return true;
    } catch (error: any) {
      const message = error.response?.data?.message || 'Giriş yapılırken bir hata oluştu.';
      toast.error(message);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const handleRegister = async (data: { email: string; firstName: string; lastName: string; password: string; confirmPassword: string }) => {
    try {
      setIsLoading(true);
      await register(data);
      toast.success('Kayıt başarıyla tamamlandı!');
      return true;
    } catch (error: any) {
      const message = error.response?.data?.message || 'Kayıt olurken bir hata oluştu.';
      toast.error(message);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  return { handleLogin, handleRegister, logout, user, isAuthenticated, isLoading };
}
