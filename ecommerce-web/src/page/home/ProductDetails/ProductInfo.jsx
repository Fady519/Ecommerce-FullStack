import React, { useContext } from 'react'
import { RiStarSFill } from "react-icons/ri";
import { FaRegHeart } from "react-icons/fa";
import { FaShare } from "react-icons/fa";
import { TiShoppingCart } from "react-icons/ti";
import { CartContext } from '../../../components/Context/CartContext';
import toast from 'react-hot-toast';
import { useNavigate } from 'react-router-dom';
function ProductInfo({ product }) {

  const { cartItems, addToCart, removeFromFavorites, addToFavorite, favorites } = useContext(CartContext);
  const isInCart = cartItems.some(i => i.id === product.id);
  const navigate = useNavigate()

  const handleAddToCart = () => {
    addToCart(product)

    toast.success(
      <div className='toast-wrapper'>
        <img src={product.images[0]} alt='' className='toast-img' />

        <div className='toast-content'>
          <strong>{product.title}</strong>
          added to Cart
          <div>
            <button className='btn' onClick={() => navigate('/cart')}> View Cart </button>
          </div>
        </div>

      </div>
      , { duration: 3500 }
    )

  }


  const isInFav = favorites.some(i => i.id === product.id);

  const handleAddToFavorite = () => {
    if (isInFav) {
      removeFromFavorites(product.id)
      toast.error(`${product.title} removed from favorites`)
    }
    else {
      addToFavorite(product)
      toast.success(`${product.title} added to favorites`)
    }
  }


  return (
    <div className="details_item">
      <h1 className="name"> {product.title} </h1>

      <div className='stars'>
        <RiStarSFill />
        <RiStarSFill />
        <RiStarSFill />
        <RiStarSFill />

      </div>
      <p className="price"> {product.price} </p>
      <h5> Availability: <span> {product.availabilityStatus} </span></h5>
      <h5> Brand: <span> {product.brand} </span></h5>
      <p className="desc"> {product.description} </p>
      <h5><span>Hurry Up! Only {product.stock} products left in stock. </span></h5>

      <button onClick={handleAddToCart} className={`btn ${isInCart ? "in-cart" : ''}`}>
        {isInCart ? "Items In Cart" : "Add To Cart"} <TiShoppingCart />
      </button>

      <div className='icons'>
        <span className={`${isInFav ? "in-fav" : ""}`} onClick={handleAddToFavorite}>
          <FaRegHeart />
        </span>

        <span>
          <FaShare />
        </span>

      </div>


    </div>
  )
}

export default ProductInfo
