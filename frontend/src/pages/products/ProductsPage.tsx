import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Search, ShoppingCartPlus } from 'lucide-react';
import { useProducts } from '../../hooks/useProducts';
import { useCartStore } from '../../stores/cartStore';
import { formatPrice } from '../../lib/utils';
import toast from 'react-hot-toast';

export default function ProductsPage() {
  const { products, categories, isLoading } = useProducts();
  const [search, setSearch] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const addItem = useCartStore((state) => state.addItem);

  const filtered = products.filter((p) => {
    const matchesSearch = p.name.toLowerCase().includes(search.toLowerCase()) ||
      p.description.toLowerCase().includes(search.toLowerCase());
    const matchesCategory = !selectedCategory || p.categoryId === selectedCategory;
    return matchesSearch && matchesCategory;
  });

  const handleAddToCart = (product: typeof products[0]) => {
    addItem({
      productId: product.id,
      productName: product.name,
      unitPrice: product.price,
      quantity: 1,
      imageUrl: product.imageUrl,
    });
    toast.success(`${product.name} sepete eklendi!`);
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
          <input type="text" value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Ürün ara..." className="input-field pl-10" />
        </div>
        <select value={selectedCategory} onChange={(e) => setSelectedCategory(e.target.value)} className="input-field sm:w-48">
          <option value="">Tüm Kategoriler</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {filtered.map((product) => (
          <div key={product.id} className="card hover:shadow-md transition-shadow">
            <Link to={`/products/${product.id}`}>
              <div className="aspect-square bg-gray-100 rounded-lg mb-4 overflow-hidden">
                {product.imageUrl ? (
                  <img src={product.imageUrl} alt={product.name} className="w-full h-full object-cover" />
                ) : (
                  <div className="w-full h-full flex items-center justify-center text-gray-400">No Image</div>
                )}
              </div>
              <h3 className="font-semibold text-gray-900 mb-1">{product.name}</h3>
              <p className="text-sm text-gray-500 mb-2 line-clamp-2">{product.description}</p>
              <p className="text-lg font-bold text-primary-600">{formatPrice(product.price)}</p>
            </Link>
            <button onClick={() => handleAddToCart(product)} className="btn-primary w-full mt-4 flex items-center justify-center gap-2">
              <ShoppingCartPlus size={18} /> Sepete Ekle
            </button>
          </div>
        ))}
      </div>

      {filtered.length === 0 && (
        <div className="text-center py-12 text-gray-500">Ürün bulunamadı.</div>
      )}
    </div>
  );
}
