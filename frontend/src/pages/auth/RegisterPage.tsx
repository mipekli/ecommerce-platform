import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

export default function RegisterPage() {
  const [form, setForm] = useState({ email: '', firstName: '', lastName: '', password: '', confirmPassword: '' });
  const { handleRegister, isLoading } = useAuth();
  const navigate = useNavigate();

  const updateField = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) =>
    setForm((prev) => ({ ...prev, [field]: e.target.value }));

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (form.password !== form.confirmPassword) {
      alert('Şifreler eşleşmiyor.');
      return;
    }
    const success = await handleRegister(form);
    if (success) navigate('/products');
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-gray-900">Kayıt Ol</h1>
          <p className="mt-2 text-gray-600">Yeni bir hesap oluşturun.</p>
        </div>
        <form onSubmit={onSubmit} className="card space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Ad</label>
            <input type="text" required value={form.firstName} onChange={updateField('firstName')} className="input-field" placeholder="Adınız" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Soyad</label>
            <input type="text" required value={form.lastName} onChange={updateField('lastName')} className="input-field" placeholder="Soyadınız" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">E-posta</label>
            <input type="email" required value={form.email} onChange={updateField('email')} className="input-field" placeholder="ornek@email.com" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Şifre</label>
            <input type="password" required value={form.password} onChange={updateField('password')} className="input-field" placeholder="••••••••" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Şifre Tekrar</label>
            <input type="password" required value={form.confirmPassword} onChange={updateField('confirmPassword')} className="input-field" placeholder="••••••••" />
          </div>
          <button type="submit" disabled={isLoading} className="btn-primary w-full">
            {isLoading ? 'Kaydediliyor...' : 'Kayıt Ol'}
          </button>
          <p className="text-center text-sm text-gray-600">
            Zaten hesabınız var mı? <Link to="/login" className="text-primary-600 hover:underline">Giriş Yap</Link>
          </p>
        </form>
      </div>
    </div>
  );
}
