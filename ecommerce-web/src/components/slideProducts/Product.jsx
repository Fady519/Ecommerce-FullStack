import React, { useContext } from 'react'
import { RiStarSFill } from "react-icons/ri";

import { FaCartArrowDown } from "react-icons/fa";
import { FaRegHeart } from "react-icons/fa";
import { FaShare } from "react-icons/fa";
import { Link, useNavigate } from 'react-router-dom';
import { CartContext } from '../Context/CartContext';
import { FaCheckCircle } from "react-icons/fa";
import toast from 'react-hot-toast';
import Cart from './../../page/Cart/Cart';

function Product({item}) {

  const navigate = useNavigate()

  const {cartItems , addToCart , favorites , removeFromFavorites , addToFavorite} = useContext(CartContext);

  const isInCart = cartItems.some(i => i.id === item.id);

  const handleAddToCart = () => {
    addToCart(item)

    toast.success(
      <div className='toast-wrapper'>
        <img src={item.images[0]} alt='' className='toast-img'/>

        <div className='toast-content'>
          <strong>{item.title}</strong>
          added to Cart
          <div>
            <button className='btn' onClick={() => navigate('/cart')}> View Cart </button>
          </div>
        </div>

      </div>  
      ,{duration: 3500}
    )

  }


  const isInFav = favorites.some(i => i.id === item.id);

  const handleAddToFavorite = () => {
    if(isInFav)
    {
      removeFromFavorites(item.id)
       toast.error(`${item.title} removed from favorites`)
    }
    else{ 
    addToFavorite(item)
    toast.success(`${item.title} added to favorites`)
  }
}


  return (
    <div className={`product ${isInCart ? 'in-cart' : ''}`}>

      <Link to={`/products/${item.id}`}>

      <span className='status-cart'> <FaCheckCircle /> in cart </span>
      
      <div className='img_product'>
        <img src={item?.images?.[0]} alt={item?.title} />

      </div>
      
      <p className='name_product'> {item.title} </p>


      <div className='stars'>
          <RiStarSFill />
          <RiStarSFill />
          <RiStarSFill />
          <RiStarSFill />

      </div>

      <p className='price'> <span> $ {item.price} </span> </p>

      
      </Link>

      <div className='icons'>
        <span className='btn_addToCart' onClick={handleAddToCart}>
          <FaCartArrowDown />
        </span>

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

export default Product
