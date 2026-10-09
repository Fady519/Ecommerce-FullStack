
import React, { useContext } from 'react'
import { CartContext } from '../../components/Context/CartContext';
import PageTransition from '../../components/PageTransition';
import Product from '../../components/slideProducts/Product';
import { Link } from "react-router-dom";
import noResults from "../../img/no-results.png";

function Favorites() {

    const { favorites } = useContext(CartContext);

    return (
        <PageTransition>

            <div className='category_products favoritesPage'>

                {favorites.length === 0 ? (

                    <div className="container no-results-container">

                        <div className="no-results">

                            <img src={noResults} alt="no favorites" />

                            <h2>No Favorites Yet</h2>

                            <p className="empty-fav">
                                You haven't added any favorites yet ❤️
                            </p>

                            <Link to="/" className="back-home">
                                Back To Home
                            </Link>

                        </div>

                    </div>

                ) : (

                    <div className='container'>

                        <div className='top_slide'>
                            <h2>Your Favorites</h2>
                        </div>

                        <div className='products'>
                            {favorites.map((item) => (
                                <Product item={item} key={item.id} />
                            ))}
                        </div>

                    </div>

                )}

            </div>

        </PageTransition>
    )
}

export default Favorites