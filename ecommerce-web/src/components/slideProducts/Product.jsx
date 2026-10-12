import React, { useContext } from 'react';
import { RiStarSFill } from "react-icons/ri";
import { HiOutlineShoppingBag, HiShoppingBag, HiOutlineHeart, HiHeart } from "react-icons/hi2";
import { FaShare } from "react-icons/fa";
import { Link, useNavigate } from 'react-router-dom';
import { CartContext } from '../Context/CartContext';
import { AuthContext } from '../Context/AuthContext';
import { FaCheckCircle } from "react-icons/fa";
import toast from 'react-hot-toast';

function Product({ item }) {
  const navigate = useNavigate();
  const { cartItems, addToCart, favorites, removeFromFavorites, addToFavorite } = useContext(CartContext);
  const { isAuthenticated } = useContext(AuthContext);

  if (!item) return null;

  // Normalize product properties whether from ASP.NET API or legacy dummy data
  const id = item.id;
  const name = item.name || item.title || "Product";
  const price = item.price || 0;
  const rawImage = item.pictureUrl || item.images?.[0] || item.thumbnail || "/src/img/no-results.png";
  const image = (rawImage.startsWith('http') || rawImage.startsWith('/')) ? rawImage : '/' + rawImage;
  const brand = item.productBrand || item.brand || "";

  const isInCart = cartItems.some(i => i.id === id);
  const isInFav = favorites.some(i => i.id === id);

  const handleAddToCart = (e) => {
    e.preventDefault();
    e.stopPropagation();

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
      brand,
      productBrand: brand
    });

    toast.success(
      <div className='toast-wrapper' style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
        <img src={image} alt={name} style={{ width: '40px', height: '40px', objectFit: 'cover', borderRadius: '4px' }}/>
        <div>
          <strong style={{ display: 'block', fontSize: '13px' }}>{name}</strong>
          <span style={{ fontSize: '12px', color: '#888' }}>Added to your cart</span>
        </div>
      </div>,
      { duration: 3000 }
    );
  };

  const handleAddToFavorite = (e) => {
    e.preventDefault();
    e.stopPropagation();

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

  const handleShare = (e) => {
    e.preventDefault();
    e.stopPropagation();
    navigator.clipboard?.writeText(window.location.origin + `/products/${id}`);
    toast.success("Product link copied to clipboard!");
  };

  return (
    <div className={`product ${isInCart ? 'in-cart' : ''}`}>
      <Link to={`/products/${id}`} className="product_link_wrap">
        {isInCart && (
          <span className='status-cart'>
            <FaCheckCircle /> In Cart
          </span>
        )}
        
        <div className='img_product'>
          <img src={image} alt={name} loading="lazy" />
        </div>

        {brand && <span className="product_brand_tag">{brand}</span>}
        <h3 className='name_product' title={name}>{name}</h3>

        <div className='stars'>
          <RiStarSFill />
          <RiStarSFill />
          <RiStarSFill />
          <RiStarSFill />
          <RiStarSFill />
        </div>

        <p className='price'>
          <span>${Number(price).toFixed(2)}</span>
        </p>
      </Link>

      <div className='icons'>
        <button 
          type="button"
          className={`action_icon_btn ${isInCart ? "active_cart" : ""}`} 
          onClick={handleAddToCart}
          title={isInCart ? "Already in cart" : "Add to Cart"}
          aria-label="Add to cart"
        >
          {isInCart ? <HiShoppingBag /> : <HiOutlineShoppingBag />}
        </button>

        <button 
          type="button"
          className={`action_icon_btn ${isInFav ? "in-fav" : ""}`} 
          onClick={handleAddToFavorite}
          title={isInFav ? "Remove from wishlist" : "Add to Wishlist"}
          aria-label="Add to wishlist"
        >
          {isInFav ? <HiHeart style={{ color: '#ef4444' }} /> : <HiOutlineHeart />}
        </button>

        <button 
          type="button"
          className="action_icon_btn" 
          onClick={handleShare}
          title="Share product"
          aria-label="Share product"
        >
          <FaShare />
        </button>
      </div>
    </div>
  );
}

export default Product;
