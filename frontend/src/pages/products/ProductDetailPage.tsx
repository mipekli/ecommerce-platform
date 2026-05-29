import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { ArrowLeft, ShoppingCartPlus } from 'lucide-react';
import { Product } from '../../types';
import { productsApi } from '../../api/products';
import { useCartStore } from '../../stores/cartStore';
import { formatPrice, formatDate } from '../../lib/utils';
import toast from 'react-hot-toast';

export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [product, setProduct] = useState<Product | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const addItem = useCartStore((state) => state.addItem);

  useEffect(() => {
    if (!id) return;
    productsApi.getById(id).then(setProduct).catch(() => toast.error('Ürün bulunamadı.')).finally(() => setIsLoading(false));
  }, [id]);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500 mb-4">Ürün bulunamadı.</p>
        <Link to="/products" className="btn-primary">Ürünlere Dön</Link>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      <Link to="/products" className="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-6">
        <ArrowLeft size={20} /> Ürünlere Dön
      </Link>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div className="aspect-square bg-gray-100 rounded-xl overflow-hidden">
          {product.imageUrl ? (
            <img src={product.imageUrl} alt={product.name} className="w-full h-full object-cover" />
          ) : (
            <div className="w-full h-full flex items-center justify-center text-gray-400">No Image</div>
          )}
        </div>
        <div className="space-y-4">
          <h1 className="text-3xl font-bold text-gray-900">{product.name}</h1>
          <span className="inline-block bg-primary-100 text-primary-800 text-sm px-3 py-1 rounded-full">
            {product.categoryName}
          </span>
          <p className="text-4xl font-bold text-primary-600">{formatPrice(product.price)}</p>
          <p className="text-gray-600">{product.description}</p>
          <div className="border-t pt-4 space-y-2 text-sm text-gray-500">
            <p><strong>SKU:</strong> {product.sku}</p>
            <p><strong>Oluşturulma:</strong> {formatDate(product.createdAt)}</p>
          </div>
          <button onClick={() => { addItem({ productId: product.id, productName: product.name, unitPrice: product.price, quantity: 1, imageUrl: product.imageUrl }); toast.success('Sepete eklendi!'); }} className="btn-primary w-full flex items-center justify-center gap-2 py-3 text-lg">
            <ShoppingCartPlus size={24} /> Sepete Ekle
          </button>
        </div>
      </div>
    </div>
  );
}
