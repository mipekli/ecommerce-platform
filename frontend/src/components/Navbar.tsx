import { Link, useNavigate } from 'react-router-dom';
import { ShoppingCart, Package, LayoutDashboard, Settings, LogOut, User } from 'lucide-react';
import { useAuthStore } from '../stores/authStore';
import { useCartStore } from '../stores/cartStore';

export default function Navbar() {
  const { user, isAuthenticated, logout } = useAuthStore();
  const itemCount = useCartStore((state) => state.getItemCount());
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="bg-white shadow-sm border-b border-gray-200">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center space-x-8">
            <Link to="/products" className="text-xl font-bold text-primary-600">
              E-Commerce
            </Link>
            <Link to="/products" className="text-gray-600 hover:text-gray-900 transition-colors">
              Ürünler
            </Link>
            {isAuthenticated && (
              <>
                <Link to="/orders" className="text-gray-600 hover:text-gray-900 transition-colors flex items-center gap-1">
                  <Package size={18} />
                  Siparişlerim
                </Link>
                <Link to="/dashboard" className="text-gray-600 hover:text-gray-900 transition-colors flex items-center gap-1">
                  <LayoutDashboard size={18} />
                  Panel
                </Link>
                {user?.role === 'Admin' && (
                  <Link to="/admin" className="text-gray-600 hover:text-gray-900 transition-colors flex items-center gap-1">
                    <Settings size={18} />
                    Yönetim
                  </Link>
                )}
              </>
            )}
          </div>
          <div className="flex items-center space-x-4">
            <Link to="/cart" className="relative text-gray-600 hover:text-gray-900 transition-colors">
              <ShoppingCart size={24} />
              {itemCount > 0 && (
                <span className="absolute -top-2 -right-2 bg-primary-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                  {itemCount}
                </span>
              )}
            </Link>
            {isAuthenticated ? (
              <div className="flex items-center space-x-4">
                <div className="flex items-center gap-2 text-sm text-gray-600">
                  <User size={18} />
                  <span>{user?.firstName} {user?.lastName}</span>
                </div>
                <button onClick={handleLogout} className="text-gray-600 hover:text-red-600 transition-colors">
                  <LogOut size={20} />
                </button>
              </div>
            ) : (
              <div className="flex items-center space-x-2">
                <Link to="/login" className="btn-secondary text-sm">Giriş Yap</Link>
                <Link to="/register" className="btn-primary text-sm">Kayıt Ol</Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
