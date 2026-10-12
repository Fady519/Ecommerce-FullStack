import React, { useContext } from 'react';
import { HiOutlineHeart, HiOutlineShoppingBag, HiOutlineSun, HiOutlineMoon } from "react-icons/hi2";
import { Link } from 'react-router-dom';
import './Header.css';
import { CartContext } from '../Context/CartContext';
import { ThemeContext } from '../Context/ThemeContext';
import SearchBox from './SearchBox';

export default function TopHeader() {
    const { cartItems, favorites } = useContext(CartContext);
    const { isDark, toggleTheme } = useContext(ThemeContext);

    return (
        <div className='top_header'>
            <div className='container'>
                <Link className='brand_logo' to='/' title="LuxeStore Home">
                    <div className="brand_logo_icon">
                        <svg width="34" height="34" viewBox="0 0 34 34" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <defs>
                                <linearGradient id="brandLogoGrad" x1="0%" y1="0%" x2="100%" y2="100%">
                                    <stop offset="0%" stopColor="#0090f0" />
                                    <stop offset="100%" stopColor="#38bdf8" />
                                </linearGradient>
                            </defs>
                            <rect width="34" height="34" rx="10" fill="url(#brandLogoGrad)" />
                            <path d="M11 13V10.5C11 7.46243 13.4624 5 16.5 5H17.5C20.5376 5 23 7.46243 23 10.5V13M8 12.5H26L24.5 27H9.5L8 12.5Z" stroke="#ffffff" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round"/>
                            <circle cx="17" cy="18.5" r="1.5" fill="#ffffff" />
                        </svg>
                    </div>
                    <div className="brand_logo_text">
                        <span className="brand_logo_title">LUXE<span>STORE</span></span>
                        <span className="brand_logo_subtitle">PREMIUM SHOP</span>
                    </div>
                </Link>

                <SearchBox />

                <div className='header_icons'>
                    <button 
                        type="button" 
                        className="header_icon_btn theme_toggle_btn" 
                        onClick={toggleTheme} 
                        title={isDark ? "Switch to Light Mode" : "Switch to Dark Mode"}
                        aria-label="Toggle theme"
                    >
                        <div className="icon_circle theme_circle">
                            {isDark ? <HiOutlineSun className="action_svg sun_icon" /> : <HiOutlineMoon className="action_svg moon_icon" />}
                        </div>
                        <div className="icon_text_label">
                            <span className="sub">Theme</span>
                            <span className="count_info">{isDark ? "Dark" : "Light"}</span>
                        </div>
                    </button>

                    <Link to="/favorites" className='header_icon_btn' title="Wishlist">
                        <div className="icon_circle">
                            <HiOutlineHeart className="action_svg" />
                            {favorites.length > 0 && (
                                <span className='badge_counter'>{favorites.length}</span>
                            )}
                        </div>
                        <div className="icon_text_label">
                            <span className="sub">Wishlist</span>
                            <span className="count_info">{favorites.length} items</span>
                        </div>
                    </Link>

                    <Link to='/cart' className='header_icon_btn cart_btn' title="Shopping Cart">
                        <div className="icon_circle cart_circle">
                            <HiOutlineShoppingBag className="action_svg" />
                            {cartItems.length > 0 && (
                                <span className='badge_counter cart_counter'>{cartItems.length}</span>
                            )}
                        </div>
                        <div className="icon_text_label">
                            <span className="sub">My Cart</span>
                            <span className="count_info">{cartItems.length} items</span>
                        </div>
                    </Link>
                </div>
            </div>
        </div>
    );
}
