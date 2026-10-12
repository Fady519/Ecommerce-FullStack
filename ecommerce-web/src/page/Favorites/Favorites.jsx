
import React, { useContext } from 'react';
import { CartContext } from '../../components/Context/CartContext';
import PageTransition from '../../components/PageTransition';
import Product from '../../components/slideProducts/Product';
import { Link } from "react-router-dom";
import noResults from "../../img/no-results.png";
import '../CategoryPage/CategoryPage.css';

function Favorites() {
    const { favorites } = useContext(CartContext);

    return (
        <PageTransition>
            <div className='category_products favoritesPage'>
                <div className='container'>
                    {favorites.length === 0 ? (
                        <div className="container no-results-container">
                            <div className="no-results">
                                <img src={noResults} alt="no favorites" style={{ maxWidth: '180px', margin: '0 auto 20px' }} />
                                <h2>No Favorites Yet</h2>
                                <p className="empty-fav">
                                    You haven't saved any favorite products yet ❤️
                                </p>
                                <Link to="/" className="back-home" style={{ display: 'inline-block', marginTop: '15px' }}>
                                    Start Browsing Products
                                </Link>
                            </div>
                        </div>
                    ) : (
                        <>
                            <div className='catalog_header_banner'>
                                <div className="banner_text">
                                    <span className="catalog_tag">Saved Wishlist</span>
                                    <h2>My Favorite Products</h2>
                                    <p>Your saved favorites are always ready to be added to your shopping cart.</p>
                                </div>
                                <div className="catalog_stats">
                                    <span className="stats_count">{favorites.length}</span>
                                    <span className="stats_label">Saved Products</span>
                                </div>
                            </div>

                            <div className='products_grid'>
                                {favorites.map((item) => (
                                    <Product item={item} key={item.id} />
                                ))}
                            </div>
                        </>
                    )}
                </div>
            </div>
        </PageTransition>
    );
}

export default Favorites;