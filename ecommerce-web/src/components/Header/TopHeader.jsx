import React, { useContext } from 'react'
import Logo from '../../img/logo2.jpg'

import { FaRegHeart } from "react-icons/fa";
import { TiShoppingCart } from "react-icons/ti";

import { Link } from 'react-router-dom';

import './Header.css';
import { CartContext } from '../Context/CartContext';
import SearchBox from './SearchBox';

export default function TopHeader() {

    const { cartItems, favorites } = useContext(CartContext);

    return (
        <div className='top_header'>

            <div className='container'>

                <Link className='logo' to='/' > <img src={Logo} alt='Logo' /> </Link>

                <SearchBox />

                <div className='header_icons'>

                    <div className='icon'>
                        <Link to="/favorites">
                            <FaRegHeart />
                            <span className='count'> {favorites.length} </span>
                        </Link>
                    </div>

                    <div className='icon'>
                        <Link to='/cart' >
                            <TiShoppingCart />
                            <span className='count'> {cartItems.length} </span>
                        </Link>
                    </div>

                </div>

            </div>

        </div>
    )
}
