import React, { useContext } from 'react';
import { RiStarSFill } from "react-icons/ri";
import { HiOutlineHeart, HiHeart, HiOutlineShoppingBag, HiShoppingBag } from "react-icons/hi2";
import { FaShare } from "react-icons/fa";
import { CartContext } from '../../../components/Context/CartContext';
import { AuthContext } from '../../../components/Context/AuthContext';
import toast from 'react-hot-toast';
import { useNavigate } from 'react-router-dom';

function ProductInfo({ product }) {
  const { cartItems, addToCart, removeFromFavorites, addToFavorite, favorites } = useContext(CartContext);
  const { isAuthenticated } = useContext(AuthContext);
  const navigate = useNavigate();

  if (!product) return null;

  const id = product.id;
  const name = product.name || product.title;
  const price = product.price || 0;
  const image = product.pictureUrl || product.images?.[0] || product.thumbnail || "/src/img/no-results.png";
  const brand = product.productBrand || product.brand;
  const type = product.productType || product.category;
  const stock = product.stock ?? 10;

  const isInCart = cartItems.some(i => i.id === id);
  const isInFav = favorites.some(i => i.id === id);

  const handleAddToCart = () => {
    if (!isAuthenticated) {
      toast.error("Please sign in or register to add products to your cart!");
      navigate("/login");
      return;
    }

    addToCart({
      id,
      title: name,
      name,
      price,
      images: [image],
      pictureUrl: image,
      brand
    });

    toast.success(
      <div className='toast-wrapper' style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
        <img src={image} alt={name} className='toast-img' style={{ width: '40px', height: '40px', objectFit: 'cover' }} />
        <div className='toast-content'>
          <strong>{name}</strong> added to Cart!
          <div style={{ marginTop: '4px' }}>
            <button className='btn' onClick={() => navigate('/cart')} style={{ fontSize: '11px', padding: '3px 8px' }}> View Cart </button>
          </div>
        </div>
      </div>,
      { duration: 3500 }
    );
  };

  const handleAddToFavorite = () => {
    if (!isAuthenticated) {
      toast.error("Please sign in or register to add products to your wishlist!");
      navigate("/login");
      return;
    }

    if (isInFav) {
      removeFromFavorites(id);
      toast.error(`${name} removed from wishlist`);
    } else {
      addToFavorite({
        id,
        title: name,
        name,
        price,
        images: [image],
        pictureUrl: image,
        brand
      });
      toast.success(`${name} added to wishlist`);
    }
  };

  return (
    <div className="details_item">
      {brand && (
        <span style={{ 
          display: 'inline-block', 
          background: '#f1f5f9', 
          color: '#0062ff', 
          padding: '4px 10px', 
          borderRadius: '6px', 
          fontSize: '12px', 
          fontWeight: '700',
          textTransform: 'uppercase',
          marginBottom: '8px'
        }}>
          {brand}
        </span>
      )}

      <h1 className="name" style={{ fontSize: '26px', color: '#0f172a', marginBottom: '10px' }}>{name}</h1>

      <div className='stars' style={{ display: 'flex', alignItems: 'center', gap: '4px', marginBottom: '12px' }}>
        <RiStarSFill fill="#f59e0b" />
        <RiStarSFill fill="#f59e0b" />
        <RiStarSFill fill="#f59e0b" />
        <RiStarSFill fill="#f59e0b" />
        <RiStarSFill fill="#f59e0b" />
        <span style={{ fontSize: '13px', color: '#64748b', marginLeft: '6px' }}>({stock > 0 ? `${stock} in stock` : 'Out of stock'})</span>
      </div>

      <p className="price" style={{ fontSize: '26px', fontWeight: '800', color: '#0062ff', marginBottom: '16px' }}>
        ${Number(price).toFixed(2)}
      </p>

      <h5>Description:</h5>
      <p className="description" style={{ color: '#475569', lineHeight: '1.6', marginBottom: '25px' }}>
        {product.description || "High-quality original product verified by our store guarantee."}
      </p>

      <div className="icons" style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
        <button 
          className="btn" 
          onClick={handleAddToCart}
          style={{ 
            display: 'flex', 
            alignItems: 'center', 
            gap: '8px', 
            padding: '12px 24px', 
            background: isInCart ? '#10b981' : '#0062ff',
            color: '#fff',
            border: 'none',
            borderRadius: '8px',
            fontWeight: '600',
            cursor: 'pointer'
          }}
        >
          {isInCart ? <HiShoppingBag /> : <HiOutlineShoppingBag />}
          {isInCart ? "Item In Cart" : "Add to Cart"}
        </button>

        <button 
          className="btn_fav" 
          onClick={handleAddToFavorite}
          style={{
            padding: '12px',
            background: isInFav ? '#fee2e2' : '#f8fafc',
            border: '1px solid #e2e8f0',
            borderRadius: '8px',
            cursor: 'pointer',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontSize: '20px'
          }}
          title={isInFav ? "Remove from wishlist" : "Add to wishlist"}
        >
          {isInFav ? <HiHeart fill="#ef4444" /> : <HiOutlineHeart />}
        </button>
      </div>
    </div>
  );
}

export default ProductInfo;
